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



        /// <summary>
        /// Displays if list of workplaces (desks).
        /// </summary>
        /// <param name="workspaceId"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(int workspaceId)
        {
            var client = _httpClientFactory.CreateClient("MyAPI");

            try
            {
                var desks = await client.GetFromJsonAsync<List<WorkplaceResponseDto>>($"api/Workplaces/workspace/{workspaceId}");

                // 8 is the default max hours if the API call fails or returns invalid data.
                int maxHours = 8;
                decimal pricePerHour = 0;

                try
                {
                    var workspace = await client.GetFromJsonAsync<WorkspaceResponseDto>($"api/Workspaces/{workspaceId}");
                    if (workspace != null)
                    {
                        if (workspace.MaxOccupationHours > 0) maxHours = workspace.MaxOccupationHours;
                        pricePerHour = workspace.PricePerHour;
                    }
                }
                catch { }

                ViewBag.MaxHours = maxHours;
                ViewBag.PricePerHour = pricePerHour;



                // Desks that the current user has occupied.
                var userOccupiedDesks = new List<int>();
                var activeOccupancies = new Dictionary<int, int>();

                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdString, out int currentUserId))
                {
                    try
                    {
                        var allActive = await client.GetFromJsonAsync<List<OccupancyResponseDto>>("api/Occupancy/active");
                        if (allActive != null)
                        {
                            foreach (var occ in allActive.Where(o => o.UserId == currentUserId))
                            {
                                userOccupiedDesks.Add(occ.WorkplaceId);
                                activeOccupancies[occ.WorkplaceId] = occ.Id;
                            }
                        }
                    }
                    catch { }
                }

                ViewBag.UserOccupiedDesks = userOccupiedDesks;
                ViewBag.ActiveOccupancies = activeOccupancies;

                return View(desks ?? new List<WorkplaceResponseDto>());
            }
            catch (Exception)
            {
                return View(new List<WorkplaceResponseDto>());
            }
        }

        /// <summary>
        /// Releases a desk by sending a request to the API to finish the occupancy.
        /// </summary>
        /// <param name="occupancyId"></param>
        /// <param name="workspaceId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Release(int occupancyId, int workspaceId)
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            var response = await client.PostAsync($"api/Occupancy/{occupancyId}/finish", null);

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "Desk released successfully!";
            else
                TempData["ErrorMessage"] = "Failed to release the desk.";

            return RedirectToAction("Index", new { workspaceId = workspaceId });
        }

        /// <summary>
        /// Confirms a desk booking.
        /// </summary>
        /// <param name="workplaceId"></param>
        /// <param name="workspaceId"></param>
        /// <param name="durationHours"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Book(int workplaceId, int workspaceId, int durationHours, bool needsMonitor, string? specialRequests, decimal pricePerHour)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var startTime = DateTime.Now;
            var endTime = startTime.AddHours(durationHours);

            var totalPrice = durationHours * pricePerHour;

            var dto = new CreateOccupancyDto
            {
                UserId = userId,
                WorkplaceId = workplaceId,
                StartTime = startTime,
                EndTime = endTime,
                NeedsMonitor = needsMonitor,
                SpecialRequests = specialRequests,
                TotalPrice = totalPrice
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
