using System.ComponentModel.DataAnnotations;

namespace DriveNow.API.Models
{
    public class Locacao
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Cliente é obrigatório.")]
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        [Required(ErrorMessage = "Veículo é obrigatório.")]
        public int VeiculoId { get; set; }
        public Veiculo Veiculo { get; set; }

        [Required(ErrorMessage = "Data de retirada é obrigatória.")]
        public DateTime DataRetirada { get; set; }

        [Required(ErrorMessage = "Data de devolução é obrigatória.")]
        public DateTime DataDevolucao { get; set; }

        public double ValorTotal { get; set; }
    }
}
