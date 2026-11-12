using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MizinhaDuCorte.Models;

public class Agendamento
{
    [Key]
    public long Id { get; set; }

    [Required]
    public string NomeCliente { get; set; } = string.Empty;

    [Required]
    public string Telefone { get; set; } = string.Empty;

    [Required]
    public string ServicoNome { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    public decimal PrecoServico { get; set; }

    public DateOnly Data { get; set; }

    public TimeOnly Hora { get; set; }

    public string Status { get; set; } = "pendente"; // "pendente", "concluido", "falta"
}