from pydantic import BaseModel, ConfigDict
from pydantic.alias_generators import to_camel

class TripStartedEvent(BaseModel):

    model_config = ConfigDict(alias_generator=to_camel, populate_by_name=True)

    trip_id: str
    truck_id: str
    origin_lat: float
    origin_lon: float
    destination_lat: float
    destination_lon: float