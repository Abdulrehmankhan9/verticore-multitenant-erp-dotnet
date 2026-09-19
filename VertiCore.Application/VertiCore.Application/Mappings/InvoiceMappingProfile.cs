using AutoMapper;
using VertiCore.Application.DTOs.Invoice;
using VertiCore.Domain.Entities;

namespace VertiCore.Application.Mappings
{
    public class InvoiceMappingProfile : Profile
    {
        public InvoiceMappingProfile()
        {
            CreateMap<Invoice, InvoiceDto>()
                .ForMember(dest => dest.ClientName,
                    opt => opt.Ignore());

            CreateMap<InvoiceItem, InvoiceItemDto>();
        }
    }
}