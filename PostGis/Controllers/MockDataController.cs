using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AL.ERA.App.MockData.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PostGis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MockDataController : ControllerBase
    {
        private readonly PgContext _context;
        private readonly IMockData _mockData;


        public MockDataController(PgContext context,IMockData data)
        {
            _context = context;
            _mockData = data;
        }
        [HttpGet]
        public IActionResult test()
        {
            var polygons=_mockData.GetAllPolygons("JsonFiles").ToList();
            return Ok();
        }
    }
}