using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.DTO
{
    public class AccountDTO
    {
            public int AccountId { get; set; }
            public string? Username { get; set; }
            public int? RoleId { get; set; }
        
    }
    public class AccountInfoDTO
    {
        public int AccountId { get; set; }
        public string? Username { get; set; }
        public int? RoleId { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public DateTime? Dob { get; set; }
        public string? Gender { get; set; }

    }
    public class AccountInformation
    {
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public DateTime? Dob { get; set; }
        public string? Gender { get; set; }

    }
    public class AccountResponse : ApiResponse<AccountInfoDTO>
    {

    }
}
