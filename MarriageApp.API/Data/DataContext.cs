using MarriageApp.API.Models;
using Microsoft.EntityFrameworkCore;
namespace MarriageApp.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options){}
        public DbSet<value> Values { get; set; }
    }
}