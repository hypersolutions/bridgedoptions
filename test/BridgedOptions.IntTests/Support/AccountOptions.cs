// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace BridgedOptions.IntTests.Support
{
    [BridgeViaType(typeof(AccountBridgeOptions))]
    public sealed class AccountOptions
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}