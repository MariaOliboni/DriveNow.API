using DriveNow.API.Data;
using DriveNow.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DriveNow.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AgenciasController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ViaCepService _viaCepService;

        public AgenciasController(AppDbContext context, ViaCepService viaCepService)
        {
            _context = context;
            _viaCepService = viaCepService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Agencia>>> GetAgencias()
        {
            var agencias = await _context.Agencias.ToListAsync();
            return agencias;
        }

        [HttpPost]
        public async Task<ActionResult<Models.Agencia>> PostAgencia(Models.Agencia agencia)
        {
            var endereco = await _viaCepService.BuscarEnderecoAsync(agencia.Cep);

            if (endereco != null)
            {
                agencia.Logradouro = endereco.logradouro;
                agencia.Bairro = endereco.bairro;
                agencia.Localidade = endereco.localidade;
                agencia.Uf = endereco.uf;
            }

            _context.Agencias.Add(agencia);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAgencia), new { id = agencia.Id }, agencia);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Agencia>> GetAgencia(int id)
        {
            var agencia = await _context.Agencias.FindAsync(id);

            if (agencia == null)
            {
                return NotFound();
            }

            return agencia;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Models.Agencia>> DeleteAgencia(int id)
        {
            var agencia = await _context.Agencias.FindAsync(id);
            if (agencia == null) return NotFound();
            _context.Remove(agencia);
            await _context.SaveChangesAsync();
            return Ok(agencia);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Models.Agencia>> UpdateAgencia(int id, Models.Agencia agencia)
        {
            var agenciaAntiga = await _context.Agencias.FindAsync(id);
            if (agenciaAntiga == null || agencia.Id != agenciaAntiga.Id) return NotFound();

            agenciaAntiga.NomeFantasia = agencia.NomeFantasia;
            agenciaAntiga.Cep = agencia.Cep;

            _context.Update(agenciaAntiga);
            await _context.SaveChangesAsync();
            return Ok(agenciaAntiga);
        }
    }
}