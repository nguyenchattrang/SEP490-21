using Org.BouncyCastle.Utilities;
using System.Text.RegularExpressions;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;

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
            public const string CALL_BACK = "https://localhost:7113/api/Authentication/auth/callback";
            public const string TOKEN_ENDPOINT = "https://accounts.google.com/o/oauth2/token";
            public const string USER_INFO_ENDPOINT = "https://www.googleapis.com/oauth2/v2/userinfo";

            public static class SCOPE
            {
                public const string PROFILE = "profile";
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
            public static int CanceledJobToHR = 12;
            public static int UpdateExamSchedule = 13;
            public static int DeleteExamSchedule = 14;
            public static int UpdateInterviewScheduleForCandidate = 15;
            public static int DeleteInterviewcheduleForCandidate = 16;
            public static int UpdateInterviewScheduleForInterviewer = 17;
            public static int DeleteInterviewcheduleForInterviewer = 18;



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

        public static readonly Dictionary<StatusChange, NotificationMessage> APPLICAION_STATUS_NOTIFICATION = new()
        {
            {
                new StatusChange()
                {
                    OldStatus = 1,
                    NewStatus = 0
                }, new NotificationMessage()
                {
                    Title = "Thông báo Kết Quả Ứng Tuyển",
                    Description =
                        "Xin chào @candidateName, chúng tôi rất tiếc phải thông báo rằng đơn ứng tuyển của bạn chưa phù hợp với vị trí @industry. Rất mong có cơ hội hợp tác với bạn trong tương lai.",
                    TargetUrl = ""
                }
            },
            {
                new StatusChange()
                {
                    OldStatus = 2,
                    NewStatus = 0
                }, new NotificationMessage()
                {
                    Title = "Thông báo Kết Quả Ứng Tuyển",
                    Description =
                        "Xin chào @candidateName, chúng tôi rất tiếc phải thông báo rằng đơn ứng tuyển của bạn chưa phù hợp với vị trí @industry. Rất mong có cơ hội hợp tác với bạn trong tương lai.",
                    TargetUrl = ""
                }
            },
            {
                new StatusChange()
                {
                    OldStatus = 3,
                    NewStatus = 0
                }, new NotificationMessage()
                {
                    Title = "Thông báo Kết Quả Ứng Tuyển",
                    Description =
                        "Xin chào @candidateName, chúng tôi rất tiếc phải thông báo rằng đơn ứng tuyển của bạn chưa phù hợp với vị trí @industry. Rất mong có cơ hội hợp tác với bạn trong tương lai.",
                    TargetUrl = ""
                }
            },
            {
                new StatusChange()
                {
                    OldStatus = 4,
                    NewStatus = 0
                }, new NotificationMessage()
                {
                    Title = "Thông báo Kết Quả Ứng Tuyển",
                    Description =
                        "Xin chào @candidateName, chúng tôi rất tiếc phải thông báo rằng đơn ứng tuyển của bạn chưa phù hợp với vị trí @industry. Rất mong có cơ hội hợp tác với bạn trong tương lai.",
                    TargetUrl = ""
                }
            },
            {
                new StatusChange()
                {
                    OldStatus = 5,
                    NewStatus = 0
                }, new NotificationMessage()
                {
                    Title = "Thông báo Kết Quả Ứng Tuyển",
                    Description =
                        "Xin chào @candidateName, chúng tôi rất tiếc phải thông báo rằng đơn ứng tuyển của bạn chưa phù hợp với vị trí @industry. Rất mong có cơ hội hợp tác với bạn trong tương lai.",
                    TargetUrl = ""
                }
            },
            {
                new StatusChange()
                {
                    OldStatus = 6,
                    NewStatus = 0
                }, new NotificationMessage()
                {
                    Title = "Thông báo Kết Quả Ứng Tuyển",
                    Description =
                        "Xin chào @candidateName, chúng tôi rất tiếc phải thông báo rằng đơn ứng tuyển của bạn chưa phù hợp với vị trí @industry. Rất mong có cơ hội hợp tác với bạn trong tương lai.",
                    TargetUrl = ""
                }
            },
            {
                new StatusChange()
                {
                    OldStatus = 7,
                    NewStatus = 0
                }, new NotificationMessage()
                {
                    Title = "Thông báo Kết Quả Ứng Tuyển",
                    Description =
                        "Xin chào @candidateName, chúng tôi rất tiếc phải thông báo rằng đơn ứng tuyển của bạn chưa phù hợp với vị trí @industry. Rất mong có cơ hội hợp tác với bạn trong tương lai.",
                    TargetUrl = ""
                }
            },
            {
                new StatusChange()
                {
                    OldStatus = 8,
                    NewStatus = 0
                }, new NotificationMessage()
                {
                    Title = "Thông báo Kết Quả Ứng Tuyển",
                    Description =
                        "Xin chào @candidateName, chúng tôi rất tiếc phải thông báo rằng đơn ứng tuyển của bạn chưa phù hợp với vị trí @industry. Rất mong có cơ hội hợp tác với bạn trong tương lai.",
                    TargetUrl = ""
                }
            },
            {
                new StatusChange()
                {
                    OldStatus = 9,
                    NewStatus = 0
                }, new NotificationMessage()
                {
                    Title = "Thông báo Kết Quả Ứng Tuyển",
                    Description =
                        "Xin chào @candidateName, chúng tôi rất tiếc phải thông báo rằng đơn ứng tuyển của bạn chưa phù hợp với vị trí @industry. Rất mong có cơ hội hợp tác với bạn trong tương lai.",
                    TargetUrl = ""
                }
            },
            {
                new StatusChange()
                {
                    OldStatus = 0,
                    NewStatus = 1
                },
                new NotificationMessage()
                {
                    Title = "Xác Nhận Đơn Ứng Tuyển",
                    Description = "Xin chào @candidateName, chúng tôi đã nhận được đơn ứng tuyển của bạn cho vị trí @industry. Chúng tôi sẽ xem xét đơn của bạn và thông báo kết quả trong thời gian sớm nhất.",
                    TargetUrl = ""
                }
            },
            {
                new StatusChange()
                {
                    OldStatus = 1,
                    NewStatus = 2
                },
                new NotificationMessage()
                {
                    Title = "Thông Báo Vượt Qua Vòng Duyệt CV",
                    Description = "Xin chào @candidateName, chúc mừng bạn đã vượt qua vòng duyệt CV. Để tiếp tục quá trình tuyển dụng, chúng tôi sẽ lên lịch cho bạn thực hiện bài thi đánh giá chuyên môn. Chúng tôi sẽ liên lạc với bạn trong thời gian sớm nhất, vui lòng kiểm tra thông báo trong email của bạn.",
                    TargetUrl = ""
                }
            },
            {
                new StatusChange()
                {
                    OldStatus = 2,
                    NewStatus = 3
                },
                new NotificationMessage()
                {
                    Title = "Thông Báo Lịch Kiểm Tra Chuyên Môn",
                    Description = "Xin chào @candidateName, chúng tôi đã sắp xếp lịch làm bài thi đánh giá chuyên môn cho bạn. Vui lòng kiểm tra email để biết thêm thông tin chi tiết.",
                    TargetUrl = ""
                }
            },
            {
                new StatusChange()
                {
                    OldStatus = 3,
                    NewStatus = 4
                },
                new NotificationMessage()
                {
                    Title = "Thông Báo Kết Quả Bài Kiểm Tra",
                    Description = "Xin chào @candidateName, đã có kết quả bài kiểm tra của bạn. Vui lòng kiểm tra kết quả chi tiết. Nếu bạn có bất kỳ câu hỏi hoặc cần thêm thông tin, hãy liên hệ với chúng tôi.",
                    TargetUrl = ""
                }
            },
            {
                new StatusChange()
                {
                    OldStatus = 4,
                    NewStatus = 5
                },
                new NotificationMessage()
                {
                    Title = "Thông Báo Vượt Qua Bài Kiểm Tra Chuyên Môn",
                    Description = "Xin chào @candidateName, chúc mừng bạn đã vượt qua bài kiểm tra chuyên môn. Chúng tôi sẽ lên lịch phỏng vấn cho bạn trong thời gian sớm nhất. Vui lòng kiểm tra thông báo trong email của bạn.",
                    TargetUrl = ""
                }
            },
            {
                new StatusChange()
                {
                    OldStatus = 5,
                    NewStatus = 6
                },
                new NotificationMessage()
                {
                    Title = "Thông Báo Đã Có Lịch Phỏng Vấn",
                    Description = "Xin chào @candidateName, chúng tôi đã sắp xếp lịch phỏng vấn cho bạn. Vui lòng kiểm tra email để biết thêm thông tin chi tiết.",
                    TargetUrl = ""
                }
            },
            {
                new StatusChange()
                {
                    OldStatus = 6,
                    NewStatus = 7
                },
                new NotificationMessage()
                {
                    Title = "Thông Báo Cập Nhật Thông Tin Đầy Đủ",
                    Description = "Xin chào @candidateName, chúc mừng bạn đã vượt qua vòng phỏng vấn và có thể gia nhập vào đại gia đình @company. Vui lòng cập nhật đầy đủ thông tin để hoàn tất quá trình ứng tuyển và xác nhận gia nhập vào đại gia đình. Nếu có bất kỳ thắc mắc nào, hãy vui lòng liên hệ với chúng tôi.",
                    TargetUrl = ""
                }
            },
            {
                new StatusChange()
                {
                    OldStatus = 7,
                    NewStatus = 8
                },
                new NotificationMessage()
                {
                    Title = "Chào mừng bạn đến với @company",
                    Description = "Xin chúc mừng @candidateName, chúng tôi hân hạnh thông báo rằng bạn đã được chọn để gia nhập với gia đình @company. Chúng tôi tin rằng bạn sẽ là một thành viên tài năng và đóng góp tích cực cho sự phát triển của công ty.",
                    TargetUrl = ""
                }
            }
        };
    }

    public static class ConstantQuestion
    {
        public static int easy = 5;
        public static int medium = 3;
        public static int hard = 2;
    }
}