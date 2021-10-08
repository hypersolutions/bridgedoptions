using Microsoft.Extensions.Options;

namespace BridgedOptions.IntTests.Support
{
    public sealed class AccountConfigureOptions : IConfigureOptions<AccountOptions>
    {
        public void Configure(AccountOptions options)
        {
            options.Username = options.Username.ToLower();
        }
    }
}