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

        CreateMap<Participante, ListarConvidadoDto>()
            .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.Nome : ""))
            .ForMember(dest => dest.Telefone, opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.Telefone : ""))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.Email : ""))
            .ForMember(dest => dest.Foto, opt => opt.MapFrom(src => src.Usuario != null && src.Usuario.Foto != null ? src.Usuario.Foto.Base64 : ""))
            .ForMember(dest => dest.QuandidadeAcompanhantes, opt => opt.MapFrom(src => src.EnviosConvite
                        .Where(ev => ev.IdEvento == src.IdEvento).Select(ev => ev.QtdAcompanhantes).FirstOrDefault()))
            .ForMember(dest => dest.StatusConfirmacao, opt => opt.MapFrom(src => src.EnviosConvite
                        .Where(ev => ev.IdEvento == src.IdEvento).Select(ev => ev.StatusEnvioConvite != null ? ev.StatusEnvioConvite.Descricao : null).FirstOrDefault() ?? "PendenteEnvio"));

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

        CreateMap<Convite, CreateConviteDto>().ReverseMap();
        CreateMap<Convite, ConviteDto>();

        // Presente Mappings
        CreateMap<CreatePresenteDto, Presente>();
        CreateMap<UpdatePresenteDto, Presente>();
        CreateMap<CategoriaPresente, CategoriaPresenteDto>();
        CreateMap<StatusPresente, StatusPresenteDto>();
        CreateMap<Presente, PresenteDto>().ForMember(dest => dest.Imagens, opt => opt.MapFrom(src =>
            src.Galerias.Where(g => g.Tipo == Domain.Enums.GaleriaTipo.Produto && g.Foto != null).Select(g => g.Foto)
                .ToList()));

        // PixEvento Mappings
        CreateMap<CreatePixEventoDto, PixEvento>();
        CreateMap<UpdatePixEventoDto, PixEvento>();
        CreateMap<PixEvento, PixEventoDto>();
    }
}
