using API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProfileController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId)) return RedirectToAction("Login", "Account");

            var client = _httpClientFactory.CreateClient("MyAPI");

            try
            {
                ViewBag.User = await client.GetFromJsonAsync<UserResponseDto>($"api/Users/{userId}");
            }
            catch
            {
                ViewBag.User = null;
            }

            try
            {
                var allActive = await client.GetFromJsonAsync<List<OccupancyResponseDto>>("api/Occupancy/active");

                var userOccupancies = allActive?
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.StartTime)
                    .ToList() ?? new List<OccupancyResponseDto>();

                return View(userOccupancies);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка при загрузке бронирований: {ex.Message}";
                return View(new List<OccupancyResponseDto>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var client = _httpClientFactory.CreateClient("MyAPI");

            var response = await client.PostAsync($"api/Occupancy/{id}/finish", null);

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "Booking finished successfully.";
            else
                TempData["ErrorMessage"] = "Failed to finish booking.";

            return RedirectToAction("Index");
        }
    }
}