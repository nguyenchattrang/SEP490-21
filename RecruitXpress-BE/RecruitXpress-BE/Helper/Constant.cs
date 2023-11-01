using System.Text.RegularExpressions;

namespace RecruitXpress_BE.Helper
{
    public static class Constant
    {
     
        public static readonly Regex validateGuidRegex = new Regex("^(?=.*?[A-Z])(?=.*?[0-9]).{8,32}$");


    }
    public static class ConstantQuestion
    {

        public static int easy = 1;
        public static int medium = 1;
        public static int hard = 1;


    }
}
