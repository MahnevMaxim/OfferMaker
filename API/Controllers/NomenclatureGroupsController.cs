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
    public class NomenclatureGroupsController : ControllerBase
    {
        private readonly APIContext _context;

        public NomenclatureGroupsController(APIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NomenclatureGroup>>> GetNomenclatureGroups()
        {
            return await _context.NomenclatureGroups.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NomenclatureGroup>> GetNomenclatureGroup(int id)
        {
            var nomenclatureGroup = await _context.NomenclatureGroups.FindAsync(id);

            if (nomenclatureGroup == null)
            {
                return NotFound();
            }

            return nomenclatureGroup;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutNomenclatureGroup(int id, NomenclatureGroup nomenclatureGroup)
        {
            if (id != nomenclatureGroup.Id)
            {
                return BadRequest();
            }

            _context.Entry(nomenclatureGroup).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NomenclatureGroupExists(id))
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

        [HttpPut]
        public async Task<ActionResult<NomenclatureGroup>> SaveNomenclatureGroups(IEnumerable<NomenclatureGroup> nomenclatureGroups)
        {
            foreach (var ng in nomenclatureGroups)
            {
                try
                {
                    if (ng.Id == 0)
                    {
                        await PostNomenclatureGroup(ng);
                    }
                    else
                    {
                        await PutNomenclatureGroup(ng.Id, ng);
                    }
                }
                catch (Exception ex)
                {
                    Log.Write("Исключение при попытке сохранить группу " + ng.Id, ex);
                }
            }
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<NomenclatureGroup>> PostNomenclatureGroup(NomenclatureGroup nomenclatureGroup)
        {
            _context.NomenclatureGroups.Add(nomenclatureGroup);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNomenclatureGroup", new { id = nomenclatureGroup.Id }, nomenclatureGroup);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNomenclatureGroup(int id)
        {
            var nomenclatureGroup = await _context.NomenclatureGroups.FindAsync(id);
            if (nomenclatureGroup == null)
            {
                return NotFound();
            }

            _context.NomenclatureGroups.Remove(nomenclatureGroup);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NomenclatureGroupExists(int id)
        {
            return _context.NomenclatureGroups.Any(e => e.Id == id);
        }
    }
}
