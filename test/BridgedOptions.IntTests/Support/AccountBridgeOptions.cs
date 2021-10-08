using System;
using System.Text;

namespace BridgedOptions.IntTests.Support
{
    public sealed class AccountBridgeOptions : IBridgeOptions<AccountOptions, IAccountInfo>
    {
        public IAccountInfo BridgeFrom(AccountOptions source)
        {
            return new AccountInfo
            {
                Username = source.Username,
                Password = Encoding.UTF8.GetString(Convert.FromBase64String(source.Password))
            };
        }
        
        private sealed class AccountInfo : IAccountInfo
        {
            public string Username { get; init; }
            public string Password { get; init; }
        }
    }
}