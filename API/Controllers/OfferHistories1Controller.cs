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
    public class OfferHistories1Controller : ControllerBase
    {
        private readonly APIContext _context;

        public OfferHistories1Controller(APIContext context)
        {
            _context = context;
        }

        // GET: api/OfferHistories1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OfferHistory>>> GetOffersHistory()
        {
            return await _context.OffersHistory.ToListAsync();
        }

        // GET: api/OfferHistories1/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OfferHistory>> GetOfferHistory(int id)
        {
            var offerHistory = await _context.OffersHistory.FindAsync(id);

            if (offerHistory == null)
            {
                return NotFound();
            }

            return offerHistory;
        }

        // PUT: api/OfferHistories1/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOfferHistory(int id, OfferHistory offerHistory)
        {
            if (id != offerHistory.Id)
            {
                return BadRequest();
            }

            _context.Entry(offerHistory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OfferHistoryExists(id))
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

        // POST: api/OfferHistories1
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OfferHistory>> PostOfferHistory(OfferHistory offerHistory)
        {
            _context.OffersHistory.Add(offerHistory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOfferHistory", new { id = offerHistory.Id }, offerHistory);
        }

        // DELETE: api/OfferHistories1/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOfferHistory(int id)
        {
            var offerHistory = await _context.OffersHistory.FindAsync(id);
            if (offerHistory == null)
            {
                return NotFound();
            }

            _context.OffersHistory.Remove(offerHistory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OfferHistoryExists(int id)
        {
            return _context.OffersHistory.Any(e => e.Id == id);
        }
    }
}
