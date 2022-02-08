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
    public class AdvertisingsController : ControllerBase
    {
        private readonly APIContext _context;

        public AdvertisingsController(APIContext context)
        {
            _context = context;
        }

        [HttpGet(Name = nameof(AdvertisingsGet))]
        public async Task<ActionResult<IEnumerable<Advertising>>> AdvertisingsGet()
        {
            return await _context.Advertisings.Where(b => !b.IsDeleted).ToListAsync();
        }

        [HttpGet("{id}", Name = nameof(AdvertisingGet))]
        public async Task<ActionResult<Advertising>> AdvertisingGet(int id)
        {
            var advertising = await _context.Advertisings.FindAsync(id);

            if (advertising == null)
            {
                return NotFound();
            }

            return advertising;
        }

        [HttpPost(Name = nameof(AdvertisingPost))]
        public async Task<ActionResult<Advertising>> AdvertisingPost(Advertising advertising)
        {
            _context.Advertisings.Add(advertising);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AdvertisingExists(advertising.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("AdvertisingsGet", new { id = advertising.Id }, advertising);
        }

        [HttpDelete("{id}", Name = nameof(AdvertisingDelete))]
        public async Task<IActionResult> AdvertisingDelete(int id)
        {
            var advertising = await _context.Advertisings.FindAsync(id);
            if (advertising == null)
            {
                return NotFound();
            }

            advertising.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AdvertisingExists(int id)
        {
            return _context.Advertisings.Any(e => e.Id == id);
        }
    }
}
