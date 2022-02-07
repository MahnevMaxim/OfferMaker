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
        private readonly string defaultPassword = "88888888";

        public UsersController(APIContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "CanControlUsers,CanAll,CanUseManager,CanSeeAllOffers")]
        [HttpGet(Name = nameof(UsersGet))]
        public async Task<ActionResult<IEnumerable<User>>> UsersGet()
        {
            var u = User.Identity.Name;
            var res = Request.Headers;
            var users = await _context.Users.Include(u => u.Position).ToListAsync();
            return users;
        }

        [Authorize(Roles = "CanControlUsers,CanAll,CanUseManager,CanSeeAllOffers")]
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

        [Authorize(Roles = "CanControlUsers,CanAll")]
        [HttpPut("{id}", Name = nameof(UserEdit))]
        public async Task<IActionResult> UserEdit(int id, User user)
        {
            User user_ = _context.Users.Include(u => u.Account).Include(u => u.Position).FirstOrDefault(x => x.Email == user.Email);

            if (id != user.Id)
            {
                return BadRequest();
            }

            user.Position = _context.Positions.Where(p => p.Id == user.Position.Id).FirstOrDefault();
            //если должность была изменена, то отзываем токен
            var posId = user_.Position?.Id;
            if (posId != user.Position.Id)
            {
                user_.Position = user.Position;
                user_.Account.IsTokenActive = false;
                user.Account = user_.Account;
            }

            _context.Entry(user_).CurrentValues.SetValues(user);

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

        [HttpPut("{id}/self", Name = nameof(UserSelfEdit))]
        public async Task<IActionResult> UserSelfEdit(int id, User user, string password)
        {
            var uName = User.Identity.Name;
            if (uName == user.Email)
            {
                User user_ = _context.Users.Include(u => u.Account).FirstOrDefault(u => u.Email == user.Email);

                var ph = new PasswordHasher();
                var isCurrentHashValid = ph.VerifyHashedPassword(user_.Account.Password, password);
                if (isCurrentHashValid != Microsoft.AspNet.Identity.PasswordVerificationResult.Success)
                {
                    return BadRequest(new { errorText = "Invalid username or password." });
                }

                if (id != user.Id)
                {
                    return BadRequest();
                }

                _context.Entry(user_).CurrentValues.SetValues(user);

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

        [Authorize(Roles = "CanControlUsers,CanAll")]
        [HttpPut("password", Name = nameof(UserChangePassword))]
        public async Task<IActionResult> UserChangePassword(User user, string password)
        {
            User user_ = _context.Users.Include(u => u.Account).Where(u => u.Email == user.Email).FirstOrDefault();

            if (user_ != null)
            {
                try
                {
                    var ph = new PasswordHasher();
                    user_.Account.Password = ph.HashPassword(password);
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

        [HttpPut("self_password", Name = nameof(UserSelfChangePassword))]
        public async Task<IActionResult> UserSelfChangePassword(User user, string oldPassword)
        {
            var uName = User.Identity.Name;
            User user_ = _context.Users.Include(u => u.Account).Where(u => u.Email == user.Email).FirstOrDefault();

            //проверяем старый пароль, если пользователь редактирует себя
            //вот хуй знает, почему для себя надо пароль, а для сброса всех остальных пользователей не надо
            if (uName == user.Email)
            {
                var ph = new PasswordHasher();
                var isCurrentHashValid = ph.VerifyHashedPassword(user_.Account.Password, oldPassword);
                if (isCurrentHashValid != Microsoft.AspNet.Identity.PasswordVerificationResult.Success)
                {
                    return BadRequest(new { errorText = "Invalid username or password." });
                }
            }
            else
            {
                return BadRequest(new { errorText = "You can't." });
            }

            if (user_ != null)
            {
                try
                {
                    var ph = new PasswordHasher();
                    user_.Account.Password = ph.HashPassword(oldPassword);
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

        [Authorize(Roles = "CanControlUsers,CanAll")]
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

        [Authorize(Roles = "CanControlUsers,CanAll")]
        [HttpPost(Name = nameof(UserCreate))]
        public async Task<ActionResult<User>> UserCreate(User user)
        {
            string positionName = user.Position.PositionName;
            var position = _context.Positions.Where(p => p.PositionName == positionName).First();
            user.Position = position;

            string password = user.Account.Password;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            if (string.IsNullOrWhiteSpace(password))
                password = defaultPassword;
            await UserChangePassword(user, password);

            return CreatedAtAction(nameof(UserGet), new { id = user.Id }, user);
        }

        [Authorize(Roles = "CanControlUsers,CanAll")]
        [HttpDelete("{id}", Name = nameof(UserDelete))]
        public async Task<IActionResult> UserDelete(int id)
        {
            var uName = User.Identity.Name;
            User user_ = _context.Users.Where(u => u.Id == id).FirstOrDefault();

            if (uName == user_.Email)
            {
                {
                    return BadRequest(new { errorText = "Can't delete self." });
                }
            }

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
