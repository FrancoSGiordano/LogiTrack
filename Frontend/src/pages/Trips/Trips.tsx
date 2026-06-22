import React, { useState } from 'react'
import type { Trip, Truck as TruckType, GeocodeResult, TripPayloadData, ValidTripStatus } from '../../types'
import styles from './Trips.module.css'
import { Check, Edit2, MapPin, Play, Plus, Truck } from 'lucide-react';
import TripModal from '../../components/TripModal/TripModal';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { fleetService } from '../../services/fleetService';
import { useTrucks } from '../../hooks/useTrucks';
import { formatCompactDate } from '../../utils';

export default function Trips() {

    
    const [selectedTrip, setSelectedTrip] = useState<Trip | null>(null);
    const [isModalOpen, setIsModalOpen] = useState(false);

    const queryClient = useQueryClient();

    const { data: availableTrucks = [] } = useTrucks();

    const { data: trips, isLoading } = useQuery({
        queryKey: ["trips"],
        queryFn: fleetService.getTrips
    })

    const createMutation = useMutation({
        mutationFn: (data: {payload : TripPayloadData}) => 
            fleetService.createTrip(data.payload),
        onSuccess: () => {
            queryClient.invalidateQueries({queryKey: ["trips"]})
        }
    })

    const handleCreate = (payload: TripPayloadData) => {
        createMutation.mutate({payload})
    }

    const updateMutation = useMutation({
        mutationFn: (data: {id: string, payload: TripPayloadData}) =>
            fleetService.updateTripDetails(data.id, data.payload),
        onSuccess: () => {
            queryClient.invalidateQueries({queryKey: ['trips']})
        }
    })

    const handleUpdate = (id: string, payload: TripPayloadData) => {
        updateMutation.mutate({id, payload})
    }

    const statusMutation = useMutation({
        mutationFn: (data: {id: string, payload: string}) => 
            fleetService.updateTripStatus(data.id, data.payload),
        onSuccess: () => {
            queryClient.invalidateQueries({queryKey: ['trips']})
        }
    })

    const handleStatusChange = (id: string, newStatus: ValidTripStatus) => {
        statusMutation.mutate({id, payload: newStatus})
    }

    const handleOpenModal = (trip?: Trip) => {
        setSelectedTrip(trip || null);
        setIsModalOpen(true);
    }

    const handleSaveTrip = async (id: string, payload: TripPayloadData) => {
        if(selectedTrip){
            handleUpdate(id, payload);
        } else {
            handleCreate (payload);
        }
    }

    const handleStartTrip = async (id: string) => {
        handleStatusChange(id, "InProgress");
    }

    const handleFinishTrip = async (id: string) => {
        handleStatusChange(id, "Completed")
    }

    if (isLoading) return <div className={styles.loading}>Cargando viajes...</div>;


    return (
        <div className={styles.container}>
            <div className={styles.header}>
                <div className={styles.headerText}>
                    <h1 className={styles.title}>Gestión de Viajes</h1>
                    <p className={styles.subtitle}>Crea y despacha órdenes de transporte</p>
                </div>
                <button className={styles.createButton} onClick={() => handleOpenModal()}>
                    <Plus size={18} className={styles.iconMargin}/> Nuevo Viaje
                </button>
            </div>

            <div className={styles.tableWrapper}>
                <table className={styles.table}>
                    <thead>
                        <tr>
                            <th>Origen</th>
                            <th>Destino</th>
                            <th>Camión Asignado</th>
                            <th>Estado</th>
                            <th>Inicio</th>
                            <th>Finalización</th>
                            <th>Acciones</th>
                        </tr>
                    </thead>
                    <tbody>
                        {trips && trips.length > 0 ? (
                            trips.map(trip => {
                            const truckInfo = availableTrucks.find(t => t.id === trip.truckId);
                            return (
                                <tr key={trip.id} className={styles.row}>
                                    <td>
                                        <div className={styles.locationCell}>
                                            <MapPin size={16} className={styles.iconOrigin}/>
                                            <div>
                                                <p className={styles.locationName}>{trip.origin}</p>
                                            </div>
                                        </div>
                                    </td>
                                    <td>
                                        <div className={styles.locationCell}>
                                            <MapPin size={16} className={styles.iconDest}/>
                                            <div>
                                                <p className={styles.locationName}>{trip.destination}</p>
                                            </div>
                                        </div>
                                    </td>
                                    <td>
                                        {truckInfo ? (
                                            <div className={styles.truckCell}>
                                                <Truck size={16} className={styles.iconTruck}/>
                                                <span>{truckInfo.licensePlate} | {truckInfo.model}</span>
                                            </div>
                                        ) : (
                                            <span className={styles.unassigned}>Sin asignar</span>
                                        )}
                                    </td>
                                    <td>
                                        <span className={`${styles.statusBadge} ${styles[trip.status]}`}>
                                            {trip.status === 'Pending' && 'Pendiente'}
                                            {trip.status === 'InProgress' && 'En Ruta'}
                                            {trip.status === 'Completed' && 'Completado'}
                                        </span>
                                    </td>
                                    <td>
                                        <div className={styles.timeContainer}>
                                            {trip.startedAt ? (
                                                <div className={styles.timeRow}>
                                                    <span className={styles.timeValue}>{formatCompactDate(trip.startedAt)}</span>
                                                </div>
                                            ) : (
                                                <span className={styles.timeValue}>Pendiente</span>
                                            )}

                                        </div>
                                    </td>
                                    <td>
                                        <div className={styles.timeContainer}>
                                            {trip.completedAt ? (
                                                <div className={styles.timeRow}>
                                                    <span className={styles.timeValue}>{formatCompactDate(trip.completedAt)}</span>
                                                </div>
                                            ) : (
                                                <span className={styles.timeValue}>Pendiente</span>
                                            )}
                                        </div>
                                    </td>
                                    <td>
                                        <div className={styles.actions}>
                                            <button 
                                                className={styles.actionButton} 
                                                title='Editar Viaje'
                                                onClick={() => handleOpenModal(trip)}
                                            >
                                                <Edit2 size={16}/>
                                            </button>
                                            {trip.status === 'Pending' && (
                                                <button
                                                    className={styles.startButton}
                                                    onClick={() => handleStartTrip(trip.id)}
                                                    disabled={!trip.truckId}
                                                >
                                                    <Play size={16} className={styles.iconMargin}/> Iniciar
                                                </button>
                                            )}
                                            {trip.status === 'InProgress' && (
                                                <button
                                                    className={styles.startButton}
                                                    onClick={() => handleFinishTrip(trip.id)}
                                                >
                                                    <Check size={16} className={styles.iconMargin}/> Finalizar
                                                </button>
                                            )}
                                        </div>
                                    </td>
                                </tr>
                            )
                        })
                        ) : (
                            <tr>
                                <td colSpan={5} style={{ textAlign: 'center', padding: '20px' }}>No hay viajes registrados.</td>
                            </tr>
                        )}
                        
                    </tbody>
                </table>
            </div>

            <TripModal
                isOpen={isModalOpen}
                isEditing={!!selectedTrip}
                initialData={selectedTrip}
                onClose={() => setIsModalOpen(false)}
                onSave={handleSaveTrip}
            />
        </div>
    )
}
