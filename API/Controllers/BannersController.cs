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
    public class BannersController : ControllerBase
    {
        private readonly APIContext _context;

        public BannersController(APIContext context)
        {
            _context = context;
        }

        [HttpGet(Name = nameof(BannersGet))]
        public async Task<ActionResult<IEnumerable<Banner>>> BannersGet()
        {
            return await _context.Banners.ToListAsync();
        }

        [HttpGet("{id}", Name = nameof(BannerGet))]
        public async Task<ActionResult<Banner>> BannerGet(int id)
        {
            var banner = await _context.Banners.FindAsync(id);

            if (banner == null)
            {
                return NotFound();
            }

            return banner;
        }

        [HttpPost(Name = nameof(BannerPost))]
        public async Task<ActionResult<Banner>> BannerPost(Banner banner)
        {
            _context.Banners.Add(banner);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BannerExists(banner.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("BannerGet", new { id = banner.Id }, banner);
        }

        [HttpDelete("{id}", Name = nameof(BannerDelete))]
        public async Task<IActionResult> BannerDelete(int id)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
            {
                return NotFound();
            }

            banner.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BannerExists(int id)
        {
            return _context.Banners.Any(e => e.Id == id);
        }
    }
}
