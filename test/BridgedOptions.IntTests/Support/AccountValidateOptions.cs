using Microsoft.Extensions.Options;

namespace BridgedOptions.IntTests.Support
{
    public sealed class AccountValidateOptions : IValidateOptions<AccountOptions>
    {
        public ValidateOptionsResult Validate(string name, AccountOptions options)
        {
            return options.Password is null 
                ? ValidateOptionsResult.Fail("Invalid password.") 
                : ValidateOptionsResult.Success;
        }
    }
}