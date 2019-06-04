using Microsoft.EntityFrameworkCore;
using UserManagement.Models;

namespace UserManagement.Helpers
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> contextOptions) : base(contextOptions) { }

        public DbSet<User> Users { get; set; }
    }
}
