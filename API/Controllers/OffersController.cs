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
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OffersController : ControllerBase
    {
        private readonly APIContext _context;

        public OffersController(APIContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "CanAll,CanSeeAllOffers")]
        [HttpGet(Name = nameof(OffersGet))]
        public async Task<ActionResult<IEnumerable<Offer>>> OffersGet()
        {
            return await _context.Offers.Where(o => o.IsDelete == false).Include(o => o.Banner_).ToListAsync();
        }

        [HttpGet("/api/Offers/self", Name = nameof(OffersSelfGet))]
        public async Task<ActionResult<IEnumerable<Offer>>> OffersSelfGet()
        {
            User user = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name);
            return await _context.Offers.Where(o => o.OfferCreatorId == user.Id).Where(o => o.IsDelete == false).Include(o => o.Banner_).ToListAsync();
        }

        [Authorize(Roles = "CanAll,CanSeeAllOffers")]
        [HttpGet("{id}", Name = nameof(OfferGet))]
        public async Task<ActionResult<Offer>> OfferGet(int id)
        {
            var offer = await _context.Offers.FindAsync(id);

            if (offer == null)
            {
                return NotFound();
            }
            else if (offer.IsDelete)
            {
                return NotFound();
            }

            return offer;
        }

        [HttpPost(Name = nameof(OfferPost))]
        public async Task<ActionResult<Offer>> OfferPost(Offer offer)
        {
            if (offer.Banner_ != null)
            {
                Banner banner = _context.Banners.Where(b => b.Guid == offer.Banner_.Guid).FirstOrDefault();
                if (banner == null)
                {
                    banner = _context.Banners.First();
                    if (banner == null)
                    {
                        return BadRequest("banner is null, upload banner to system");
                    }
                    else
                    {
                        offer.Banner_ = banner;
                    }
                }
                else
                {
                    offer.Banner_ = banner;
                }
            }
            _context.Offers.Add(offer);
            await _context.SaveChangesAsync();
            TryAddHints(offer, _context);
            return CreatedAtAction("OfferGet", new { id = offer.Id }, offer);
        }

        [Authorize(Roles = "CanAll,CanControlArchive")]
        [HttpDelete("{id}", Name = nameof(OfferDelete))]
        public async Task<IActionResult> OfferDelete(int id)
        {
            var offer = await _context.Offers.FindAsync(id);
            if (offer == null)
            {
                return NotFound();
            }

            offer.IsDelete = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OfferExists(int id)
        {
            return _context.Offers.Any(e => e.Id == id);
        }

        public static void TryAddHints(Offer offer, APIContext _context)
        {
            var newHintsStrings = offer.OfferGroups.Select(o => o.GroupTitle);
            var oldHintsStrings = _context.Hints.Select(h => h.HintString);
            var result = newHintsStrings.Except(oldHintsStrings);
            List<Hint> hints = new List<Hint>();
            result.ToList().ForEach(h => hints.Add(new Hint() { HintString = h.ToUpper() }));
            _context.Hints.AddRange(hints);
            _context.SaveChanges();
        }
    }
}
