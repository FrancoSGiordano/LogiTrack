import L from 'leaflet'

export const createTruckIcon = (color: string) => {
  return L.divIcon({
    html: `
      <div style="
        background-color: #1e293b;
        border: 2px solid ${color};
        border-radius: 50%;
        width: 36px;
        height: 36px;
        display: flex;
        align-items: center;
        justify-content: center;
        box-shadow: 0 0 12px ${color}80; /* Resplandor tipo neón */
      ">
        <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="${color}" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <path d="M10 17h4V5H2v12h3"></path>
          <path d="M20 17h2v-3.34a4 4 0 0 0-1.17-2.83L19 9h-5"></path>
          <path d="M14 17h1"></path>
          <circle cx="7.5" cy="17.5" r="2.5"></circle>
          <circle cx="17.5" cy="17.5" r="2.5"></circle>
        </svg>
      </div>
    `,
    className: '',
    iconSize: [36, 36],
    iconAnchor: [18, 18],
    popupAnchor: [0, -20]
  });
};

