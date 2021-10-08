# Bridged Options

## Getting Started

You can find this packages via NuGet: https://www.nuget.org/packages/BridgedOptions

## Overview

There are situations where you wish to intercept the options and perform additional operations. One example is when you have encrypted values stored 
in the _appsettings_ and need to decrypt them in code, in order to call some service.

Within the .net framework there already exists a hook to do this - the _IConfigureOptions_ interface. This mutates the existing loaded options which 
may be considered as a code smell, as well as being more difficult to provide interface segregation - in which the same options may be split into different usages via interfaces.

In this scenario, it would be better to not change the existing options but rather return a new object, with any changes applied. This also allows you to have 
the interface segregation (if you desired).

All of this is achieved by using the bridge pattern. This pattern uncouples the desired options interface from the source via a _bridging_ class. This allows you to perform 
any extra logic before returning your _bridged_ version, without the need to mutate the original options.

Also you do not need to expose your new concrete object, just your interface as your returning type, so you can be define concrete implementation within your bridge, as a nested class.

### Simple Example

Given the following options class:

```c#
[BridgeViaType(typeof(AccountBridgeOptions))]
public sealed class AccountOptions
{
    public string Username { get; set; }
    public string Password { get; set; }
}
```

You decorate the options that is used to load _appsettings_ with an attribute defining your bridge. The bridge class can look like:

```c#
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
```

where the interface _IAccountInfo_ is:

```c#
public interface IAccountInfo
{
    string Username { get; }
    string Password { get; }
}
```

Finally we need to register our options with an extension method in the startup:

```c#
services.AddOptionsBridge<AccountOptions, IAccountInfo>(configuration.GetSection("Account"));
```

This assumes that you _appsettings_ has a section like:

```json
{
  "Account": {
    "Username": "homers",
    "Password": "51mp50n5"
  }
}
```

The above now means that you can inject the _IAccountInfo_ into any class, as required. You can also benefit from injecting other services into 
the bridge class too. The options are still registered as normal so if you wish to still use _IOptions<T>_ then this still works.

Along with the above extension for _IOptions_ there are two other extensions for the _IOptionsSnapshot<T>_ and _IOptionsMonitor<T>_ if required.

## Developer Notes

### Building and Publishing

From the root, to build, run:

```bash
dotnet build --configuration Release
```

To run all the unit and integration tests, run:

```bash
dotnet test --no-build --configuration Release
```

To create the packages, run (**optional** as the build generates the packages):
 
```bash
cd src/BridgedOptions
dotnet pack --no-build --configuration Release
```

To publish the packages to the nuget feed on nuget.org:

```bash
dotnet nuget push ./bin/Release/BridgedOptions.1.0.0.nupkg -k [THE API KEY] -s https://api.nuget.org/v3/index.json
```

## Links

* **GitFlow** https://datasift.github.io/gitflow/IntroducingGitFlow.html
