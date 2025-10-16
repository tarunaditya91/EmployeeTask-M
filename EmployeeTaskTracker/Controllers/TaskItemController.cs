using EmployeeTaskTracker.Data;
using EmployeeTaskTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        // ✅ Manager only: View all tasks
        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTaskItems()
        {
            return await _context.TaskItems.Include(t => t.Employee).ToListAsync();
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

            return tasks;
        }

        // ✅ Manager only: Assign task
        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<ActionResult<TaskItem>> CreateTaskItem(TaskItem taskItem)
        {
            taskItem.CreatedDate = DateTime.UtcNow;
            _context.TaskItems.Add(taskItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTaskItems), new { id = taskItem.TaskId }, taskItem);
        }

        // ✅ Employee: Update only status
        [Authorize(Roles = "Employee")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] string status)
        {
            var taskItem = await _context.TaskItems.FindAsync(id);
            if (taskItem == null) return NotFound();

            taskItem.Status = status;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ✅ Manager only: Delete task
        [Authorize(Roles = "Manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskItem(int id)
        {
            var taskItem = await _context.TaskItems.FindAsync(id);
            if (taskItem == null) return NotFound();

            _context.TaskItems.Remove(taskItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}