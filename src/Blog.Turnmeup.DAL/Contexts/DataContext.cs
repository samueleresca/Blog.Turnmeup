using Blog.Turnmeup.DAL.Models;
using Blog.Turnmeup.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Turnmeup.Contexts
{
    public class DataContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=turnmeup;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }
     
    }
}
