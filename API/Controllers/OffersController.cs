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
            return await _context.Offers.ToListAsync();
        }

        [HttpGet("/self", Name = nameof(OffersSelfGet))]
        public async Task<ActionResult<IEnumerable<Offer>>> OffersSelfGet()
        {
            User user = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name);
            return await _context.Offers.Where(o => o.OfferCreatorId == user.Id).ToListAsync();
        }

        [HttpGet("{id}", Name = nameof(OfferGet))]
        public async Task<ActionResult<Offer>> OfferGet(int id)
        {
            var offer = await _context.Offers.FindAsync(id);

            if (offer == null)
            {
                return NotFound();
            }

            return offer;
        }

        //[HttpPut("{id}", Name = nameof(OfferEdit))]
        //public async Task<IActionResult> OfferEdit(int id, Offer offer)
        //{
        //    if (id != offer.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(offer).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!OfferExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        [HttpPost(Name = nameof(OfferPost))]
        public async Task<ActionResult<Offer>> OfferPost(Offer offer)
        {
            _context.Offers.Add(offer);
            await _context.SaveChangesAsync();

            TryAddHints(offer);

            return CreatedAtAction("OfferGet", new { id = offer.Id }, offer);
        }

        [HttpDelete("{id}", Name = nameof(OfferDelete))]
        public async Task<IActionResult> OfferDelete(int id)
        {
            var offer = await _context.Offers.FindAsync(id);
            if (offer == null)
            {
                return NotFound();
            }

            _context.Offers.Remove(offer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OfferExists(int id)
        {
            return _context.Offers.Any(e => e.Id == id);
        }

        private void TryAddHints(Offer offer)
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
