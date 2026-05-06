using API.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }



        /// <summary>
        /// Empty GET method to display the registration form. The actual registration logic is handled in the POST method below.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Registers a new user by sending their details to the API. If registration is successful, redirects to the login page. If it fails (e.g., email already taken), displays an error message.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Empty GET method to display the login form. The actual login logic is handled in the POST method below.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Logins a user by sending their credentials to the API. If login is successful, creates an authentication cookie and redirects to the workspaces page. If it fails (e.g., invalid email or password), displays an error message.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var client = _httpClientFactory.CreateClient("MyAPI");
            var response = await client.PostAsJsonAsync("api/Users/login", dto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponseWrapper>();
                var user = result?.Data;


                if (user != null && !string.IsNullOrEmpty(result?.Token))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                        new Claim(ClaimTypes.Email, user.Email)
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    Response.Cookies.Append("JwtToken", result.Token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddDays(1)
                    });

                    return RedirectToAction("Index", "Workspaces");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(dto);
        }

        /// <summary>
        /// Logouts the user by clearing the authentication cookie and redirects to the workspaces page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("JwtToken");
            return RedirectToAction("Index", "Workspaces");
        }
    }

    /// <summary>
    /// Helper class to wrap the API response for login.
    /// </summary>
    public class LoginResponseWrapper
    {
        public string? Token { get; set; }
        public UserResponseDto? Data { get; set; }
        public string? Message { get; set; }
    }
}
