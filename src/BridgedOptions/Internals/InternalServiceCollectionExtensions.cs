using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BridgedOptions.Internals
{
    internal static class InternalServiceCollectionExtensions
    {
        internal static Type RegisterOptionsAndBridge<TOptions>(
            this IServiceCollection services, 
            IConfigurationSection section)
            where TOptions : class
        {
            services.AddOptions();
            services.Configure<TOptions>(section);

            var bridgeViaTypeAttr = typeof(TOptions).GetCustomAttribute<BridgeViaTypeAttribute>();

            if (bridgeViaTypeAttr is null)
                throw new InvalidOperationException(
                    $"The BridgeViaTypeAttribute has not been defined on the {typeof(TOptions).Name} class.");
            
            services.TryAddSingleton(bridgeViaTypeAttr.BridgeType);

            return bridgeViaTypeAttr.BridgeType;
        }
    }
}