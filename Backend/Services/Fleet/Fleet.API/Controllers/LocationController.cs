using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fleet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly HttpClient httpClient;

        public LocationController(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            httpClient.DefaultRequestHeaders.Add("User-Agent", "LogiTrack/1.0 (lineasdecampo@gmail.com)");
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "es-AR,es;q=0.9");
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchAddress([FromQuery] string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("El término de busqueda es requerido.");
            }

            var baseUrl = "https://nominatim.openstreetmap.org/search";
            var queryString = $"format=json&q={Uri.EscapeDataString(query)}&limit=5&countrycodes=ar";
            var fullUrl = $"{baseUrl}?{queryString}";

            using var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
            request.Headers.Add("User-Agent", "LogiTrack/1.0 (lineasdecampo@gmail.com)");
            request.Headers.Add("Accept-Language", "es-AR,es;q=0.9");

            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Error en el servidor de mapas");

            var content = await response.Content.ReadAsStringAsync();

            return new ContentResult
            {
                Content = content,
                ContentType = "application/json",
                StatusCode = 200
            };
        }
    }
}
