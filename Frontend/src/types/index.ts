export type Trip = {
    id: string;
    truckId: string;
    destinationLat: number;
    destinationLon: number;
    isActive: boolean;
    startTime: string;
    endTime: string;
}

export type TruckPing = {
    truckId: string;
    latitude: number;
    longitude: number;
    timestamp: string;
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

