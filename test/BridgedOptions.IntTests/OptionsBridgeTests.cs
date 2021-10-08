using System.Collections.Generic;
using BridgedOptions.IntTests.Support;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace BridgedOptions.IntTests
{
    public class OptionsBridgeTests
    {
        [Fact]
        public void Base64EncodedPasswordIsReturnedDecoded()
        {
            var services = new ServiceCollection();
            var section = BuildConfigSection("homers", "NTFtcDUwbjU=");
            
            services.AddOptionsBridge<AccountOptions, IAccountInfo>(section);
            
            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IAccountInfo>();
            options.Username.ShouldBe("homers");
            options.Password.ShouldBe("51mp50n5");
        }
        
        [Fact]
        public void NullPasswordIsValidatedByRegisteredValidateOptions()
        {
            var services = new ServiceCollection();
            var section = BuildConfigSection("homer2", null);
            services.AddOptionsBridge<AccountOptions, IAccountInfo>(section);
            services.TryAddEnumerable(ServiceDescriptor
                .Singleton<IValidateOptions<AccountOptions>, AccountValidateOptions>());
            var provider = services.BuildServiceProvider();
            
            var exception = Should.Throw<OptionsValidationException>(() => provider.GetRequiredService<IAccountInfo>());
            
            exception.Message.ShouldBe("Invalid password.");
        }
        
        [Fact]
        public void UppercasePasswordIsLowercasedByRegisteredConfigureOptions()
        {
            var services = new ServiceCollection();
            var section = BuildConfigSection("HOMERS", "NTFtcDUwbjU=");
            services.AddOptionsBridge<AccountOptions, IAccountInfo>(section);
            services.AddSingleton<IConfigureOptions<AccountOptions>, AccountConfigureOptions>();
            var provider = services.BuildServiceProvider();
            
            var options = provider.GetRequiredService<IAccountInfo>();
            options.Username.ShouldBe("homers");
            options.Password.ShouldBe("51mp50n5");
        }
        
        private static IConfigurationSection BuildConfigSection(string username, string password)
        {
            var memoryConfig = new MemoryConfigurationSource();
            var items = new List<KeyValuePair<string, string>>
            {
                new("Account:Username", username),
                new("Account:Password", password)
            };
            memoryConfig.InitialData = items;
            var configuration = new ConfigurationBuilder().Add(memoryConfig).Build();
            return configuration.GetSection("Account");
        }
    }
}