using Eventhub.Application.Interfaces;
using Eventhub.Application.Services;
using Eventhub.Domain.Interfaces;
using Eventhub.Infra.Data;
using Eventhub.Infra.Repositories;
using Eventhub.Infra.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Eventhub.Application.Profiles;

namespace Eventhub.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<EventhubDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPasswordSecurity, PasswordSecurity>();

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IEventoRepository, EventoRepository>();
        services.AddScoped<IConvidadosRepository, ConvidadosRepository>();
        services.AddScoped<IProgramacaoEventoRepository, ProgramacaoEventoRepository>();
        services.AddScoped<INotificacaoRepository, NotificacaoRepository>();
        services.AddScoped<IGaleriaRepository, GaleriaRepository>();
        services.AddScoped<ITipoEventoRepository, TipoEventoRepository>();
        services.AddScoped<IFotosRepository, FotosRepository>();

        // Services
        services.AddScoped<IEventoService, EventoService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IConvidadosService, ConvidadosService>();
        services.AddScoped<IProgramacaoEventoService, ProgramacaoEventoService>();
        services.AddScoped<INotificacaoService, NotificacaoService>();
        services.AddScoped<IGaleriaService, GaleriaService>();
        services.AddScoped<ITipoEventoService, TipoEventoService>();
        services.AddScoped<IFotosService, FotosService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IKeycloakService, KeycloakService>();

        // HttpClient
        services.AddHttpClient();

        // AutoMapper
        services.AddAutoMapper(typeof(EventhubMappingProfile));

        return services;
    }
}
