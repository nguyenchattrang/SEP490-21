using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RecruitXpress_BE.Models
{
    public partial class Profile
    {
        public Profile()
        {
            ComputerProficiencies = new HashSet<ComputerProficiency>();
            EducationalBackgrounds = new HashSet<EducationalBackground>();
            FamilyInformations = new HashSet<FamilyInformation>();
            JobApplications = new HashSet<JobApplication>();
            LanguageProficiencies = new HashSet<LanguageProficiency>();
            ScheduleDetails = new HashSet<ScheduleDetail>();
            WorkExperiences = new HashSet<WorkExperience>();
            Training = new HashSet<Training>();
        }
        [JsonIgnore]
        public int ProfileId { get; set; }
        [JsonIgnore]
        public int? AccountId { get; set; }
        [JsonIgnore]
        public int? StatusId { get; set; }
        [JsonIgnore]
        public string? Name { get; set; }
        [JsonIgnore]
        public string? Email { get; set; }
        [JsonIgnore]
        public string? PhoneNumber { get; set; }
        [JsonIgnore]
        public string? Address { get; set; }
        [JsonIgnore]
        public string? Avatar { get; set; }
        [JsonIgnore]
        public DateTime? DateOfBirth { get; set; }
        [JsonIgnore]
        public string? Gender { get; set; }
        [JsonIgnore]
        public string? Skills { get; set; }
        [JsonIgnore]
        public string? Accomplishment { get; set; }
        [JsonIgnore]
        public string? Strength { get; set; }
        [JsonIgnore]
        public string? Imperfection { get; set; }
        [JsonIgnore]
        public string? ResearchWork { get; set; }
        [JsonIgnore]
        public string? Article { get; set; }
        [JsonIgnore]
        public virtual Account? Account { get; set; }
        [JsonIgnore]
        public virtual MaritalStatus? Status { get; set; }
        [JsonIgnore]
        public virtual ICollection<ComputerProficiency> ComputerProficiencies { get; set; }
        [JsonIgnore]
        public virtual ICollection<EducationalBackground> EducationalBackgrounds { get; set; }
        [JsonIgnore]
        public virtual ICollection<FamilyInformation> FamilyInformations { get; set; }
        [JsonIgnore]
        public virtual ICollection<JobApplication> JobApplications { get; set; }
        [JsonIgnore]
        public virtual ICollection<LanguageProficiency> LanguageProficiencies { get; set; }
        [JsonIgnore]
        public virtual ICollection<ScheduleDetail> ScheduleDetails { get; set; }
        [JsonIgnore]
        public virtual ICollection<WorkExperience> WorkExperiences { get; set; }
        [JsonIgnore]
        public virtual ICollection<Training> Training { get; set; }
      
    }
}
