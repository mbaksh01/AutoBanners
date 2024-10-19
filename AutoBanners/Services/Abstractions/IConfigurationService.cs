using AutoBanners.Models;

namespace AutoBanners.Services.Abstractions;

public interface IConfigurationService
{
    Configuration? Configuration { get; set; }
    Task LoadConfigurationAsync();
}