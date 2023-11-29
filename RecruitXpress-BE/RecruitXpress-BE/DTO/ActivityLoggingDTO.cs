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
            Interviewers = new HashSet<Interviewer>();
            JobApplications = new HashSet<JobApplicationDTO>();
            LanguageProficiencies = new HashSet<LanguageProficiencyDTO>();
            ScheduleDetails = new HashSet<ScheduleDetailDTO>();
            Schedules = new HashSet<ScheduleDTO>();
            WorkExperiences = new HashSet<WorkExperienceDTO>();
            training = new HashSet<training>();
        }
        public int ProfileId { get; set; }
        public int? AccountId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Skills { get; set; }
        public string? Accomplishment { get; set; }
        public string? Strength { get; set; }
        public string? Imperfection { get; set; }
        public string? ResearchWork { get; set; }
        public string? Article { get; set; }

        public virtual MaritalStatusDTO? Status { get; set; }
        public virtual ICollection<ComputerProficiencyDTO> ComputerProficiencies { get; set; }
        public virtual ICollection<EducationalBackgroundDTO> EducationalBackgrounds { get; set; }
        public virtual ICollection<EvaluateDTO> Evaluates { get; set; }
        public virtual ICollection<FamilyInformationDTO> FamilyInformations { get; set; }
        public virtual ICollection<GeneralTestDTO> GeneralTests { get; set; }
        public virtual ICollection<Interviewer> Interviewers { get; set; }
        public virtual ICollection<JobApplicationDTO> JobApplications { get; set; }
        public virtual ICollection<LanguageProficiencyDTO> LanguageProficiencies { get; set; }
        public virtual ICollection<ScheduleDetailDTO> ScheduleDetails { get; set; }
        public virtual ICollection<ScheduleDTO> Schedules { get; set; }
        public virtual ICollection<WorkExperienceDTO> WorkExperiences { get; set; }
        public virtual ICollection<training> training { get; set; }


    }
    }
