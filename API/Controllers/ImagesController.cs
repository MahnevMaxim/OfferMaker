using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Shared;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        public static IWebHostEnvironment _environment;
        static string uploadDir;

        public ImagesController(IWebHostEnvironment environment)
        {
            _environment = environment;
            uploadDir = environment.WebRootPath + "/Upload/";
        }

        [HttpPost]
        public async Task<string> Post(IFormFile file)
        {
            try
            {
                if (file.Length > 0)
                {
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    using (FileStream fileStream = System.IO.File.Create(uploadDir + file.FileName))
                    {
                        await file.CopyToAsync(fileStream);
                        fileStream.Flush();
                        return uploadDir + file.FileName;
                    }
                }
                else
                {
                    return "Failed";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpGet]
        public PhysicalFileResult Get(string guid)
        {
            try
            {
                var files = Directory.GetFiles(uploadDir);
                var file = files.Where(f => f.Contains(guid)).FirstOrDefault();
                string ext = Path.GetExtension(file).Replace(".", "");
                string fileName = Path.GetFileName(file);
                return PhysicalFile(file, "image/" + ext, fileName);
            }
            catch(Exception ex)
            {
                Log.Write("ImagesController Get" + ex);
                return null;
            }
        }
    }
}
