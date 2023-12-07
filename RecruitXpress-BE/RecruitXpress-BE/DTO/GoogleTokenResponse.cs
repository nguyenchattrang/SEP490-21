﻿namespace RecruitXpress_BE.DTO;

public class GoogleTokenResponse
{
    public string access_token { get; set; }

    public long expires_in { get; set; }

    public string refresh_token { get; set; }

    public string scope { get; set; }

    public string token_type { get; set; }
    public string id_token { get; set; }
}

public class GoogleUserInfo
{
    public string Email { get; set; }
}