using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepository;

public interface IJobPostingRepository
{
    Task<List<JobPosting>> GetListJobPostings();
    Task<JobPosting?> GetJobPosting(int id);
    Task<JobPosting> AddJobPosting(JobPosting jobPosting);
    Task<JobPosting> UpdateJobPostings(int id, JobPosting jobPosting);
    void DeleteJobPosting(int jobId);
}