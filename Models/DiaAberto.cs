using System.ComponentModel.DataAnnotations;

namespace MizinhaDuCorte.Models;

public class DiaAberto
{
    [Key]
    public long Id { get; set; }

    public DateOnly Data { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly HoraFim { get; set; }
}