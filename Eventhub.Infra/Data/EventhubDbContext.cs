using Microsoft.EntityFrameworkCore;

namespace Eventhub.Infra.Data;

public class EventhubDbContext : DbContext
{
    public EventhubDbContext(DbContextOptions<EventhubDbContext> options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventhubDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Domain.Entities.Participante> Participantes { get; set; } = null!;
    public DbSet<Domain.Entities.Galeria> Galerias { get; set; } = null!;
    public DbSet<Domain.Entities.Usuario> Usuarios { get; set; } = null!;
    public DbSet<Domain.Entities.Fotos> Fotos { get; set; } = null!;
    public DbSet<Domain.Entities.Evento> Eventos { get; set; } = null!;
    public DbSet<Domain.Entities.TipoEvento> TipoEvento { get; set; } = null!;
    public DbSet<Domain.Entities.StatusEvento> StatusEvento { get; set; } = null!;
    public DbSet<Domain.Entities.EnderecoEvento> EnderecoEvento { get; set; } = null!;
    public DbSet<Domain.Entities.Notificacao> Notificacoes { get; set; } = null!;
    public DbSet<Domain.Entities.ProgramacaoEvento> ProgramacaoEvento { get; set; } = null!;
    public DbSet<Domain.Entities.ResponsavelProgramacao> ResponsavelProgramacao { get; set; } = null!;
    public DbSet<Domain.Entities.StatusProgramacao> StatusProgramacao { get; set; } = null!;
    public DbSet<Domain.Entities.Perfil> Perfis { get; set; } = null!;
    public DbSet<Domain.Entities.Modulo> Modulos { get; set; } = null!;
    public DbSet<Domain.Entities.Permissao> Permissoes { get; set; } = null!;
    public DbSet<Domain.Entities.PerfilPermissao> PerfilPermissoes { get; set; } = null!;
    public DbSet<Domain.Entities.UsuarioPermissao> UsuarioPermissoes { get; set; } = null!;
    public DbSet<Domain.Entities.StatusEnvioConvite> StatusEnvioConvites { get; set; } = null!;
    public DbSet<Domain.Entities.Presente> Presentes { get; set; } = null!;
    public DbSet<Domain.Entities.CategoriaPresente> CategoriaPresentes { get; set; } = null!;
}
