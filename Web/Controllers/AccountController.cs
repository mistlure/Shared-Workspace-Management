using API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }



        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var client = _httpClientFactory.CreateClient("MyAPI");
            var response = await client.PostAsJsonAsync("api/Users", dto);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }

            ModelState.AddModelError(string.Empty, "Registration failed. Email might be already taken.");
            return View(dto);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var client = _httpClientFactory.CreateClient("MyAPI");
            var response = await client.PostAsJsonAsync("api/Users/login", dto);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Workspaces");
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(dto);
        }
    }
}
