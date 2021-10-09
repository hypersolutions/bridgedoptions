using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace BridgedOptions.UnitTests
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void ValidateBridgeOptions_AddOptionsBridge_RegistersOptions()
        {
            var services = new ServiceCollection();
            var section = BuildConfigSection();
            
            services.AddOptionsBridge<AccountOptions, IAccountInfo>(section);
            
            var provider = services.BuildServiceProvider();
            var options = provider.GetService<IAccountInfo>();
            options.ShouldNotBeNull();
        }
        
        [Fact]
        public void InvalidateBridgeOptions_AddOptionsBridge_ThrowsInvalidOperationException()
        {
            var services = new ServiceCollection();
            var section = BuildConfigSection();

            var exception = Should.Throw<InvalidOperationException>(() =>
                services.AddOptionsBridge<UndecoratedAccountOptions, IAccountInfo>(section));

            exception.Message.ShouldBe(
                "The BridgeViaTypeAttribute has not been defined on the UndecoratedAccountOptions class.");
        }
        
        [Fact]
        public void ValidateBridgeOptions_AddOptionsBridge_RegistersBridgedOptionsAsSingleton()
        {
            var services = new ServiceCollection();
            var section = BuildConfigSection();
            
            services.AddOptionsBridge<AccountOptions, IAccountInfo>(section);
            
            var provider = services.BuildServiceProvider();

            var options1 = GetRequiredService<IAccountInfo>(provider);
            var options2 = GetRequiredService<IAccountInfo>(provider);
            options1.ShouldBe(options2);
        }
        
        [Fact]
        public void ValidateBridgeOptions_AddScopedOptionsBridge_RegistersOptions()
        {
            var services = new ServiceCollection();
            var section = BuildConfigSection();
            
            services.AddScopedOptionsBridge<AccountOptions, IAccountInfo>(section);
            
            var provider = services.BuildServiceProvider();
            var options = provider.GetService<IAccountInfo>();
            options.ShouldNotBeNull();
        }
        
        [Fact]
        public void InvalidateBridgeOptions_AddScopedOptionsBridge_ThrowsInvalidOperationException()
        {
            var services = new ServiceCollection();
            var section = BuildConfigSection();

            var exception = Should.Throw<InvalidOperationException>(() =>
                services.AddScopedOptionsBridge<UndecoratedAccountOptions, IAccountInfo>(section));

            exception.Message.ShouldBe(
                "The BridgeViaTypeAttribute has not been defined on the UndecoratedAccountOptions class.");
        }

        [Fact]
        public void ValidateBridgeOptions_AddScopedOptionsBridge_RegistersBridgedOptionsAsScoped()
        {
            var services = new ServiceCollection();
            var section = BuildConfigSection();
            
            services.AddScopedOptionsBridge<AccountOptions, IAccountInfo>(section);
            
            var provider = services.BuildServiceProvider();

            var options1 = GetRequiredService<IAccountInfo>(provider);
            var options2 = GetRequiredService<IAccountInfo>(provider);
            options1.ShouldNotBe(options2);
        }
        
        [Fact]
        public void ValidateBridgeOptions_AddMonitoredOptionsBridge_RegistersOptions()
        {
            var services = new ServiceCollection();
            var section = BuildConfigSection();
            
            services.AddMonitoredOptionsBridge<AccountOptions, IAccountInfo>(section);
            
            var provider = services.BuildServiceProvider();
            var options = provider.GetService<IAccountInfo>();
            options.ShouldNotBeNull();
        }
        
        [Fact]
        public void InvalidateBridgeOptions_AddMonitoredOptionsBridge_ThrowsInvalidOperationException()
        {
            var services = new ServiceCollection();
            var section = BuildConfigSection();

            var exception = Should.Throw<InvalidOperationException>(() =>
                services.AddMonitoredOptionsBridge<UndecoratedAccountOptions, IAccountInfo>(section));

            exception.Message.ShouldBe(
                "The BridgeViaTypeAttribute has not been defined on the UndecoratedAccountOptions class.");
        }
        
        [Fact]
        public void ValidateBridgeOptions_AddMonitoredOptionsBridge_RegistersBridgedOptionsAsTransient()
        {
            var services = new ServiceCollection();
            var section = BuildConfigSection();
            
            services.AddMonitoredOptionsBridge<AccountOptions, IAccountInfo>(section);
            
            var provider = services.BuildServiceProvider();

            var options1 = GetRequiredService<IAccountInfo>(provider);
            var options2 = GetRequiredService<IAccountInfo>(provider);
            options1.ShouldNotBe(options2);
        }
        
        [Fact]
        public void MonitoredOptionsChange_AddMonitoredOptionsBridge_UpdatesBridgeOptionsValue()
        {
            var services = new ServiceCollection();
            var section = BuildConfigSection();
            
            services.AddMonitoredOptionsBridge<AccountOptions, IAccountInfo>(section);
            
            var provider = services.BuildServiceProvider();
            
            using (var scope = provider.CreateScope())
            {
                var options = scope.ServiceProvider.GetRequiredService<IOptionsMonitor<AccountOptions>>();
                options.CurrentValue.Username = "test";
            }
            
            var options2 = GetRequiredService<IAccountInfo>(provider);
            options2.Username.ShouldBe("test");
        }
        
        private static IConfigurationSection BuildConfigSection()
        {
            var memoryConfig = new MemoryConfigurationSource();
            var items = new List<KeyValuePair<string, string>>
            {
                new("Account:Username", "homers"),
                new("Account:Password", "51mp50n5")
            };
            memoryConfig.InitialData = items;
            var configuration = new ConfigurationBuilder().Add(memoryConfig).Build();
            return configuration.GetSection("Account");
        }

        private static T GetRequiredService<T>(IServiceProvider provider)
        {
            using var scope = provider.CreateScope();
            return scope.ServiceProvider.GetService<T>();
        }
        
        public interface IAccountInfo
        {
            string Username { get; }
            string Password { get; }
        }
    
        [BridgeViaType(typeof(AccountBridgeOptions))]
        public sealed class AccountOptions
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
        
        public sealed class UndecoratedAccountOptions
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
        
        public sealed class AccountBridgeOptions : IBridgeOptions<AccountOptions, IAccountInfo>
        {
            public IAccountInfo BridgeFrom(AccountOptions source)
            {
                return new AccountInfo
                {
                    Username = source.Username,
                    Password = source.Password
                };
            }
        
            private sealed class AccountInfo : IAccountInfo
            {
                public string Username { get; init; }
                public string Password { get; init; }
            }
        }
    }
}