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
    public class OfferTemplatesController : ControllerBase
    {
        private readonly APIContext _context;

        public OfferTemplatesController(APIContext context)
        {
            _context = context;
        }

        [HttpGet(Name = nameof(OfferTemplatesGet))]
        public async Task<ActionResult<IEnumerable<OfferTemplate>>> OfferTemplatesGet()
        {
            return await _context.OfferTemplates.ToListAsync();
        }

        [HttpGet("{id}", Name = nameof(OfferTemplateGet))]
        public async Task<ActionResult<OfferTemplate>> OfferTemplateGet(int id)
        {
            var offerTemplate = await _context.OfferTemplates.FindAsync(id);

            if (offerTemplate == null)
            {
                return NotFound();
            }

            return offerTemplate;
        }

        [HttpPut("{id}", Name = nameof(OfferTemplateEdit))]
        public async Task<IActionResult> OfferTemplateEdit(int id, OfferTemplate offerTemplate)
        {
            if (id != offerTemplate.Id)
            {
                return BadRequest();
            }

            _context.Entry(offerTemplate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OfferTemplateExists(id))
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

        [HttpPost(Name = nameof(OfferTemplatePost))]
        public async Task<ActionResult<OfferTemplate>> OfferTemplatePost(OfferTemplate offerTemplate)
        {
            _context.OfferTemplates.Add(offerTemplate);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(OfferTemplateGet), new { id = offerTemplate.Id }, offerTemplate);
        }

        [HttpDelete("{id}", Name = nameof(OfferTemplateDelete))]
        public async Task<IActionResult> OfferTemplateDelete(int id)
        {
            var offerTemplate = await _context.OfferTemplates.FindAsync(id);
            if (offerTemplate == null)
            {
                return NotFound();
            }

            _context.OfferTemplates.Remove(offerTemplate);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OfferTemplateExists(int id)
        {
            return _context.OfferTemplates.Any(e => e.Id == id);
        }
    }
}
