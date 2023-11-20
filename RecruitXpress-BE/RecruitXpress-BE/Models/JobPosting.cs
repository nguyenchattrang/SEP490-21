using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class JobPosting
    {
        public JobPosting()
        {
            JobApplications = new HashSet<JobApplication>();
            ShortListings = new HashSet<ShortListing>();
            WishLists = new HashSet<WishList>();
        }

        public int JobId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Company { get; set; }
        public string? Location { get; set; }
        public string? EmploymentType { get; set; }
        public string? Industry { get; set; }
        public string? Requirements { get; set; }
        public long? MinSalary { get; set; }
        public long? MaxSalary { get; set; }
        public DateTime? ApplicationDeadline { get; set; }
        public DateTime? DatePosted { get; set; }
        public string? ContactPerson { get; set; }
        public string? ApplicationInstructions { get; set; }
        public int? Status { get; set; }

        public virtual ICollection<JobApplication> JobApplications { get; set; }
        public virtual ICollection<ShortListing> ShortListings { get; set; }
        public virtual ICollection<WishList> WishLists { get; set; }
    }
}
