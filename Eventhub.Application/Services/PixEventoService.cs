using AutoMapper;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Application.Validations;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Enums;
using Eventhub.Domain.Exceptions;
using Eventhub.Domain.Interfaces;

namespace Eventhub.Application.Services;

public class PixEventoService : BaseService, IPixEventoService
{
    private readonly IPixEventoRepository _pixEventoRepository;
    private readonly IEventoRepository _eventoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PixEventoService(
        IPixEventoRepository pixEventoRepository,
        IEventoRepository eventoRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _pixEventoRepository = pixEventoRepository;
        _eventoRepository = eventoRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PixEventoDto> CreateAsync(CreatePixEventoDto dto)
    {
        // Validar se o evento existe
        var evento = await _eventoRepository.GetByIdAsync(dto.IdEvento);
        if (evento == null)
            throw new ExceptionValidation("Evento não encontrado.");

        // Verificar se já existe PIX para a mesma finalidade
        var pixExistente = await _pixEventoRepository.GetByEventoAndFinalidadeAsync(dto.IdEvento, dto.Finalidade);

        // Se existe, DELETAR o anterior
        if (pixExistente != null)
        {
            _pixEventoRepository.Remove(pixExistente);
        }

        // Criar novo PIX
        var novoPix = _mapper.Map<PixEvento>(dto);
        novoPix.DataCadastro = DateTime.UtcNow;

        // Validar entidade
        ExecutarValidacao(new PixEventoValidation(), novoPix);

        await _pixEventoRepository.AddAsync(novoPix);
        await _unitOfWork.SaveChangesAsync();

        var pixDto = _mapper.Map<PixEventoDto>(novoPix);
        pixDto.FinalidadeDescricao = novoPix.Finalidade.ToString();

        return pixDto;
    }

    public async Task<PixEventoDto> UpdateAsync(UpdatePixEventoDto dto)
    {
        var pixEvento = await _pixEventoRepository.GetByIdAsync(dto.Id);
        if (pixEvento == null)
            throw new ExceptionValidation("PIX não encontrado.");

        // Atualizar apenas campos editáveis (NomeBeneficiario e QRCodePix)
        // NÃO permite alterar Finalidade ou IdEvento
        pixEvento.NomeBeneficiario = dto.NomeBeneficiario;
        pixEvento.QRCodePix = dto.QRCodePix;

        // Validar entidade
        ExecutarValidacao(new PixEventoValidation(), pixEvento);

        _pixEventoRepository.Update(pixEvento);
        await _unitOfWork.SaveChangesAsync();

        var pixDto = _mapper.Map<PixEventoDto>(pixEvento);
        pixDto.FinalidadeDescricao = pixEvento.Finalidade.ToString();

        return pixDto;
    }

    public async Task DeleteAsync(int id)
    {
        var pixEvento = await _pixEventoRepository.GetByIdAsync(id);
        if (pixEvento == null)
            throw new ExceptionValidation("PIX não encontrado.");

        _pixEventoRepository.Remove(pixEvento);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<PixEventoDto?> GetByIdAsync(int id)
    {
        var pixEvento = await _pixEventoRepository.GetByIdAsync(id);
        if (pixEvento == null)
            return null;

        var pixDto = _mapper.Map<PixEventoDto>(pixEvento);
        
        pixDto.FinalidadeDescricao = pixEvento.Finalidade.ToString();

        return pixDto;
    }

    public async Task<IEnumerable<PixEventoDto>> GetByEventoIdAsync(int idEvento)
    {
        var pixEventos = await _pixEventoRepository.GetByEventoIdAsync(idEvento);
        
        return pixEventos.Select(p =>
        {
            var dto = _mapper.Map<PixEventoDto>(p);
            dto.FinalidadeDescricao = p.Finalidade.ToString();
            return dto;
        });
    }

    public async Task<PixEventoDto?> GetByEventoAndFinalidadeAsync(int idEvento, FinalidadePix finalidade)
    {
        var pixEvento = await _pixEventoRepository.GetByEventoAndFinalidadeAsync(idEvento, finalidade);
        
        if (pixEvento == null)
            return null;

        var dto = _mapper.Map<PixEventoDto>(pixEvento);
        dto.FinalidadeDescricao = pixEvento.Finalidade.ToString();
        return dto;
    }
}
