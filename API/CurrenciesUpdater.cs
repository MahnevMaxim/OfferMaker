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

namespace API
{
    public class CurrenciesUpdater
    {
        /// <summary>
        /// Обновление валют на сервере.
        /// </summary>
        async public static void Update()
        {
            try
            {
                WebClient client = new WebClient();
                var xml = client.DownloadString("https://www.cbr-xml-daily.ru/daily.xml");
                XDocument xdoc = XDocument.Parse(xml);
                var el = xdoc.Element("ValCurs").Elements("Valute").ToList();
#if DEBUG
                string con = "Server=(localdb)\\mssqllocaldb;Database=APIContext;Trusted_Connection=True;MultipleActiveResultSets=true";
#else
                                string con = Config.DbConnectionString;
#endif
                DbContextOptions<APIContext> dbContextOptions = new DbContextOptionsBuilder<APIContext>()
                    .UseSqlServer(con)
                    .Options;
                APIContext context = new APIContext(dbContextOptions);
                var controller = new CurrenciesController(context);

                var date = xdoc.Element("ValCurs").Attribute("Date").Value;
                DateTime rateDatetime = DateTime.ParseExact(date, "dd.MM.yyyy", null);
                foreach (var item in el)
                {
                    var code = item.Element("CharCode").Value;
                    var rate = item.Element("Value").Value;
                    Currency curr = new Currency() { CharCode = code, Rate = decimal.Parse(rate, new CultureInfo("ru-RU")), RateDatetime = rateDatetime };
                    var res = controller.CurrencyEdit(0, curr).Result;
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
    }
}
