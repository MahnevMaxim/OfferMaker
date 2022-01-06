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
using Microsoft.AspNetCore.Identity;
using System.Web.Helpers;
using Microsoft.AspNet.Identity;

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
            var users = await _context.Users.Include(u => u.Position).ToListAsync();
            users.ForEach(u => u.Pwd = null);
            return users;
        }

        [HttpGet("{id}", Name = nameof(UserGet))]
        public async Task<ActionResult<User>> UserGet(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            user.Pwd = null;
            return user;
        }

        [HttpPut("{id}", Name = nameof(UserEdit))]
        public async Task<IActionResult> UserEdit(int id, User user)
        {
            var uName = User.Identity.Name;
            var claimCanAll = User.Claims.ToList().Where(c=>c.Value==Permissions.CanAll.ToString() || c.Value == Permissions.CanEditUsers.ToString()).FirstOrDefault();
            if(claimCanAll!=null || uName==user.Email)
            {
                User user_ = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == user.Email);

                //проверяем пароль, если пользователь редактирует себя
                if (uName == user.Email)
                {
                    var ph = new PasswordHasher();
                    var isCurrentHashValid = ph.VerifyHashedPassword(user_.Pwd, user.Pwd);
                    if (isCurrentHashValid != Microsoft.AspNet.Identity.PasswordVerificationResult.Success)
                    {
                        return BadRequest(new { errorText = "Invalid username or password." });
                    }
                }

                if (id != user.Id)
                {
                    return BadRequest();
                }

                if (user.Position != null)
                {
                    user.Position = _context.Positions.Where(p => p.Id == user.Position.Id).FirstOrDefault();
                }

                user.Pwd = user_.Pwd;
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
            return BadRequest();
        }

        [HttpPut("password", Name = nameof(UserChangePassword))]
        public async Task<IActionResult> UserChangePassword(User user, string oldPassword)
        {
            var uName = User.Identity.Name;
            var claimCanEdit = User.Claims.ToList().Where(c => c.Value == Permissions.CanAll.ToString() || c.Value == Permissions.CanEditUsers.ToString()).FirstOrDefault();
            User user_ = _context.Users.Where(u => u.Email == user.Email).FirstOrDefault();

            //проверяем старый пароль, если пользователь редактирует себя
            //вот хуй знает, почему для себя надо пароль, а для сброса всех остальных пользователей не надо
            if (uName == user.Email)
            {
                var ph = new PasswordHasher();
                var isCurrentHashValid = ph.VerifyHashedPassword(user_.Pwd, oldPassword);
                if (isCurrentHashValid != Microsoft.AspNet.Identity.PasswordVerificationResult.Success)
                {
                    return BadRequest(new { errorText = "Invalid username or password." });
                }
            }

            if (user_ != null)
            {
                try
                {
                    var ph = new PasswordHasher();
                    user_.Pwd = ph.HashPassword(user.Pwd);
                    await _context.SaveChangesAsync();
                    return NoContent();
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                    throw;
                }
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPut(Name = nameof(UsersEdit))]
        public async Task<ActionResult<IEnumerable<User>>> UsersEdit(IEnumerable<User> users)
        {
            List<User> res = new List<User>();
            foreach (var user in users)
            {
                try
                {
                    await UserEdit(user.Id, user);
                    res.Add(user);
                }
                catch (Exception ex)
                {
                    Log.Write("Исключение при попытке сохранить пользователя " + user.Id, ex);
                }
            }
            return new ActionResult<IEnumerable<User>>(res);
        }

        [HttpPost(Name = nameof(UserCreate))]
        public async Task<ActionResult<User>> UserCreate(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(UserGet), new { id = user.Id }, user);
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
