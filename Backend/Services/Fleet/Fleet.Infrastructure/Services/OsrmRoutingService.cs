using Fleet.Application.DTOs.OSRM;
using Fleet.Application.Interfaces;
using System.Text.Json;

namespace Fleet.Infrastructure.Services
{
    public class OsrmRoutingService : IRoutingService
    {
        private readonly HttpClient httpClient;

        public OsrmRoutingService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            this.httpClient.BaseAddress = new Uri("http://router.project-osrm.org");
        }

        public async Task<List<Coordinate>> GetRoutePolyline(double originLat, double originLon, double destinationLat, double destinationLon)
        {
            string coordinates = $"{originLon.ToString(System.Globalization.CultureInfo.InvariantCulture)},{originLat.ToString(System.Globalization.CultureInfo.InvariantCulture)};{destinationLon.ToString(System.Globalization.CultureInfo.InvariantCulture)},{destinationLat.ToString(System.Globalization.CultureInfo.InvariantCulture)}";

            string requestUri = $"/route/v1/driving/{coordinates}?geometries=geojson&overview=full";

            var response = await httpClient.GetAsync(requestUri);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Error al consultar OSRM. Status: {response.StatusCode}");
            }

            var jsonString = await response.Content.ReadAsStringAsync();

            var osrmData = JsonSerializer.Deserialize<OsrmResponse>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var routeCoordinates = new List<Coordinate>();

            if(osrmData?.Routes != null && osrmData.Routes.Count > 0) 
            {
                foreach(var point in osrmData.Routes[0].Geometry.Coordinates) 
                {
                    routeCoordinates.Add(new Coordinate(Lat: point[1], Lon: point[0]));
                }
            }

            return routeCoordinates;

        }

    }
}
