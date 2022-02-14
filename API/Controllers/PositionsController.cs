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
    public class PositionsController : ControllerBase
    {
        private readonly APIContext _context;

        public PositionsController(APIContext context)
        {
            _context = context;
        }

        [HttpGet(Name = nameof(PositionsGet))]
        public async Task<ActionResult<IEnumerable<Position>>> PositionsGet()
        {
            return await _context.Positions.ToListAsync();
        }

        [HttpGet("{id}", Name = nameof(PositionGet))]
        public async Task<ActionResult<Position>> PositionGet(int id)
        {
            var position = await _context.Positions.FindAsync(id);

            if (position == null)
            {
                return NotFound();
            }

            return position;
        }

        [Authorize(Roles = "CanControlUsers,CanAll")]
        [HttpPut("{id}", Name = nameof(PositionEdit))]
        public async Task<IActionResult> PositionEdit(int id, Position position)
        {
            if (id != position.Id)
            {
                return BadRequest();
            }

            //проверяем, изменились ли права, если изменились, то отзываем токены
            var permissions = _context.Positions.AsNoTracking().First(p => p.Id == id).Permissions;
            var except1 = permissions.Except(position.Permissions);
            var except2 = position.Permissions.Except(permissions);

            _context.Entry(position).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                if (except1.Count() != 0 || except2.Count() != 0)
                {
                    await _context.Users.Include(u => u.Account).Include(u => u.Position)
                       .Where(u => u.Position != null && u.Position.Id == id && u.Account != null).ForEachAsync(u =>
                       {
                           if (u.Account != null)
                               u.Account.IsTokenActive = false;
                       });
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PositionExists(id))
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

        [Authorize(Roles = "CanControlUsers,CanAll")]
        [HttpPut(Name = nameof(PositionsEdit))]
        public async Task<ActionResult<IEnumerable<Position>>> PositionsEdit(IEnumerable<Position> positions)
        {
            List<Position> res = new List<Position>();
            foreach (var pos in positions)
            {
                try
                {
                    await PositionEdit(pos.Id, pos);
                    res.Add(pos);
                }
                catch (Exception ex)
                {
                    Log.Write("Исключение при попытке сохранить должность " + pos.Id, ex);
                }
            }
            return new ActionResult<IEnumerable<Position>>(res);
        }

        [Authorize(Roles = "CanControlUsers,CanAll")]
        [HttpPost(Name = nameof(PositionPost))]
        public async Task<ActionResult<Position>> PositionPost(Position position)
        {
            if (_context.Positions.Where(p => p.PositionName == position.PositionName).Count() > 0)
            {
                return StatusCode(409);
            }
            _context.Positions.Add(position);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PositionGet", new { id = position.Id }, position);
        }

        [Authorize(Roles = "CanControlUsers,CanAll")]
        [HttpDelete("{id}", Name = nameof(PositionDelete))]
        public async Task<IActionResult> PositionDelete(int id)
        {
            var position = await _context.Positions.FindAsync(id);
            if (position == null)
            {
                return NotFound();
            }

            if (position.Users != null)
            {
                string users = null;
                position.Users.ToList().ForEach(u => users += u.FirstName + " " + u.LastName + ", ");
                users = users.Remove(users.Length - 2);
                return BadRequest("Нельзя удалить должность, пока эта должность назначена пользователям: " + users);
            }

            _context.Positions.Remove(position);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PositionExists(int id)
        {
            return _context.Positions.Any(e => e.Id == id);
        }
    }
}
