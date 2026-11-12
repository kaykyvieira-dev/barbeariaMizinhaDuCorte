using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MizinhaDuCorte.Models;

public class Servico
{
    [Key]
    public long Id { get; set; }

    [Required(ErrorMessage = "Informe o nome do serviço")]
    public string Nome { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    public decimal Preco { get; set; }

    public string Tempo { get; set; } = "30 min";
}