using AutoMapper;
using Microsoft.Extensions.Configuration;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;
using Profile = AutoMapper.Profile;

namespace RecruitXpress_BE.Contracts
{
    public class MappingProfile : Profile
    {


        public MappingProfile()
        {
            CreateMap<Option, OptionDTO>();
            CreateMap<Question, QuestionDTO>()
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options))
                .ForMember(dest => dest.CreatedByAccount, opt => opt.MapFrom(src => src.CreatedByNavigation))
                .ForMember(dest => dest.Question1, opt => opt.MapFrom(src => src.Question1));
            CreateMap<Account, AccountDTO>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Account1));
            CreateMap<JobApplication, JobApplicationDTO>();
            //.ForMember(dest => dest.Job, opt => opt.MapFrom(src => src.Job))
            //.ForMember(dest => dest.Profile, opt => opt.MapFrom(src => src.Profile))
            //.ForMember(dest => dest.Template, opt => opt.MapFrom(src => src.Template))
            //.ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Job.Location))
            //.ForMember(dest => dest.EmploymentType, opt => opt.MapFrom(src => src.Job.EmploymentType))
            //.ForMember(dest => dest.Industry, opt => opt.MapFrom(src => src.Job.Industry))
            //.ForMember(dest => dest.SalaryRange, opt => opt.MapFrom(src => src.Job.SalaryRange))
            //.ForMember(dest => dest.NameCandidate, opt => opt.MapFrom(src => src.Profile.Name))
            //.ForMember(dest => dest.PhoneCandidate, opt => opt.MapFrom(src => src.Profile.PhoneNumber))
            //.ForMember(dest => dest.EmailCandidate, opt => opt.MapFrom(src => src.Profile.Email))
            //.ForMember(dest => dest.ApplicationDeadline, opt => opt.MapFrom(src => src.Job.ApplicationDeadline));


        }
    }
}
