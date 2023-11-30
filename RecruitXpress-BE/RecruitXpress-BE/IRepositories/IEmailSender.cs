namespace RecruitXpress_BE.IRepositories
{
    public interface IEmailSender
    {
        void Send(string to, string subject, string html, string from = null);
        void SendWithAttach(string to, string subject, string html, string filePath, string fileName, string from = null);
    }
}
