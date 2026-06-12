import { API_FLEET_URL, handleResponse } from "./api";
import type { Truck, TruckDetailsPayload, TruckStatusPayload } from '../types/index'

const FLEET_ENDPOINT = `${API_FLEET_URL}/Truck`;

export const fleetService = {
    getAll: async (): Promise<Truck[]> => {
        const response = await fetch(FLEET_ENDPOINT);
        return handleResponse(response);
    },

    create: async (truckData: TruckDetailsPayload): Promise<Truck> => {
        const response = await fetch(FLEET_ENDPOINT, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(truckData),
        });
        return handleResponse(response);
    },

    updateDetails: async (id: string, truckData: TruckDetailsPayload): Promise<Truck> => {
        const response = await fetch(`${FLEET_ENDPOINT}/${id}/details`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(truckData),
        });
        return handleResponse(response);
    },

    updateStatus: async (id: string, newStatus: string): Promise<Truck> => {
        const response = await fetch(`${FLEET_ENDPOINT}/${id}/status`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(newStatus),
        });
        return handleResponse(response);
    },
}