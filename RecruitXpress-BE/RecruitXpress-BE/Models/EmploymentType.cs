using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class EmploymentType
    {
        public EmploymentType()
        {
            JobPostings = new HashSet<JobPosting>();
        }

        public int EmploymentTypeId { get; set; }
        public string? EmploymentTypeName { get; set; }

        public virtual ICollection<JobPosting> JobPostings { get; set; }
    }
}
