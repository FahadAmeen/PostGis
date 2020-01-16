using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nancy.Json;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PostGis;
using PostGis.Models;
using Geometry = NetTopologySuite.Geometries.Geometry;

namespace PostGis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FakeClassesController : ControllerBase
    {
        private readonly PgContext _context;

        public FakeClassesController(PgContext context)
        {
            _context = context;
        }

        // GET: api/FakeClasses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FakeClass>>> Getfake()
        {
            using (StreamReader r = new StreamReader("IMO.json"))
            {
                var jsondata = r.ReadToEnd();
                var listUrl = JsonConvert.DeserializeObject<List<RootObject>>(jsondata);

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
            }


            return Ok();
        }
        private decimal DegreeToDecimal(decimal d, decimal m, decimal s) { return d + (m / 60) + (s / 3600); }
        private decimal GetDegreeToDecimal(string n)
        {
            try
            {
                var indexD = n.IndexOf("°");
                
                
                var sd = n.Substring(0, indexD);
                n = n.Remove(0,indexD+1);
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
            }catch(Exception ex)
            {
                return 0;
            }
        }
        private Coordinate GetCoordinates(string n,string w)
        {
            return new Coordinate(Convert.ToDouble(GetDegreeToDecimal(n)), Convert.ToDouble(GetDegreeToDecimal(w)));
        }
        // GET: api/FakeClasses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FakeClass>> GetFakeClass(int id)
        {
            var fakeClass = await _context.fake.FindAsync(id);

            if (fakeClass == null)
            {
                return NotFound();
            }

            return fakeClass;
        }

        // PUT: api/FakeClasses/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFakeClass(int id, FakeClass fakeClass)
        {
            if (id != fakeClass.Id)
            {
                return BadRequest();
            }

            _context.Entry(fakeClass).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FakeClassExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private IEnumerable<Coordinate> readJson(string jsonFile= "IMO.json")
        {
            using (StreamReader r = new StreamReader(jsonFile))
            {
                var jsondata = r.ReadToEnd();
                var jsonData = JsonConvert.DeserializeObject<PolygonDTO>(jsondata);

                var geometry = jsonData.geometries;
                var coor = geometry[0].coordinates[0][0];

                foreach (var coordinate in coor)
                {
                   yield return  new Coordinate(coordinate[0], coordinate[1]);
                }

            }
        }

        private Polygon DrawPolygon()
        {
            return new Polygon(new LinearRing(this.readJson().ToArray()));
        }

        // POST: api/FakeClasses
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<FakeClass>> PostFakeClass()
        {
            FakeClass fakeClass=new FakeClass();
            fakeClass.polygon = this.DrawPolygon();
            fakeClass.Name = "point";
            fakeClass.Location = new Point(0, 0);

            var a=_context.fake.Add(fakeClass);
            
            await _context.SaveChangesAsync();
            var json = JsonConvert.SerializeObject(a.Entity);
            return Content(json);
        
        }

        // DELETE: api/FakeClasses/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<FakeClass>> DeleteFakeClass(int id)
        {
            var fakeClass = await _context.fake.FindAsync(id);
            if (fakeClass == null)
            {
                return NotFound();
            }

            _context.fake.Remove(fakeClass);
            await _context.SaveChangesAsync();

            return fakeClass;
        }

        private bool FakeClassExists(int id)
        {
            return _context.fake.Any(e => e.Id == id);
        }
    }
}
