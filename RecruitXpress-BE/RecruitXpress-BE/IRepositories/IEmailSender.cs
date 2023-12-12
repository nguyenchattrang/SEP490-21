namespace RecruitXpress_BE.IRepositories
{
    public interface IEmailSender
    {
        Task Send(string to, string subject, string html, string from = null);
        Task SendWithAttach(string to, string subject, string html, string filePath, string fileName, string from = null);
    }
}
