using Domain.Entities;
using Domain.Interfaces;

namespace API.Services
{
    public class OccupancyService : IOccupancyService
    {
        private readonly IOccupancyRepository _occupancyRepository;

        public OccupancyService(IOccupancyRepository occupancyRepository)
        {
            _occupancyRepository = occupancyRepository;
        }

        public async Task<Occupancy> CreateOccupancyAsync(Occupancy occupancy)
        {
            if (!occupancy.EndTime.HasValue)
            {
                throw new ArgumentException("End time is required for booking.");
            }

            if (occupancy.StartTime >= occupancy.EndTime.Value)
            {
                throw new ArgumentException("The start time must be before the end time.");
            }

            if (occupancy.StartTime < DateTime.UtcNow.AddMinutes(-5))
            {
                throw new ArgumentException("You cannot book a workplace in the past.");
            }

            bool isOccupied = await _occupancyRepository.HasOverlapAsync(
                occupancy.WorkplaceId,
                occupancy.StartTime,
                occupancy.EndTime.Value);

            if (isOccupied)
            {
                throw new InvalidOperationException("This workplace is already booked for the selected time.");
            }

            int newId = await _occupancyRepository.AddAsync(occupancy);
            occupancy.Id = newId;

            return occupancy;
        }
    }
}
