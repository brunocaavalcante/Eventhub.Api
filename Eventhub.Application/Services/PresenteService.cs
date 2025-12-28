using AutoMapper;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Application.Validations;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Enums;
using Eventhub.Domain.Exceptions;
using Eventhub.Domain.Interfaces;

namespace Eventhub.Application.Services;

public class PresenteService : BaseService, IPresenteService
{
    private readonly IPresenteRepository _presenteRepository;
    private readonly IFotosService _fotosService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PresenteService(
        IPresenteRepository presenteRepository,
        IFotosService fotosService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _presenteRepository = presenteRepository;
        _fotosService = fotosService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PresenteDto> AdicionarAsync(CreatePresenteDto dto)
    {
        var presente = _mapper.Map<Presente>(dto);
        presente.DataCadastro = DateTime.UtcNow;
        presente.IdStatus = (int)StatusPresenteEnum.Disponivel;

        ExecutarValidacao(new PresenteValidation(), presente);
        await _presenteRepository.AddAsync(presente);

        foreach (var imageDto in dto.Imagens)
        {
            var fotoDto = await _fotosService.UploadAsync(imageDto);

            var galeria = new Galeria
            {
                IdEvento = presente.IdEvento,
                IdFoto = fotoDto.Id,
                Visibilidade = "Publica",
                Tipo = GaleriaTipo.Produto,
                Data = DateTime.UtcNow
            };

            presente.Galerias.Add(galeria);
        }

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PresenteDto>(presente);
    }

    public async Task<PresenteDto> AtualizarAsync(UpdatePresenteDto dto)
    {
        var presente = await _presenteRepository.GetByIdAsync(dto.Id);
        if (presente == null)
            throw new ExceptionValidation("Presente não encontrado.");

        _mapper.Map(dto, presente);

        ExecutarValidacao(new PresenteValidation(), presente);

        _presenteRepository.Update(presente);

        foreach (var imageDto in dto.Imagens)
        {
            await _fotosService.UpdateAsync(imageDto);
        }

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PresenteDto>(presente);
    }

    public async Task RemoverAsync(int id)
    {
        var presente = await _presenteRepository.GetByIdAsync(id, p => p.Galerias);

        if (presente == null)
            throw new ExceptionValidation("Presente não encontrado.");

        if (presente.IdStatus != (int)StatusPresenteEnum.Disponivel)
            throw new ExceptionValidation("Somente presentes com status 'Disponível' podem ser removidos.", true);

        var galerias = presente.Galerias.ToList();

        foreach (var galeria in galerias)
        {
            await _fotosService.RemoverAsync(galeria.IdFoto);
        }

        _presenteRepository.Remove(presente);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<PresenteDto?> ObterPorIdAsync(int id)
    {
        var presente = await _presenteRepository.GetByIdAsync(id);
        return presente != null ? _mapper.Map<PresenteDto>(presente) : null;
    }

    public async Task<IEnumerable<PresenteDto>> ListarTodosAsync()
    {
        var presentes = await _presenteRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<PresenteDto>>(presentes);
    }

    public async Task<IEnumerable<CategoriaPresenteDto>> ListarCategoriaPresentesAsync()
    {
        var categorias = await _presenteRepository.GetByCategoryAsync();
        return _mapper.Map<IEnumerable<CategoriaPresenteDto>>(categorias);
    }
}