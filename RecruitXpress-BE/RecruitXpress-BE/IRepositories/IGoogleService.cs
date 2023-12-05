using RecruitXpress_BE.DTO;

namespace RecruitXpress_BE.IRepositories;

public interface IGoogleService
{
    string GetAuthUrl();
    Task<GoogleTokenResponse?> GetTokens(string? code);
    Task<GoogleUserInfo?> GetUserInfo(string accessToken);
}