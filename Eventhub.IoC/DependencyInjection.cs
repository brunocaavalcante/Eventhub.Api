using Eventhub.Application.Interfaces;
using Eventhub.Application.Services;
using Eventhub.Domain.Interfaces;
using Eventhub.Infra.Data;
using Eventhub.Infra.Repositories;
using Eventhub.Infra.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        services.AddScoped<IParticipanteRepository, ParticipanteRepository>();
        services.AddScoped<IProgramacaoEventoRepository, ProgramacaoEventoRepository>();
        services.AddScoped<INotificacaoRepository, NotificacaoRepository>();
        services.AddScoped<IGaleriaRepository, GaleriaRepository>();
        services.AddScoped<ITipoEventoRepository, TipoEventoRepository>();
        services.AddScoped<IFotosRepository, FotosRepository>();
        services.AddScoped<IPerfilRepository, PerfilRepository>();
        services.AddScoped<IStatusEventoRepository, StatusEventoRepository>();
        services.AddScoped<IEnvioConviteRepository, EnvioConviteRepository>();
        services.AddScoped<IConviteRepository, ConviteRepository>();
        services.AddScoped<IPresenteRepository, PresenteRepository>();

        // Services
        services.AddScoped<IEventoService, EventoService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IParticipanteService, ParticipanteService>();
        services.AddScoped<IProgramacaoEventoService, ProgramacaoEventoService>();
        services.AddScoped<INotificacaoService, NotificacaoService>();
        services.AddScoped<IGaleriaService, GaleriaService>();
        services.AddScoped<ITipoEventoService, TipoEventoService>();
        services.AddScoped<IFotosService, FotosService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAuth0Service, Auth0Service>();
        services.AddScoped<IPerfilService, PerfilService>();
        services.AddScoped<IEnvioConviteService, EnvioConviteService>();
        services.AddScoped<IConviteService, ConviteService>();
        services.AddScoped<IPresenteService, PresenteService>();

        // HttpClient
        services.AddHttpClient();

        // AutoMapper
        services.AddAutoMapper(typeof(EventhubMappingProfile));

        return services;
    }

    /// <summary>
    /// Aplica migrations pendentes ao banco de dados no startup da aplicação
    /// </summary>
    public static void ApplyDatabaseMigrations(this IApplicationBuilder app)
    {
        try
        {
            using var scope = app.ApplicationServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<EventhubDbContext>();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("DatabaseMigration");

            logger.LogInformation("Aplicando migrations pendentes ao banco de dados...");
            db.Database.Migrate();
            logger.LogInformation("Migrations aplicadas com sucesso!");
        }
        catch (Exception ex)
        {
            var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("DatabaseMigration");
            logger.LogError(ex, "Erro ao aplicar migrations no banco de dados. A aplicação não será iniciada.");
            throw;
        }
    }
}