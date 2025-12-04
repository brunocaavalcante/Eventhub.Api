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
    }
}
