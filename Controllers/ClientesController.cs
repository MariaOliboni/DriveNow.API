using DriveNow.API.Data;
using DriveNow.API.Models;
using DriveNow.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DriveNow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly CpfService _cpfService;
        private readonly EmailService _emailService;

        public ClientesController(AppDbContext context, CpfService cpfService, EmailService emailService)
        {
            _context = context;
            _cpfService = cpfService;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            var clientes = await _context.Clientes.ToListAsync();
            return Ok(clientes);
        }

        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
        {
            if (!_emailService.ValidarEmail(cliente.Email))
                throw new Exception("Email inválido");

            var cpfSemFormatacao = new string(cliente.Cpf.Where(char.IsDigit).ToArray());
            if (!_cpfService.ValidarCpf(cpfSemFormatacao))
                throw new Exception("CPF inválido");

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCliente), new { id = cliente.Id }, cliente);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Cliente>> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
                return NotFound();

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return Ok(cliente);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Cliente>> UpdateCliente(int id, Cliente cliente)
        {
            var clienteOld = await _context.Clientes.FindAsync(id);

            if (clienteOld == null || cliente.Id != clienteOld.Id)
                return NotFound();

            if (!_emailService.ValidarEmail(cliente.Email))
                throw new Exception("Email inválido");

            var cpfSemFormatacao = new string(cliente.Cpf.Where(char.IsDigit).ToArray());
            if (!_cpfService.ValidarCpf(cpfSemFormatacao))
                throw new Exception("CPF inválido");

            clienteOld.Nome = cliente.Nome;
            clienteOld.Email = cliente.Email;
            clienteOld.Cpf = cliente.Cpf;

            _context.Update(clienteOld);
            await _context.SaveChangesAsync();

            return Ok(clienteOld);
        }
    }
}