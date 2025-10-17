using System.Text.Json.Serialization;

namespace EmployeeTaskTracker.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Designation { get; set; }
        public DateTime DateOfJoining { get; set; }

        public string Role { get; set; } // "Manager" or "Employee"

        public string PasswordHash { get; set; }

        public int? ManagerId { get; set; }

        [JsonIgnore]
        public ICollection<TaskItem>? TaskItems { get; set; }
    }
}