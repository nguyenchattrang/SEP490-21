using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Repositories
{
    public class ExamRepository
    {
        private readonly RecruitXpressContext _context;

        public ExamRepository(RecruitXpressContext context)
        {
            _context = context;
        }

        public async Task<List<Exam>> GetAllExams()
        {
            return await _context.Exams.ToListAsync();
        }

        public async Task<Exam> GetExamById(int examId)
        {
            return await _context.Exams.FirstOrDefaultAsync(e => e.ExamId == examId);
        }

        public async Task<Exam> CreateExamWithFile(Exam exam, IFormFile fileData)
        {
            try
            {
                if (exam.AccountId == 0)
                {
                    throw new ArgumentException("Invalid AccountId");
                }

                if (fileData == null || fileData.Length == 0)
                {
                    throw new ArgumentException("Please upload a file");
                }

                // Get the file extension
                var fileExtension = Path.GetExtension(fileData.FileName);
                int timestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                var fileName = $"{timestamp}_{exam.AccountId}{fileExtension}";

                string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Upload\\ExamFiles"));

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // Save the file bytes
                var filePath = Path.Combine(path, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await fileData.CopyToAsync(fileStream);
                }

                // Set the FileUrl property in the exam object
                exam.FileUrl = "\\" + fileName;

                // Add the exam to the database
                _context.Exams.Add(exam);
                await _context.SaveChangesAsync();

                return exam;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateExam(Exam exam)
        {
            _context.Entry(exam).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
