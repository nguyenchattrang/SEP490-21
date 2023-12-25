using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.DTO
{
    public class ActivityLoggingDTO
    {
        public ActivityLoggingDTO()
        {
            ComputerProficiencies = new HashSet<ComputerProficiencyDTO>();
            EducationalBackgrounds = new HashSet<EducationalBackgroundDTO>();
            Evaluates = new HashSet<EvaluateDTO>();
            FamilyInformations = new HashSet<FamilyInformationDTO>();
            GeneralTests = new HashSet<GeneralTestDTO>();
            Interviewers = new HashSet<Interview>();
            JobApplications = new HashSet<ShortJobApplicationDTO>();
            LanguageProficiencies = new HashSet<LanguageProficiencyDTO>();
            ScheduleDetails = new HashSet<ScheduleDetailDTO>();
            Schedules = new HashSet<ScheduleDTO>();
            WorkExperiences = new HashSet<WorkExperienceDTO>();
            training = new HashSet<TrainigDTO>();
        }
        public int ProfileId { get; set; }
        public int? AccountId { get; set; }
        public int? StatusId { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public DateTime? Dob { get; set; }
        public string? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public string? Skills { get; set; }
        public string? Accomplishment { get; set; }
        public string? Strength { get; set; }
        public string? Imperfection { get; set; }
        public string? ResearchWork { get; set; }
        public string? Article { get; set; }


        public virtual MaritalStatusDTO? Status { get; set; }
        public virtual ICollection<ShortJobApplicationDTO> JobApplications { get; set; }
        public virtual ICollection<ComputerProficiencyDTO> ComputerProficiencies { get; set; }
        public virtual ICollection<EducationalBackgroundDTO> EducationalBackgrounds { get; set; }
        public virtual ICollection<EvaluateDTO> Evaluates { get; set; }
        public virtual ICollection<FamilyInformationDTO> FamilyInformations { get; set; }
        public virtual ICollection<GeneralTestDTO> GeneralTests { get; set; }
        public virtual ICollection<Interview> Interviewers { get; set; }
        public virtual ICollection<LanguageProficiencyDTO> LanguageProficiencies { get; set; }
        public virtual ICollection<ScheduleDetailDTO> ScheduleDetails { get; set; }
        public virtual ICollection<ScheduleDTO> Schedules { get; set; }
        public virtual ICollection<WorkExperienceDTO> WorkExperiences { get; set; }
        public virtual ICollection<TrainigDTO> training { get; set; }


    }

    public class ShortJobApplicationDTO
    {

        public int ApplicationId { get; set; }
        public int? JobId { get; set; }
        public int? ProfileId { get; set; }
        public int? TemplateId { get; set; }
        public int? Status { get; set; }
        public int? Shorted { get; set; }
        public int? AssignedFor { get; set; }
        public string? CommentHR { get; set; }
        public string? UrlCandidateCV { get; set; }
        public DateTime? CreatedAt { get; set; }
        public virtual ShortJobDTO? Job { get; set; }
        public virtual ScheduleDTO? Schedule { get; set; }
        public virtual ICollection<ScheduleDetailResponse> ScheduleDetails { get; set; }
        public virtual SpecializedExamDTO? SpecializedExam { get; set; }
        public virtual AssignedProfileDTO? AssignedForInfor { get; set; }
        public virtual ICollection<ExamInformation> Exams { get; set; }

    }

    public partial class ShortJobDTO
    {
        public string? Title { get; set; }
        public string? Company { get; set; }

    }

}
