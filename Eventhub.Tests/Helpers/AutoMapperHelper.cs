using AutoMapper;
using Eventhub.Application.Profiles;

namespace Eventhub.Tests.Helpers;

public static class AutoMapperHelper
{
    public static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<EventhubMappingProfile>();
        });

        return config.CreateMapper();
    }
}