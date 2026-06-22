import { useEffect, useState } from "react"
import type { Truck, TruckDetailsPayload} from "../../types"
import styles from './TruckModal.module.css'

type TruckModalProps = {
    isOpen: boolean,
    isEditing: boolean,
    initialData: Truck | null;
    onClose: () => void;
    onSave: (id: string, payload: TruckDetailsPayload) => void
}

export default function TruckModal({ isOpen, isEditing, initialData, onClose, onSave } : TruckModalProps) {

    const [truckId, setTruckId] = useState('');
    
    const [formData, setFormData] = useState<TruckDetailsPayload>({
        model: '',
        licensePlate: '',
        maxCargoCapacityKg: 0,
    })

    const [errors, setErrors] = useState<{
        model?: string,
        licensePlate?: string,
        maxCargoCapacityKg?: string
    }>({})

  
    useEffect(() => {

        setErrors({})

        if(initialData && isOpen){
            setTruckId(initialData.id)
            setFormData({
              model: initialData.model,
              licensePlate: initialData.licensePlate,
              maxCargoCapacityKg : initialData.maxCargoCapacityKg
            })
        } else if (isOpen) {
            setFormData({
                model: "",
                licensePlate: "",
                maxCargoCapacityKg: 0
            });
        }
    }, [isOpen, initialData]);

    if(!isOpen) return null;

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setFormData({ ...formData, [name]: value})

        if(errors[name as keyof typeof errors]){
            setErrors({ ...errors, [name]: undefined})
        }
    }

    const validateForm = (): boolean => {
        const newErrors : {
            model?: string,
            licensePlate?: string,
            maxCargoCapacityKg?: string
        } = {}

        let isValid = true;

        if(!formData.model.trim()){
            newErrors.model = 'El modelo es obligatorio';
            isValid = false;
        }

        if (!formData.licensePlate.trim()) {
            newErrors.licensePlate = 'La patente es obligatoria.';
            isValid = false;
        } else if (formData.licensePlate.trim().length < 6) {
            newErrors.licensePlate = 'La patente debe tener al menos 6 caracteres.';
            isValid = false;
        }

        if(formData.maxCargoCapacityKg <= 0){
            newErrors.maxCargoCapacityKg = 'La capacidad de carga debe ser mayor que cero.';
            isValid = false
        }

        setErrors(newErrors);
        return isValid;
    }

    const handleSubmit = (e: React.SubmitEvent) => {
        e.preventDefault();

        if (!validateForm()) {
            return; 
        }

        onSave(truckId, formData);
        onClose()
    };
    
    return (
        <div className={styles.modalOverlay} onClick={onClose}>
          <div className={styles.modal} onClick={(e) => e.stopPropagation()}>
            <h2 className={styles.modalTitle}>
              {isEditing ? 'Editar Camión' : 'Nuevo Camión'}
            </h2>
            
            <form onSubmit={handleSubmit}>
              
              <div className={styles.formGroup}>
                <label>Modelo del Vehículo</label>
                <input 
                  type="text" 
                  name="model" 
                  value={formData.model} 
                  onChange={handleChange} 
                  className={styles.formInput} 
                />
                {errors.model && <span className={styles.errorText}>{errors.model}</span>}
              </div>

              <div className={styles.formGroup}>
                <label>Patente</label>
                <input 
                  type="text" 
                  name="licensePlate" 
                  value={formData.licensePlate} 
                  onChange={handleChange} 
                  className={styles.formInput} 
                />
                {errors.licensePlate && <span className={styles.errorText}>{errors.licensePlate}</span>}
              </div>

              <div className={styles.formGroup}>
                <label>Capacidad de Carga</label>
                <input 
                  type="number" 
                  name="maxCargoCapacityKg" 
                  value={formData.maxCargoCapacityKg} 
                  onChange={handleChange} 
                  className={styles.formInput} 
                />
                {errors.maxCargoCapacityKg && <span className={styles.errorText}>{errors.maxCargoCapacityKg}</span>}
              </div>

              <div className={styles.modalActions}>
                <button type="button" className={styles.cancelBtn} onClick={onClose}>
                  Cancelar
                </button>
                <button type="submit" className={styles.saveBtn}>
                  Guardar Cambios
                </button>
              </div>
            </form>
          </div>
        </div>
    )
}
