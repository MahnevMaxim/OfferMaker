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
    public class HintsController : ControllerBase
    {
        private readonly APIContext _context;

        public HintsController(APIContext context)
        {
            _context = context;
        }

        [HttpGet(Name = nameof(HintsGet))]
        public async Task<ActionResult<IEnumerable<Hint>>> HintsGet()
        {
            return await _context.Hints.ToListAsync();
        }
    }
}
