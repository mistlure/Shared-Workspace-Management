using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Routing;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiDescriptionController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetApiDescription()
        {
            // Get the assembly containing the controllers.
            var assembly = Assembly.GetExecutingAssembly();

            var controllers = assembly.GetTypes()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type) && !type.IsAbstract)
                .Select(type => new
                {
                    ControllerName = type.Name.Replace("Controller", ""),
                    Endpoints = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                        .Where(m => m.GetCustomAttributes<HttpMethodAttribute>().Any())
                        .Select(method => new
                        {
                            MethodName = method.Name,
                            HttpVerb = method.GetCustomAttribute<HttpMethodAttribute>()?.HttpMethods.First(),

                            Route = method.GetCustomAttribute<RouteAttribute>()?.Template
                                    ?? method.GetCustomAttribute<HttpMethodAttribute>()?.Template
                                    ?? "Default"
                        })
                });
            return Ok(new
            {
                Project = "Shared Workspace Management API",
                Version = "1.0",
                Description = "Auto-generated API documentation using Reflection",
                Controllers = controllers
            });
        }

    }
}
