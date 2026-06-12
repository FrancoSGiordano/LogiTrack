import { useState, useEffect } from 'react';
import { MapContainer, TileLayer, Marker, Popup, useMap } from 'react-leaflet';
import { Truck, MapPin, Activity, TimerIcon, Focus } from 'lucide-react';
import type { TruckPing, Truck as TruckType }  from '../../types';
import { createTruckIcon } from '../../utils/index'
import 'leaflet/dist/leaflet.css';
import styles from './Dashboard.module.css';
import type { LeafletMouseEvent } from 'leaflet';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { fleetService } from '../../services/fleetService';

const AutoPan = ({ position, isTracking }: { position: [number, number], isTracking: boolean }) => {
  const map = useMap();
  
  useEffect(() => {
    if (position && isTracking) {
      map.flyTo(position, 14, { 
          animate: true, 
          duration: 1.5
      });
    }
  }, [map, position, isTracking]);
  
  return null;
};


export default function Dashboard() {

    const [truckCatalog, setTruckCatalog] = useState<Record<string, TruckType>>({})
    const [truckData, setTruckData] = useState<TruckPing | null>(null);
    const [isTracking, setIsTracking] = useState<boolean>(true);

    const [livePositions, setLivePositions] = useState<Record<string, TruckPing>>({});
    const [selectedTruckId, setSelectedTruckId] = useState<string | null>(null);
    const selectedTruck = selectedTruckId ? livePositions[selectedTruckId] : null;

    const [searchTerm, setSearchTerm] = useState<string>("");

    useEffect(() => {
        const getTrucks = async () => {
            try{
                const data = await fleetService.getAll();
                const trucksMap = data.reduce((acc : any, truck: TruckType) => {
                    acc[truck.id] = truck;
                    return acc;
                }, {});
                setTruckCatalog(trucksMap);
                console.log(trucksMap)
            } catch (error){
                console.error("Error cargando catálogo:", error);
            }
        }
        getTrucks();
    }, [])

    useEffect(() => {
        const connection = new HubConnectionBuilder()
            .withUrl("http://localhost:5050/tracking-hub")
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build();
        
        connection.on("UpdatePosition", (ping: TruckPing) => {
            setLivePositions(prevPositions => ({
                ...prevPositions,
                [ping.truckId]: ping
            }));
        });

        const startConnection = async () => {
            try {
                await connection.start();
                console.log("🟢 ¡Conectado exitosamente a SignalR!");
            } catch (error) {
                console.error("🔴 Error al conectar con SignalR:", error);
            }
        };

        startConnection();

        return () => {
            connection.stop();
        };
    }, []);

    
    const currentPos : [number, number] = truckData
        ? [truckData.latitude, truckData.longitude]
        : [-34.6037, -58.3816];

   const filteredTrucks = Object.values(livePositions).filter(truck => 
        truck.truckId.toLocaleLowerCase().includes(searchTerm)
   )


    return (
        <div className={styles.container}>
            <div className={styles.statsPanel}>
                <h2 className={styles.panelTitle}>
                    <Activity color='#60a5fa'/> Telemetría en Vivo
                </h2>

                <button
                    className={`${styles.trackButton} ${isTracking ? styles.trackActive : styles.trackInactive}`}
                    onClick={() => setIsTracking(!isTracking)}
                    disabled={!selectedTruckId}
                >
                    <Focus size={18}/>
                    {isTracking ? 'SEGUIMIENTO AUTOMÁTICO: ON' : 'SEGUIMIENTO AUTOMÁTICO: OFF'}
                </button>

                {selectedTruck && (
                    <>
                        <div className={styles.statBox}>
                            <p className={styles.statLabel}><Truck size={14}/>Unidad Seleccionada</p>
                            <p className={styles.statValue} style={{ color: 'white' }}>{selectedTruck.truckId}</p>
                        </div>

                        <div className={styles.statBox}>
                            <p className={styles.statLabel}><MapPin size={14} /> Coordenadas</p>
                            <p className={styles.statValue}>
                                {selectedTruck.latitude.toFixed(5)}, {selectedTruck.longitude.toFixed(5)}
                            </p>
                        </div>

                        <div className={styles.statBox}>
                            <p className={styles.statLabel}><TimerIcon size={14}/>Último Ping</p>
                            <p className={styles.statValue}>
                                {new Date(selectedTruck.timestamp).toLocaleTimeString()}
                            </p>
                        </div>
                        <div className={styles.connectionStatus}>
                            <div className={styles.pingDot}></div>
                            Recibiendo señal
                        </div>
                    </>
                )}

                <div className={styles.selectTruckContainer}>
                    <p className={styles.selectTruckText}>
                        Flota activa ({Object.keys(livePositions).length}):
                    </p>

                    <input 
                        type="text" 
                        placeholder="Buscar unidad por ID..." 
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                        className={styles.searchInput}
                    />

                    <div className={styles.truckListWrapper}>
                        {filteredTrucks.map(truck => {
                            const isCurrentlySelected = selectedTruckId === truck.truckId
                            const truckInfo = truckCatalog[truck.truckId] || { model: "Desconocido", plate: "---" };
                            const minutesSinceLastPing = (Date.now() - new Date(truck.timestamp).getTime()) / 60000;

                            let statusClass = styles.dotActive;
                            let statusText = "Activo";
                            let statusTextColor = "#10b981";

                            if (minutesSinceLastPing > 5) {
                                statusClass = styles.dotOffline;
                                statusText = "Sin señal";
                                statusTextColor = "#ef4444";
                            } else if (minutesSinceLastPing > 1) {
                                statusClass = styles.dotDelayed;
                                statusText = "Retraso";
                                statusTextColor = "#f59e0b"; 
                            }

                            return (
                                <button
                                    key={truck.truckId}
                                    onClick={() => setSelectedTruckId(truck.truckId)}
                                    style={{ flexDirection: 'column', alignItems: 'flex-start'}}
                                    className={`${styles.truckListItem} ${isCurrentlySelected ? styles.truckListItemSelected : ''}`}
                                >
                                    <div className={styles.truckItemHeader}>
                                        <span className={styles.truckItemTitle}>{truck.truckId.substring(0, 8)}...</span>
                                        
                                        <div className={styles.statusIndicator} style={{ color: statusTextColor }}>
                                            <div className={`${styles.statusDot} ${statusClass}`}></div>
                                            {statusText}
                                        </div>
                                    </div>
                                    <div className={styles.truckItemDetails}>
                                        <span>Ult: {new Date(truck.timestamp).toLocaleTimeString()}</span>
                                        <span className={isCurrentlySelected ? styles.truckListStatusSelected : styles.truckListStatus}>
                                            {isCurrentlySelected ? 'En foco' : 'Ruta'}
                                        </span>
                                    </div>
                                </button>
                            )
                        })}
                    </div>
                    
                </div>      
            </div>

            <div className={styles.mapWrapper}>
                <MapContainer
                    center={currentPos}
                    zoom={10}
                    zoomControl={false}
                    style={{ height: '100%', width: '100%' }}
                >
                    <TileLayer
                        url='https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png'
                        attribution='&copy; <a href="https://carto.com/">CARTO</a>'
                    />

                    {Object.values(livePositions).map((truck) => {
                        const truckInfo = truckCatalog[truck.truckId] || { model: "Desconocido", plate: "---" };
                        const isSelected = selectedTruckId === truck.truckId;
                        const truckPos: [number, number] = [truck.latitude, truck.longitude];

                        return (
                            <Marker
                                key={truck.truckId}
                                position={truckPos}
                                icon={createTruckIcon(isSelected ? "#10b981" : "#3b82f6")}
                                eventHandlers={{
                                    click: () => {
                                        setSelectedTruckId(truck.truckId)
                                    },
                                    mouseover: (e: any) => e.target.openPopup(),
                                    mouseout: (e: any) => e.target.closePopup(),
                                }}
                            >
                                <Popup>
                                    <strong>{truckInfo.model}</strong><br />
                                    <strong>{truckInfo.licensePlate}</strong><br />
                                    LAT: {truck.latitude.toFixed(4)} | LON: {truck.longitude.toFixed(4)}
                                </Popup>

                            </Marker>
                        )
                    })}

                    {selectedTruck && (
                        <AutoPan
                            position={[selectedTruck.latitude, selectedTruck.longitude]} 
                            isTracking={isTracking}
                        />
                    )}
                    
                </MapContainer>
            </div>
        </div>
    );
};
