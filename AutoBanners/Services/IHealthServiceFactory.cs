namespace AutoBanners.Services;

public interface IHealthServiceFactory
{
    IHealthService<TConfiguration> Create<TConfiguration>(HttpClient client)
        where TConfiguration : class;
}