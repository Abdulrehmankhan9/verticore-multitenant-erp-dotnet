using AutoMapper;
using VertiCore.Application.DTOs.Client;
using VertiCore.Domain.Entities;

namespace VertiCore.Application.Mappings
{
    public class ClientMappingProfile : Profile
    {
        public ClientMappingProfile()
        {
            CreateMap<Client, ClientDto>();
            CreateMap<CreateClientRequest, Client>();
            CreateMap<UpdateClientRequest, Client>();
        }
    }
}