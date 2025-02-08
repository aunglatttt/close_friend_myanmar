using CloseFriendMyanamr.Models;
using CloseFriendMyanamr.Models.CashManagement;
using CloseFriendMyanamr.Models.UserManagement;
using Microsoft.EntityFrameworkCore;

namespace SimpleDataWebsite.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
        }

        #region configuration
        public DbSet<EmployeeType> EmployeeType { get; set; }
        public DbSet<EmployeeModel> Employee { get; set; }
        public DbSet<IncomeTitleModel> IncomeTitle { get; set; }
        public DbSet<ExpenseTitleModel> ExpenseTitle { get; set; }
        public DbSet<FacilityModel> Facilities { get; set; }
        public DbSet<PropertyTypeModel> PropertyType { get; set; }
        public DbSet<BuildingTypeModel> BuildingType { get; set; }
        public DbSet<BankAccountModel> BankAccount { get; set; }
        public DbSet<LogModel> Log { get; set; }
        #endregion

        #region user management
        public DbSet<ClientModel> Client { get; set; }
        public DbSet<OwnerModel> Owner { get; set; }
        public DbSet<AgentModel> Agent { get; set; }


        #endregion

        #region company income
        public DbSet<CompanyIncome> Income { get; set; }
        #endregion
    }
}
