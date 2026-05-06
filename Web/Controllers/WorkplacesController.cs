using API.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Security.Claims;

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
                var desks = await client.GetFromJsonAsync<List<WorkplaceResponseDto>>($"api/Workplaces/workspace/{workspaceId}");

                int maxHours = 8;
                try
                {
                    var workspace = await client.GetFromJsonAsync<WorkspaceResponseDto>($"api/Workspaces/{workspaceId}");
                    if (workspace != null && workspace.MaxOccupationHours > 0)
                    {
                        maxHours = workspace.MaxOccupationHours;
                    }
                }
                catch
                {
                }

                ViewBag.MaxHours = maxHours;

                return View(desks ?? new List<WorkplaceResponseDto>());
            }
            catch (Exception)
            {
                return View(new List<WorkplaceResponseDto>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Book(int workplaceId, int workspaceId, int durationHours)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var startTime = DateTime.Now;
            var endTime = startTime.AddHours(durationHours);

            var dto = new CreateOccupancyDto
            {
                UserId = userId,
                WorkplaceId = workplaceId,
                StartTime = startTime,
                EndTime = endTime
            };

            var client = _httpClientFactory.CreateClient("MyAPI");
            var response = await client.PostAsJsonAsync("api/Occupancy", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Desk booked successfully!";
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"API Error ({response.StatusCode}): {errorContent}";
            }

            return RedirectToAction("Index", new { workspaceId = workspaceId });
        }
    }
}
