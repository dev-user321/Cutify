using Cutify.Models;
using Microsoft.EntityFrameworkCore;

namespace Cutify.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<AppUser> Users { get; set; }   
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<WorkHour> WorkHours { get; set; }
    }
}
