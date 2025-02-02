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
        public DbSet<EmployeeModel> Employees { get; set; }
        public DbSet<IncomeTitleModel> IncomeTitles { get; set; }
        public DbSet<ExpenseModel> Expenses { get; set; }
        public DbSet<FacilityModel> Facilities { get; set; }
        public DbSet<PropertyTypeModel> PropertyTypes { get; set; }
        public DbSet<BuildingTypeModel> BuildingTypes { get; set; }
        public DbSet<BankAccountInfoModel> BankAccountInfos { get; set; }
    }
}
