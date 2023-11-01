using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RecruitXpress_BE.Models
{
    public partial class Cvtemplate
    {
        public Cvtemplate()
        {
            JobApplications = new HashSet<JobApplication>();
        }

        public int TemplateId { get; set; }
        public int? AccountId { get; set; }
        public string? Url { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? Status { get; set; }

        public virtual Account? Account { get; set; }
        [JsonIgnore]
        public virtual ICollection<JobApplication> JobApplications { get; set; }
    }
}
