using API.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace Web.Controllers
{
    [Authorize]
    public class WorkplacesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public WorkplacesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(int workspaceId)
        {
            var client = _httpClientFactory.CreateClient("MyAPI");

            try
            {
                var workplaces = await client.GetFromJsonAsync<List<WorkplaceResponseDto>>($"api/Workplaces/workspace/{workspaceId}");

                return View(workplaces ?? new List<WorkplaceResponseDto>());
            }
            catch (Exception)
            {
                return View(new List<WorkplaceResponseDto>());
            }
        }
    }
}
