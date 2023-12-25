using System.Net.Http.Headers;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using System.Text;
using Google.Apis.Auth.OAuth2.Responses;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Helper;
using RecruitXpress_BE.IRepositories;

namespace RecruitXpress_BE.Repositories;

public class GoogleService : IGoogleService
{
    private readonly HttpClient _httpClient = new();
    private readonly IConfiguration _configuration;

    public GoogleService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetAuthUrl()
    {
        try
        {
            return
                $"{_configuration["GoogleService:ScopeUrl"]}" +
                // "prompt=consent" +
                $"redirect_uri={GoogleHelper.urlEncodeForGoogle(_configuration["Website:HostUrl"] + "/api/Authentication/auth/callback")}" +
                $"&response_type={Constant.GOOGLE_SERVICE.RESPONSE_TYPE}" +
                $"&client_id={_configuration["GoogleService:ClientId"]}" +
                $"&scope={Constant.GOOGLE_SERVICE.SCOPE.EMAIL}+{Constant.GOOGLE_SERVICE.SCOPE.PROFILE}" +
                $"&access_type={Constant.GOOGLE_SERVICE.ACCESS_TYPE}";
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }

    public async Task<GoogleTokenResponse?> GetTokens(string? code)
    {
        var content = new StringContent(
            $"code={code}" +
            $"&redirect_uri={Uri.EscapeDataString(_configuration["Website:HostUrl"] + "/api/Authentication/auth/callback")}" +
            $"&response_type={Constant.GOOGLE_SERVICE.RESPONSE_TYPE}" +
            $"&client_id={_configuration["GoogleService:ClientId"]}" +
            $"&client_secret={_configuration["GoogleService:ClientSecret"]}" +
            $"&scope={Constant.GOOGLE_SERVICE.SCOPE.EMAIL}+{Constant.GOOGLE_SERVICE.SCOPE.PROFILE}" +
            $"&access_type={Constant.GOOGLE_SERVICE.ACCESS_TYPE}" +
            $"&grant_type=authorization_code",
            Encoding.UTF8, "application/x-www-form-urlencoded");

        var response = await _httpClient.PostAsync(Constant.GOOGLE_SERVICE.TOKEN_ENDPOINT, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleTokenResponse>(responseContent);
        }
        else
        {
            // Handle the error case when authentication fails
            throw new Exception($"Lỗi trong quá trình đăng nhập: {responseContent}");
        }
    }
    
    public async Task<GoogleUserInfo?> GetUserInfo(string accessToken)
    {
        var userInfoRequest = new HttpRequestMessage(HttpMethod.Get, Constant.GOOGLE_SERVICE.USER_INFO_ENDPOINT);
        userInfoRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var userInfoResponse = await _httpClient.SendAsync(userInfoRequest);
        if (userInfoResponse.IsSuccessStatusCode)
        {
            var userInfoContent = await userInfoResponse.Content.ReadAsStringAsync();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleUserInfo>(userInfoContent);
        }
        else
        {
            throw new Exception($"Lỗi trong quá trình lấy thông tin người dùng: {userInfoResponse.ReasonPhrase}");
        }
    }
}