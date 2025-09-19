using AutoMapper;
using WebApiTemplate.API.Models;
using WebApiTemplate.Application.Models;

namespace WebApiTemplate.API.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<SignUpRequest, SignUpInfo>();
    }
}