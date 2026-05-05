using API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class WorkspacesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public WorkspacesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("MyAPI");

            try
            {
                var workspaces = await client.GetFromJsonAsync<List<WorkspaceResponseDto>>("api/Workspaces");

                return View(workspaces ?? new List<WorkspaceResponseDto>());
            }
            catch (Exception)
            {
                return View(new List<WorkspaceResponseDto>());
            }
        }
    }
}