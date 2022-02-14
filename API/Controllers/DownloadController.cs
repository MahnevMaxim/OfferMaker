using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class DownloadController : ControllerBase
    {
        public static IWebHostEnvironment _environment;
        static string uploadDir;

        public DownloadController(IWebHostEnvironment environment)
        {
            _environment = environment;
            uploadDir = environment.WebRootPath + "/Downloads/";
        }

        [HttpGet(Name = nameof(FileGet))]
        public PhysicalFileResult FileGet(string fileName)
        {
            try
            {
                var files = Directory.GetFiles(uploadDir);
                var file = files.Where(f => f.Contains(fileName)).FirstOrDefault();
                if (file == null)
                    return null;
                return PhysicalFile(file, "application/zip", "OfferMaker.zip");
            }
            catch (Exception ex)
            {
                Log.Write("in FileGet()\n" + ex);
                return null;
            }
        }
    }
}
