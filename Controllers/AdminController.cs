using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MizinhaDuCorte.Data;
using MizinhaDuCorte.Models;

namespace MizinhaDuCorte.Controllers;

[Authorize]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AdminController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<IActionResult> Index(string filtro = "mes", int? mes = null, int? ano = null)
    {
        var agendamentos = await _context.Agendamentos
            .OrderByDescending(a => a.Data)
            .ThenByDescending(a => a.Hora)
            .ToListAsync();

        DateTime hoje = DateTime.Today;
        int anoAlvo = ano ?? hoje.Year;
        int mesAlvo = mes ?? hoje.Month;

        DateTime dtRef = new DateTime(anoAlvo, mesAlvo, 1);
        var culturePt = new CultureInfo("pt-BR");

        ViewBag.MesAtual = mesAlvo;
        ViewBag.AnoAtual = anoAlvo;
        ViewBag.NomeMes = dtRef.ToString("MMMM yyyy", culturePt);

        var dtAnterior = dtRef.AddMonths(-1);
        var dtProximo = dtRef.AddMonths(1);

        ViewBag.MesAnterior = dtAnterior.Month;
        ViewBag.AnoAnterior = dtAnterior.Year;
        ViewBag.MesProximo = dtProximo.Month;
        ViewBag.AnoProximo = dtProximo.Year;

        DateOnly hojeDateOnly = DateOnly.FromDateTime(hoje);
        var concluidos = agendamentos.Where(a => a.Status == "concluido");

        if (filtro == "hoje")
            concluidos = concluidos.Where(a => a.Data == hojeDateOnly);
        else if (filtro == "semana")
            concluidos = concluidos.Where(a => a.Data >= hojeDateOnly.AddDays(-7));
        else if (filtro == "mes")
            concluidos = concluidos.Where(a => a.Data.Month == hojeDateOnly.Month && a.Data.Year == hojeDateOnly.Year);
        else if (filtro == "ano")
            concluidos = concluidos.Where(a => a.Data.Year == hojeDateOnly.Year);

        ViewBag.Faturamento = concluidos.Sum(a => a.PrecoServico);
        ViewBag.TotalClientes = concluidos.Count();
        ViewBag.FiltroAtual = filtro;

        ViewBag.Servicos = await _context.Servicos.OrderBy(s => s.Nome).ToListAsync();
        ViewBag.Produtos = await _context.Produtos.OrderBy(p => p.Nome).ToListAsync();
        ViewBag.DiasAbertos = await _context.DiasAbertos.ToListAsync();
        ViewBag.Bloqueios = await _context.Bloqueios.ToListAsync();

        return View(agendamentos);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
            return RedirectToAction(nameof(Index));

        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string usuario, string senha)
    {
        string supabaseUrl = _configuration["Supabase:Url"] ?? string.Empty;
        string supabaseKey = _configuration["Supabase:AnonKey"] ?? string.Empty;

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);

        var payload = new { email = usuario, password = senha };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{supabaseUrl}/auth/v1/token?grant_type=password", content);

        if (response.IsSuccessStatusCode)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction(nameof(Index));
        }

        ViewBag.Erro = "E-mail ou senha incorretos no Supabase Auth!";
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> AlterarStatus(long id, string novoStatus)
    {
        var agendamento = await _context.Agendamentos.FindAsync(id);
        if (agendamento != null)
        {
            agendamento.Status = novoStatus;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ExcluirAgendamento(long id)
    {
        var agendamento = await _context.Agendamentos.FindAsync(id);
        if (agendamento != null)
        {
            _context.Agendamentos.Remove(agendamento);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> SalvarServico(Servico servico)
    {
        // Tratamento da conversão de ponto/vírgula para evitar o erro de valor multiplicado por 100
        string precoStr = Request.Form["Preco"].ToString().Replace(',', '.');
        if (decimal.TryParse(precoStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precoConvertido))
        {
            servico.Preco = precoConvertido;
        }

        if (servico.Id == 0)
            _context.Servicos.Add(servico);
        else
            _context.Servicos.Update(servico);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ExcluirServico(long id)
    {
        var s = await _context.Servicos.FindAsync(id);
        if (s != null)
        {
            _context.Servicos.Remove(s);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> SalvarProduto(Produto produto)
    {
        // Tratamento da conversão de ponto/vírgula para evitar o erro de valor multiplicado por 100
        string precoStr = Request.Form["Preco"].ToString().Replace(',', '.');
        if (decimal.TryParse(precoStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precoConvertido))
        {
            produto.Preco = precoConvertido;
        }

        if (produto.Id == 0)
            _context.Produtos.Add(produto);
        else
            _context.Produtos.Update(produto);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ExcluirProduto(long id)
    {
        var p = await _context.Produtos.FindAsync(id);
        if (p != null)
        {
            _context.Produtos.Remove(p);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> SalvarDiaAberto(DiaAberto dia)
    {
        _context.DiasAbertos.Add(dia);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> SalvarBloqueio(Bloqueio bloqueio)
    {
        _context.Bloqueios.Add(bloqueio);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}