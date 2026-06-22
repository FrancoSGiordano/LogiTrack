import { API_FLEET_URL, handleResponse } from "./api";
import type { Truck, TruckDetailsPayload, Trip, TripPayloadData } from '../types/index'

const TRUCK_ENDPOINT = `${API_FLEET_URL}/Truck`;

const TRIP_ENDPOINT = `${API_FLEET_URL}/Trip`;

export const fleetService = {
    getTrucks: async (): Promise<Truck[]> => {
        const response = await fetch(TRUCK_ENDPOINT);
        return handleResponse(response);
    },

    createTruck: async (truckData: TruckDetailsPayload): Promise<Truck> => {
        const response = await fetch(TRUCK_ENDPOINT, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(truckData),
        });
        return handleResponse(response);
    },

    updateTruckDetails: async (id: string, truckData: TruckDetailsPayload): Promise<Truck> => {
        const response = await fetch(`${TRUCK_ENDPOINT}/${id}/details`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(truckData),
        });
        return handleResponse(response);
    },

    updateTruckStatus: async (id: string, newStatus: string): Promise<Truck> => {
        const response = await fetch(`${TRUCK_ENDPOINT}/${id}/status`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(newStatus),
        });
        return handleResponse(response);
    },

    getTrips: async (): Promise<Trip[]> => {
        const response = await fetch(TRIP_ENDPOINT);
        return handleResponse(response);
    },

    createTrip: async (tripData: TripPayloadData): Promise<Trip> => {
        const response = await fetch(TRIP_ENDPOINT, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(tripData),
        });
        return handleResponse(response);
    },

    updateTripDetails: async (id: string, tripData: TripPayloadData): Promise<Trip> => {
        const response = await fetch(`${TRIP_ENDPOINT}/${id}/details`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(tripData),
        });
        return handleResponse(response);
    },

    updateTripStatus: async (id: string, newStatus: string): Promise<Trip> => {
        const response = await fetch(`${TRIP_ENDPOINT}/${id}/status`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(newStatus),
        });
        return handleResponse(response);
    },
}