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

        [HttpGet(Name = nameof(GetPositions))]
        public async Task<ActionResult<IEnumerable<Position>>> GetPositions()
        {
            return await _context.Positions.ToListAsync();
        }

        [HttpGet("{id}", Name = nameof(GetPosition))]
        public async Task<ActionResult<Position>> GetPosition(int id)
        {
            var position = await _context.Positions.FindAsync(id);

            if (position == null)
            {
                return NotFound();
            }

            return position;
        }

        [HttpPut("{id}", Name = nameof(EditPosition))]
        public async Task<IActionResult> EditPosition(int id, Position position)
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

        [HttpPost(Name = nameof(AddPosition))]
        public async Task<ActionResult<Position>> AddPosition(Position position)
        {
            _context.Positions.Add(position);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPosition", new { id = position.Id }, position);
        }

        [HttpDelete("{id}", Name = nameof(DeletePosition))]
        public async Task<IActionResult> DeletePosition(int id)
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
