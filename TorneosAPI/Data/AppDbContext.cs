using Microsoft.EntityFrameworkCore;
using TorneosAPI.Models;

namespace TorneosAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Torneo> Torneos { get; set; }
        
    }
}
