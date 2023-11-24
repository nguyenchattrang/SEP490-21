using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class Industry
    {
        public Industry()
        {
            JobPostings = new HashSet<JobPosting>();
        }

        public int IndustryId { get; set; }
        public string? IndustryName { get; set; }

        public virtual ICollection<JobPosting> JobPostings { get; set; }
    }
}
