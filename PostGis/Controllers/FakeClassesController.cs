using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostGis;
using PostGis.Models;

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
            return await _context.fake.ToListAsync();
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
        public async Task<ActionResult<FakeClass>> PostFakeClass(FakeClass fakeClass)
        {
            _context.fake.Add(fakeClass);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFakeClass", new { id = fakeClass.Id }, fakeClass);
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
