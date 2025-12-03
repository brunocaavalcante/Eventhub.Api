using AutoMapper;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Application.Validations;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Exceptions;
using Eventhub.Domain.Interfaces;

namespace Eventhub.Application.Services;

public class FotosService : BaseService, IFotosService
{
    private readonly IFotosRepository _fotosRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public FotosService(IFotosRepository fotosRepository, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _fotosRepository = fotosRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<FotoDto> UploadAsync(UploadFotoDto dto)
    {
        ExecutarValidacao(new UploadFotoValidator(), dto);

        var foto = _mapper.Map<Fotos>(dto);
        foto.DataUpload = DateTime.UtcNow;
        foto.TamanhoKB = (int)(Convert.FromBase64String(dto.Base64).Length / 1024);
        
        await _fotosRepository.AddAsync(foto);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<FotoDto>(foto);
    }

    public async Task<FotoDto> UpdateAsync(UpdateFotoDto dto)
    {
        ExecutarValidacao(new UpdateFotoValidator(), dto);
        var foto = await _fotosRepository.GetByIdAsync(dto.Id);
        if (foto == null)
            throw new ExceptionValidation("Foto não encontrada.");

        foto.NomeArquivo = dto.NomeArquivo;
        foto.Base64 = dto.Base64;
        foto.TamanhoKB = (int)(Convert.FromBase64String(dto.Base64).Length / 1024);
        _fotosRepository.Update(foto);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<FotoDto>(foto);
    }

    public async Task<FotoDto?> GetByIdAsync(int id)
    {
        var foto = await _fotosRepository.GetByIdAsync(id);
        return foto == null ? null : _mapper.Map<FotoDto>(foto);
    }

    public async Task RemoverAsync(int id)
    {
        var foto = await _fotosRepository.GetByIdAsync(id);
        if (foto == null)
            throw new ExceptionValidation("Foto não encontrada.");
        _fotosRepository.Remove(foto);
        await _unitOfWork.SaveChangesAsync();
    }
}
