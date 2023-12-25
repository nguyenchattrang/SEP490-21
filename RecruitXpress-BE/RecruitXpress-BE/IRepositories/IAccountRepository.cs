﻿using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories;

public interface IAccountRepository
{
    Task<ApiResponse<AccountInfoDTO>> GetListAccount(AccountRequest request);
    Task<Account?> GetAccount(int id);

    Task<Account> AddAccount(Account account);
    Task<Account> UpdateAccount(int id, Account account);
    void DeleteAccount(int accountId);
}