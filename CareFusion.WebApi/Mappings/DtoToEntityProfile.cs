using AutoMapper;
using CareFusion.Core.Entities;
using CareFusion.Model.Dtos;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CareFusion.WebApi.Mappings;

public class DtoToEntityProfile : Profile
{
    public DtoToEntityProfile()
    {
        CreateMap<PatientDto, Patient>();
        CreateMap<ExamDto, Exam>();
        CreateMap<UserDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // Password handled separately
        CreateMap<ClinicSiteDto, ClinicSite>();
        CreateMap<PatientNoteDto, PatientNote>();
    }
}
