using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace InterviewManagement.Models
{
    public class InterviewManagementContext : DbContext
    {
        public InterviewManagementContext() { }

        public InterviewManagementContext(DbContextOptions<InterviewManagementContext> options) : base(options) { 
        }

        public virtual DbSet<Benefit> Benefit { get; set; } = null!;
        public virtual DbSet<Candidate> Candidate { get; set; } = null!;

        public virtual DbSet<Contract> Contract { get; set; } = null!;
        public virtual DbSet<Department> Department { get; set; } = null!;

        public virtual DbSet<Employee> Employee { get; set; } = null!;

        public virtual DbSet<Job> Job { get; set; } = null!;

        public virtual DbSet<Level> Level { get; set; } = null!;
        public virtual DbSet<HighestLevel> HighestLevel { get; set; }
        public virtual DbSet<Offer> Offer { get; set; } = null!;
        public virtual DbSet<Position> Position { get; set; } = null!;

        public virtual DbSet<Role> Role { get; set; } = null!;

        public virtual DbSet<Schedule> Schedule { get; set; } = null!;

        public virtual DbSet<Skill> Skill { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("ConnectionName");
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(config);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình bảng Employee
            modelBuilder.Entity<Employee>().ToTable("Employee");
            modelBuilder.Entity<Candidate>().ToTable("Candidate");
        }
    }
}
