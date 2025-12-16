using Eventhub.Api.Controllers;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Eventhub.Tests.Controllers;

public class PerfisControllerTests
{
    [Fact]
    public async Task ObterPerfisAtivos_ReturnsCustomResponseWithPerfis()
    {
        // Arrange
        var perfis = new List<PerfilDto>
        {
            new PerfilDto { Id = 1, Descricao = "Administrador", Icon = "admin_panel_settings", Status = 'A' },
            new PerfilDto { Id = 2, Descricao = "Músico", Icon = "music_note", Status = 'A' }
        };
        var mockService = new Mock<IPerfilService>();
        mockService.Setup(s => s.ObterPerfisAtivosAsync()).ReturnsAsync(perfis);
        var controller = new PerfisController(mockService.Object);

        // Act
        var result = await controller.ObterPerfisAtivos();

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        var customResponse = Assert.IsType<CustomResponse<IEnumerable<PerfilDto>>>(objectResult.Value);
        Assert.Equal(perfis, customResponse.Data);
    }
}

