using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace AL.ERA.App.MockData.Interfaces
{
    public interface IMockData
    {
        IEnumerable<List<Coordinate>> GetCoordiantesFromJson(string jsonFilePath);
        IEnumerable<List<Polygon>> GetAllPolygons(string jsonFolderPath);

    }
}
