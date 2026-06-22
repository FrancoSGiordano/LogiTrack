import os
from dotenv import load_dotenv
from faststream import FastStream
from faststream.rabbit import RabbitBroker, RabbitExchange, RabbitQueue, ExchangeType
from models.events import TripStartedEvent

load_dotenv()

RABBITMQ_URL = os.getenv("RABBITMQ_URL", "amqp://admin:admin@localhost:5672/")

broker = RabbitBroker(RABBITMQ_URL)

app = FastStream(broker)

mt_exchange = RabbitExchange("TripStartedEvent", type=ExchangeType.FANOUT)

sim_queue = RabbitQueue("trip_simulator_queue")

@broker.subscriber(queue=sim_queue, exchange=mt_exchange)
async def on_trip_started(event: TripStartedEvent):
    print(f"✅ ¡Nuevo viaje recibido desde .NET!")
    print(f"🚚 Viaje ID: {event.trip_id} | Camión ID: {event.truck_id}")
    print(f"📍 Origen: ({event.origin_lat}, {event.origin_lon})")
    print(f"🏁 Destino: ({event.destination_lat}, {event.destination_lon})")
    print("-" * 50)