namespace AutoBanners.Services.Abstractions;

public interface IHealthServiceFactory
{
    IHealthService<TConfiguration> Create<TConfiguration>(HttpClient client)
        where TConfiguration : class;
}