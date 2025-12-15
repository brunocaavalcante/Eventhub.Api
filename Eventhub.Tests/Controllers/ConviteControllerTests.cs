using Moq;
using Eventhub.Api.Controllers;
using Eventhub.Application.Interfaces;
using Eventhub.Domain.Interfaces;
using Eventhub.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Eventhub.Api.Models;

namespace Eventhub.Tests.Controllers;

public class ConviteControllerTests
{
    private readonly Mock<IConviteService> _conviteServiceMock;
    private readonly Mock<IEnvioConviteService> _envioConviteServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ConviteController _controller;

    public ConviteControllerTests()
    {
        _conviteServiceMock = new Mock<IConviteService>();
        _envioConviteServiceMock = new Mock<IEnvioConviteService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _controller = new ConviteController(_conviteServiceMock.Object, _envioConviteServiceMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task ObterPorEvento_ShouldReturnConviteDto()
    {
        var conviteDto = new ConviteDto { Id = 1 };
        _conviteServiceMock.Setup(s => s.ObterPorEventoAsync(1)).ReturnsAsync(conviteDto);

        var result = await _controller.ObterPorEvento(1);
        var customResult = result as ObjectResult;
        Assert.NotNull(customResult);
        var response = customResult.Value as CustomResponse<ConviteDto>;
        Assert.NotNull(response);
        Assert.NotNull(response.Data);
        response.Data.Id.Should().Be(1);
    }

    [Fact]
    public async Task ObterPorEvento_NotFound_ShouldReturn404()
    {
        _conviteServiceMock.Setup(s => s.ObterPorEventoAsync(1)).ReturnsAsync((ConviteDto?)null);

        var result = await _controller.ObterPorEvento(1);
        var customResult = result as ObjectResult;
        Assert.NotNull(customResult);
        customResult.StatusCode.Should().Be(404);
        var response = customResult.Value as CustomResponse<object>;
        Assert.NotNull(response);
        response.Erros.Should().Contain("Convite não encontrado.");
    }

    [Fact]
    public async Task Criar_ShouldReturnCreatedConviteDto()
    {
        var createDto = new CreateConviteDto { Nome = "Convite 1", Nome2 = "Convite 2", Mensagem = "Mensagem", Foto = new UploadFotoDto { NomeArquivo = "img.jpg", Base64 = Convert.ToBase64String(new byte[1024]) } };
        var conviteDto = new ConviteDto { Id = 1 };
        _conviteServiceMock.Setup(s => s.CriarAsync(createDto)).ReturnsAsync(conviteDto);

        var result = await _controller.Criar(createDto);
        var customResult = result as ObjectResult;
        Assert.NotNull(customResult);
        customResult.StatusCode.Should().Be(201);
        var response = customResult.Value as CustomResponse<ConviteDto>;
        Assert.NotNull(response);
        Assert.NotNull(response.Data);
        response.Data.Id.Should().Be(1);
    }

    [Fact]
    public async Task Criar_ShouldHandleExceptionAndRollback()
    {
        var createDto = new CreateConviteDto { Nome = "Convite 1", Nome2 = "Convite 2", Mensagem = "Mensagem", Foto = new UploadFotoDto { NomeArquivo = "img.jpg", Base64 = Convert.ToBase64String(new byte[1024]) } };
        _conviteServiceMock.Setup(s => s.CriarAsync(createDto)).ThrowsAsync(new Exception("Erro ao criar convite"));

        var result = await _controller.Criar(createDto);
        var customResult = result as ObjectResult;
        Assert.NotNull(customResult);
        var response = customResult.Value as CustomResponse<object>;
        Assert.NotNull(response);
        response.Erros.Should().Contain("Erro ao criar convite");

        _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
    } 

    [Fact]
    public async Task Atualizar_ShouldReturnUpdatedConviteDto()
    {
        var updateDto = new UpdateConviteDto { Nome = "Convite Atualizado", Nome2 = "Convite 2", Mensagem = "Mensagem Atualizada", Foto = new UpdateFotoDto { Id = 1, NomeArquivo = "img_atualizada.jpg", Base64 = Convert.ToBase64String(new byte[2048]) } };
        var conviteDto = new ConviteDto { Id = 1 };
        _conviteServiceMock.Setup(s => s.AtualizarAsync(1, updateDto)).ReturnsAsync(conviteDto);

        var result = await _controller.Atualizar(1, updateDto);
        var customResult = result as ObjectResult;
        Assert.NotNull(customResult);
        var response = customResult.Value as CustomResponse<ConviteDto>;
        Assert.NotNull(response);
        Assert.NotNull(response.Data);
        response.Data.Id.Should().Be(1);
    }

    [Fact]
    public async Task Atualizar_ShouldHandleExceptionAndRollback()
    {
        var updateDto = new UpdateConviteDto { Nome = "Convite Atualizado", Nome2 = "Convite 2", Mensagem = "Mensagem Atualizada", Foto = new UpdateFotoDto { Id = 1, NomeArquivo = "img_atualizada.jpg", Base64 = Convert.ToBase64String(new byte[2048]) } };
        _conviteServiceMock.Setup(s => s.AtualizarAsync(1, updateDto)).ThrowsAsync(new Exception("Erro ao atualizar convite"));

        var result = await _controller.Atualizar(1, updateDto);
        var customResult = result as ObjectResult;
        Assert.NotNull(customResult);
        var response = customResult.Value as CustomResponse<object>;
        Assert.NotNull(response);
        response.Erros.Should().Contain("Erro ao atualizar convite");

        _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
    } 
}
