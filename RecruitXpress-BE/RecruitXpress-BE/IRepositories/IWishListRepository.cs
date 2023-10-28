using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories;

public interface IWishListRepository
{
    Task<List<WishList>> GetListWishLists(string? searchString, string? orderBy, bool? isSortAscending, int? page, int? size);
    Task<WishList?> GetWishList(int id);
    Task<WishList> AddWishList(WishList wishList);
    Task<bool> DeleteWishList(int jobId);
}