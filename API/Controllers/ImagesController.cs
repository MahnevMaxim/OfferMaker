using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Shared;
using Microsoft.AspNetCore.Authorization;
using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ImagesController : ControllerBase
    {
        public static IWebHostEnvironment _environment;
        static string uploadDir;
        private readonly APIContext _context;

        public ImagesController(IWebHostEnvironment environment, APIContext context)
        {
            _context = context;
            _environment = environment;
            uploadDir = environment.WebRootPath + "/Upload/";
        }

        [HttpPost(Name = nameof(ImagePost))]
        public async Task<IActionResult> ImagePost(IFormFile file)
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
                        try
                        {
                            _context.ImageGuids.Add(new ImageGuid() { Guid = file.FileName.Split('.')[0] });
                            _context.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            //здесь может быть исключение в теории, если во время отладки до сохранения планировщик сохранит в базу уже
                            Log.Write(ex);
                        }
                        return Ok(uploadDir + file.FileName);
                    }
                }
                else
                {
                    return BadRequest("Upload failed, file lenght is 0");
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return BadRequest("Upload image exeption:\n" + ex.StackTrace);
            }
        }

        [HttpGet(Name = nameof(ImageGet))]
        public PhysicalFileResult ImageGet(string guid)
        {
            try
            {
                var files = Directory.GetFiles(uploadDir);
                var file = files.Where(f => f.Contains(guid)).FirstOrDefault();
                if (file == null)
                    return null;
                string ext = Path.GetExtension(file).Replace(".", "");
                string fileName = Path.GetFileName(file);
                return PhysicalFile(file, "image/" + ext, fileName);
            }
            catch (Exception ex)
            {
                Log.Write("ImagesController Get" + ex);
                return null;
            }
        }
    }
}
