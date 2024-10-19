using AutoBanners.Models;

namespace AutoBanners.Services;

public interface IBannerService
{
    Task<string> CreateBannerAsync(Banner banner);
    Task DeleteBannerAsync(string title);
    void Dispose();
}