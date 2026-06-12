export const API_FLEET_URL = 'https://localhost:5001/api'

export const API_TRACKING_URL = 'http://localhost:5050/api'

export const handleResponse = async (response: Response) => {
    if(!response.ok){
        const errorData = await response.json().catch(() => null);
        throw new Error(errorData?.message || `Error HTTP: ${response.status}`);
    }

    if (response.status === 204) {
        return null; 
    }

    return response.json();
}