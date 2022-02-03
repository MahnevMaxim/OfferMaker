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

        [HttpGet(Name = nameof(NomenclatureGroupsGet))]
        public async Task<ActionResult<IEnumerable<NomenclatureGroup>>> NomenclatureGroupsGet()
        {
            return await _context.NomenclatureGroups.ToListAsync();
        }

        [HttpGet("{id}", Name = nameof(NomenclatureGroupGet))]
        public async Task<ActionResult<NomenclatureGroup>> NomenclatureGroupGet(int id)
        {
            var nomenclatureGroup = await _context.NomenclatureGroups.FindAsync(id);

            if (nomenclatureGroup == null)
            {
                return NotFound();
            }

            return nomenclatureGroup;
        }

        [Authorize(Roles = "CanEditProducts,CanAll")]
        [HttpPut("{id}", Name = nameof(NomenclatureGroupEdit))]
        public async Task<IActionResult> NomenclatureGroupEdit(int id, NomenclatureGroup nomenclatureGroup)
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

        [Authorize(Roles = "CanEditProducts,CanAll")]
        [HttpPut(Name = nameof(NomenclatureGroupsSave))]
        public async Task<ActionResult<NomenclatureGroup>> NomenclatureGroupsSave(IEnumerable<NomenclatureGroup> nomenclatureGroups)
        {
            foreach (var ng in nomenclatureGroups)
            {
                try
                {
                    if (ng.Id == 0)
                    {
                        await NomenclatureGroupPost(ng);
                    }
                    else
                    {
                        await NomenclatureGroupEdit(ng.Id, ng);
                    }
                }
                catch (Exception ex)
                {
                    Log.Write("Исключение при попытке сохранить группу " + ng.Id, ex);
                }
            }
            return NoContent();
        }

        [Authorize(Roles = "CanEditProducts,CanAll")]
        [HttpPost(Name = nameof(NomenclatureGroupPost))]
        public async Task<ActionResult<NomenclatureGroup>> NomenclatureGroupPost(NomenclatureGroup nomenclatureGroup)
        {
            _context.NomenclatureGroups.Add(nomenclatureGroup);
            await _context.SaveChangesAsync();

            return CreatedAtAction("NomenclatureGroupGet", new { id = nomenclatureGroup.Id }, nomenclatureGroup);
        }

        [Authorize(Roles = "CanEditProducts,CanAll")]
        [HttpDelete("{id}", Name = nameof(NomenclatureGroupDelete))]
        public async Task<IActionResult> NomenclatureGroupDelete(int id)
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
