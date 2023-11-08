using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class ScheduleDetail
    {
        public int ScheduleDetailId { get; set; }
        public int? ScheduleId { get; set; }
        public int? ProfileId { get; set; }
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
        public string? Strengths { get; set; }
        public string? Weaknesses { get; set; }
        public string? Result { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? CreatedBy { get; set; }

        public virtual Profile? Profile { get; set; }
        public virtual Schedule? Schedule { get; set; }
    }
}
