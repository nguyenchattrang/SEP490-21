using RecruitXpress_BE.Helper;
using RecruitXpress_BE.IRepositories;

namespace RecruitXpress_BE.Repositories;

public class GoogleService : IGoogleService
{
    private readonly HttpClient _httpClient;

    public GoogleService()
    {
        _httpClient = new HttpClient();
    }

    public string GetAuthUrl(string redirectUrl)
    {
        try
        {
            return Constant.GOOGLE_SERVICE.SCOPE_URL
                + "redirect_uri=" + GoogleHelper.urlEncodeForGoogle(redirectUrl)
                + "&response_type=" + Constant.GOOGLE_SERVICE.RESPONSE_TYPE
                + "&client_id=" + Constant.GOOGLE_SERVICE.CLIENT_ID
                + "&scope=" + Constant.GOOGLE_SERVICE.SCOPE
                + "&access_type" + Constant.GOOGLE_SERVICE.ACCESS_TYPE;
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }
}