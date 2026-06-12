using Fleet.Application.DTOs.Truck;
using Fleet.Application.Exceptions;
using Fleet.Application.Trucks.Commands.CreateTruck;
using Fleet.Application.Trucks.Commands.UpdateTruck;
using Fleet.Application.Trucks.Commands.UpdateTruckStatus;
using Fleet.Application.Trucks.Queries.GetTruckById;
using Fleet.Application.Trucks.Queries.GetTrucks;
using Fleet.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fleet.API.Controllers
{
    /// <summary>
    /// Controlador para la gestión de la flota de camiones.
    /// Permite registrar, consultar y actualizar el estado de los vehículos.
    /// </summary>
    [ApiController]
    [Route("/api/[controller]")]
    public class TruckController : ControllerBase
    {
        private readonly IMediator mediator;
        public TruckController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        /// <summary>
        /// Obtiene la lista completa de todos los camiones registrados en la flota.
        /// </summary>
        /// <returns>Una lista de camiones con sus detalles básicos.</returns>
        /// <response code="200">Devuelve la lista de camiones exitosamente.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TruckResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await mediator.Send(new GetTrucksQuery());
            return new JsonResult(result) { StatusCode = 200 };
        }

        /// <summary>
        /// Obtiene los detalles de un camión específico mediante su identificador único.
        /// </summary>
        /// <param name="id">El identificador único (GUID) del camión.</param>
        /// <returns>Los detalles completos del camión solicitado.</returns>
        /// <response code="200">Devuelve el camión encontrado.</response>
        /// <response code="404">Si no existe ningún camión con el ID proporcionado.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(TruckResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var result = await mediator.Send(new GetTruckByIdQuery(id));
                return new JsonResult(result) { StatusCode = 200 };
            }
            catch(TruckNotFoundException ex)
            {
                return new JsonResult(ex.Message) { StatusCode = 404 };
            }
        }

        /// <summary>
        /// Registra un nuevo camión en el sistema logístico.
        /// </summary>
        /// <remarks>
        /// Por defecto, todo camión recién creado se inicializa con el estado **InBase** (En Base).
        /// </remarks>
        /// <param name="request">Objeto que contiene el modelo, la patente y la capacidad máxima de carga en Kg.</param>
        /// <returns>El camión recién creado y su ruta de acceso.</returns>
        /// <response code="201">El camión se registró correctamente.</response>
        /// <response code="400">Si los datos enviados son inválidos (ej. patente vacía/repetida o capacidad menor a cero).</response>
        [HttpPost]
        [ProducesResponseType(typeof(TruckResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] TruckRequest request)
        {
            try
            {
                var command = new CreateTruckCommand(request.Model, request.LicensePlate, request.MaxCargoCapacityKg);
                var result = await mediator.Send(command);
                return new JsonResult(result) { StatusCode = 201 };
            }
            catch (LicensePlateAlreadyExistsException ex)
            {
                return new JsonResult(ex.Message) { StatusCode = 400 };
            }
            catch (ArgumentException ex)
            {
                return new JsonResult(ex.Message) { StatusCode = 400 };
            }

        }


        /// <summary>
        /// Actualiza el estado operativo de un camión (Despachar, Retornar a base, Enviar a taller).
        /// </summary>
        /// <remarks>
        /// Valores permitidos para el estado:
        /// - **OnRoute**: El camión sale a realizar entregas.
        /// - **InBase**: El camión está estacionado y disponible.
        /// - **UnderMaintenance**: El camión está inhabilitado por reparaciones.
        /// - **OutOfService**: El camión esta fuera de servicio.
        /// </remarks>
        /// <param name="id">El identificador único del camión.</param>
        /// <param name="newStatus">El nuevo estado en formato de texto plano.</param>
        /// <returns>Los datos del camión con su estado actualizado.</returns>
        /// <response code="200">El estado se actualizó correctamente.</response>
        /// <response code="400">Si el estado solicitado no existe o viola una regla de negocio.</response>
        /// <response code="404">Si no existe ningún camión con el ID proporcionado.</response>
        [HttpPut("{id:guid}/status")]
        [ProducesResponseType(typeof(TruckResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string newStatus)
        {
            try 
            {
                var command = new UpdateTruckStatusCommand(id, newStatus);
                var result = await mediator.Send(command);
                return Ok(result);
            }
            catch (TruckNotFoundException ex)
            {
                return new JsonResult(ex.Message) { StatusCode = 404 };
            }
            catch (ArgumentException ex)
            {
                return new JsonResult(ex.Message) { StatusCode = 400 };
            }
            catch (TruckUnavailableException ex)
            {
                return new JsonResult(ex.Message) { StatusCode = 400 };
            }
        }


        /// <summary>
        /// Corrige o actualiza los datos maestros de un camión (Solo para uso administrativo).
        /// </summary>
        /// <param name="id">El identificador único del camión que se va a editar.</param>
        /// <param name="request">Los nuevos datos de modelo, patente o capacidad de carga.</param>
        /// <returns>Los datos maestros actualizados del camión.</returns>
        /// <response code="200">Los datos se actualizaron correctamente.</response>
        /// <response code="400">Si los datos son inválidos.</response>
        /// <response code="404">Si no existe ningún camión con el ID proporcionado.</response>
        [HttpPut("{id:guid}/details")]
        [ProducesResponseType(typeof(TruckResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDetails(Guid id, [FromBody] TruckRequest request)
        {
            try
            {
                var command = new UpdateTruckCommand(id, request.Model, request.LicensePlate, request.MaxCargoCapacityKg);
                var result = await mediator.Send(command);
                return Ok(result);
            }
            catch (TruckNotFoundException ex)
            {
                return new JsonResult(ex.Message) { StatusCode = 404 };
            }
            catch (ArgumentException ex)
            {
                return new JsonResult(ex.Message) { StatusCode = 400 };
            }
            catch (LicensePlateAlreadyExistsException ex)
            {
                return new JsonResult(ex.Message) { StatusCode = 400 };
            }
        }

    }
}
