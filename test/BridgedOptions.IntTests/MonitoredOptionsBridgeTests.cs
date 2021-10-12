using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BridgedOptions.IntTests.Support;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace BridgedOptions.IntTests
{
    public class MonitoredOptionsBridgeTests
    {
        [Fact]
        public async Task UpdatingConfigOnDiskUpdatesBridgeOptions()
        {
            UpdateConfig("homers", "Simpsons");
            var configuration = BuildConfig();
            
            var services = new ServiceCollection();
            services.AddMonitoredOptionsBridge<AccountOptions, IAccountInfo>(configuration.GetSection("Account"));
            
            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IAccountInfo>();
            options.Username.ShouldBe("homers");
            options.Password.ShouldBe("Simpsons");
            
            UpdateConfig("homers", "51mp50n5");

            var hasChanged = false;
            var attempts = 20;
            var sourceOptions = provider.GetRequiredService<IOptionsMonitor<AccountOptions>>();
            sourceOptions.OnChange(_ => hasChanged = true);

            while (!hasChanged)
            {
                await Task.Delay(100);
                attempts--;

                if (attempts < 0) break;
            }
            
            hasChanged.ShouldBeTrue("Failed to update the config in a timely manner.");
            options = provider.GetRequiredService<IAccountInfo>();
            options.Username.ShouldBe("homers");
            options.Password.ShouldBe("51mp50n5");
        }

        private static void UpdateConfig(string username, string password)
        {
            var options = new AccountOptions
            {
                Username = username, 
                Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(password))
            };
            var json = JsonSerializer.Serialize(new Options { Account = options });
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            File.WriteAllText(path, json);
        }

        private static IConfiguration BuildConfig()
        {
            const bool notOptional = false;
            const bool reloadable = true;
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", notOptional, reloadable);
            return builder.Build();
        }

        private class Options
        {
            public AccountOptions Account { get; set; }
        }
    }
}