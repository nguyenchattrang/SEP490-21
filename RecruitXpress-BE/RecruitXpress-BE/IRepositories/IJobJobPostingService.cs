namespace RecruitXpress_BE.IRepositories;

public interface IJobJobPostingService
{
    void FireAndForgetJob();
    void ReccuringJob();
    void DelayedJob();
    void ContinuationJob();
}