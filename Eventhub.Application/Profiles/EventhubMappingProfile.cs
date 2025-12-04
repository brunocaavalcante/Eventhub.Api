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
        CreateMap<Perfil, PerfilDto>();
        CreateMap<Usuario, UsuarioInfoDto>();
        CreateMap<Participante, ParticipanteDto>()
            .ForMember(dest => dest.Usuario, opt => opt.MapFrom(src => src.Usuario))
            .ForMember(dest => dest.Perfil, opt => opt.MapFrom(src => src.Perfil))
            .ForMember(dest => dest.CadastroPendente, opt => opt.MapFrom(src => src.CadastroPendente))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Usuario.Status));
    }
}
