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
    public class NomenclaturesController : ControllerBase
    {
        private readonly APIContext _context;

        public NomenclaturesController(APIContext context)
        {
            _context = context;
        }

        [HttpGet(Name = nameof(NomenclaturesGet))]
        public async Task<ActionResult<IEnumerable<Nomenclature>>> NomenclaturesGet()
        {
            return await _context.Nomenclatures.Where(n => !n.IsDelete).ToListAsync();
        }

        [HttpGet("{id}", Name = nameof(NomenclatureGet))]
        public async Task<ActionResult<Nomenclature>> NomenclatureGet(int id)
        {
            var nomenclature = await _context.Nomenclatures.FindAsync(id);

            if (nomenclature == null || nomenclature.IsDelete)
            {
                return NotFound();
            }

            return nomenclature;
        }

        [Authorize(Roles = "CanEditProducts,CanAll")]
        [HttpPut("{id}", Name = nameof(NomenclatureEdit))]
        public async Task<IActionResult> NomenclatureEdit(int id, Nomenclature nomenclature)
        {
            if (id != nomenclature.Id)
            {
                return BadRequest();
            }

            _context.Entry(nomenclature).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NomenclatureExists(id))
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
        [HttpPut(Name = nameof(NomenclaturesEdit))]
        public async Task<ActionResult<Nomenclature>> NomenclaturesEdit(IEnumerable<Nomenclature> noms)
        {
            foreach (var nom in noms)
            {
                try
                {
                    if (nom.Id == 0)
                    {
                        await NomenclaturePost(nom);
                    }
                    else
                    {
                        await NomenclatureEdit(nom.Id, nom);
                    }
                }
                catch (Exception ex)
                {
                    Log.Write("Исключение при попытке сохранить номенклатуру " + nom.Id, ex);
                }
            }
            return NoContent();
        }

        [Authorize(Roles = "CanEditProducts,CanAll")]
        [HttpPost(Name = nameof(NomenclaturePost))]
        public async Task<ActionResult<Nomenclature>> NomenclaturePost(Nomenclature nomenclature)
        {
            try
            {
                _context.Nomenclatures.Add(nomenclature);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }

            return CreatedAtAction("NomenclaturesGet", new { id = nomenclature.Id }, nomenclature);
        }

        [Authorize(Roles = "CanEditProducts,CanAll")]
        [HttpDelete("{id}", Name = nameof(DeleteNomenclature))]
        public async Task<IActionResult> DeleteNomenclature(int id)
        {
            var nomenclature = await _context.Nomenclatures.FindAsync(id);
            if (nomenclature == null)
            {
                return NotFound();
            }

            //_context.Nomenclatures.Remove(nomenclature);
            nomenclature.IsDelete = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NomenclatureExists(int id)
        {
            return _context.Nomenclatures.Any(e => e.Id == id);
        }
    }
}
