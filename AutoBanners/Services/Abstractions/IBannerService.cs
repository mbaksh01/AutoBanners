using AutoBanners.Models;

namespace AutoBanners.Services.Abstractions;

public interface IBannerService : IDisposable
{
    Task<string> CreateBannerAsync(Banner banner);
    Task DeleteBannerAsync(string title);
}