using System;
using Microsoft.Extensions.DependencyInjection;

namespace BridgedOptions.Internals
{
    internal static class InternalServiceProviderExtensions
    {
        internal static TOptionsInterface ResolveBridgedInstance<TOptions, TOptionsInterface>(
            this IServiceProvider provider, 
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