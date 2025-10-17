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

            
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.Employee)
                .WithMany(e => e.TaskItems)
                .HasForeignKey(t => t.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}