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
    public class ImageGuidsController : ControllerBase
    {
        private readonly APIContext _context;

        public ImageGuidsController(APIContext context)
        {
            _context = context;
        }

        [HttpGet(Name = nameof(ImageGuidsGet))]
        public async Task<ActionResult<IEnumerable<ImageGuid>>> ImageGuidsGet()
        {
            return await _context.ImageGuids.ToListAsync();
        }
    }
}
