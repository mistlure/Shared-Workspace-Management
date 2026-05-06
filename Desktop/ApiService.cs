using System;
using System.Collections.Generic;
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

        #region ================= AUTHENTICATION =================

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

        #endregion

        #region ================= USERS =================

        public async Task<List<UserResponseDto>> GetUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Users");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<UserResponseDto>>(content) ?? new List<UserResponseDto>();
                }
                return new List<UserResponseDto>();
            }
            catch
            {
                return new List<UserResponseDto>();
            }
        }

        public async Task<bool> UpdateUserAsync(UserResponseDto user)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Users/{user.Id}", user);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var response = await _httpClient.DeleteAsync($"api/Users/{userId}");
            return response.IsSuccessStatusCode;
        }

        #endregion

        #region ================= WORKSPACES (Здания) =================

        public async Task<List<WorkspaceResponseDto>> GetWorkspacesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Workspaces");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<WorkspaceResponseDto>>(content) ?? new List<WorkspaceResponseDto>();
                }
                return new List<WorkspaceResponseDto>();
            }
            catch
            {
                return new List<WorkspaceResponseDto>();
            }
        }

        public async Task<bool> CreateWorkspaceAsync(CreateWorkspaceDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Workspaces", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateWorkspaceAsync(WorkspaceResponseDto workspace)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Workspaces/{workspace.Id}", workspace);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteWorkspaceAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Workspaces/{id}");
            return response.IsSuccessStatusCode;
        }

        #endregion

        #region ================= WORKPLACES (Столы) =================

        public async Task<List<WorkplaceResponseDto>> GetWorkplacesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Workplaces");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<WorkplaceResponseDto>>(content) ?? new List<WorkplaceResponseDto>();
                }
                return new List<WorkplaceResponseDto>();
            }
            catch
            {
                return new List<WorkplaceResponseDto>();
            }
        }

        public async Task<bool> CreateWorkplaceAsync(CreateWorkplaceDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Workplaces", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateWorkplaceAsync(WorkplaceResponseDto workplace)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Workplaces/{workplace.Id}", workplace);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteWorkplaceAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Workplaces/{id}");
            return response.IsSuccessStatusCode;
        }

        #endregion
    }

    #region ================= HELPER CLASSES =================

    public class LoginResponseWrapper
    {
        public string Token { get; set; } = string.Empty;
        public UserResponseDto? Data { get; set; }
    }

    #endregion
}