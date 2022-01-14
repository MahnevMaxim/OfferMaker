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
    //[Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly APIContext _context;

        public CategoriesController(APIContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            var entry = _context.Categories.Find(id);
            _context.Entry(entry).CurrentValues.SetValues(category);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
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

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> SaveCategories(IEnumerable<Category> categories)
        {
            DeleteUnusedCategories(categories);

            foreach (var category in categories)
            {
                try
                {
                    if (category.Id == 0)
                    {
                        var res = _context.Categories.Where(c => c.Guid == category.Guid).FirstOrDefault();
                        if(res==null)
                        {
                            await PostCategory(category);
                        }
                    }
                    else
                    {
                        await PutCategory(category.Id, category);
                    }
                }
                catch (Exception ex)
                {
                    Log.Write("Исключение при попытке сохранить категорию " + category.Id, ex);
                }
            }
            return NoContent();
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }

        private void DeleteUnusedCategories(IEnumerable<Category> categories)
        {
            try
            {
                var currentList = _context.Categories.ToList();
                var newListIds = categories.Select(e => e.Id).ToList();
                foreach (Category cat in currentList)
                {
                    if (!newListIds.Contains(cat.Id))
                        _context.Categories.Remove(cat);
                }
            }
            catch(Exception ex)
            {
                Log.Write("",ex);
            }
            
            _context.SaveChanges();
        }
    }
}
