using ApiLib;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blazor.Client.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAll();
    }

    public class UserService : IUserService
    {
        private ApiService _apiService;

        public UserService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            var client = await _apiService.GetApiClient();
            var response =  await client.UsersGetAsync();
            return response.Result;
        }
    }
}
