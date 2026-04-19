using Microsoft.EntityFrameworkCore;

namespace IncidentAPI_ISIMM_MP1_GL.Models
{
    public class IncidentsDbContext : DbContext
    {
        public IncidentsDbContext(DbContextOptions<IncidentsDbContext> options) 
            : base(options)
        {
        }
        public virtual DbSet<Incident> Incidents { get; set; }
    }
}
