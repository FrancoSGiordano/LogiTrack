namespace Fleet.Application.DTOs.OSRM
{
    public record Coordinate(double Lat, double Lon);

    public record OsrmResponse(List<OsrmRoute> Routes);
    public record OsrmRoute(OsrmGeometry Geometry);
    public record OsrmGeometry(List<double[]> Coordinates);
}
