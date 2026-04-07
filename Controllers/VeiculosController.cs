using DriveNow.API.Data;
using DriveNow.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Consultorio.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VeiculosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VeiculosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Veiculo>>> GetVeiculos()
        {
            var veiculos = await _context.Veiculos.Include(v => v.Agencia).ToListAsync();
            return Ok(veiculos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Veiculo>> GetVeiculo(int id)
        {
            var veiculo = await _context.Veiculos
                .Include(v => v.Agencia)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (veiculo == null)
                return NotFound("Veículo não encontrado");

            return Ok(veiculo);
        }

        [HttpPost]
        public async Task<ActionResult<Veiculo>> PostVeiculo(Veiculo veiculo)
        {
            var agencia = await _context.Agencias.FindAsync(veiculo.AgenciaId);
            if (agencia == null) return NotFound();
            veiculo.Agencia = agencia;
            await _context.AddAsync(veiculo);
            await _context.SaveChangesAsync();
            return Ok(veiculo);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Veiculo>> DeleteVeiculo(int id)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null) NotFound();
            _context.Remove(veiculo);
            await _context.SaveChangesAsync();
            return Ok(veiculo);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Veiculo>> UpdateVeiculo(Veiculo veiculo, int id)
        {
            var veiculoAntigo = await _context.Veiculos.FindAsync(id);
            if (veiculoAntigo == null || veiculo.Id != veiculoAntigo.Id) return NotFound();

            var agencia = await _context.Agencias.FindAsync(veiculo.AgenciaId);
            veiculoAntigo.Agencia = agencia;

            veiculoAntigo.Modelo = veiculo.Modelo;
            veiculoAntigo.Placa = veiculo.Placa;
            veiculoAntigo.ValorDiaria = veiculo.ValorDiaria;

            _context.Veiculos.Update(veiculoAntigo);
            await _context.SaveChangesAsync();
            return Ok(veiculoAntigo);

        }
    }
}