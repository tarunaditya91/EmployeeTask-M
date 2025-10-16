using EmployeeTaskTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeTaskTracker.Data
{
    public class ApplicationDBcontext : DbContext
    {
        public ApplicationDBcontext(DbContextOptions<ApplicationDBcontext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Seed Manager and Employee users
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    EmployeeId = 1,
                    FullName = "manager1",
                    Designation = "Manager",
                    DateOfJoining = new DateTime(2025, 10, 16),
                    Role = "Manager",
                    Password = "AQAAAAEAACcQAAAAEJz9k3v9+u5uT1r6zjvYJpQh5rJ8k1vQ=="
                },
                new Employee
                {
                    EmployeeId = 2,
                    FullName = "employee1",
                    Designation = "Developer",
                    DateOfJoining = new DateTime(2025, 10, 16),
                    Role = "Employee",
                    Password = "AQAAAAEAACcQAAAAEJz9k3v9+u5uT1r6zjvYJpQh5rJ8k1vQ=="
                },
                new Employee
                {
                    EmployeeId = 3,
                    FullName = "employee2",
                    Designation = "Tester",
                    DateOfJoining = new DateTime(2025, 10, 16),
                    Role = "Employee",
                    Password = "AQAAAAEAACcQAAAAEJz9k3v9+u5uT1r6zjvYJpQh5rJ8k1vQ=="
                }
            );
        }
    }
}