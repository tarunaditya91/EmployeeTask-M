using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EmployeeTaskTracker.Models
{
    public class TaskItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } // Pending, In Progress, Completed
        public DateTime DueDate { get; set; }
        public DateTime CreatedDate { get; set; }

        
        public int EmployeeId { get; set; }
        [JsonIgnore]
        public Employee? Employee { get; set; }

    }
}