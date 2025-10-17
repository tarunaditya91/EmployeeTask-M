using EmployeeTaskTracker.Data;
using EmployeeTaskTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeTaskTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskItemController : ControllerBase
    {
        private readonly ApplicationDBcontext _context;

        public TaskItemController(ApplicationDBcontext context)
        {
            _context = context;
        }

        // ✅ Manager only: View all tasks with employee details
        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetTaskItems()
        {
            var tasks = await _context.TaskItems
                .Include(t => t.Employee)
                .Select(t => new
                {
                    t.TaskId,
                    t.Title,
                    t.Description,
                    t.Status,
                    t.DueDate,
                    t.CreatedDate,
                    EmployeeName = t.Employee.FullName
                })
                .ToListAsync();

            return Ok(tasks);
        }

        // ✅ Employee: View only their tasks
        [Authorize(Roles = "Employee")]
        [HttpGet("my-tasks")]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetMyTasks()
        {
            var username = User.Identity?.Name;
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.FullName == username);

            if (employee == null) return Unauthorized();

            var tasks = await _context.TaskItems
                .Where(t => t.EmployeeId == employee.EmployeeId)
                .ToListAsync();

            return Ok(tasks);
        }

        // ✅ Manager only: Assign a new task to an employee
        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<ActionResult<TaskItem>> CreateTaskItem(TaskItem taskItem)
        {
            // Validate employee exists
            var employeeExists = await _context.Employees.AnyAsync(e => e.EmployeeId == taskItem.EmployeeId);
            if (!employeeExists)
                return BadRequest("Invalid EmployeeId");

            taskItem.CreatedDate = DateTime.UtcNow;
            _context.TaskItems.Add(taskItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTaskItems), new { id = taskItem.TaskId }, taskItem);
        }

        // ✅ Employee: Update only their own task status
        [Authorize(Roles = "Employee")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] string status)
        {
            var username = User.Identity?.Name;
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.FullName == username);

            if (employee == null) return Unauthorized();

            var taskItem = await _context.TaskItems.FirstOrDefaultAsync(t => t.TaskId == id && t.EmployeeId == employee.EmployeeId);
            if (taskItem == null) return NotFound("Task not found or not assigned to you");

            taskItem.Status = status;
            await _context.SaveChangesAsync();

            return Ok("Task status updated successfully");
        }

        // ✅ Manager only: Delete a task
        [Authorize(Roles = "Manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskItem(int id)
        {
            var taskItem = await _context.TaskItems.FindAsync(id);
            if (taskItem == null) return NotFound();

            _context.TaskItems.Remove(taskItem);
            await _context.SaveChangesAsync();

            return Ok("Task deleted successfully");
        }
    }
}