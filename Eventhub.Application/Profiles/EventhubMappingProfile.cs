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

        CreateMap<Perfil, PerfilDto>();
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
            .ForMember(dest => dest.TipoEvento, opt => opt.MapFrom(src => src.TipoEvento.Nome))
            .ForMember(dest => dest.IdStatus, opt => opt.MapFrom(src => src.Status.Id))
            .ForMember(dest => dest.FotoCapaBase64, opt => opt.MapFrom(src =>
                src.Galerias != null && src.Galerias.FirstOrDefault(g => g.Tipo == Domain.Enums.GaleriaTipo.Capa) != null
                    ? src.Galerias.FirstOrDefault(g => g.Tipo == Domain.Enums.GaleriaTipo.Capa).Foto.Base64 : ""));

        CreateMap<StatusEvento, StatusEventoDto>();
    }
}
