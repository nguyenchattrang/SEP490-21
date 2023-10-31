﻿
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.DTO
{
    public class JobApplicationDTO
    {
        public int ApplicationId { get; set; }
        public int? JobId { get; set; }
        public int? ProfileId { get; set; }
        public int? TemplateId { get; set; }
        public int? Status { get; set; }

        public virtual JobPosting? Job { get; set; }

        public virtual Profile? Profile { get; set; }

        public virtual Cvtemplate? Template { get; set; }

    }
}