using BridgedOptions.Internals;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace BridgedOptions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOptionsBridge<TOptions, TOptionsInterface>(
            this IServiceCollection services,
            IConfigurationSection section)
            where TOptions : class
            where TOptionsInterface : class
        {
            var bridgeType = services.RegisterOptionsAndBridge<TOptions>(section);
            
            services.TryAddSingleton(provider =>
            {
                var options = provider.GetRequiredService<IOptions<TOptions>>();
                return provider.ResolveBridgedInstance<TOptions, TOptionsInterface>(options.Value, bridgeType);
            });
            return services;
        }
        
        public static IServiceCollection AddScopedOptionsBridge<TOptions, TOptionsInterface>(
            this IServiceCollection services,
            IConfigurationSection section)
            where TOptions : class
            where TOptionsInterface : class
        {
            var bridgeType = services.RegisterOptionsAndBridge<TOptions>(section);
            
            services.TryAddScoped(provider =>
            {
                var options = provider.GetRequiredService<IOptionsSnapshot<TOptions>>();
                return provider.ResolveBridgedInstance<TOptions, TOptionsInterface>(options.Value, bridgeType);
            });
            return services;
        }
        
        public static IServiceCollection AddMonitoredOptionsBridge<TOptions, TOptionsInterface>(
            this IServiceCollection services,
            IConfigurationSection section)
            where TOptions : class
            where TOptionsInterface : class
        {
            var bridgeType = services.RegisterOptionsAndBridge<TOptions>(section);

            services.TryAddTransient(provider =>
            {
                var options = provider.GetRequiredService<IOptionsMonitor<TOptions>>();
                return provider.ResolveBridgedInstance<TOptions, TOptionsInterface>(options.CurrentValue, bridgeType);
            });
            return services;
        }
    }
}
