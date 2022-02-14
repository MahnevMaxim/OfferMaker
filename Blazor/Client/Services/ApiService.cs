using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using ApiLib;
using Blazor.Client.Services;
using System.Net.Http.Headers;
using Blazored.LocalStorage;

namespace Blazor.Client
{
    public class ApiService
    {
        HttpClient _httpClient;
        ApiLib.Client apiClient;
        ILocalStorageService _localStorageService;
        string apiBaseUrl = "https://localhost:44313";

        public ApiService(HttpClient httpClient, ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
            _httpClient = httpClient;
        }

        async public Task<ApiLib.Client> GetApiClient()
        {
            User user = await _localStorageService.GetItemAsync<User>("user");
            if (user != null)
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Account.Token);
            apiClient = new ApiLib.Client(apiBaseUrl, _httpClient);
            return apiClient;
        }

        public string GetApiBaseUrl() => apiBaseUrl;
    }
}
