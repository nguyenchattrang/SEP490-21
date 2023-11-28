using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class District
    {
        public District()
        {
            JobPostings = new HashSet<JobPosting>();
        }

        public int DistrictId { get; set; }
        public int? CityId { get; set; }
        public string? DistrictName { get; set; }

        public virtual City? City { get; set; }
        public virtual ICollection<JobPosting> JobPostings { get; set; }
    }
}
