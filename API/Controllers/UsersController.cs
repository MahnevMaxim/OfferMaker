using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using Shared;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly APIContext _context;

        public UsersController(APIContext context)
        {
            _context = context;
        }

        [HttpGet(Name = nameof(UsersGet))]
        public async Task<ActionResult<IEnumerable<User>>> UsersGet()
        {
            var u = User.Identity.Name;
            var res = Request.Headers;
            return await _context.Users.ToListAsync();
        }

        [HttpGet("{id}", Name = nameof(UserGet))]
        public async Task<ActionResult<User>> UserGet(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPut("{id}", Name = nameof(UserEdit))]
        public async Task<IActionResult> UserEdit(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost(Name = nameof(UserAdd))]
        public async Task<ActionResult<User>> UserAdd(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        [HttpDelete("{id}", Name = nameof(UserDelete))]
        public async Task<IActionResult> UserDelete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
