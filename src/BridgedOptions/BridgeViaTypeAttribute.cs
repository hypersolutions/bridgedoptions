using System;
using System.Linq;

namespace BridgedOptions
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class BridgeViaTypeAttribute : Attribute
    {
        public BridgeViaTypeAttribute(Type bridgeType)
        {
            BridgeType = ValidateAndSetType(bridgeType);
        }
        
        public Type BridgeType { get; }

        private static Type ValidateAndSetType(Type bridgeType)
        {
            if (bridgeType is null)
                throw new ArgumentNullException(nameof(bridgeType));

            if (!bridgeType.GetInterfaces().Any(IsInterfaceTypeBridge))
                throw new ArgumentException(
                    "The bridge type does not implement the IBridgeOptions interface.", nameof(bridgeType));

            return bridgeType;
        }

        private static bool IsInterfaceTypeBridge(Type interfaceType)
        {
            return interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IBridgeOptions<,>);
        }
    }
}