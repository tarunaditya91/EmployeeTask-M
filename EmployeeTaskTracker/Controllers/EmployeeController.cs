using EmployeeTaskTracker.Data;
using EmployeeTaskTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeTaskTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDBcontext _context;

        public EmployeeController(ApplicationDBcontext context)
        {
            _context = context;
        }

        // ✅ Manager only: View all employees (hide PasswordHash)
        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetEmployees()
        {
            var employees = await _context.Employees
                .Select(e => new
                {
                    e.EmployeeId,
                    e.FullName,
                    e.Designation,
                    e.DateOfJoining,
                    e.Role,
                    e.ManagerId
                })
                .ToListAsync();

            return Ok(employees);
        }

        // ✅ Employee: View own profile
        [Authorize(Roles = "Employee")]
        [HttpGet("my-profile")]
        public async Task<ActionResult<object>> GetMyProfile()
        {
            var username = User.Identity?.Name;
            var employee = await _context.Employees
                .Where(e => e.FullName == username)
                .Select(e => new
                {
                    e.EmployeeId,
                    e.FullName,
                    e.Designation,
                    e.DateOfJoining,
                    e.Role,
                    e.ManagerId
                })
                .FirstOrDefaultAsync();

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        // ✅ Manager only: Update employee details (not password)
        [Authorize(Roles = "Manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, Employee updatedEmployee)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound();

            employee.FullName = updatedEmployee.FullName;
            employee.Designation = updatedEmployee.Designation;
            employee.DateOfJoining = updatedEmployee.DateOfJoining;
            employee.Role = updatedEmployee.Role;
            employee.ManagerId = updatedEmployee.ManagerId;

            await _context.SaveChangesAsync();
            return Ok("Employee updated successfully");
        }

        // ✅ Manager only: Delete employee
        [Authorize(Roles = "Manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound();

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return Ok("Employee deleted successfully");
        }
    }
}