﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostGis.Models
{
    public class RootObject
    {
        public string name { get; set; }
        public List<List<string>> locations { get; set; }
    }

}