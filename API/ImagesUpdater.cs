using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Xml.Linq;
using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Shared;
using System.Globalization;
using System.IO;
using Quartz;

namespace API
{
    public class ImagesUpdater
    {

        internal static void Update()
        {
            try
            {
                string uploadDir = Directory.GetCurrentDirectory() + "/wwwroot/Upload/";
#if DEBUG
                string con = Startup.connectionString;
#else
                string con = Config.DbConnectionString;
#endif
                DbContextOptions<APIContext> dbContextOptions = new DbContextOptionsBuilder<APIContext>()
                    .UseSqlServer(con)
                    .Options;
                APIContext context = new APIContext(dbContextOptions);

                var files = Directory.GetFiles(uploadDir).ToList();
                HashSet<string> filesSet = new HashSet<string>();
                files.ForEach(f => filesSet.Add(Path.GetFileName(f).Split('.')[0]));
                var oldFiles = context.ImageGuids.Select(i=>i.Guid);
                filesSet.SymmetricExceptWith(oldFiles);
                List<ImageGuid> newImages = new List<ImageGuid>();
                foreach (string guid in filesSet)
                {
                    newImages.Add(new ImageGuid() { Guid= guid });
                }
                context.ImageGuids.AddRange(newImages);
                context.SaveChanges();
            }
            catch(Exception ex)
            {
                Log.Write(ex);
            }
        }
    }
}
