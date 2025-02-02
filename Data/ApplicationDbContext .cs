using CloseFriendMyanamr.Models;
using Microsoft.EntityFrameworkCore;

namespace SimpleDataWebsite.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
        }

        public DbSet<EmployeeType> EmployeeType { get; set; }
        public DbSet<EmployeeModel> Employee { get; set; }
        public DbSet<IncomeTitleModel> IncomeTitle { get; set; }
        public DbSet<ExpenseTitleModel> ExpenseTitle { get; set; }
        public DbSet<FacilityModel> Facilities { get; set; }
        public DbSet<PropertyTypeModel> PropertyType { get; set; }
        public DbSet<BuildingTypeModel> BuildingType { get; set; }
        public DbSet<BankAccountModel> BankAccount { get; set; }
    }
}
