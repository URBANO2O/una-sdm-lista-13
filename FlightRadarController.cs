using AmericanAirlinesApi.Data;
using Microsoft.AspNetCore.Mvc;

namespace AmericanAirlinesApi.Controllers
{
    [ApiController]
    [Route("api/radar")]
    public class FlightRadarController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FlightRadarController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("proximos-destinos")]
        public IActionResult GetProximosDestinos()
        {
            // Simulação de latência de satélite
            Thread.Sleep(2000);

            var destinos = _context.Voos
                .Where(v => v.Status == "Em Voo" || v.Status == "Agendado")
                .GroupBy(v => v.Destino)
                .Select(g => new
                {
                    Destino = g.Key,
                    QuantidadeVoos = g.Count()
                })
                .ToList();

            return Ok(destinos);
        }
    }
}