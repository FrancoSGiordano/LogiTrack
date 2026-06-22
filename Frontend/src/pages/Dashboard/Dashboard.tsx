import { useState, useEffect, useRef, useMemo } from 'react';
import { MapContainer, TileLayer, Marker, Popup, useMap } from 'react-leaflet';
import { Truck, MapPin, Activity, TimerIcon, Focus, CheckCircle2 } from 'lucide-react';
import type { TruckPing, Truck as TruckType }  from '../../types';
import { createTruckIcon } from '../../utils/index'
import 'leaflet/dist/leaflet.css';
import styles from './Dashboard.module.css';
import { HubConnectionBuilder } from '@microsoft/signalr';
import { fleetService } from '../../services/fleetService';
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { useTrucks } from '../../hooks/useTrucks';

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

    const { data: trucksArray = [], isLoading } = useTrucks()

    const truckCatalog = useMemo(() => {
        return trucksArray?.reduce((acc: Record<string, TruckType>, truck: TruckType) => {
            acc[truck.id] = truck;
            return acc;
        }, {})
    }, [trucksArray])
    

    const [isTracking, setIsTracking] = useState<boolean>(true); 
    const [livePositions, setLivePositions] = useState<Record<string, TruckPing>>({});
    const [selectedTruckId, setSelectedTruckId] = useState<string | null>(null);
    const selectedTruck = selectedTruckId ? livePositions[selectedTruckId] : null;

    const [searchTerm, setSearchTerm] = useState<string>("");

    const notifiedTrucks = useRef<Set<string>>(new Set());

    const catalogRef = useRef(truckCatalog);

    useEffect(() => {
        catalogRef.current = truckCatalog;
    }, [truckCatalog]);

   useEffect(() => {
        const connection = new HubConnectionBuilder()
            .withUrl("http://localhost:5050/hubs/tracking")
            .withAutomaticReconnect()
            .build();
        
        connection.start()
            .then(() => {
                console.log("Conectado a SignalR");

                connection.on("ReceiveTruckPosition", (truckPing : TruckPing) => {

                    const id = truckPing.TruckId

                    if(!id) return;

                    setLivePositions(prevPositions => ({
                        ...prevPositions,
                        [id]: truckPing
                    }));

                    if(truckPing.IsCompleted && !notifiedTrucks.current.has(truckPing.TruckId)){
                        notifiedTrucks.current.add(id);

                        const truckInfo = catalogRef.current[id]
                        const licensePlate = truckInfo.licensePlate || id.substring(0, 6);;

                        toast.success(`🏁 ¡El camión ${licensePlate} llegó a su destino!`, {
                            position: "top-right",
                            autoClose: 5000,
                            theme: "dark"
                        });
                    }
                });
            })
            .catch (err => console.error("Error conectando a SignalR: ", err));

        return () => {

            connection.off("ReceiveTruckPosition");

            if(connection.state === "Connected"){
                connection.stop()
            }
        };
   }, []);   

    if (isLoading) {
        return <div className={styles.container}><h2 style={{color: 'white', padding: '2rem'}}>Cargando flota...</h2></div>;
    }

    const currentPos : [number, number] = [-34.6037, -58.3816];

    const filteredTrucks = Object.values(livePositions).filter(truck => 
            truckCatalog[truck.TruckId].licensePlate.toLocaleLowerCase().includes(searchTerm)
    )

    return (
        <div className={styles.container}>
            <ToastContainer/>
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
                            <p className={styles.statValue} style={{ color: 'white' }}>{truckCatalog[selectedTruck.TruckId].licensePlate} | {truckCatalog[selectedTruck.TruckId].model}</p>
                        </div>

                        <div className={styles.statBox}>
                            <p className={styles.statLabel}><MapPin size={14} /> Coordenadas</p>
                            <p className={styles.statValue}>
                                {selectedTruck.Latitude.toFixed(5)}, {selectedTruck.Longitude.toFixed(5)}
                            </p>
                        </div>

                        <div className={styles.statBox}>
                            <p className={styles.statLabel}><TimerIcon size={14}/>Último Ping</p>
                            <p className={styles.statValue}>
                                {new Date(selectedTruck.Timestamp).toLocaleTimeString()}
                            </p>
                        </div>
                        <div className={styles.connectionStatus}>
                            {selectedTruck.IsCompleted ? (
                                <>
                                    <CheckCircle2 size={14} color="#10b981" />
                                    <span className={styles.tripCompleteText}>Viaje Finalizado</span>
                                </>
                            ) : (
                                <>
                                    <div className={styles.pingDot}></div>
                                    Recibiendo señal
                                </>
                            )}
                            
                        </div>
                    </>
                )}

                <div className={styles.selectTruckContainer}>
                    <p className={styles.selectTruckText}>
                        Flota activa ({Object.keys(livePositions).length}):
                    </p>

                    <input 
                        type="text" 
                        placeholder="Buscar unidad por Patente..." 
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                        className={styles.searchInput}
                    />

                    <div className={styles.truckListWrapper}>
                        {filteredTrucks.map(truck => {
                            const isCurrentlySelected = selectedTruckId === truck.TruckId;
                            const truckInfo = truckCatalog[truck.TruckId];
                            const minutesSinceLastPing = (Date.now() - new Date(truck.Timestamp).getTime()) / 60000;

                            let statusClass = styles.dotActive;
                            let statusText = "Activo";
                            let statusTextColor = "#10b981";

                            if(truck.IsCompleted){
                                statusClass = styles.dotActive; 
                                statusText = "Destino";
                                statusTextColor = "#10b981";
                            }
                            else if (minutesSinceLastPing > 5) {
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
                                    key={truck.TruckId}
                                    onClick={() => setSelectedTruckId(truck.TruckId)}
                                    style={{ flexDirection: 'column', alignItems: 'flex-start'}}
                                    className={`${styles.truckListItem} ${isCurrentlySelected ? styles.truckListItemSelected : ''}`}
                                >
                                    <div className={styles.truckItemHeader}>
                                        <span className={styles.truckItemTitle}>{truckInfo.licensePlate}</span>
                                        
                                        <div className={styles.statusIndicator} style={{ color: statusTextColor }}>
                                            <div className={`${styles.statusDot} ${statusClass}`}></div>
                                            {statusText}
                                        </div>
                                    </div>
                                    <div className={styles.truckItemDetails}>
                                        <span>Ult: {new Date(truck.Timestamp).toLocaleTimeString()}</span>
                                        <span className={isCurrentlySelected ? styles.truckListStatusSelected : styles.truckListStatus}>
                                            {truck.IsCompleted ? 'Completado' : (isCurrentlySelected ? 'En foco' : 'Ruta')}
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
                        const truckInfo = truckCatalog[truck.TruckId] || { model: "Desconocido", plate: "---" };
                        const isSelected = selectedTruckId === truck.TruckId;
                        const truckPos: [number, number] = [truck.Latitude, truck.Longitude];

                        return (
                            <Marker
                                key={truck.TruckId}
                                position={truckPos}
                                icon={createTruckIcon(isSelected ? "#10b981" : (truck.IsCompleted ? "#9ca3af" : "#3b82f6"))}
                                eventHandlers={{
                                    click: () => {
                                        setSelectedTruckId(truck.TruckId)
                                    },
                                    mouseover: (e: any) => e.target.openPopup(),
                                    mouseout: (e: any) => e.target.closePopup(),
                                }}
                            >
                                <Popup>
                                    <div className={styles.popupContainer}>
                                        
                                        {/* Encabezado: Icono + Modelo */}
                                        <div className={styles.popupHeader}>
                                            <Truck size={18} color={truck.IsDeviated ? "#ef4444" : "#3b82f6"} />
                                            <div className={styles.popupTitleGroup}>
                                                <h4 className={styles.popupModel}>
                                                    {truckInfo.model || "Camión Scania"}
                                                </h4>
                                                <span className={styles.popupPlate}>
                                                    {truckInfo.licensePlate || truck.TruckId.substring(0, 8)}
                                                </span>
                                            </div>
                                        </div>

                                        {/* Cuerpo: Datos de Telemetría */}
                                        <div className={styles.popupBody}>
                                            <div className={styles.popupRow}>
                                                <span>Velocidad:</span>
                                                <span className={styles.speedBadge}>
                                                    ⚡ {Math.round(truck.Speed)} km/h
                                                </span>
                                            </div>
                                            
                                            <div className={styles.popupRow}>
                                                <span>Último reporte:</span>
                                                <span className={styles.timestamp}>
                                                    {new Date(truck.Timestamp).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', second: '2-digit' })}
                                                </span>
                                            </div>
                                        </div>

                                        {/* Alerta de Desvío Dinámica */}
                                        <div className={styles.popupFooter}>
                                            {truck.IsCompleted ? (
                                                <div style={{ display: 'flex', alignItems: 'center', gap: '6px', color: '#10b981', fontWeight: 'bold', fontSize: '12px' }}>
                                                    <CheckCircle2 size={16} />
                                                    <span>Viaje Finalizado Exitosamente</span>
                                                </div>
                                            ) : truck.IsDeviated ? (
                                                <div className={styles.alertDeviated}>
                                                    <span>⚠️</span>
                                                    <span>ALERTA: Fuera de Ruta</span>
                                                </div>
                                            ) : (
                                                <div className={styles.alertNormal}>
                                                    <div className={styles.dotGreen}></div>
                                                    <span>Ruta Correcta</span>
                                                </div>
                                            )}                                     
                                        </div>
                                    </div>
                                </Popup>

                            </Marker>
                        )
                    })}

                    {selectedTruck && (
                        <AutoPan
                            position={[selectedTruck.Latitude, selectedTruck.Longitude]} 
                            isTracking={isTracking}
                        />
                    )}
                    
                </MapContainer>
            </div>
        </div>
    );
};
