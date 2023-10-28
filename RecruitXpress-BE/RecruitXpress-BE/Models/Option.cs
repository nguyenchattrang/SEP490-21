using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class Option
    {
        public int OptionId { get; set; }
        public int? QuestionId { get; set; }
        public string? OptionText { get; set; }
        public bool? IsCorrect { get; set; }
        public int? Status { get; set; }

        public virtual Question? Question { get; set; }
    }
}
