using AutoMapper;
using Eventhub.Application.DTOs;
using Eventhub.Application.Services;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;
using Moq;

namespace Eventhub.Tests.Services;
public class PerfilServiceTests
{
    [Fact]
    public async Task ObterPerfisAtivosAsync_ReturnsMappedPerfis()
    {
        // Arrange
        var perfis = new List<Perfil>
            {
                new Perfil { Id = 1, Descricao = "Administrador", Icon = "admin_panel_settings", Status = 'A' },
                new Perfil { Id = 2, Descricao = "Músico", Icon = "music_note", Status = 'A' }
            };
        var mockRepo = new Mock<IPerfilRepository>();
        mockRepo.Setup(r => r.GetPerfisAtivosAsync()).ReturnsAsync(perfis);
        var config = new MapperConfiguration(cfg => cfg.CreateMap<Perfil, PerfilDto>());
        var mapper = config.CreateMapper();
        var service = new PerfilService(mockRepo.Object, mapper);

        // Act
        var result = await service.ObterPerfisAtivosAsync();

        // Assert
        Assert.Collection(result,
            p => Assert.Equal("Administrador", p.Descricao),
            p => Assert.Equal("Músico", p.Descricao));
    }
}