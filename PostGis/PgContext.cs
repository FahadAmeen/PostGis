using Microsoft.EntityFrameworkCore;
using PostGis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostGis
{
    public class PgContext : DbContext
    {
        public PgContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasPostgresExtension("postgis");
            
        }
        public DbSet<FakeClass> fake { get; set; }
    }
}
