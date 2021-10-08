using System;
using System.Reflection;
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
            var bridgeType = RegisterOptionsAndBridge<TOptions>(services, section);
            
            services.TryAddSingleton(provider =>
            {
                var options = provider.GetRequiredService<IOptions<TOptions>>();
                return ResolveBridgedInstance<TOptions, TOptionsInterface>(provider, options.Value, bridgeType);
            });
            return services;
        }
        
        public static IServiceCollection AddScopedOptionsBridge<TOptions, TOptionsInterface>(
            this IServiceCollection services,
            IConfigurationSection section)
            where TOptions : class
            where TOptionsInterface : class
        {
            var bridgeType = RegisterOptionsAndBridge<TOptions>(services, section);
            
            services.TryAddScoped(provider =>
            {
                var options = provider.GetRequiredService<IOptionsSnapshot<TOptions>>();
                return ResolveBridgedInstance<TOptions, TOptionsInterface>(provider, options.Value, bridgeType);
            });
            return services;
        }
        
        public static IServiceCollection AddMonitoredOptionsBridge<TOptions, TOptionsInterface>(
            this IServiceCollection services,
            IConfigurationSection section)
            where TOptions : class
            where TOptionsInterface : class
        {
            var bridgeType = RegisterOptionsAndBridge<TOptions>(services, section);

            services.TryAddTransient(provider =>
            {
                var options = provider.GetRequiredService<IOptionsMonitor<TOptions>>();
                return ResolveBridgedInstance<TOptions, TOptionsInterface>(provider, options.CurrentValue, bridgeType);
            });
            return services;
        }

        private static Type RegisterOptionsAndBridge<TOptions>(
            IServiceCollection services,
            IConfigurationSection section)
            where TOptions : class
        {
            services.AddOptions();
            services.Configure<TOptions>(section.Bind);

            var bridgeViaTypeAttr = typeof(TOptions).GetCustomAttribute<BridgeViaTypeAttribute>();

            if (bridgeViaTypeAttr is null)
                throw new InvalidOperationException(
                    $"The BridgeViaTypeAttribute has not been defined on the {typeof(TOptions).Name} class.");
            
            services.TryAddSingleton(bridgeViaTypeAttr.BridgeType);

            return bridgeViaTypeAttr.BridgeType;
        }

        private static TOptionsInterface ResolveBridgedInstance<TOptions, TOptionsInterface>(
            IServiceProvider provider, 
            TOptions options,
            Type bridgeType)
            where TOptions : class
            where TOptionsInterface : class
        {
            var bridgeOptions = (IBridgeOptions<TOptions, TOptionsInterface>)provider.GetRequiredService(bridgeType);
            return bridgeOptions.BridgeFrom(options);
        }
    }
}
