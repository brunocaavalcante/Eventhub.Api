using Eventhub.Api.Configuracao;
using Eventhub.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.
         AddJsonFile($"appsettings.json", true, true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
        .AddEnvironmentVariables().Build();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfiguration();
builder.Services.ConfigAuth0(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200",  "https://eventhub-web-itzo.onrender.com")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.ApplyDatabaseMigrations();

app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowLocalAngular");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
