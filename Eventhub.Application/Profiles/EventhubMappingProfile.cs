using System.Linq;
using AutoMapper;
using Eventhub.Application.DTOs;
using Eventhub.Domain.Entities;

namespace Eventhub.Application.Profiles;

public class EventhubMappingProfile : Profile
{
    public EventhubMappingProfile()
    {
        CreateMap<TipoEvento, TipoEventoDto>();
        CreateMap<Evento, EventoDto>();

        CreateMap<EventoCadastroDto, Evento>()
            .ForMember(dest => dest.Endereco, opt => opt.MapFrom(src => src.Endereco))
            .ForMember(dest => dest.Galerias, opt => opt.Ignore())
            .ForMember(dest => dest.Participantes, opt => opt.Ignore());

        CreateMap<Modulo, ModuloDto>();
        CreateMap<Permissao, PermissaoDto>();

        CreateMap<Perfil, PerfilDto>();
        CreateMap<Perfil, PermissoesPerfilDto>()
            .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Descricao))
            .ForMember(dest => dest.Permissoes, opt => opt.MapFrom(src => src.PerfilPermissoes.Select(pp => pp.Permissao)));
            
        CreateMap<EnderecoEventoDto, EnderecoEvento>().ReverseMap();

        CreateMap<Usuario, UsuarioInfoDto>()
            .ForMember(dest => dest.FotoBase64, opt => opt.MapFrom(src => src.Foto != null ? src.Foto.Base64 : null));

        CreateMap<Participante, ParticipanteDto>()
            .ForMember(dest => dest.Usuario, opt => opt.MapFrom(src => src.Usuario))
            .ForMember(dest => dest.Perfil, opt => opt.MapFrom(src => src.Perfil))
            .ForMember(dest => dest.CadastroPendente, opt => opt.MapFrom(src => src.CadastroPendente))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Usuario.Status));

        CreateMap<UploadFotoDto, Fotos>().ReverseMap();
        CreateMap<UpdateFotoDto, Fotos>().ReverseMap();
        CreateMap<FotoDto, Fotos>().ReverseMap();

        CreateMap<Evento, EventoAtivoDto>()
            .ForMember(dest => dest.IdTipoEvento, opt => opt.MapFrom(src => src.TipoEvento.Id))
            .ForMember(dest => dest.TipoEvento, opt => opt.MapFrom(src => src.TipoEvento.Nome))
            .ForMember(dest => dest.IdStatus, opt => opt.MapFrom(src => src.Status.Id))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Descricao))
            .ForMember(dest => dest.FotoCapaBase64, opt => opt.MapFrom(src =>
                src.Galerias != null
                    ? src.Galerias
                        .Where(g => g.Tipo == Domain.Enums.GaleriaTipo.Capa && g.Foto != null)
                        .Select(g => g.Foto.Base64)
                        .FirstOrDefault() ?? string.Empty
                    : string.Empty));
        
        CreateMap<Evento, EventoDto>()
            .ForMember(dest => dest.IdTipoEvento, opt => opt.MapFrom(src => src.TipoEvento.Id))
            .ForMember(dest => dest.Endereco, opt => opt.MapFrom(src => src.Endereco));

        CreateMap<StatusEvento, StatusEventoDto>();
    }
}
