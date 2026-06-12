using System;
using System.Collections.Generic;
using System.Text;

namespace Fleet.Application.DTOs.Truck
{
    public record TruckRequest
    {
        public string Model { get; init; }
        public string LicensePlate { get; init; }
        public double MaxCargoCapacityKg { get; init; }
    }
}
