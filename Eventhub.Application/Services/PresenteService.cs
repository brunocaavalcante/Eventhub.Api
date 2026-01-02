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
        await _unitOfWork.SaveChangesAsync();

        foreach (var imageDto in dto.Imagens)
        {
            var fotoDto = await _fotosService.UploadAsync(imageDto);

            var galeria = new Galeria
            {
                IdEvento = presente.IdEvento,
                IdFoto = fotoDto.Id,
                IdPresente = presente.Id,
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
        var presente = await _presenteRepository.GetByIdCompletoAsync(dto.Id);
        if (presente == null)
            throw new ExceptionValidation("Presente não encontrado.");

        _mapper.Map(dto, presente);

        ExecutarValidacao(new PresenteValidation(), presente);

        _presenteRepository.Update(presente);

        await ProcessarImagensAsync(presente, dto.Imagens);

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PresenteDto>(presente);
    }

    private async Task ProcessarImagensAsync(Presente presente, ICollection<UpdateFotoDto>? imagens)
    {
        // Obtem galerias existentes para este presente (filtradas pelo tipo Produto)
        var galeriasExistentes = presente.Galerias
            .Where(g => g.Tipo == GaleriaTipo.Produto).ToList();

        var idsFotosNoDto = imagens?
            .Where(img => img.Id > 0)
            .Select(img => img.Id)
            .ToHashSet() ?? new HashSet<int>();

        // Obtem imagens removidas
        var galeriasParaRemover = galeriasExistentes.Where(g => !idsFotosNoDto.Contains(g.IdFoto)).ToList();

        foreach (var galeria in galeriasParaRemover)
        {
            await _fotosService.RemoverAsync(galeria.IdFoto);
            presente.Galerias.Remove(galeria);
        }

        // Process images from DTO
        if (imagens != null)
        {
            foreach (var imageDto in imagens)
            {
                if (imageDto.Id > 0)
                {
                    await _fotosService.UpdateAsync(imageDto);
                }
                else
                {
                    var uploadFotoDto = new UploadFotoDto
                    {
                        NomeArquivo = imageDto.NomeArquivo,
                        Base64 = imageDto.Base64,
                        TipoImagem = "image/jpeg"
                    };

                    var fotoDto = await _fotosService.UploadAsync(uploadFotoDto);

                    var galeria = new Galeria
                    {
                        IdEvento = presente.IdEvento,
                        IdFoto = fotoDto.Id,
                        IdPresente = presente.Id,
                        Visibilidade = "Publica",
                        Tipo = GaleriaTipo.Produto,
                        Data = DateTime.UtcNow
                    };

                    presente.Galerias.Add(galeria);
                }
            }
        }
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
        var presente = await _presenteRepository.GetByIdCompletoAsync(id);
        return presente != null ? _mapper.Map<PresenteDto>(presente) : null;
    }

    public async Task<IEnumerable<PresenteDto>> ListarTodosAsync(int idEvento)
    {
        if (idEvento <= 0) throw new ExceptionValidation("ID do evento inválido.");

        var presentes = await _presenteRepository.GetByEventIdAsync(idEvento);
        return _mapper.Map<IEnumerable<PresenteDto>>(presentes);
    }

    public async Task<IEnumerable<CategoriaPresenteDto>> ListarCategoriaPresentesAsync()
    {
        var categorias = await _presenteRepository.GetByCategoryAsync();
        return _mapper.Map<IEnumerable<CategoriaPresenteDto>>(categorias);
    }
}