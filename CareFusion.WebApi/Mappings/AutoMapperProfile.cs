// Placeholder for Mappings/AutoMapperProfile.cs
using AutoMapper;
using CareFusion.Core.Entities;
using CareFusion.Model.Dtos;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CareFusion.WebApi.Mappings;

public class EntityToDtoProfile : Profile
{
    public EntityToDtoProfile()
    {
        CreateMap<Patient, PatientDto>()
            .ForMember(dest => dest.ExamCount, opt => opt.MapFrom(src => src.Exams.Count));

        CreateMap<Exam, ExamDto>();
        CreateMap<User, UserDto>();
    }
}
