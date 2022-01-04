using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using Shared;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        [HttpPut("{id}", Name = nameof(PositionEdit))]
        public async Task<IActionResult> PositionEdit(int id, Position position)
        {
            if (id != position.Id)
            {
                return BadRequest();
            }

            _context.Entry(position).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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

        [HttpPut(Name = nameof(PositionsSave))]
        public async Task<ActionResult<IEnumerable<Position>>> PositionsSave(IEnumerable<Position> positions)
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

        [HttpPost(Name = nameof(PositionPost))]
        public async Task<ActionResult<Position>> PositionPost(Position position)
        {
            if (_context.Positions.Where(p=>p.PositionName==position.PositionName).Count()>0)
            {
                return StatusCode(409);
            }
            _context.Positions.Add(position);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PositionGet", new { id = position.Id }, position);
        }

        [HttpDelete("{id}", Name = nameof(PositionDelete))]
        public async Task<IActionResult> PositionDelete(int id)
        {
            var position = await _context.Positions.FindAsync(id);
            if (position == null)
            {
                return NotFound();
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
