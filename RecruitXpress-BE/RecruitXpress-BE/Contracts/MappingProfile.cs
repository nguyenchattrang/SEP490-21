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
            CreateMap<JobApplication, JobApplicationDTO>();
            CreateMap<ComputerProficiency, ComputerProficiencyDTO>();
            CreateMap<MaritalStatus, MaritalStatusDTO>();
            CreateMap<LanguageProficiency, LanguageProficiencyDTO>();
            CreateMap<EducationalBackground, EducationalBackgroundDTO>();
            CreateMap<FamilyInformation, FamilyInformationDTO>();
            CreateMap<training, TrainigDTO>();
            CreateMap<WorkExperience, WorkExperienceDTO>();
            CreateMap<GeneralTest, GeneralTestDTO>()
            .ForMember(dest => dest.GeneralTestDetails, opt => opt.MapFrom(src => src.GeneralTestDetails))
            .ForMember(dest => dest.CreatedByAccount, opt => opt.MapFrom(src => src.CreatedByNavigation));
            CreateMap<JobPosting, JobDTO>();
            CreateMap<Cvtemplate, CvtemplateDTO>().ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url))
                .ForMember(dest => dest.TemplateId, opt => opt.MapFrom(src => src.TemplateId));
            CreateMap<Models.Profile, ProfileDTO>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth));




            // Mapping cua Trang
            CreateMap<Option, OptionDTO>();

            CreateMap<Question, QuestionDTO>()
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options))
                .ForMember(dest => dest.CreatedByAccount, opt => opt.MapFrom(src => src.CreatedByNavigation))
                .ForMember(dest => dest.Question1, opt => opt.MapFrom(src => src.Question1));

            CreateMap<Account, AccountDTO>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Account1));

            CreateMap<Option, ExamOptionDTO>();

            CreateMap<Question, ExamQuestionDTO>()
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options))
                .ForMember(dest => dest.Question1, opt => opt.MapFrom(src => src.Question1));

            CreateMap<SpecializedExam, SpecializedExamDTO>()
           .ForMember(dest => dest.CreatedByAccount, opt => opt.MapFrom(src => src.CreatedByNavigation));
            CreateMap<GeneralTestDetail, GeneralTestDetailDTO>();
            CreateMap<Exam, ExamDTO>();

        }
    }
}
