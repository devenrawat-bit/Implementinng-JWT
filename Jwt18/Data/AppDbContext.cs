using Jwt18.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Jwt18.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options):DbContext(options)
    {
       public DbSet<User> users { get; set; } 
    }
}
