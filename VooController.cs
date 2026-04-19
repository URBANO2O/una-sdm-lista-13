using AmericanAirlinesApi.Data;
using AmericanAirlinesApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmericanAirlinesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VooController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VooController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CriarVoo([FromBody] Voo voo)
        {
            var aeronaveExiste = _context.Aeronaves.Find(voo.AeronaveId);
            if (aeronaveExiste == null) return NotFound("Aeronave não encontrada.");

            var aeronaveEmUso = _context.Voos
                .Any(v => v.AeronaveId == voo.AeronaveId && v.Status == "Em Voo");

            if (aeronaveEmUso)
            {
                return Conflict("Aeronave indisponível, encontra-se em trânsito.");
            }

            _context.Voos.Add(voo);
            _context.SaveChanges();
            return CreatedAtAction(nameof(CriarVoo), new { id = voo.Id }, voo);
        }

        [HttpPatch("{id}/status")]
        public IActionResult AtualizarStatus(int id, [FromBody] string novoStatus)
        {
            var voo = _context.Voos.Find(id);
            if (voo == null) return NotFound();

            if ((voo.Status == "Finalizado" || voo.Status == "Cancelado") && novoStatus == "Em Voo")
            {
                return BadRequest("Um voo finalizado ou cancelado não pode voltar para 'Em Voo'.");
            }

            voo.Status = novoStatus;
            _context.SaveChanges();
            return Ok(voo);
        }
    }
}