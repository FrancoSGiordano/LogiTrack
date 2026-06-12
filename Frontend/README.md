# 🚛 LogiTrack - Sistema de Telemetría y Gestión de Flotas

LogiTrack es una plataforma basada en microservicios diseñada para gestionar flotas de camiones y procesar telemetría (GPS) en tiempo real, detectando desvíos de ruta mediante cálculos geométricos avanzados.

## 🛠️ Tecnologías Utilizadas

**Backend & Arquitectura**
* **.NET 10 (C#)** - Framework principal.
* **Clean Architecture** - Separación estricta en capas (Core, Application, Infrastructure, Presentation/Worker).
* **CQRS (Command Query Responsibility Segregation)** - Para la gestión de estados e histórico en las APIs.
* **Entity Framework Core** - ORM con PostgreSQL.

**Frontend**
* **React + TypeScript** - Interfaz de usuario estricta y altamente tipada para el mapa y tableros.

**Infraestructura & Datos**
* **PostgreSQL** - Base de datos relacional principal para persistencia de datos históricos y de negocio.
* **Redis** - Caché distribuido ultrarrápido (para rutas activas) y sistema de mensajería interna Pub/Sub.
* **RabbitMQ** - Message broker encargado de la ingesta masiva, asíncrona y segura de coordenadas.
* **OSRM (Open Source Routing Machine)** - API de mapas para obtener las polilíneas reales de las calles.

**Testing & Simulación**
* **Python** - Scripts de simulación que inyectan pings de coordenadas falsas directamente a RabbitMQ para someter al sistema a pruebas de estrés y validar el algoritmo de desvíos.

---

## Estructura y Responsabilidad de los Microservicios (Monorepo)

El ecosistema está dividido en componentes independientes y desacoplados que se comunican entre sí de forma eficiente:

### 1. Fleet.API
Encargado de la gestión administrativa de la flota (CRUD de camiones, choferes y estados de los viajes).
* **Flujo Clave**: Al cambiar el estado de un viaje a `INPROGRESS`, consulta la API de OSRM para descargar la polilínea topológica exacta (GeoJSON) de las calles del recorrido planeado y la inyecta inmediatamente en **Redis** con un tiempo de expiración controlado.

### 2. Tracking.Worker
Un *BackgroundService* (demonio en segundo plano) de alta disponibilidad dedicado exclusivamente a la ingesta masiva de datos.
* **Flujo Clave**: Escucha de forma asíncrona la cola `truck_pings` en **RabbitMQ**. Por cada ping recibido, rescata la ruta teórica desde **Redis** y ejecuta el algoritmo de desvío. Finalmente, persiste la coordenada en el historial de **PostgreSQL**.

### 3. Tracking.API
La puerta de entrada para la lectura y visualización de la telemetría.
* **Flujo Clave**: Expone los endpoints de consulta para ver el historial de posiciones de un camión. Además, actúa como un puente de comunicación en tiempo real: se suscribe al canal Pub/Sub de **Redis** (donde el Worker avisa que procesó un ping) y utiliza **SignalR** (WebSockets) para empujar instantáneamente las nuevas coordenadas y alertas de desvío hacia el mapa de **React**.

### 4. Tracking.Core (Librería de Dominio)
El cerebro matemático del sistema, completamente aislado de bases de datos o frameworks externos.
* **Flujo Clave**: Aloja al `GeoCalculator`, que procesa geometría esférica en base a la fórmula de *Haversine*. Transforma el ping del GPS en una proyección perpendicular contra los segmentos de la polilínea real del mapa (*Cross-Track distance*), determinando con precisión de metros si el camión sigue en su ruta o se desvió. (Deben modificarse las operaciones para calcular adecuadamente los desvios.)

---

## Próximos Pasos

* [ ] Levantar el *Hub* de **SignalR** en `Tracking.API` conectado al Pub/Sub de Redis para enviar los datos vivos a la UI.
* [ ] Conectar SignalR con el frontend para dibujar en tiempo real el recorrido de los camiones en el mapa y llevar a cabo el monitoreo.
* [ ] Implementar el servicio de **Inteligencia Artificial (Python/FastAPI)** para predecir tiempos de llegada (ETA) y detectar comportamientos anómalos en los conductores basándose en el historial de telemetría.
