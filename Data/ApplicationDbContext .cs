using CloseFriendMyanamr.Models;
using Microsoft.EntityFrameworkCore;

namespace SimpleDataWebsite.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
        }

        public DbSet<EmployeeType> EmployeeTypes { get; set; }
        public DbSet<IncomeTitleModel> IncomeTitles { get; set; }
    }
}
