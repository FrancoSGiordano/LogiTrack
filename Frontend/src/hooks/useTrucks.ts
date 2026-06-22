import { useQuery } from "@tanstack/react-query"
import { fleetService } from "../services/fleetService"


export const useTrucks = () => {
    return useQuery({
        queryKey: ['trucks'],
        queryFn: async () => fleetService.getTrucks()
    });
}