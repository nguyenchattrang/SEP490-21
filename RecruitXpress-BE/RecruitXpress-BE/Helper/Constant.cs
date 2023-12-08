﻿using Org.BouncyCastle.Utilities;
using System.Text.RegularExpressions;

namespace RecruitXpress_BE.Helper
{
    public static class Constant
    {

        public static readonly Regex validateGuidRegex = new Regex("^(?=.*?[A-Z])(?=.*?[0-9]).{8,32}$");
        public static int MaxFileSize = 25 * 1024 * 1024; //(Bytes = 25mb)
        public static int ExpireExamDays = 5;
        public static int ExpireRegisterAccountDays = 1;
        public static int ExpireForgotPasswordDays = 1;
        public static class GOOGLE_SERVICE
        {
            public const string SCOPE_URL = "https://accounts.google.com/o/oauth2/auth/oauthchooseaccount?";
            public const string CLIENT_ID = "10396174275-i2n51c5e8cs7embr3adli3tsqlkviibf.apps.googleusercontent.com";
            public const string CLIENT_SERCRET = "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";
            public const string RESPONSE_TYPE = "code";
            public const string ACCESS_TYPE = "offline";
            public const string CALL_BACK = "https://localhost:7113/auth/callback";
            public const string TOKEN_ENDPOINT = "https://accounts.google.com/o/oauth2/token";
            public const string USER_INFO_ENDPOINT = "https://www.googleapis.com/oauth2/v2/userinfo";
            public static class SCOPE
            {
                // public const string EMAIL = "https://www.googleapis.com/auth/userinfo.email";
                public const string EMAIL = "email";
            }
        }
        
        public static class ENTITY_STATUS
        {
            public const int ACTIVE = 1;
            public const int INACTIVE = 0;
            
        }
        
        public static class ROLE
        {
            public const int ADMIN = 1;
            public const int CANDIDATE = 2;
            public const int HUMAN_RESOURCE = 3;
            public const int INTERVIEWER = 4;
            public const int EXPERT = 5;
        }

        public static class MailType
        {
            public static int REFUSED = 0;
            public static int SUBMIT = 1;
            public static int PASSCV = 2;
            public static int EXAMSCHEDULE = 3;
            public static int GRADERESULT = 4;
            public static int INTERVIEWSCHEDULE = 6;
            public static int PASSINTERVIEW = 7;
            public static int ACCEPTED = 8;
            public static int CANCEL = 9;
            public static int HRINTERVIEWCV = 10;
            public static int HRINTERVIEWSCHEDULE = 11;
        }
        
        public static class SCHEDULE_TYPE
        {
            public static int EXAM = 2;
            public static int INTERVIEW = 1;
        }
        
        public static class APPLICATION_STATUS
        {
            public static int NOT_PASSED = 0;
            public static int WAIT_FOR_CV_CHECK = 1;
            public static int WAIT_FOR_EXAM_SCHEDULE = 2;
            public static int WAIT_FOR_EXAM = 3;
            public static int WAIT_FOR_GRADE = 4;
            public static int WAIT_FOR_INTERVIEW_SCHEDULE = 5;
            public static int WAIT_FOR_INTERVIEW = 6;
            public static int WAIT_FOR_SUBMIT_INFO = 7;
            public static int PASSED = 8;
            public static int CANCEL_BY_CANDIDATE = 9;
        }

        public static readonly Dictionary<int, List<string>> APPLICAION_STATUS_NOTIFICATION = new()
        {
            {0, new List<string>(){"Thông báo Kết Quả Ứng Tuyển", "Xin chào [Tên Ứng viên], chúng tôi xin được thông báo rằng đơn ứng tuyển của bạn chưa phù hợp với vị trí [industry]. Rất mong có cơ hội hợp tác với bạn trong tương lai."}},
            {1, new List<string>(){"Xác Nhận Đơn Ứng Tuyển", "Xin chào [candidateName], chúng tôi đã nhận được đơn ứng tuyển của bạn cho vị trí [industry]. Chúng tôi sẽ xem xét đơn của bạn và thông báo kết quả trong thời gian sớm nhất."}},
            {2, new List<string>(){"Thông Báo Vượt Qua Vòng Duyệt CV", "Xin chào [candidateName], chúc mừng bạn đã vượt qua vòng duyệt CV. Để tiếp tục quá trình tuyển dụng, chúng tôi sẽ lên lịch cho bạn thực hiện bài thi đánh giá chuyên môn. Chúng tôi sẽ liên lạc với bạn trong thời gian sớm nhất, vui lòng kiểm tra thông báo trong email của bạn."}},
            {3, new List<string>(){"Thông Báo Lịch Kiểm Tra Chuyên Môn", "Xin chào [candidateName], chúng tôi đã sắp xếp lịch làm bài thi đánh giá chuyên môn cho bạn. Vui lòng kiểm tra email để biết thêm thông tin chi tiết."}},
            {4, new List<string>(){"Thông Báo Kết Quả Bài Kiểm Tra", "Xin chào [candidateName], đã có kết quả bài kiểm tra của bạn. Vui lòng kiểm tra kết quả chi tiết. Nếu bạn có bất kỳ câu hỏi hoặc cần thêm thông tin, hãy liên hệ với chúng tôi."}},
            {5, new List<string>(){"Thông Báo Vượt Qua Bài Kiểm Tra Chuyên Môn", "Xin chào [candidateName], chúc mừng bạn đã vượt qua bài kiểm tra chuyên môn. Chúng tôi sẽ lên lịch phỏng vấn cho bạn trong thời gian sớm nhất. Vui lòng kiểm tra thông báo trong email của bạn."}},
            {6, new List<string>(){"Thông Báo Đã Có Lịch Phỏng Vấn", "Xin chào [candidateName], chúng tôi đã sắp xếp lịch phỏng vấn cho bạn. Vui lòng kiểm tra email để biết thêm thông tin chi tiết."}},
            {7, new List<string>(){"Thông Báo Cập Nhật Thông Tin Đầy Đủ", "Xin chào [candidateName], chúc mừng bạn đã vượt qua vòng phỏng vấn và có thể gia nhập vào đại gia đình [company]. Vui lòng cập nhật đầy đủ thông tin để hoàn tất quá trình uứng tuyển và xác nhận gia nhập vào đại gia đình. Nếu có bất kỳ thắc mắc nào, hãy vui lòng liên hệ với chúng tôi."}},
            {8, new List<string>(){"Chào mừng bạn đến với [company]", "Xin chúc mừng [Tên Ứng viên], chúng tôi chúc mừng bạn đã xuất sắc vượt qua các bài phỏng vấn để chính thức tiếp nhận vị trí [industry]. Chào mừng bạn gia nhập vào đại gia đình [company]."}},
            {9, new List<string>(){"Xác nhận Nhận Đơn Ứng viên", "Xin chào [candidateName], Chúng tôi đã nhận được đơn ứng tuyển của bạn cho vị trí [industry]. Chúng tôi sẽ xem xét đơn của bạn và thông báo kết quả trong thời gian sớm nhất."}},
        };
    }
    public static class ConstantQuestion
    {
        public static int easy = 5;
        public static int medium = 3;
        public static int hard = 2;
    }
}
