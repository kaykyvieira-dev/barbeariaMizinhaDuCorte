using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MizinhaDuCorte.Data;
using MizinhaDuCorte.Models;

namespace MizinhaDuCorte.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Servicos = await _context.Servicos.OrderBy(s => s.Nome).ToListAsync();
        ViewBag.Produtos = await _context.Produtos.OrderBy(p => p.Nome).ToListAsync();
        ViewBag.DiasAbertos = await _context.DiasAbertos.OrderBy(d => d.Data).ToListAsync();

        return View();
    }

    [HttpGet]
    public async Task<JsonResult> ObterHorariosDisponiveis(string data)
    {
        if (!DateOnly.TryParse(data, out DateOnly dataSelecionada))
            return Json(new { erro = "Data inválida." });

        var diaAberto = await _context.DiasAbertos.FirstOrDefaultAsync(d => d.Data == dataSelecionada);
        if (diaAberto == null)
            return Json(new List<string>());

        var bloqueioTotal = await _context.Bloqueios.AnyAsync(b => b.Data == dataSelecionada && b.Hora == null);
        if (bloqueioTotal)
            return Json(new List<string>());

        var agendamentosExistentes = await _context.Agendamentos
            .Where(a => a.Data == dataSelecionada && a.Status != "falta")
            .Select(a => a.Hora.ToString())
            .ToListAsync();

        var bloqueiosHorarios = await _context.Bloqueios
            .Where(b => b.Data == dataSelecionada && b.Hora != null)
            .Select(b => b.Hora!.Value.ToString())
            .ToListAsync();

        var indisponiveis = agendamentosExistentes
            .Select(h => h.Substring(0, 5))
            .Concat(bloqueiosHorarios.Select(h => h.Substring(0, 5)))
            .ToHashSet();

        var horariosDisponiveis = new List<string>();

        TimeOnly inicio = diaAberto.HoraInicio;
        TimeOnly fim = diaAberto.HoraFim;

        while (inicio <= fim)
        {
            string slot = inicio.ToString("HH:mm");
            if (!indisponiveis.Contains(slot))
            {
                horariosDisponiveis.Add(slot);
            }
            inicio = inicio.AddMinutes(15);
        }

        return Json(horariosDisponiveis);
    }

    [HttpPost]
    public async Task<IActionResult> CriarAgendamento([FromBody] Agendamento model)
    {
        if (ModelState.IsValid)
        {
            model.Status = "pendente";
            _context.Agendamentos.Add(model);
            await _context.SaveChangesAsync();
            return Json(new { sucesso = true, message = "Agendamento realizado com sucesso!" });
        }
        return Json(new { sucesso = false, message = "Dados inválidos." });
    }
}