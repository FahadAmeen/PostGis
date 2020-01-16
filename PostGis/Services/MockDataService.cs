using AL.ERA.App.MockData.Interfaces;
using AL.ERA.App.MockData.Models;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AL.ERA.App.MockData.Services
{
    public class MockDataService:IMockData
    {
        private IEnumerable<string> GetFilesFromFolder(string folderName= "JsonFiles")
        {
            return Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), folderName),"*.json");
        }

        public IEnumerable<List<Coordinate>> GetCoordiantesFromJson(string jsonFilePath = "IMO.json")
        {
            //var filePath = Path.Combine(Directory.GetCurrentDirectory(), "JsonFiles", fileName);
            using (StreamReader r = new StreamReader("IMO.json"))
            {
                var jsondata = r.ReadToEnd();
                var listUrl = JsonConvert.DeserializeObject<List<Models.CoordinateList>>(jsondata);

                List<List<Coordinate>> coordinateListList = new List<List<Coordinate>>();
                foreach (var a in listUrl)
                {
                    var countB = 0;
                    List<Coordinate> coordinateList = new List<Coordinate>();
                    foreach (var b in a.locations)
                    {
                        int countC = 0;
                        Coordinate co = new Coordinate();
                        var coorX = Convert.ToDouble(GetDegreeToDecimal(b[countC]).ToString());
                        var coorY = Convert.ToDouble(GetDegreeToDecimal(b[countC++]).ToString());
                        co.X = coorX;
                        co.Y = coorY;
                        coordinateList.Add(co);
                        countB++;
                    }
                    coordinateListList.Add(coordinateList);
                }
                return coordinateListList;

            }
        }
        public Polygon GetPolygon(IEnumerable<Coordinate> coordinates)
        {
            return new Polygon(new LinearRing(coordinates.ToArray()));
        }

        public IEnumerable<List<Polygon>> GetAllPolygons(string jsonFolderPath = "JsonFiles")
        {
            List<string> fileNames = this.GetFilesFromFolder(jsonFolderPath).ToList();
            foreach (var file in fileNames)
            {
                IEnumerable<List<Coordinate>> coordiantes = this.GetCoordiantesFromJson(file);
                yield return this.GetPolygons(coordiantes).ToList();
            }
        }
        private IEnumerable<Polygon> GetPolygons(IEnumerable<List<Coordinate>> coordinateListList)
        {
            foreach (var polygon in coordinateListList)
            {
                yield return new Polygon(new LinearRing(polygon.ToArray()));
            }
        }

        //private IEnumerable<Polygon> GetPolygons(List<List<Coordinate>> coordinateListList)
        //{
        //    foreach (var polygon in coordinateListList)
        //    {
        //        yield return new Polygon(new LinearRing(polygon.ToArray()));
        //    }
        //}
        private decimal DegreeToDecimal(decimal d, decimal m, decimal s) { return d + (m / 60) + (s / 3600); }
        private decimal GetDegreeToDecimal(string n)
        {
            try
            {
                var indexD = n.IndexOf("°");


                var sd = n.Substring(0, indexD);
                n = n.Remove(0, indexD + 1);
                var indexM = n.IndexOf("'");
                var sm = n.Substring(0, indexM);
                n = n.Remove(0, indexM + 1);
                var indexS = n.IndexOf("\"");
                var ss = n.Substring(0, indexS);
                var d = Convert.ToDecimal(sd);
                var m = Convert.ToDecimal(sm);
                var s = Convert.ToDecimal(ss);

                var decimalCoordinate = DegreeToDecimal(d, m, s);
                return decimalCoordinate;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
