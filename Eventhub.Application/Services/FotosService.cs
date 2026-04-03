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
    private readonly IImageStorageService _imageStorageService;

    public FotosService(
        IFotosRepository fotosRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IImageStorageService imageStorageService)
    {
        _fotosRepository = fotosRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _imageStorageService = imageStorageService;
    }

    public async Task<FotoDto> UploadAsync(UploadFotoDto dto)
    {
        ExecutarValidacao(new UploadFotoValidator(), dto);

        var uploadResult = await _imageStorageService.UploadAsync(new ImageStorageUploadRequest
        {
            FileName = dto.NomeArquivo,
            Content = DecodeBase64(dto.Base64),
            ContentType = dto.TipoArquivo
        });

        var foto = new Fotos
        {
            NomeArquivo = dto.NomeArquivo,
            DataUpload = DateTime.UtcNow,
            TamanhoKB = CalculateKb(uploadResult.Bytes),
            Url = uploadResult.Url,
            PublicId = uploadResult.PublicId,
            ContentType = uploadResult.ContentType,
        };

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

        if (!string.IsNullOrWhiteSpace(dto.Base64))
        {
            var uploadResult = await _imageStorageService.UploadAsync(new ImageStorageUploadRequest
            {
                FileName = dto.NomeArquivo,
                Content = DecodeBase64(dto.Base64),
                ContentType = dto.TipoArquivo
            });

            var previousPublicId = foto.PublicId;

            foto.Url = uploadResult.Url;
            foto.PublicId = uploadResult.PublicId;
            foto.ContentType = uploadResult.ContentType;
            foto.TamanhoKB = CalculateKb(uploadResult.Bytes);
            foto.DataUpload = DateTime.UtcNow;

            _fotosRepository.Update(foto);
            await _unitOfWork.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(previousPublicId) && !string.Equals(previousPublicId, uploadResult.PublicId, StringComparison.Ordinal))
            {
                await _imageStorageService.DeleteAsync(previousPublicId!);
            }
        }
        else
        {
            _fotosRepository.Update(foto);
            await _unitOfWork.SaveChangesAsync();
        }

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

        var publicId = foto.PublicId;

        _fotosRepository.Remove(foto);
        await _unitOfWork.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(publicId))
        {
            await _imageStorageService.DeleteAsync(publicId!);
        }
    }

    private static byte[] DecodeBase64(string base64)
    {
        var sanitized = FotoBase64Helper.Sanitize(base64);
        return Convert.FromBase64String(sanitized);
    }

    private static int CalculateKb(long bytes)
    {
        if (bytes <= 0)
            return 0;

        return (int)Math.Max(1, Math.Ceiling(bytes / 1024m));
    }
}
