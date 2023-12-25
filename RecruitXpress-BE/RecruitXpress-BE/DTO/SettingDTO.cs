using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.DTO
{
    public class SettingDTO
    {
            public int NumberOfEasyQuestion { get; set; }
            public int NumberOfMediumQuestion { get; set; }
            public int NumberOfHardQuestion { get; set; }
            public int MaxFileSize { get; set; }
            public int ExpireExamTokenExpert { get; set; }
            public int ExpireRegisterAccountToken { get; set; }
            public int ExpireForgotPasswordAccountToken { get; set; }
        
    }

 
}
