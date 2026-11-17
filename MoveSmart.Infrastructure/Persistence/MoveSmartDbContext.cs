using Microsoft.EntityFrameworkCore;
using MoveSmart.Domain.Entities;

namespace MoveSmart.Infrastructure.Persistence;

public class MoveSmartDbContext : DbContext
{
    public MoveSmartDbContext(DbContextOptions<MoveSmartDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Linha> Linhas => Set<Linha>();
    public DbSet<Favorito> Favoritos => Set<Favorito>();
    public DbSet<RelatoLotacao> Relatos => Set<RelatoLotacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(e =>
        {
            e.ToTable("Usuarios");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nome).IsRequired().HasMaxLength(100);
            e.Property(x => x.Email).IsRequired().HasMaxLength(150);
            e.HasIndex(x => x.Email).IsUnique();

            e.HasMany(x => x.Favoritos).WithOne(f => f.Usuario)
                .HasForeignKey(f => f.UsuarioId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.Relatos).WithOne(r => r.Usuario)
                .HasForeignKey(r => r.UsuarioId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Linha>(e =>
        {
            e.ToTable("Linhas");
            e.HasKey(x => x.Id);
            e.Property(x => x.Codigo).IsRequired().HasMaxLength(20);
            e.Property(x => x.Origem).IsRequired().HasMaxLength(100);
            e.Property(x => x.Destino).IsRequired().HasMaxLength(100);
            e.Property(x => x.Tipo).HasConversion<string>().HasMaxLength(20);
            e.HasIndex(x => x.Codigo).IsUnique();

            e.HasMany(x => x.Favoritos).WithOne(f => f.Linha)
                .HasForeignKey(f => f.LinhaId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.Relatos).WithOne(r => r.Linha)
                .HasForeignKey(r => r.LinhaId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Favorito>(e =>
        {
            e.ToTable("Favoritos");
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.UsuarioId, x.LinhaId }).IsUnique();
        });

        modelBuilder.Entity<RelatoLotacao>(e =>
        {
            e.ToTable("RelatosLotacao");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nivel).HasConversion<string>().HasMaxLength(10);
            e.Property(x => x.Observacao).HasMaxLength(200);
            e.HasIndex(x => new { x.LinhaId, x.DataRelato });
        });
    }
}
