using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OccupancyController : ControllerBase
    {
        private readonly IOccupancyRepository _occupancyRepository;

        public OccupancyController(IOccupancyRepository occupancyRepository)
        {
            _occupancyRepository = occupancyRepository;
        }




    }
}
