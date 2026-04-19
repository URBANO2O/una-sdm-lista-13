using AmericanAirlinesApi.Data;
using AmericanAirlinesApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmericanAirlinesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReservaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CriarReserva([FromBody] Reserva reserva)
        {
            var voo = _context.Voos.Include(v => v.Aeronave).FirstOrDefault(v => v.Id == reserva.VooId);
            if (voo == null) return NotFound("Voo não encontrado.");

            var totalReservas = _context.Reservas.Count(r => r.VooId == reserva.VooId);

            // 1. Validação de Overbooking
            if (totalReservas >= voo.Aeronave.CapacidadePassageiros)
            {
                return BadRequest("Voo lotado. Não é possível realizar novas reservas.");
            }

            // 2. Lógica de Assento
            if (reserva.Assento.EndsWith("A", StringComparison.OrdinalIgnoreCase) || 
                reserva.Assento.EndsWith("F", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Assento na janela reservado com sucesso! Taxa de $50,00 aplicada.");
            }

            _context.Reservas.Add(reserva);
            _context.SaveChanges();
            return CreatedAtAction(nameof(CriarReserva), new { id = reserva.Id }, reserva);
        }
    }
}