using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class Evaluate
    {
        public int EvaluateId { get; set; }
        public int JobApplicationId { get; set; }
        public int? CalendarId { get; set; }
        public int? ProfileId { get; set; }
        public string? Comments { get; set; }
        public string? Strengths { get; set; }
        public string? Weaknesses { get; set; }
        public double? Score { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? Status { get; set; }

        public virtual Calendar? Calendar { get; set; }
        public virtual JobApplication JobApplication { get; set; } = null!;
        public virtual Profile? Profile { get; set; }
    }
}
