using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using API.DTOs;
using Newtonsoft.Json;

namespace Desktop
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private string _token = string.Empty;

        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7199/");
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var loginDto = new UserLoginDto
            {
                Email = email,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync("api/Users/login", loginDto);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<LoginResponseWrapper>(content);

                if (result != null && !string.IsNullOrEmpty(result.Token))
                {
                    _token = result.Token;
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", _token);
                    return true;
                }
            }
            return false;
        }
    }

    public class LoginResponseWrapper
    {
        public string Token { get; set; } = string.Empty;
        public UserResponseDto? Data { get; set; }
    }
}