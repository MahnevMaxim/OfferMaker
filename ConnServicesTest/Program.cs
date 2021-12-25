using System;
using System.Threading.Tasks;

namespace ConnServicesTest
{
    class Program
    {
        static System.Net.Http.HttpClient httpClient;

        static void Main(string[] args)
        {
            Test();
        }

        async private static void Test()
        {
            httpClient = new System.Net.Http.HttpClient();
            string apiEndpoint = "https://localhost:44333/";
            await Task.Delay(5000);
            ClientNs.ClientClass cl = new ClientNs.ClientClass(apiEndpoint, httpClient);
            var res = await cl.CurrenciesAllAsync();
        }
    }
}
