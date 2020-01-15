﻿using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PostGis.Models
{
    public class FakeClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Column(TypeName = "geography")]
        public Point Location { get; set; }
        public Polygon polygon { get; set; }
    }
}
