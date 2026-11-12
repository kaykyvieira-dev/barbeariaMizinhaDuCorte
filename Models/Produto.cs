using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MizinhaDuCorte.Models;

public class Produto
{
    [Key]
    public long Id { get; set; }

    [Required(ErrorMessage = "Informe o nome do produto")]
    public string Nome { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    public decimal Preco { get; set; }

    public int Quantidade { get; set; }
}