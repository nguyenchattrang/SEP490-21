using System.Text.RegularExpressions;

namespace RecruitXpress_BE.Helper
{
    public static class Constant
    {
     
        public static readonly Regex validateGuidRegex = new Regex("^(?=.*?[A-Z])(?=.*?[0-9]).{8,32}$");
        
        public static class GOOGLE_SERVICE
        {
            public const string SCOPE_URL = "https://accounts.google.com/o/oauth2/auth/oauthchooseaccount?";
            public const string CLIENT_ID = "738514220360-t74atgpqn4p0ooho0c9trgnukd1b13n8.apps.googleusercontent.com";
            public const string SCOPE = "https://www.googleapis.com/auth/userinfo.email";
            public const string RESPONSE_TYPE = "code";
            public const string ACCESS_TYPE = "offline";
        }
    }
}
