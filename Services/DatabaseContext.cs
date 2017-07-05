using SSUMAP.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace SSUMAP.Services {
    public class DatabaseContext : DbContext {

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options){ }
        public DbSet<Spot> Spots { get; set; }
    }
}