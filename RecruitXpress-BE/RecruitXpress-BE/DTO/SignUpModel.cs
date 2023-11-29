using System.ComponentModel.DataAnnotations;

namespace RecruitXpress_BE.DTO
{
    public class SignUpModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string? FullName { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime? Dob { get; set; }
        [Required]
        public string? Gender { get; set; }
    }
}
