using Mapster;
using Tracking.Application.DTOs;
using Tracking.Application.Interfaces;
using Tracking.Core.Entities;

namespace Tracking.Application.Services
{
    public class TruckPositionService : ITruckPositionService
    {
        private readonly ITruckPositionRepository repository;

        public TruckPositionService(ITruckPositionRepository repository)
        {
            this.repository = repository;
        }

        public async Task AddTruckPosition(TruckPing TruckPing)
        {
            var position = TruckPing.Adapt<TruckPosition>();

            await repository.AddTruckPosition(position);
        }

        
    }
}
