using Microsoft.AspNetCore.Authentication.Cookies;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddHttpClient("MyAPI", (serviceProvider, client) =>
            {
                client.BaseAddress = new Uri("https://localhost:7199/");

                var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
                var context = httpContextAccessor.HttpContext;

                if (context != null && context.Request.Cookies.TryGetValue("JwtToken", out var token))
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
            });

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.Cookie.Name = "SharedWorkspace.Auth";
                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Shared/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Workspaces}/{action=Index}/{id?}");

            app.Run();
        }
    }
}