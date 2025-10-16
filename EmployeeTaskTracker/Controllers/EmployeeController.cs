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

        // ✅ Manager only: View all employees
        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            return await _context.Employees.ToListAsync();
        }

        // ✅ Employee: View own profile
        [Authorize(Roles = "Employee")]
        [HttpGet("my-profile")]
        public async Task<ActionResult<Employee>> GetMyProfile()
        {
            var username = User.Identity?.Name;
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.FullName == username);

            if (employee == null)
                return NotFound();

            return employee;
        }

        // ✅ Manager only: Add employee
        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployee(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEmployees), new { id = employee.EmployeeId }, employee);
        }

        // ✅ Manager only: Update employee
        [Authorize(Roles = "Manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, Employee employee)
        {
            if (id != employee.EmployeeId)
                return BadRequest();

            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
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
            return NoContent();
        }
    }
}