using Fleet.Domain.Enums;
using Fleet.Domain.Exceptions;

namespace Fleet.Domain.Entities
{
    public class Truck
    {
        public Guid Id { get; private set; }
        public string Model { get; private set; }
        public string LicensePlate { get; private set; }
        public double MaxCargoCapacityKg { get; private set; }
        public TruckStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Truck() { }

        public Truck(string model, string licensePlate, double maxCargoCapacityKg)
        {
            ValidateData(model, licensePlate, maxCargoCapacityKg);

            Id = Guid.NewGuid();
            Model = model;
            LicensePlate = licensePlate.ToUpper();
            MaxCargoCapacityKg = maxCargoCapacityKg;
            Status = TruckStatus.InBase;
            CreatedAt = DateTime.UtcNow;
        }

        public void DispatchToRoute()
        {
            if(Status != TruckStatus.Available)
            {
                throw new TruckUnavailableException(Id, LicensePlate);
            }

            Status = TruckStatus.OnRoute;
        }

        public void SetAvailable()
        {
            Status = TruckStatus.Available;
        }

        public void ReturnToBase()
        {
            Status = TruckStatus.InBase;
        }

        public void SendToMaintenance()
        {
            Status = TruckStatus.UnderMaintenance;
        }

        public void SetOutOfService()
        {
            Status = TruckStatus.OutOfService;
        }

        public void UpdateDetails(string newModel, string newLicensePlate, double newMaxCargoCapacityKg)
        {
            ValidateData(newModel, newLicensePlate, newMaxCargoCapacityKg);
            Model = newModel.ToUpper();
            LicensePlate = newLicensePlate.ToUpper();
            MaxCargoCapacityKg = newMaxCargoCapacityKg;
        }

        private void ValidateData(string model, string licensePlate, double maxCargoCapacityKg)
        {
            if (string.IsNullOrWhiteSpace(model))
                throw new ArgumentException("El modelo no puede estar vacío.");
            
            if (string.IsNullOrWhiteSpace(licensePlate))
                throw new ArgumentException("La patente no puede estar vacía.");

            if (maxCargoCapacityKg <= 0)
                throw new ArgumentException("La capacidad debe ser mayor a 0.");
        }
    }
}
