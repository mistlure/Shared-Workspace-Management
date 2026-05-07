using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class ApiDescriptionController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetApiDescription()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var controllers = assembly.GetTypes()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type) && !type.IsAbstract)
                .Select(type =>
                {
                    // Получаем базовый роут контроллера (например, "api/Users")
                    var controllerName = type.Name.Replace("Controller", "");
                    var controllerRoute = type.GetCustomAttribute<RouteAttribute>()?.Template?.Replace("[controller]", controllerName) ?? $"api/{controllerName}";

                    return new
                    {
                        ControllerName = controllerName,
                        Endpoints = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                            .Where(m => m.GetCustomAttributes<HttpMethodAttribute>().Any())
                            .Select(method =>
                            {
                                var httpVerb = method.GetCustomAttribute<HttpMethodAttribute>()?.HttpMethods.First() ?? "UNKNOWN";

                                var actionRoute = method.GetCustomAttribute<RouteAttribute>()?.Template ?? method.GetCustomAttribute<HttpMethodAttribute>()?.Template;
                                var fullRoute = string.IsNullOrEmpty(actionRoute) ? controllerRoute : $"{controllerRoute}/{actionRoute}";

                                
                                var returnType = method.ReturnType;
                                if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
                                {
                                    returnType = returnType.GetGenericArguments()[0];
                                }
                                if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(ActionResult<>))
                                {
                                    returnType = returnType.GetGenericArguments()[0];
                                }
                                string cleanReturnType = returnType.Name;

                                var parameters = method.GetParameters().Select(p => new
                                {
                                    Name = p.Name,
                                    Type = p.ParameterType.Name,
                                    Source = p.GetCustomAttribute<FromBodyAttribute>() != null ? "Body" : "Route/Query"
                                });

                                return new
                                {
                                    MethodName = method.Name,
                                    HttpVerb = httpVerb,
                                    Route = "/" + fullRoute,
                                    Parameters = parameters,
                                    ReturnType = cleanReturnType == "IActionResult" ? "JSON Response" : cleanReturnType
                                };
                            })
                    };
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