using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Blazor.Client.Helpers;
using Blazor.Client.Services;

namespace Blazor.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddHttpClient<HostService>(client =>
            {
                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
            });

            builder.Services.AddHttpClient<ApiService>(client =>
            {
                client.BaseAddress = null;
            });

            builder.Services.AddBlazoredLocalStorage();

            builder.Services
               .AddScoped<IAuthenticationService, AuthenticationService>()
               .AddScoped<IUserService, UserService>()
               .AddScoped<IHttpService, HttpService>();

            // configure http client
            builder.Services.AddScoped(x => {
                var apiUrl = new Uri(builder.Configuration["apiUrl"]);

                // use fake backend if "fakeBackend" is "true" in appsettings.json
                if (builder.Configuration["fakeBackend"] == "true")
                    return new HttpClient(new FakeBackendHandler()) { BaseAddress = apiUrl };

                return new HttpClient() { BaseAddress = apiUrl };
            });

            await builder.Build().RunAsync();
        }
    }
}
