using AutoMapper;
using VertiCore.Application.DTOs.AuditLog;
using VertiCore.Domain.Entities;

namespace VertiCore.Application.Mappings
{
    public class AuditLogMappingProfile : Profile
    {
        public AuditLogMappingProfile()
        {
            CreateMap<AuditLog, AuditLogDto>()
                .ForMember(dest => dest.UserName,
                    opt => opt.Ignore());
        }
    }
}