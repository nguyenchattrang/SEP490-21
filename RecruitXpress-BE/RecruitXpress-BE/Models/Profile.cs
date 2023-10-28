using System;
using System.Collections.Generic;

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
            training = new HashSet<training>();
        }

        public int ProfileId { get; set; }
        public int? AccountId { get; set; }
        public int? StatusId { get; set; }
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

        public virtual Account? Account { get; set; }
        public virtual MaritalStatus? Status { get; set; }
        public virtual ICollection<ComputerProficiency> ComputerProficiencies { get; set; }
        public virtual ICollection<EducationalBackground> EducationalBackgrounds { get; set; }
        public virtual ICollection<FamilyInformation> FamilyInformations { get; set; }
        public virtual ICollection<JobApplication> JobApplications { get; set; }
        public virtual ICollection<LanguageProficiency> LanguageProficiencies { get; set; }
        public virtual ICollection<ScheduleDetail> ScheduleDetails { get; set; }
        public virtual ICollection<WorkExperience> WorkExperiences { get; set; }
        public virtual ICollection<training> training { get; set; }
    }
}
