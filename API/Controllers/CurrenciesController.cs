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
    public class CurrenciesController : ControllerBase
    {
        private readonly APIContext _context;

        public CurrenciesController(APIContext context)
        {
            _context = context;
        }

        [HttpGet(Name = nameof(CurrenciesGet))]
        public async Task<ActionResult<IEnumerable<Currency>>> CurrenciesGet()
        {
            return await _context.Currencies.ToListAsync();
        }

        [HttpGet("{id}", Name = nameof(CurrencyGet))]
        public async Task<ActionResult<Currency>> CurrencyGet(int id)
        {
            var currency = await _context.Currencies.FindAsync(id);

            if (currency == null)
            {
                return NotFound();
            }

            return currency;
        }

        [Authorize(Roles = "CanEditCurrencies,CanAll")]
        [HttpPut("{id}", Name = nameof(CurrencyEdit))]
        public async Task<IActionResult> CurrencyEdit(int id, Currency currency)
        {
            if (id != currency.Id)
            {
                return BadRequest();
            }

            //это для планировщика
            if (id == 0)
            {
                var res = _context.Currencies.Where(c => c.CharCode == currency.CharCode).FirstOrDefault();
                if (res != null)
                {
                    res.Rate = currency.Rate;
                    res.RateDatetime = currency.RateDatetime;
                    _context.Currencies.Where(c => c.CharCode == "RUB").FirstOrDefault().RateDatetime = currency.RateDatetime;
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CurrencyExists(currency.CharCode))
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

            _context.Entry(currency).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CurrencyExists(id))
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

        [Authorize(Roles = "CanEditCurrencies,CanAll")]
        [HttpPut(Name = nameof(CurrenciesEdit))]
        public async Task<IActionResult> CurrenciesEdit(IEnumerable<Currency> currencies)
        {
            int id = 0;
            try
            {
                foreach (Currency currency in currencies)
                {
                    id = currency.Id;
                    await CurrencyEdit(currency.Id, currency);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CurrencyExists(id))
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

        [Authorize(Roles = "CanEditCurrencies,CanAll")]
        [HttpPost(Name = nameof(CurrencyPost))]
        public async Task<ActionResult<Currency>> CurrencyPost(Currency currency)
        {
            _context.Currencies.Add(currency);
            await _context.SaveChangesAsync();

            return CreatedAtAction("CurrencyGet", new { id = currency.Id }, currency);
        }

        [Authorize(Roles = "CanEditCurrencies,CanAll")]
        [HttpDelete("{id}", Name = nameof(CurrencyDelete))]
        public async Task<IActionResult> CurrencyDelete(int id)
        {
            var currency = await _context.Currencies.FindAsync(id);
            if (currency == null)
            {
                return NotFound();
            }

            _context.Currencies.Remove(currency);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CurrencyExists(int id)
        {
            return _context.Currencies.Any(e => e.Id == id);
        }

        private bool CurrencyExists(string charCode)
        {
            return _context.Currencies.Any(e => e.CharCode == charCode);
        }
    }
}
