using Microsoft.EntityFrameworkCore;
using MizinhaDuCorte.Models;

namespace MizinhaDuCorte.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Servico> Servicos { get; set; } = null!;
    public DbSet<Produto> Produtos { get; set; } = null!;
    public DbSet<Agendamento> Agendamentos { get; set; } = null!;
    public DbSet<DiaAberto> DiasAbertos { get; set; } = null!;
    public DbSet<Bloqueio> Bloqueios { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Mapeamento da tabela 'servicos'
        modelBuilder.Entity<Servico>(entity =>
        {
            entity.ToTable("servicos");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nome).HasColumnName("nome");
            entity.Property(e => e.Preco).HasColumnName("preco");
            entity.Property(e => e.Tempo).HasColumnName("tempo");
        });

        // Mapeamento da tabela 'produtos'
        modelBuilder.Entity<Produto>(entity =>
        {
            entity.ToTable("produtos");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nome).HasColumnName("nome");
            entity.Property(e => e.Preco).HasColumnName("preco");
            entity.Property(e => e.Quantidade).HasColumnName("quantidade");
        });

        // Mapeamento da tabela 'agendamentos'
        modelBuilder.Entity<Agendamento>(entity =>
        {
            entity.ToTable("agendamentos");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NomeCliente).HasColumnName("nome_cliente");
            entity.Property(e => e.Telefone).HasColumnName("telefone");
            entity.Property(e => e.ServicoNome).HasColumnName("servico_nome");
            entity.Property(e => e.PrecoServico).HasColumnName("preco_servico");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.Hora).HasColumnName("hora");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        // Mapeamento da tabela 'dias_abertos'
        modelBuilder.Entity<DiaAberto>(entity =>
        {
            entity.ToTable("dias_abertos");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.HoraInicio).HasColumnName("hora_inicio");
            entity.Property(e => e.HoraFim).HasColumnName("hora_fim");
        });

        // Mapeamento da tabela 'bloqueios'
        modelBuilder.Entity<Bloqueio>(entity =>
        {
            entity.ToTable("bloqueios");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.Hora).HasColumnName("hora");
        });
    }
}