using RecruitXpress_BE.Models;
using System.ComponentModel.DataAnnotations;

namespace RecruitXpress_BE.DTO
{
    public class EvaluateDTO
    {
        public int EvaluateId { get; set; }
        public int JobApplicationId { get; set; }
        [Required]
        public int? CalendarId { get; set; }
        public int? ProfileId { get; set; }
        public string? Comments { get; set; }
        public string? Strengths { get; set; }
        public string? Weaknesses { get; set; }
        public double? Score { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? Status { get; set; }

    }
}
