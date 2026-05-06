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



        /// <summary>
        /// Displays the user's profile information.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId)) return RedirectToAction("Login", "Account");

            var client = _httpClientFactory.CreateClient("MyAPI");

            // Try-catch saves us from potential API errors when fetching user info.
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

                // Filter for the current user.
                var userOccupancies = allActive?
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.StartTime)
                    .ToList() ?? new List<OccupancyResponseDto>();

                return View(userOccupancies);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Booking error: {ex.Message}";
                return View(new List<OccupancyResponseDto>());
            }
        }

        /// <summary>
        /// Cancels an active booking by sending a request to the API to finish the occupancy.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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