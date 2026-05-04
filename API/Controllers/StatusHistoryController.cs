using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusHistoryController : ControllerBase
    {
        private readonly IStatusHistoryRepository _statusHistoryRepository;

        public StatusHistoryController(IStatusHistoryRepository statusHistoryRepository)
        {
            _statusHistoryRepository = statusHistoryRepository;
        }



    }
}
