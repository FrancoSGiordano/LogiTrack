export type Trip = {
    id: string;
    truckId: string | null;
    origin: string;
    destination: string;
    originLat: number;
    originLon: number;
    destinationLat: number;
    destinationLon: number;
    finishLat: number;
    finishLon: number;
    status: 'Pending' | 'InProgress' | 'Completed';
    createdAt: string;
    startedAt: string;
    completedAt: string;
}

export type TripPayloadData = Pick<Trip, 'truckId' | 'origin' | 'destination' | 'originLat' | 'originLon' | 'destinationLat' | 'destinationLon'>

export type GeocodeResult = {
    display_name: string;
    lat: string;
    lon: string;
}

export type TruckPing = {
    TruckId: string;
    TripId: string;
    Latitude: number;
    Longitude: number;
    Speed: number;
    IsDeviated: boolean;
    IsCompleted: boolean;
    Timestamp: string;
}

export type Truck = {
    id: string;
    model: string;
    licensePlate: string;
    maxCargoCapacityKg: number;
    status: 'OnRoute' | "InBase" | "UnderMaintenance" | "OutOfService";
    createdAt: string;
}

export type TruckDetailsPayload = Omit<Truck, 'id' | 'status' | 'createdAt'>

export type ValidTruckStatus = "OnRoute" | "InBase" | "UnderMaintenance" | "OutOfService";

export type ValidTripStatus = "Pending" | "InProgress" | "Completed";

