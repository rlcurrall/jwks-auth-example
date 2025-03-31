using Microsoft.Extensions.Options;

namespace AuthServer.Extensions;

public static class RegisterConfigurationExtension
{
    public static IServiceCollection RegisterConfiguration<TCustomConfiguration>(
        this IServiceCollection services,
        string sectionName,
        ServiceLifetime serviceLifetime
    )
        where TCustomConfiguration : class, new()
    {
        ArgumentNullException.ThrowIfNull(services);

        if (string.IsNullOrWhiteSpace(sectionName))
        {
            throw new ArgumentNullException(nameof(sectionName));
        }

        _ = services
            .AddOptions<TCustomConfiguration>()
            .Configure<IConfiguration>(
                (customSetting, configuration) =>
                {
                    configuration.GetSection(sectionName).Bind(customSetting);
                }
            );

        services.Add(
            new ServiceDescriptor(
                typeof(TCustomConfiguration),
                provider =>
                {
                    var options = provider.GetRequiredService<IOptions<TCustomConfiguration>>();
                    return options.Value;
                },
                serviceLifetime
            )
        );

        return services;
    }

    public static IServiceCollection RegisterConfiguration<TCustomConfiguration>(
        this IServiceCollection services,
        ServiceLifetime serviceLifetime
    )
        where TCustomConfiguration : class, new()
    {
        return services.RegisterConfiguration<TCustomConfiguration>(
            typeof(TCustomConfiguration).Name,
            serviceLifetime
        );
    }
}
