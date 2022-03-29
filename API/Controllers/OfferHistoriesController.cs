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
    public class OfferHistoriesController : ControllerBase
    {
        private readonly APIContext _context;

        public OfferHistoriesController(APIContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "CanAll,CanSeeAllOffers")]
        [HttpGet( Name = nameof(OffersHistoryGet))]
        public async Task<ActionResult<IEnumerable<OfferHistory>>> OffersHistoryGet()
        {
            return await _context.OffersHistory.Where(o => o.IsDelete == false).Include(o => o.Banner_).ToListAsync();
        }

        [Authorize(Roles = "CanAll,CanSeeAllOffers")]
        [HttpGet("{id}", Name = nameof(OfferHistoryGet))]
        public async Task<ActionResult<OfferHistory>> OfferHistoryGet(int id)
        {
            var offerHistory = await _context.OffersHistory.FindAsync(id);

            if (offerHistory == null)
            {
                return NotFound();
            }

            return offerHistory;
        }

        [HttpGet("/api/OfferHistories/self", Name = nameof(OffersHistorySelfGet))]
        public async Task<ActionResult<IEnumerable<OfferHistory>>> OffersHistorySelfGet()
        {
            User user = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name);
            return await _context.OffersHistory.Where(o => o.OfferCreatorId == user.Id).Where(o => o.IsDelete == false).Include(o => o.Banner_).ToListAsync();
        }

        [HttpPost(Name = nameof(OfferHistoryPost))]
        public async Task<ActionResult<OfferHistory>> OfferHistoryPost(OfferHistory offer)
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

            if (offer.ParentGuid == null)
                throw new Exception("не установлен guid родительского архива");

            //проверяем, что есть в наличии родительский архив
            var parent = _context.Offers.Where(o => o.Guid == offer.ParentGuid).FirstOrDefault();
            if(parent==null)
                throw new Exception("нет родительского архива в базе");

            var maxChildId = _context.OffersHistory.Where(o => o.ParentGuid == offer.ParentGuid).Max(o => o.ChildId) ?? 0;
            offer.ChildId = ++maxChildId;
            //не факт, что ParentId установлен(при сценарии оффлайн работы не устанавливается), а если установлен, то нет гарантий
            offer.ParentId = parent.Id;
            _context.OffersHistory.Add(offer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("OfferHistoryGet", new { id = offer.Id }, offer);
        }

        [Authorize(Roles = "CanAll,CanControlArchive")]
        [HttpDelete("{id}", Name = nameof(OfferHistoryDelete))]
        public async Task<IActionResult> OfferHistoryDelete(int id)
        {
            var offer = await _context.OffersHistory.FindAsync(id);
            if (offer == null)
            {
                return NotFound();
            }

            offer.IsDelete = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
