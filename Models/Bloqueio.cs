using System.ComponentModel.DataAnnotations;

namespace MizinhaDuCorte.Models;

public class Bloqueio
{
    [Key]
    public long Id { get; set; }

    public DateOnly Data { get; set; }

    public TimeOnly? Hora { get; set; } // Nullable para quando o dia inteiro estiver bloqueado
}