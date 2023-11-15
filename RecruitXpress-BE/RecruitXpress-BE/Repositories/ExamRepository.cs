using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Helper;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Repositories
{
    public class ExamRepository : IExamRepository
    {
        private readonly RecruitXpressContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailSender _sender;
        private readonly IConfiguration _configuration;
        public ExamRepository(RecruitXpressContext context, IMapper mapper, IEmailSender sender, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _sender = sender;
            _configuration = configuration;
        }

        public async Task<List<ExamDTO>> GetAllExams(ExamRequest request)
        {
            var query = _context.Exams
       .Include(e => e.Account)
            .AsQueryable();

            if (!string.IsNullOrEmpty(request.FileUrl))
            {
                query = query.Where(e => e.FileUrl.Contains(request.FileUrl));
            }

            if (request.TestDate.HasValue)
            {
                var testDate = request.TestDate.Value.Date;
                query = query.Where(e => e.TestDate != null && e.TestDate.Value.Date == testDate);
            }

            if (!string.IsNullOrEmpty(request.Point))
            {
                query = query.Where(e => e.Point.Contains(request.Point));
            }

            if (!string.IsNullOrEmpty(request.Comment))
            {
                query = query.Where(e => e.Comment != null && e.Comment.Contains(request.Comment));
            }

            if (!string.IsNullOrEmpty(request.MarkedBy))
            {
                query = query.Where(e => e.MarkedBy != null && e.MarkedBy.Contains(request.MarkedBy));
            }

            if (request.MarkedDate.HasValue)
            {
                var markedDate = request.MarkedDate.Value.Date;
                query = query.Where(e => e.MarkedDate != null && e.MarkedDate.Value.Date == markedDate);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(e => e.Status == request.Status);
            }

            if (request.AccountId.HasValue)
            {
                query = query.Where(e => e.AccountId == request.AccountId);
            }

            if (request.SortBy != null)
            {
                switch (request.SortBy)
                {
                    case "testDate":
                        query = request.OrderByAscending
                            ? query.OrderBy(e => e.TestDate)
                            : query.OrderByDescending(e => e.TestDate);
                        break;

                    case "point":
                        query = request.OrderByAscending
                            ? query.OrderBy(e => e.Point)
                            : query.OrderByDescending(e => e.Point);
                        break;

                    // Add other sorting options if needed.
                    default:
                        query = request.OrderByAscending
                            ? query.OrderBy(e => e.ExamId)
                            : query.OrderByDescending(e => e.ExamId);
                        break;
                }
            }

            var pageNumber = request.Page > 0 ? request.Page : 1;
            var pageSize = request.Size > 0 ? request.Size : 20;

            var exams = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var examDTOs = _mapper.Map<List<ExamDTO>>(exams);
            return examDTOs;
        }

        public async Task<List<ExamDTO>> GetListExamWithSpecializedExamId(ExamRequest request, int sid)
        {
            var query = _context.Exams
            .Where(e => e.SpecializedExamId == sid)
            .Include(e => e.Account)
            .AsQueryable();

            if (!string.IsNullOrEmpty(request.FileUrl))
            {
                query = query.Where(e => e.FileUrl.Contains(request.FileUrl));
            }

            if (request.TestDate.HasValue)
            {
                var testDate = request.TestDate.Value.Date;
                query = query.Where(e => e.TestDate != null && e.TestDate.Value.Date == testDate);
            }

            if (!string.IsNullOrEmpty(request.Point))
            {
                query = query.Where(e => e.Point.Contains(request.Point));
            }

            if (!string.IsNullOrEmpty(request.Comment))
            {
                query = query.Where(e => e.Comment != null && e.Comment.Contains(request.Comment));
            }

            if (!string.IsNullOrEmpty(request.MarkedBy))
            {
                query = query.Where(e => e.MarkedBy != null && e.MarkedBy.Contains(request.MarkedBy));
            }

            if (request.MarkedDate.HasValue)
            {
                var markedDate = request.MarkedDate.Value.Date;
                query = query.Where(e => e.MarkedDate != null && e.MarkedDate.Value.Date == markedDate);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(e => e.Status == request.Status);
            }

            if (request.AccountId.HasValue)
            {
                query = query.Where(e => e.AccountId == request.AccountId);
            }

            if (request.SortBy != null)
            {
                switch (request.SortBy)
                {
                    case "testDate":
                        query = request.OrderByAscending
                            ? query.OrderBy(e => e.TestDate)
                            : query.OrderByDescending(e => e.TestDate);
                        break;

                    case "point":
                        query = request.OrderByAscending
                            ? query.OrderBy(e => e.Point)
                            : query.OrderByDescending(e => e.Point);
                        break;

                    // Add other sorting options if needed.
                    default:
                        query = request.OrderByAscending
                            ? query.OrderBy(e => e.ExamId)
                            : query.OrderByDescending(e => e.ExamId);
                        break;
                }
            }

            var pageNumber = request.Page > 0 ? request.Page : 1;
            var pageSize = request.Size > 0 ? request.Size : 20;

            var exams = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var examDTOs = _mapper.Map<List<ExamDTO>>(exams);
            return examDTOs;
        }

        public async Task<ExamDTO> GetExamById(int examId)
        {
            var exam = await _context.Exams.FirstOrDefaultAsync(e => e.ExamId == examId);
            return _mapper.Map<ExamDTO>(exam);
        }

        public async Task<List<ExamDTO>> GetListExamWithSpecializedExamCode(ExamRequest request, string code, string expertEmail)
        {
            var allowed = await _context.AccessCodes
                 .Where(s => s.ExamCode.Contains(code) && s.Email.Equals(expertEmail) && s.ExpirationTimestamp > DateTime.Now).FirstOrDefaultAsync();
          
            if (allowed == null || !allowed.ExamCode.Equals(code))
            {
                throw new ArgumentException("Không có quyền truy cập");
            }

            var specializedExam = await _context.SpecializedExams
                 .Include(s => s.CreatedByNavigation)
                 .FirstOrDefaultAsync(s => s.Code.Contains(code));

            var query = _context.Exams
            .Where(e => e.SpecializedExamId == specializedExam.ExamId)
            .Include(e => e.Account)
            .AsQueryable();

            if (!string.IsNullOrEmpty(request.FileUrl))
            {
                query = query.Where(e => e.FileUrl.Contains(request.FileUrl));
            }

            if (request.TestDate.HasValue)
            {
                var testDate = request.TestDate.Value.Date;
                query = query.Where(e => e.TestDate != null && e.TestDate.Value.Date == testDate);
            }

            if (!string.IsNullOrEmpty(request.Point))
            {
                query = query.Where(e => e.Point.Contains(request.Point));
            }

            if (!string.IsNullOrEmpty(request.Comment))
            {
                query = query.Where(e => e.Comment != null && e.Comment.Contains(request.Comment));
            }

            if (!string.IsNullOrEmpty(request.MarkedBy))
            {
                query = query.Where(e => e.MarkedBy != null && e.MarkedBy.Contains(request.MarkedBy));
            }

            if (request.MarkedDate.HasValue)
            {
                var markedDate = request.MarkedDate.Value.Date;
                query = query.Where(e => e.MarkedDate != null && e.MarkedDate.Value.Date == markedDate);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(e => e.Status == request.Status);
            }

            if (request.AccountId.HasValue)
            {
                query = query.Where(e => e.AccountId == request.AccountId);
            }

            if (request.SortBy != null)
            {
                switch (request.SortBy)
                {
                    case "testDate":
                        query = request.OrderByAscending
                            ? query.OrderBy(e => e.TestDate)
                            : query.OrderByDescending(e => e.TestDate);
                        break;

                    case "point":
                        query = request.OrderByAscending
                            ? query.OrderBy(e => e.Point)
                            : query.OrderByDescending(e => e.Point);
                        break;

                    // Add other sorting options if needed.
                    default:
                        query = request.OrderByAscending
                            ? query.OrderBy(e => e.ExamId)
                            : query.OrderByDescending(e => e.ExamId);
                        break;
                }
            }

            var pageNumber = request.Page > 0 ? request.Page : 1;
            var pageSize = request.Size > 0 ? request.Size : 20;

            var exams = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var examDTOs = _mapper.Map<List<ExamDTO>>(exams);
            return examDTOs;
        }
        public async Task<Exam> CreateExamWithFile(ExamRequestClass exam, IFormFile fileData)
        {
            try
            {
                if (exam.AccountId == 0)
                {
                    throw new ArgumentException("Invalid AccountId");
                }
                if (exam.SpecializedExamId == 0)
                {
                    throw new ArgumentException("Invalid ExamId");
                }
                if (fileData == null || fileData.Length == 0)
                {
                    throw new ArgumentException("Please upload a file");
                }

                if (fileData.Length > Constant.MaxFileSize)
                {
                    throw new ArgumentException("File size exceeds the maximum allowed (25MB)");
                }

                var specExam = _context.SpecializedExams.Where(e => e.ExamId == exam.SpecializedExamId).FirstOrDefault();
                if (string.IsNullOrEmpty(specExam.Code))
                {
                    throw new ArgumentException("Invalid ExamId");
                }
                // Get the file extension
                var fileExtension = Path.GetExtension(fileData.FileName);
                int timestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                var fileName = $"{timestamp}_{exam.AccountId}{fileExtension}";

                string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, $"Upload\\ExamFiles"));
                string folder = $"{specExam.Code}";
                var newPath = Path.Combine(path, folder);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                // Save the file bytes
                var filePath = Path.Combine(path, folder, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await fileData.CopyToAsync(fileStream);
                }

                // Set the FileUrl property in the exam object
                var newExam = new Exam
                {
                    AccountId = exam.AccountId,
                    SpecializedExamId = exam.SpecializedExamId,
                    TestDate = DateTime.Now.Date,
                    TestTime = DateTime.Now,
                    FileUrl = Path.Combine(folder, fileName),
                    Status = specExam.EndDate > DateTime.Now ? 1 : 0,
                };


                // Add the exam to the database
                _context.Exams.Add(newExam);
                await _context.SaveChangesAsync();

                return newExam;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task AssignExpertToSystem(string email, string examCode)
        {
            var a = new AccessCode
            {
                Email = email,
                Code = GenerateUniqueCode(),
                ExamCode = examCode,
                ExpirationTimestamp = DateTime.Now.AddDays(Constant.ExpireExamDays),
            };
            _context.AccessCodes.AddAsync(a);
            var sExam=   _context.SpecializedExams.Where(s=> s.Code == examCode).FirstOrDefault();
             if(  sExam.ExpertEmail==null)
            { sExam.ExpertEmail = email; }
             else
            {
                sExam.ExpertEmail = sExam.ExpertEmail + "; "+ email ;
            }    
            await _context.SaveChangesAsync();
            string url = _configuration["Website:ClientUrl"] + "/LoginExpert";
            string urlExam = _configuration["Website:ClientUrl"] + "/Exam/"+examCode;
            _sender.Send(email, "Grant access", "Bạn được cấp quyền chấm bài cho bài thi có Examcode là: "+examCode +
                ".\nMật khẩu của bạn là: " + a.Code + ".\n Địa chỉ truy cập đăng nhập: "+url + ".\n Địa chỉ chấm bài: "+urlExam);
        }
        public async Task GradeExam(GradeExamRequest e)
        {
            var exam = _context.Exams.FirstOrDefault(ex => ex.ExamId == e.ExamId);
            if (exam == null)
            {
                throw new Exception("Không tìm được bài thi");
            }

            exam.Comment = e.Comment;
            exam.Point = e.Point;
            exam.MarkedBy = e.MarkedBy;
            exam.MarkedDate = DateTime.Now;

            await _context.SaveChangesAsync();
        }


        public async Task UpdateExam(Exam exam)
        {
            _context.ChangeTracker.Clear();
            _context.Entry(exam).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteExam(int examId)
        {

            var exam = await _context.Exams.Where(o => o.ExamId == examId).FirstOrDefaultAsync();
            if (exam == null)
            {
                return false;
            }
            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync();
            return true;
        }

        private string GenerateUniqueCode()
        {
            int maxAttempts = 10; // Set a maximum number of attempts.
            int attemptCount = 0;
            string generatedCode;

            do
            {
                // Generate a random code.
                generatedCode = TokenHelper.GenerateRandomToken(10);
                attemptCount++;

                // Check if the generated code is unique.
                var isCodeUnique = !_context.AccessCodes.Any(e => e.Code == generatedCode);

                if (isCodeUnique)
                {
                    return generatedCode;
                }
            } while (attemptCount < maxAttempts);

            // Handle the case where a unique code couldn't be generated.
            throw new Exception("Failed to generate a unique code after multiple attempts.");
        }
    }
}
