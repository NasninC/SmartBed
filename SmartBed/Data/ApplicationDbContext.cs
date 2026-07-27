using Microsoft.EntityFrameworkCore;
using SmartBed.Models;

namespace SmartBed.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Admin> Admin { get; set; }

        public DbSet<Hospital> Hospital { get; set; }

        public DbSet<User> Users { get; set; }
    }
}