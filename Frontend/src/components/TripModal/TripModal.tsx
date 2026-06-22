import { useEffect, useState } from "react"
import  type { GeocodeResult, Trip, TripPayloadData } from "../../types";
import styles from './TripModal.module.css'
import { useTrucks } from "../../hooks/useTrucks";
import { API_FLEET_URL } from "../../services/api";
import { useDebounce } from "../../hooks/useDebounce";

type TripModalProps = {
    isOpen: boolean,
    isEditing: boolean,
    initialData: Trip | null,
    onClose: () => void,
    onSave: (id: string, payload: TripPayloadData) => void
}

export default function TripModal({isOpen, isEditing, initialData, onClose, onSave} : TripModalProps) {

    const { data: availableTrucks = [] } = useTrucks()

    const [tripId, setTripId] = useState('');
    const [originName, setOriginName] = useState('');
    const [destinationName, setDestinationName] = useState('');
    const [selectTruckId, setSelectedTruckId] = useState<string | undefined>('');

    const [tripPayload, setTripPayload] = useState<TripPayloadData>({
        truckId: '',
        origin: '',
        destination: '',
        originLat: 0.0,
        originLon: 0.0,
        destinationLat: 0.0,
        destinationLon: 0.0,
    })

    const [originResults, setOriginResults] = useState<GeocodeResult[]>([]);
    const [destinationResults, setDestinationResults] = useState<GeocodeResult[]>([]);
    const [isSearchingOrigin, setIsSearchingOrigin] = useState(false);
    const [isSearchingDestination, setIsSearchingDestination] = useState(false);

    const debouncedOrigin = useDebounce(originName, 1000);
    const debouncedDestination = useDebounce(destinationName, 1000);

    useEffect(() => {
        if(debouncedOrigin && debouncedOrigin !== tripPayload.origin && debouncedOrigin === originName){
            searchAddress(debouncedOrigin, setOriginResults, setIsSearchingOrigin)
        };
    }, [debouncedOrigin, tripPayload.origin, originName]);

    useEffect(() => {
        if(debouncedDestination && debouncedDestination !== tripPayload.destination && debouncedDestination === destinationName){
            searchAddress(debouncedDestination, setDestinationResults, setIsSearchingDestination)
        };
    }, [debouncedDestination, tripPayload.destination, destinationName]);

    const [errors, setErrors] = useState<{
        origin?: string,
        destination?: string,
        truck?: string
    }>({})

    const searchAddress = async (query: string, setResults: (res: GeocodeResult[]) => void, setLoading: (s: boolean) => void) => {
        if(!query.trim()) {
            setResults([]);
            return;
        }
        setLoading(true);
        try{
            const response = await fetch(`${API_FLEET_URL}/location/search?query=${encodeURIComponent(query)}`)
            const data = await response.json()
            setResults(data);
        } catch (error) {
            console.error("Error buscando dirección:", error);
        } finally {
            setLoading(false);
        }
    };

    const selectOrigin = (place: GeocodeResult) => {

        setOriginName(place.display_name);

        setTripPayload(prev => ({
            ...prev,
            origin: place.display_name,
            originLat: parseFloat(place.lat),
            originLon: parseFloat(place.lon)
        }))
        setOriginResults([]);
    };

    const selectDestination = (place: GeocodeResult) => {

        setDestinationName(place.display_name);

        setTripPayload(prev => ({
            ...prev,
            destination: place.display_name,
            destinationLat: parseFloat(place.lat),
            destinationLon: parseFloat(place.lon)
        }))
        setDestinationResults([]);
    }

    useEffect(() => {
        setErrors({})
        setOriginResults([]);
        setDestinationResults([]);
        if(initialData && isOpen){
            setTripId(initialData.id)
            setOriginName(initialData.origin)
            setDestinationName(initialData.destination)
            setSelectedTruckId(initialData.truckId || undefined)
            setTripPayload({
                truckId: initialData.truckId,
                origin: initialData.origin,
                destination: initialData.destination,
                originLat: initialData.originLat,
                originLon: initialData.originLon,
                destinationLat: initialData.destinationLat,
                destinationLon: initialData.destinationLon,
            })
        } else if (isOpen){
            setTripId('');
            setOriginName('');
            setDestinationName('');
            setSelectedTruckId('');
            setOriginResults([]);
            setDestinationResults([]);
        }
    }, [initialData, isOpen])

    const validateForm = () : boolean => {
        const newErrors : {
            origin?: string,
            destination?: string,
            truck?: string
        } = {}

        let isValid = true;

        if(!tripPayload.origin.trim()) {
            newErrors.origin = "Por favor, seleccione un origen";
            isValid = false;
        }

        if(!tripPayload.destination.trim()) {
            newErrors.destination = "Por favor, seleccione un destino";
            isValid = false;
        }

        if(initialData && initialData.startedAt) {
            newErrors.truck = "No puedes asignar otra unidad, el viaje ya comenzó";
            isValid = false;
        }

        setErrors(newErrors)
        return isValid;
    } 

    const handleSubmit = (e: React.SubmitEvent) => {
        e.preventDefault();

        if(!validateForm()) return;
        

        const finalPayload = {
            ...tripPayload, 
            truckId: (!selectTruckId || selectTruckId === "") ? null : selectTruckId
        };

        console.log(finalPayload) 

        onSave(tripId, finalPayload);
        onClose();
    }

    const handleTruckChange = (id: string) => {
        setSelectedTruckId(id);
        setTripPayload(prev => ({
            ...prev,
            truckId: id === "" ? null : id
        }))
    }

    if(!isOpen) return null;


    return (
        <div className={styles.modalOverlay} onClick={onClose}>
            <div className={styles.modal} onClick={e => e.stopPropagation()}>
                <div className={styles.modalHeader}>
                    <h2>{isEditing ? 'Modificar Viaje' : 'Crear Viaje'}</h2>
                </div>

                <form onSubmit={handleSubmit} className={styles.form}>
                    <div className={styles.formSection}>
                        <h3>📍 Origen</h3>
                        <div className={styles.searchContainer}>
                            <input 
                                type="text" 
                                placeholder="Buscar dirección o ciudad..."
                                value={originName}
                                onChange={(e) => {
                                    const val = e.target.value
                                    setOriginName(val)

                                    if (val.trim() !== "" && val !== tripPayload.origin) {
                                        setIsSearchingOrigin(true);
                                    } else {
                                        setIsSearchingOrigin(false);
                                        setOriginResults([]);
                                    }
                                }}
                                className={styles.inputFull} 
                            />
                        </div>
                        {isSearchingOrigin && <span className={styles.searchingCoords}>Buscando...</span>}
                        {originResults.length > 0 && (
                            <ul className={styles.searchResults}>
                                {originResults.map((place, idx) => (
                                    <li key={idx} onClick={() => selectOrigin(place)}>
                                        {place.display_name}
                                    </li>
                                ))}
                            </ul>
                        )}
                        {errors.origin && <span className={styles.errorText}>{errors.origin}</span>}
                    </div>

                    <div className={styles.formSection}>
                        <h3>🏁 Destino</h3>
                        <div className={styles.searchContainer}>
                            <input 
                                type="text" 
                                placeholder="Buscar dirección o ciudad..."
                                value={destinationName}
                                onChange={(e) => {
                                    const val = e.target.value;
                                    setDestinationName(val);
                                    
                                    if (val.trim() !== "" && val !== tripPayload.destination) {
                                        setIsSearchingDestination(true);
                                    } else {
                                        setIsSearchingDestination(false);
                                        setDestinationResults([]);
                                    }
                                }}
                                className={styles.inputFull}
                            />                          
                        </div>
                        {isSearchingDestination && <span className={styles.searchingCoords}>Buscando...</span>}
                        {destinationResults.length > 0 && (
                            <ul className={styles.searchResults}>
                                {destinationResults.map((place, idx) => (
                                    <li key={idx} onClick={() => selectDestination(place)}>
                                        {place.display_name}
                                    </li>
                                ))}
                            </ul>
                        )}
                        {errors.destination && <span className={styles.errorText}>{errors.destination}</span>}
                    </div>

                    <div className={styles.formSection}>
                        <h3>🚛 Asignar Unidad</h3>
                        <select
                            value={selectTruckId || ""}
                            onChange={e => handleTruckChange(e.target.value)}
                            className={styles.selectFull}
                        >
                            <option value="">-- Sin asignar --</option>
                            {availableTrucks.map(truck => (
                                <option key={truck.id} value={truck.id}>
                                    {truck.licensePlate} | {truck.model}
                                </option>
                            ))}
                        </select>
                    </div>

                    <div className={styles.modalActions}>
                        <button type="button" onClick={onClose} className={styles.cancelButton}>Cancelar</button>
                        <button type="submit" className={styles.submitButton}>Guardar</button>
                    </div>   
                </form>
            </div>
        </div>
    )
}
