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

            var record = await _context.fake.FindAsync(22);
            var polygon = record.polygon;
            //var coor = new Point(72.92304670276317, 33.63573935);
            var coor = new Point(72.92304670276317, 11.63573935);

            var isWithin =polygon.Contains(coor);

            

            return Ok();
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

        // POST: api/FakeClasses
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<FakeClass>> PostFakeClass()
        {
            List<Coordinate> items = new List<Coordinate>();
            using (StreamReader r = new StreamReader("islamabadArea.json"))
            {
                var jsondata = r.ReadToEnd();
                var myJObject = JObject.Parse(jsondata);
                //var t = myJObject.SelectToken("geometries.type").Value<string>();
                var jsonData = JsonConvert.DeserializeObject<PolygonDTO>(jsondata);

                var geometry = jsonData.geometries;
                var coor = geometry[0].coordinates[0][0];
                
                foreach (var coordinate in coor)
                {
                    Coordinate c = new Coordinate(coordinate[0], coordinate[1]);
                    items.Add(c);
                }

            }


            FakeClass fakeClass=new FakeClass();
            fakeClass.Name = "point";
            fakeClass.Location = new Point(0, 0);
            Polygon polygon1 = new Polygon(new LinearRing(new Coordinate[] { new Coordinate(0,0), new Coordinate(1,0), 
                new Coordinate(1,1), new Coordinate(0,1), new Coordinate(0,0) }));
            Polygon polygon = new Polygon(new LinearRing(items.ToArray()));

            fakeClass.polygon = polygon;

            var a=_context.fake.Add(fakeClass);
            fakeClass.Name = fakeClass.polygon.Area.ToString();
            
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
