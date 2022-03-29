using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft;
using Newtonsoft.Json;

namespace API
{
    public class BackupMaker
    {
        static string serverId = "1494887";
        static string token = "8bb81cae9b4a14df6d1a8e7dcb144f2de79f9b52396b0e5cb4fecac786efb7d7a92e3decba1f1feb0ef4e564b0502fe3";
        static string apiEndpoint = "https://api.cloudvps.reg.ru/v1/";

        /// <summary>
        /// Job стремится, чтобы был, хотябы:
        /// 1 бэкап недельной давности
        /// 1 бэкап 2-х дневной давности
        /// 1 бэкап за последние сутки
        /// </summary>
        async internal static void Backup()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await httpClient.GetStringAsync(apiEndpoint + "snapshots");
                Console.WriteLine(response);
                JObject jo = JObject.Parse(response);

                //удаляем пока не останется 2 последних снэпшота
                int count = jo["snapshots"].Children().Count();
                while (count > 2)
                {
                    var lastSnId = jo["snapshots"][0]["id"].ToString();
                    var responseDel = await httpClient.DeleteAsync(apiEndpoint + "snapshots/" + lastSnId);
                    //на всякий случай подождём минутку, а то хуй его знает
                    await Task.Delay(60000);

                    response = await httpClient.GetStringAsync(apiEndpoint + "snapshots");
                    jo = JObject.Parse(response);
                    count = jo["snapshots"].Children().Count();
                }

                //добавляем 3-й снэпшот
                var content = new { type = "snapshot", name = "backup" };
                string url = apiEndpoint + "reglets/" + serverId + "/actions";

                var stringContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
                var responsePost = await httpClient.PostAsync(url, stringContent);
                var responseString = await responsePost.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }

        }
    }
}
