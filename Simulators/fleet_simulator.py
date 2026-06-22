import pika
import json
import time
import threading
import requests
import polyline
from datetime import datetime, timezone

CITIES = {
    "BsAs": (-34.6037, -58.3816),
    "MDQ": (-38.0055, -57.5562),
    "Rosario": (-32.9468, -60.6393),
    "Cordoba": (-31.4201, -64.1888)
}

def get_real_route(start_coord, end_coord):

    url = f"http://router.project-osrm.org/route/v1/driving/{start_coord[1]},{start_coord[0]};{end_coord[1]},{end_coord[0]}?overview=full"

    response = requests.get(url)
    data = response.json()

    encoded_polyline = data['routes'][0]['geometry']
    route_points = polyline.decode(encoded_polyline)

    return route_points

def simulate_truck(trip_id, truck_id, start_city, end_city, simulate_deviation=False):
    credentials = pika.PlainCredentials('admin', 'admin')
    parameters = pika.ConnectionParameters(
        host='localhost',
        port=5672,
        virtual_host='/',
        credentials=credentials
    )
    connection = pika.BlockingConnection(parameters)
    channel = connection.channel()
    queue_name = 'truck_pings'
    channel.queue_declare(queue=queue_name, durable=True)

    points = get_real_route(start_city, end_city)

    step = 20
    fast_points = points[::step]

    total_points = len(fast_points)

    for index, (lat, lon) in enumerate(fast_points):

        if simulate_deviation and index > (total_points / 2):
            lat += 2.0
            lon += 2.0
        
        ping = {
            "TripId": trip_id,
            "TruckId": truck_id,
            "Latitude": lat,
            "Longitude": lon,
            "Timestamp": datetime.now(timezone.utc).isoformat(),
        }

        channel.basic_publish(
            exchange='',
            routing_key=queue_name,
            body=json.dumps(ping)
        )

        time.sleep(3)

    print(f"🏁 ¡El camión {truck_id} ha llegado a su destino!")
    connection.close()

if __name__ == "__main__":
    print("🚀 Iniciando el Centro de Despacho Logístico con Rutas Reales...")

    routes = [
        ("b71f3063-4df8-4086-96b5-2b01ccea35f6", "ed0a5c6d-7cf9-4a26-a078-c87a36d6aeb6", CITIES["BsAs"], CITIES["MDQ"], False),
        ("b86b8d47-8401-46aa-b677-ab1bda6776ed", "6ba63cc1-c20d-47bc-b563-fe15bd44d8b5", CITIES["BsAs"], CITIES["Rosario"], False),
        ("fbcc244d-1344-4d58-9ac4-67cf0276fc83", "f258e025-3a4c-4c6a-a930-5c5f12ea0282", CITIES["Rosario"], CITIES["Cordoba"], False)
    ]

    threads = []

    for trip_id, truck_id, start, end, deviate in routes:
        t = threading.Thread(target=simulate_truck, args=(trip_id, truck_id, start, end, deviate))
        threads.append(t)
        t.start()

    for t in threads:
        t.join()

    print("✅ Toda la flota ha llegado a sus destinos.")