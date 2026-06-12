using Fleet.Application.DTOs.OSRM;

namespace Fleet.Application.Interfaces
{
    public interface IRoutingService
    {
        Task<List<Coordinate>> GetRoutePolyline(double originLat, double originLon, double destinationLat, double destinationLon);
    }
}
