using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.DTO
{
    public class ProfileDTO
    {
        public int ProfileId { get; set; }
        public int? AccountId { get; set; }
        public int? StatusId { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public DateTime? Dob { get; set; }
        public string? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public string? Skills { get; set; }
        public string? Accomplishment { get; set; }
        public string? Strength { get; set; }
        public string? Imperfection { get; set; }
        public string? ResearchWork { get; set; }
        public string? Article { get; set; }

       
    }
}
