using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.API.Data;
using ExpenseTracker.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ExpenseTracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return Ok(await _context.Users.ToListAsync());
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound(new { message = "User not found." });

            return Ok(user);
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Optional: check for duplicate email
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                return BadRequest(new { message = "Email already exists." });

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, [FromBody] User user)
        {
            if (id != user.Id)
                return BadRequest(new { message = "User ID mismatch." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
                return NotFound(new { message = "User not found." });

            // Optional: check for duplicate email on update (exclude current user)
            if (await _context.Users.AnyAsync(u => u.Email == user.Email && u.Id != id))
                return BadRequest(new { message = "Email already exists." });

            // Update properties
            existingUser.Name = user.Name;
            existingUser.Email = user.Email;

            await _context.SaveChangesAsync();

            return Ok(existingUser);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            // Load user including related expenses
            var user = await _context.Users
                .Include(u => u.Expenses)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound(new { message = "User not found." });

            // Delete all related expenses first
            if (user.Expenses != null && user.Expenses.Count > 0)
            {
                _context.Expenses.RemoveRange(user.Expenses);
            }

            // Then delete the user
            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // New: GET: api/users/{userId}/expenses
        [HttpGet("{userId}/expenses")]
        public async Task<ActionResult<IEnumerable<Expense>>> GetUserExpenses(int userId)
        {
            var expenses = await _context.Expenses
                                .Where(e => e.UserId == userId)
                                .ToListAsync();

            return Ok(expenses);
        }

        // New: POST: api/users/{userId}/expenses
        [HttpPost("{userId}/expenses")]
        public async Task<ActionResult<Expense>> AddExpenseForUser(int userId, [FromBody] Expense expense)
        {
            if (userId != expense.UserId)
            {
                return BadRequest(new { message = "User ID mismatch in request." });
            }

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserExpenses), new { userId = userId }, expense);
        }
    }
}
