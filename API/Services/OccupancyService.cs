using Domain.Entities;
using Domain.Interfaces;
using Domain.Enums;
using System.Transactions;

namespace API.Services
{
    public class OccupancyService : IOccupancyService
    {
        private readonly IOccupancyRepository _occupancyRepository;
        private readonly IWorkplaceRepository _workplaceRepository;
        private readonly IStatusHistoryRepository _statusHistoryRepository;
        private readonly IWorkspaceRepository _workspaceRepository;

        public OccupancyService(
            IOccupancyRepository occupancyRepository,
            IWorkplaceRepository workplaceRepository,
            IStatusHistoryRepository statusHistoryRepository,
            IWorkspaceRepository workspaceRepository)
        {
            _occupancyRepository = occupancyRepository;
            _workplaceRepository = workplaceRepository;
            _statusHistoryRepository = statusHistoryRepository;
            _workspaceRepository = workspaceRepository;
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



            var workplace = await _workplaceRepository.GetByIdAsync(occupancy.WorkplaceId);
            if (workplace == null)
                throw new ArgumentException($"Workplace with ID {occupancy.WorkplaceId} not found.");

            var workspace = await _workspaceRepository.GetByIdAsync(workplace.WorkspaceId);
            if (workspace == null)
                throw new ArgumentException("Associated Workspace not found.");



            TimeSpan requestedDuration = occupancy.EndTime.Value - occupancy.StartTime;

            if (requestedDuration.TotalHours > workspace.MaxOccupationHours)
            {
                throw new ArgumentException($"Booking duration ({requestedDuration.TotalHours:F1} hours) exceeds the maximum allowed ({workspace.MaxOccupationHours} hours) for this workspace.");
            }

            bool isOccupied = await _occupancyRepository.HasOverlapAsync(
                occupancy.WorkplaceId,
                occupancy.StartTime,
                occupancy.EndTime.Value);

            if (isOccupied)
            {
                throw new InvalidOperationException("This workplace is already booked for the selected time.");
            }

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                int newId = await _occupancyRepository.AddAsync(occupancy);
                occupancy.Id = newId;

                workplace.CurrentStatus = WorkplaceStatus.Occupied;
                await _workplaceRepository.UpdateAsync(workplace);

                var historyRecord = new StatusHistory
                {
                    WorkplaceId = occupancy.WorkplaceId,
                    Status = WorkplaceStatus.Occupied,
                    ChangedAt = DateTime.UtcNow
                };
                await _statusHistoryRepository.AddAsync(historyRecord);

                scope.Complete();
            }
            return occupancy;
        }

        public async Task FinishOccupancyAsync(int occupancyId)
        {
            var occupancy = await _occupancyRepository.GetByIdAsync(occupancyId);

            if (occupancy == null)
            {
                throw new ArgumentException($"Occupancy with ID {occupancyId} not found.");
            }

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                occupancy.EndTime = DateTime.UtcNow;

                await _occupancyRepository.UpdateAsync(occupancy);

                var workplace = await _workplaceRepository.GetByIdAsync(occupancy.WorkplaceId);
                if (workplace != null)
                {
                    workplace.CurrentStatus = WorkplaceStatus.Available;
                    await _workplaceRepository.UpdateAsync(workplace);
                }

                var historyRecord = new StatusHistory
                {
                    WorkplaceId = occupancy.WorkplaceId,
                    Status = WorkplaceStatus.Available,
                    ChangedAt = DateTime.UtcNow
                };
                await _statusHistoryRepository.AddAsync(historyRecord);

                scope.Complete();
            }
        }
    }
}
