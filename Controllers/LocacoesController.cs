using DriveNow.API.Data;
using DriveNow.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DriveNow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocacoesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LocacoesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Locacao>>> GetLocacoes()
        {
            var locacoes = await _context.Locacoes
                .Include(l => l.Cliente)
                .Include(l => l.Veiculo)
                .ToListAsync();

            if (locacoes == null) return NotFound();
            return Ok(locacoes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Locacao>> GetLocacao(int id)
        {
            var locacao = await _context.Locacoes
                .Include(l => l.Cliente)
                .Include(l => l.Veiculo)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (locacao == null) return NotFound();
            return Ok(locacao);
        }

        [HttpPost]
        public async Task<ActionResult<Locacao>> PostLocacao(Locacao locacao)
        {
            var cliente = await _context.Clientes.FindAsync(locacao.ClienteId);
            var veiculo = await _context.Veiculos.FindAsync(locacao.VeiculoId);

            if (cliente == null) return NotFound("Cliente inválido");
            if (veiculo == null) return NotFound("Veículo inválido");

            if (locacao.DataDevolucao <= locacao.DataRetirada)
                return BadRequest("Data de devolução deve ser maior que a de retirada");

            var agora = DateTime.Now;

            var veiculoOcupado = await _context.Locacoes
                .AnyAsync(l => l.VeiculoId == locacao.VeiculoId
                            && agora >= l.DataRetirada
                            && agora <= l.DataDevolucao);

            if (veiculoOcupado)
                return BadRequest("Veículo já está alugado no momento");

            var dias = (locacao.DataDevolucao - locacao.DataRetirada).Days;

            if (dias <= 0)
                return BadRequest("A locação deve ter pelo menos 1 dia");

            locacao.ValorTotal = (double)(dias * veiculo.ValorDiaria);

            await _context.AddAsync(locacao);
            await _context.SaveChangesAsync();

            return Ok(locacao);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Locacao>> DeleteLocacao(int id)
        {
            var locacao = await _context.Locacoes.FindAsync(id);

            if (locacao == null) return NotFound();

            _context.Remove(locacao);
            await _context.SaveChangesAsync();

            return Ok(locacao);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Locacao>> UpdateLocacao(Locacao locacao, int id)
        {
            var oldLocacao = await _context.Locacoes.FindAsync(id);

            if (oldLocacao == null) return NotFound();
            if (id != locacao.Id)
                return BadRequest("Id do objeto e da rota são diferentes!");

            oldLocacao.ClienteId = locacao.ClienteId;
            oldLocacao.VeiculoId = locacao.VeiculoId;
            oldLocacao.DataRetirada = locacao.DataRetirada;
            oldLocacao.DataDevolucao = locacao.DataDevolucao;

            var dias = (locacao.DataDevolucao - locacao.DataRetirada).Days;

            if (dias <= 0)
                return BadRequest("A locação deve ter pelo menos 1 dia");

            var veiculo = await _context.Veiculos.FindAsync(locacao.VeiculoId);

            if (veiculo == null)
                return NotFound("Veículo inválido");

            oldLocacao.ValorTotal = (double)(dias * veiculo.ValorDiaria);

            _context.Update(oldLocacao);
            await _context.SaveChangesAsync();

            return Ok(oldLocacao);
        }
    }
}