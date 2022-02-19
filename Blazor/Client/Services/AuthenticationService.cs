using ApiLib;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using System.Text.Json;
using Newtonsoft.Json;
using Blazored.LocalStorage;
using System;

namespace Blazor.Client.Services
{
    public interface IAuthenticationService
    {
        User User { get; }
        Task Initialize();
        Task Login(string username, string password);
        Task Logout();
    }

    public class AuthenticationService : IAuthenticationService
    {
        private NavigationManager _navigationManager;
        private ILocalStorageService _localStorageService;
        private ApiService _apiService;

        public User User { get; private set; }

        public AuthenticationService(
            NavigationManager navigationManager,
            ILocalStorageService localStorageService,
            ApiService apiService
        )
        {
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
            _apiService = apiService;
        }

        public async Task Initialize()
        {
            User = await _localStorageService.GetItemAsync<User>("user");
        }

        public async Task Login(string username, string password)
        {
            var client = await _apiService.GetApiClient();
            var response = await client.AccountGetTokenAsync(username, password);
            JsonElement authRes = (JsonElement)response.Result;
            var userAsString = authRes.GetProperty("user").ToString();
            User = JsonConvert.DeserializeObject<User>(userAsString);
            await _localStorageService.SetItemAsync("user", User);
        }

        public async Task Logout()
        {
            User = null;
            await _localStorageService.RemoveItemAsync("user");
            Console.WriteLine("Logout()");
            _navigationManager.NavigateTo("login");
        }
    }
}
