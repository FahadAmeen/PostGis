using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostGis.Models
{
    public class Geometry
    {
        public string type { get; set; }
        public List<List<List<List<double>>>> coordinates { get; set; }
    }

    public class PolygonDTO
    {
        public string type { get; set; }
        public List<Geometry> geometries { get; set; }
    }
}
