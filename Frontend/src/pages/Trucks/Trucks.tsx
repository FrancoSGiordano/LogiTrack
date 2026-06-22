import { useState } from "react"
import { Truck, Plus, Edit2 } from 'lucide-react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import styles from './Trucks.module.css'
import type { Truck as TruckType, TruckDetailsPayload, ValidTruckStatus } from '../../types/index'
import TruckModal from "../../components/TruckModal/TruckModal";
import { fleetService } from "../../services/fleetService";

export default function Trucks() {

    const queryClient = useQueryClient()

    const [isModalOpen, setIsModalOpen] = useState(false);
    const [selectedTruck, setSelectedTruck] = useState<TruckType | null>(null);

    const handleOpenModal = (truck?: TruckType) => {
        setSelectedTruck(truck || null);
        setIsModalOpen(true);
    }

    const { data: trucks, isLoading } = useQuery({
        queryKey: ["trucks"],
        queryFn: fleetService.getTrucks
    })

    const createMutation = useMutation({
        mutationFn: (data: {payload : TruckDetailsPayload}) => 
            fleetService.createTruck(data.payload),

        onSuccess: () => {
            queryClient.invalidateQueries({queryKey: ['trucks']})
        }
    })

    const handleCreate = (payload: TruckDetailsPayload) => {
        createMutation.mutate({payload})
    }

    const updateMutation = useMutation({
        mutationFn: (data: {id: string, payload: TruckDetailsPayload}) =>
            fleetService.updateTruckDetails(data.id, data.payload),

        onSuccess: () => {
            queryClient.invalidateQueries({queryKey: ['trucks']})
        }
    })

    const handleUpdate = (id: string, payload: TruckDetailsPayload) => {
        updateMutation.mutate({id, payload})
    }

    const statusMutation = useMutation({
        mutationFn: (data: {id: string, payload: string}) => 
            fleetService.updateTruckStatus(data.id, data.payload),

        onSuccess: () => {
            queryClient.invalidateQueries({queryKey: ['trucks']})
        }
    })

    const handleStatusChange = (id: string, newStatus: ValidTruckStatus) => {
        
        statusMutation.mutate({id, payload: newStatus})
    }

    

    const handleSaveTruck = async (id: string, payload: TruckDetailsPayload) => {
        if (selectedTruck) {
            handleUpdate(id, payload)
        } else {
            handleCreate(payload)
        }
    };

    const renderStatusBadge = (truck: TruckType) => {
        let badgeClass = styles.badgeAvailable;
        if (truck.status === 'OnRoute') badgeClass = styles.badgeOnTrip;
        if (truck.status === 'UnderMaintenance') badgeClass = styles.badgeMaintenance;
        if (truck.status === 'OutOfService') badgeClass = styles.badgeOutOfService;

        return(
            <select
                value={truck.status}
                onChange={(e : React.ChangeEvent<HTMLSelectElement>) => handleStatusChange(truck.id, e.target.value as ValidTruckStatus)}
                disabled={statusMutation.isPending}
                className={`${styles.badge} ${badgeClass} ${styles.statusSelect}`}
            >
                <option value="InBase">En Base</option>
                <option value="OnRoute">En Ruta</option>
                <option value="UnderMaintenance">En Taller</option>
                <option value="OutOfService">Fuera de Servicio</option>
            </select>
        )
    };

    if (isLoading) return <div className={styles.loading}>Cargando flota...</div>;

    return (
        <div className={styles.container}>
            <header className={styles.header}>
                <h1 className={styles.title}>
                    <Truck color="#60a5fa" size={28}/> Gestión de Flota
                </h1>
                <button className={styles.addButton} onClick={() => handleOpenModal()}>
                    <Plus size={20}/> Nuevo Camión
                </button>
            </header>

            <div className={styles.tableContainer}>
                <table className={styles.table}>
                    <thead>
                        <tr>
                            <th>ID Unidad</th>
                            <th>Patente</th>
                            <th>Modelo</th>
                            <th>Carga Maxima</th>
                            <th>Estado</th>
                            <th>Acciones</th>
                        </tr>
                    </thead>
                    <tbody>
                        {trucks && trucks.length > 0 ? (
                            trucks.map((truck) => (
                                <tr key={truck.id}>
                                    <td style={{ fontWeight: 'bold', color: '#60a5fa' }}>{truck.id}</td>
                                    <td style={{ fontFamily: 'monospace' }}>{truck.licensePlate}</td>
                                    <td>{truck.model}</td>
                                    <td>{truck.maxCargoCapacityKg} KG</td>
                                    <td>{renderStatusBadge(truck)}</td>
                                    <td>
                                        <div className={styles.actionButton}>
                                            <button 
                                                className={styles.iconBtn} 
                                                aria-label="Editar" 
                                                title="Editar"
                                                onClick={() => handleOpenModal(truck)}
                                            >
                                                <Edit2 size={18}/>
                                            </button>
                                        </div>
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan={5} style={{ textAlign: 'center', padding: '20px' }}>No hay unidades registradas.</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

            <TruckModal
                isOpen={isModalOpen}
                isEditing={!!selectedTruck}
                initialData={selectedTruck}
                onClose={() => setIsModalOpen(false)}
                onSave={handleSaveTruck}
            />
        </div>
    )
}
