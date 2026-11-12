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

        // Filtra apenas produtos com estoque maior que zero para o cliente não escolher itens esgotados
        ViewBag.Produtos = await _context.Produtos
            .Where(p => p.Quantidade > 0)
            .OrderBy(p => p.Nome)
            .ToListAsync();

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
    public async Task<IActionResult> CriarAgendamento([FromBody] AgendamentoRequest model)
    {
        if (model == null)
            return Json(new { sucesso = false, message = "Dados inválidos." });

        // Valida se o cliente escolheu pelo menos um serviço ou um produto
        if (!model.ServicoId.HasValue && !model.ProdutoId.HasValue)
            return Json(new { sucesso = false, message = "Escolha pelo menos um serviço ou produto." });

        var novoAgendamento = new Agendamento
        {
            NomeCliente = model.NomeCliente,
            Telefone = model.Telefone,
            Data = DateOnly.Parse(model.Data),
            Hora = TimeOnly.Parse(model.Hora),
            Status = "pendente"
        };

        // 1. Processa o Serviço (se escolhido)
        if (model.ServicoId.HasValue)
        {
            var servico = await _context.Servicos.FindAsync(model.ServicoId.Value);
            if (servico != null)
            {
                novoAgendamento.ServicoNome = servico.Nome;
                novoAgendamento.PrecoServico = servico.Preco;
            }
        }
        else
        {
            novoAgendamento.ServicoNome = "Apenas Compra de Produto";
            novoAgendamento.PrecoServico = 0;
        }

        // 2. Processa o Produto (se escolhido) e dá baixa no estoque
        if (model.ProdutoId.HasValue)
        {
            var produto = await _context.Produtos.FindAsync(model.ProdutoId.Value);
            if (produto != null && produto.Quantidade > 0)
            {
                // Soma o preço do produto ao total do atendimento/venda
                novoAgendamento.PrecoServico += produto.Preco;

                // Dá baixa no estoque
                produto.Quantidade -= 1;
                _context.Produtos.Update(produto);
            }
            else
            {
                return Json(new { sucesso = false, message = "Produto esgotado ou inválido." });
            }
        }

        _context.Agendamentos.Add(novoAgendamento);
        await _context.SaveChangesAsync();

        return Json(new { sucesso = true, message = "Solicitação realizada com sucesso!" });
    }
}

// Classe auxiliar para receber os IDs vindos do JavaScript do Front-End
public class AgendamentoRequest
{
    public string NomeCliente { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public long? ServicoId { get; set; }
    public string ServicoNome { get; set; } = string.Empty;
    public decimal PrecoServico { get; set; }
    public long? ProdutoId { get; set; }
    public string Data { get; set; } = string.Empty;
    public string Hora { get; set; } = string.Empty;
}