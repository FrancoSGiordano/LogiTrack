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

def simulate_truck(truck_id, start_city, end_city, duration_minutes=15):
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

    step = 10
    fast_points = points[::step]

    for lat, lon in fast_points:
        
        ping = {
            "TruckId": truck_id,
            "Latitude": lat,
            "Longitude": lon,
            "Timestamp": datetime.now(timezone.utc).isoformat()
        }

        channel.basic_publish(
            exchange='',
            routing_key=queue_name,
            body=json.dumps(ping)
        )

        time.sleep(20)

    print(f"🏁 ¡El camión {truck_id} ha llegado a su destino!")
    connection.close()

if __name__ == "__main__":
    print("🚀 Iniciando el Centro de Despacho Logístico con Rutas Reales...")

    routes = [
        ("434f2223-c6e4-4574-bed4-44ce30a9358f", CITIES["BsAs"], CITIES["MDQ"]),
        ("7840ba03-69b2-4a98-aa1a-6ed684583385", CITIES["BsAs"], CITIES["Rosario"]),
        ("f11cd993-9443-413b-b3b4-793fbd5092e0", CITIES["Rosario"], CITIES["Cordoba"])
    ]

    threads = []

    for truck_id, start, end in routes:
        t = threading.Thread(target=simulate_truck, args=(truck_id, start, end, 60))
        threads.append(t)
        t.start()

    for t in threads:
        t.join()

    print("✅ Toda la flota ha llegado a sus destinos.")