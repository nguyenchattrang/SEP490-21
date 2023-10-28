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
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options));
           

        }
    }
}
