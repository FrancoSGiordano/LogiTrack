import requests
import time
import polyline
from datetime import datetime, timezone

API_URL = "http://localhost:5050/api/tracking/ping"

TRUCK_ID = "34a50025-de92-435d-a72b-a350900e3b09"
START_LON, START_LAT = -58.3816, -34.6037 # Obelisco
END_LON, END_LAT = -57.5426, -38.0055 # Mar del Plata

print("🗺️ Consultando a OSRM la ruta exacta por las calles y rutas...")

osrm_url = f"http://router.project-osrm.org/route/v1/driving/{START_LON},{START_LAT};{END_LON},{END_LAT}?overview=full"
response = requests.get(osrm_url).json()

if(response["code"]) != "Ok":
    print("Error al obtener la ruta")
    exit()

encoded_polyline = response["routes"][0]["geometry"]

route_coordinates = polyline.decode(encoded_polyline)

total_points = len(route_coordinates)
print(f"Ruta obtenida. El viaje tiene {total_points} coordenadas exactas.")

jumps = 20

for step in range(0, total_points, jumps):
    current_lat, current_lon = route_coordinates[step]

    payload = {
        "truckId": TRUCK_ID,
        "latitude": round(current_lat, 6),
        "longitude": round(current_lon, 6),
        "timestamp": datetime.now(timezone.utc).isoformat()
    }

    try:
        api_resp = requests.post(API_URL, json=payload)
        print(f"[Ping {step}/{total_points}] Lat: {payload['latitude']}, Lon: {payload['longitude']}")
    except Exception as e:
        print(f"Error: {e}")

    time.sleep(2)

print("\n🏁 ¡Llegamos a Mar del Plata siguiendo perfectamente la Ruta 2!")

