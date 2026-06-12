using Fleet.Application.DTOs.Trip;
using Fleet.Application.DTOs.Truck;
using Fleet.Application.Exceptions;
using Fleet.Application.Trips.Commands.CreateTrip;
using Fleet.Application.Trips.Commands.UpdateTrip;
using Fleet.Application.Trips.Commands.UpdateTripStatus;
using Fleet.Application.Trips.Queries.GetTripById;
using Fleet.Application.Trips.Queries.GetTrips;
using Fleet.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fleet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : ControllerBase
    {
        private readonly IMediator mediator;

        public TripController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        /// <summary>
        /// Obtiene la lista completa de todos los viajes registrados en el sistema.
        /// </summary>
        /// <returns>Una lista de viajes con sus detalles básicos.</returns>
        /// <response code="200">Devuelve la lista de viajes exitosamente.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TripResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await mediator.Send(new GetTripsQuery());
            return new JsonResult(result) { StatusCode = 200 };
        }

        /// <summary>
        /// Obtiene los detalles de un viaje específico mediante su identificador único.
        /// </summary>
        /// <param name="id">El identificador único (GUID) del viaje.</param>
        /// <returns>Los detalles completos del viaje solicitado.</returns>
        /// <response code="200">Devuelve el viaje encontrado.</response>
        /// <response code="404">Si no existe ningún viaje con el ID proporcionado.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(TripResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var result = await mediator.Send(new GetTripByIdQuery(id));
                return new JsonResult(result) { StatusCode = 200 };
            }
            catch (TripNotFoundException ex)
            {
                return new JsonResult(ex.Message) { StatusCode = 404 };
            }
        }


        /// <summary>
        /// Registra un nuevo viaje en el sistema logístico.
        /// </summary>
        /// <remarks>
        /// Por defecto, todo viaje recién creado se inicializa con el estado **Pending** (Pendiente).
        /// </remarks>
        /// <param name="request">Objeto que contiene el identificador del camión que va a realizar el viaje y los datos geograficos del mismo.</param>
        /// <returns>El viaje recién creado y su ruta de acceso.</returns>
        /// <response code="201">El viaje se registró correctamente.</response>
        /// <response code="400">Si los datos enviados son inválidos.</response>
        [HttpPost]
        [ProducesResponseType(typeof(TripResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] TripRequest request)
        {
            var command = new CreateTripCommand(request.TruckId, request.Origin, request.Destination, request.OriginLat, request.OriginLon, request.DestinationLat, request.DestinationLon);
            var result = await mediator.Send(command);
            return new JsonResult(result) { StatusCode = 201 };            
        }


        /// <summary>
        /// Actualiza el estado de un viaje (En progreso, Completado).
        /// </summary>
        /// <remarks>
        /// Valores permitidos para el estado:
        /// - **InProgress**: El viaje esta en progreso.
        /// - **Completed**: El viaje fue completado.
        /// </remarks>
        /// <param name="id">El identificador único del viaje.</param>
        /// <param name="newStatus">El nuevo estado en formato de texto plano.</param>
        /// <returns>Los datos del viaje con su estado actualizado.</returns>
        /// <response code="200">El estado se actualizó correctamente.</response>
        /// <response code="400">Si el estado solicitado no existe o viola una regla de negocio.</response>
        /// <response code="404">Si no existe ningún estado con el ID proporcionado.</response>
        [HttpPut("{id:guid}/status")]
        [ProducesResponseType(typeof(TripResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string newStatus)
        {
            try
            {
                var command = new UpdateTripStatusCommand(id, newStatus);
                var result = await mediator.Send(command);
                return Ok(result);
            }
            catch (InvalidTripStatusException ex)
            {
                return new JsonResult(ex.Message) { StatusCode = 400 };
            }
            catch (TripNotFoundException ex)
            {
                return new JsonResult(ex.Message) { StatusCode = 404 };
            }
            catch (TruckUnavailableException ex)
            {
                return new JsonResult(ex.Message) { StatusCode = 400 };
            }
            catch (TripAlreadyCompletedException ex)
            {
                return new JsonResult(ex.Message) { StatusCode = 400 };
            }
        }


        /// <summary>
        /// Corrige o actualiza los datos maestros de un viaje (Solo para uso administrativo).
        /// </summary>
        /// <param name="id">El identificador único del viaje que se va a editar.</param>
        /// <param name="request">Los nuevos datos del viaje.</param>
        /// <returns>Los datos maestros actualizados del camión.</returns>
        /// <response code="200">Los datos se actualizaron correctamente.</response>
        /// <response code="400">Si los datos son inválidos.</response>
        /// <response code="404">Si no existe ningún viaje con el ID proporcionado.</response>
        [HttpPut("{id:guid}/details")]
        [ProducesResponseType(typeof(TripResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDetails(Guid id, [FromBody] TripRequest request)
        {
            try
            {
                var command = new UpdateTripCommand(id, request.TruckId, request.Origin, request.Destination, request.OriginLat, request.OriginLon, request.DestinationLat, request.DestinationLon);
                var result = await mediator.Send(command);
                return Ok(result);
            }
            catch (TripNotFoundException ex)
            {
                return new JsonResult(ex.Message) { StatusCode = 404 };
            }           
        }
    }
}
