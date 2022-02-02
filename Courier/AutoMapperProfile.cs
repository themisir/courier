using AutoMapper;
using Courier.Data.Models;
using Courier.Models.Dto;

namespace Courier;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Package, PackageDto>()
            .ForMember(a => a.Id, x => x.MapFrom(b => b.Id))
            .ForMember(a => a.Name, x => x.MapFrom(b => b.Name))
            .ForMember(a => a.Versions, x => x.MapFrom(b => b.Versions!.OrderByDescending(v => v.CreatedAt)))
            .ForMember(a => a.Latest, x => x.MapFrom(b => b.Versions!
                .OrderByDescending(v => v.CreatedAt)
                .LastOrDefault()));

        CreateMap<PackageVersion, PackageVersionDto>()
            .ForMember(a => a.Version, x => x.MapFrom(b => b.VersionName))
            .ForMember(a => a.Published, x => x.MapFrom(b => b.CreatedAt))
            .ForMember(a => a.ArchiveKey, x => x.MapFrom(b => b.ArchiveKey))
            .ForMember(a => a.Pubspec, x => x.MapFrom(b => b.Metadata));
    }
}