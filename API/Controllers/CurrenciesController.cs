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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Currency>>> GetCurrencies()
        {
            return await _context.Currencies.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Currency>> GetCurrency(int id)
        {
            var currency = await _context.Currencies.FindAsync(id);

            if (currency == null)
            {
                return NotFound();
            }

            return currency;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCurrency(int id, Currency currency)
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

        [HttpPut]
        public async Task<IActionResult> PutCurrencies(IEnumerable<Currency> currencies)
        {
            int id = 0;
            try
            {
                foreach (Currency currency in currencies)
                {
                    id = currency.Id;
                    await PutCurrency(currency.Id, currency);
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

        [HttpPost]
        public async Task<ActionResult<Currency>> PostCurrency(Currency currency)
        {
            _context.Currencies.Add(currency);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCurrency", new { id = currency.Id }, currency);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCurrency(int id)
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
