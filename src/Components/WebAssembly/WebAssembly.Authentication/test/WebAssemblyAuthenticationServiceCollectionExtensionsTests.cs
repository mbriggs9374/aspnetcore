// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Microsoft.AspNetCore.Components.WebAssembly.Authentication
{
    public class WebAssemblyAuthenticationServiceCollectionExtensionsTests
    {
        [Fact]
        public void CanResolve_AccessTokenProvider()
        {
            var builder = new WebAssemblyHostBuilder(new TestWebAssemblyJSRuntimeInvoker());
            builder.Services.AddApiAuthorization();
            var host = builder.Build();

            host.Services.GetRequiredService<IAccessTokenProvider>();
        }

        [Fact]
        public void CanResolve_IRemoteAuthenticationService()
        {
            var builder = new WebAssemblyHostBuilder(new TestWebAssemblyJSRuntimeInvoker());
            builder.Services.AddApiAuthorization();
            var host = builder.Build();

            host.Services.GetRequiredService<IRemoteAuthenticationService<RemoteAuthenticationState>>();
        }

        [Fact]
        public void ApiAuthorizationOptions_ConfigurationDefaultsGetApplied()
        {
            var builder = new WebAssemblyHostBuilder(new TestWebAssemblyJSRuntimeInvoker());
            builder.Services.AddApiAuthorization();
            var host = builder.Build();

            var options = host.Services.GetRequiredService<IOptions<RemoteAuthenticationOptions<ApiAuthorizationProviderOptions>>>();

            var paths = options.Value.AuthenticationPaths;

            Assert.Equal("authentication/login", paths.LogInPath);
            Assert.Equal("authentication/login-callback", paths.LogInCallbackPath);
            Assert.Equal("authentication/login-failed", paths.LogInFailedPath);
            Assert.Equal("authentication/register", paths.RegisterPath);
            Assert.Equal("authentication/profile", paths.ProfilePath);
            Assert.Equal("Identity/Account/Register", paths.RemoteRegisterPath);
            Assert.Equal("Identity/Account/Manage", paths.RemoteProfilePath);
            Assert.Equal("authentication/logout", paths.LogOutPath);
            Assert.Equal("authentication/logout-callback", paths.LogOutCallbackPath);
            Assert.Equal("authentication/logout-failed", paths.LogOutFailedPath);
            Assert.Equal("authentication/logged-out", paths.LogOutSucceededPath);

            var user = options.Value.UserOptions;
            Assert.Equal("Microsoft.AspNetCore.Components.WebAssembly.Authentication.Tests", user.AuthenticationType);
            Assert.Equal("scope", user.ScopeClaim);
            Assert.Equal("role", user.RoleClaim);
            Assert.Equal("name", user.NameClaim);

            Assert.Equal(
                "_configuration/Microsoft.AspNetCore.Components.WebAssembly.Authentication.Tests",
                options.Value.ProviderOptions.ConfigurationEndpoint);
        }

        [Fact]
        public void ApiAuthorizationOptionsConfigurationCallback_GetsCalledOnce()
        {
            var builder = new WebAssemblyHostBuilder(new TestWebAssemblyJSRuntimeInvoker());
            var calls = 0;
            builder.Services.AddApiAuthorization(options =>
            {
                calls++;
            });

            var host = builder.Build();

            var options = host.Services.GetRequiredService<IOptions<RemoteAuthenticationOptions<ApiAuthorizationProviderOptions>>>();

            var user = options.Value.UserOptions;
            Assert.Equal("Microsoft.AspNetCore.Components.WebAssembly.Authentication.Tests", user.AuthenticationType);

            // Make sure that the defaults are applied on this overload
            Assert.Equal("role", user.RoleClaim);

            Assert.Equal(
                "_configuration/Microsoft.AspNetCore.Components.WebAssembly.Authentication.Tests",
                options.Value.ProviderOptions.ConfigurationEndpoint);

            Assert.Equal(1, calls);
        }

        [Fact]
        public void ApiAuthorizationTestAuthenticationState_SetsUpConfiguration()
        {
            var builder = new WebAssemblyHostBuilder(new TestWebAssemblyJSRuntimeInvoker());
            var calls = 0;
            builder.Services.AddApiAuthorization<TestAuthenticationState>(options => calls++);

            var host = builder.Build();

            var options = host.Services.GetRequiredService<IOptions<RemoteAuthenticationOptions<ApiAuthorizationProviderOptions>>>();

            var user = options.Value.UserOptions;
            // Make sure that the defaults are applied on this overload
            Assert.Equal("role", user.RoleClaim);

            Assert.Equal(
                "_configuration/Microsoft.AspNetCore.Components.WebAssembly.Authentication.Tests",
                options.Value.ProviderOptions.ConfigurationEndpoint);

            var authenticationService = host.Services.GetService<IRemoteAuthenticationService<TestAuthenticationState>>();
            Assert.NotNull(authenticationService);
            Assert.IsType<RemoteAuthenticationService<TestAuthenticationState, RemoteUserAccount, ApiAuthorizationProviderOptions>>(authenticationService);

            Assert.Equal(1, calls);
        }

        [Fact]
        public void ApiAuthorizationTestAuthenticationState_NoCallback_SetsUpConfiguration()
        {
            var builder = new WebAssemblyHostBuilder(new TestWebAssemblyJSRuntimeInvoker());
            builder.Services.AddApiAuthorization<TestAuthenticationState>();

            var host = builder.Build();

            var options = host.Services.GetRequiredService<IOptions<RemoteAuthenticationOptions<ApiAuthorizationProviderOptions>>>();

            var user = options.Value.UserOptions;
            // Make sure that the defaults are applied on this overload
            Assert.Equal("role", user.RoleClaim);

            Assert.Equal(
                "_configuration/Microsoft.AspNetCore.Components.WebAssembly.Authentication.Tests",
                options.Value.ProviderOptions.ConfigurationEndpoint);

            var authenticationService = host.Services.GetService<IRemoteAuthenticationService<TestAuthenticationState>>();
            Assert.NotNull(authenticationService);
            Assert.IsType<RemoteAuthenticationService<TestAuthenticationState, RemoteUserAccount, ApiAuthorizationProviderOptions>>(authenticationService);
        }

        [Fact]
        public void ApiAuthorizationCustomAuthenticationStateAndAccount_SetsUpConfiguration()
        {
            var builder = new WebAssemblyHostBuilder(new TestWebAssemblyJSRuntimeInvoker());
            var calls = 0;
            builder.Services.AddApiAuthorization<TestAuthenticationState, TestAccount>(options => calls++);

            var host = builder.Build();

            var options = host.Services.GetRequiredService<IOptions<RemoteAuthenticationOptions<ApiAuthorizationProviderOptions>>>();

            var user = options.Value.UserOptions;
            // Make sure that the defaults are applied on this overload
            Assert.Equal("role", user.RoleClaim);

            Assert.Equal(
                "_configuration/Microsoft.AspNetCore.Components.WebAssembly.Authentication.Tests",
                options.Value.ProviderOptions.ConfigurationEndpoint);

            var authenticationService = host.Services.GetService<IRemoteAuthenticationService<TestAuthenticationState>>();
            Assert.NotNull(authenticationService);
            Assert.IsType<RemoteAuthenticationService<TestAuthenticationState, TestAccount, ApiAuthorizationProviderOptions>>(authenticationService);

            Assert.Equal(1, calls);
        }

        [Fact]
        public void ApiAuthorizationTestAuthenticationStateAndAccount_NoCallback_SetsUpConfiguration()
        {
            var builder = new WebAssemblyHostBuilder(new TestWebAssemblyJSRuntimeInvoker());
            builder.Services.AddApiAuthorization<TestAuthenticationState, TestAccount>();

            var host = builder.Build();

            var options = host.Services.GetRequiredService<IOptions<RemoteAuthenticationOptions<ApiAuthorizationProviderOptions>>>();

            var user = options.Value.UserOptions;
            // Make sure that the defaults are applied on this overload
            Assert.Equal("role", user.RoleClaim);

            Assert.Equal(
                "_configuration/Microsoft.AspNetCore.Components.WebAssembly.Authentication.Tests",
                options.Value.ProviderOptions.ConfigurationEndpoint);

            var authenticationService = host.Services.GetService<IRemoteAuthenticationService<TestAuthenticationState>>();
            Assert.NotNull(authenticationService);
            Assert.IsType<RemoteAuthenticationService<TestAuthenticationState, TestAccount, ApiAuthorizationProviderOptions>>(authenticationService);
        }

        [Fact]
        public void ApiAuthorizationOptions_DefaultsCanBeOverriden()
        {
            var builder = new WebAssemblyHostBuilder(new TestWebAssemblyJSRuntimeInvoker());
            builder.Services.AddApiAuthorization(options =>
            {
                options.AuthenticationPaths = new RemoteAuthenticationApplicationPathsOptions
                {
                    LogInPath = "a",
                    LogInCallbackPath = "b",
                    LogInFailedPath = "c",
                    RegisterPath = "d",
                    ProfilePath = "e",
                    RemoteRegisterPath = "f",
                    RemoteProfilePath = "g",
                    LogOutPath = "h",
                    LogOutCallbackPath = "i",
                    LogOutFailedPath = "j",
                    LogOutSucceededPath = "k",
                };
                options.UserOptions = new RemoteAuthenticationUserOptions
                {
                    AuthenticationType = "l",
                    ScopeClaim = "m",
                    RoleClaim = "n",
                    NameClaim = "o",
                };
                options.ProviderOptions = new ApiAuthorizationProviderOptions
                {
                    ConfigurationEndpoint = "p"
                };
            });

            var host = builder.Build();

            var options = host.Services.GetRequiredService<IOptions<RemoteAuthenticationOptions<ApiAuthorizationProviderOptions>>>();

            var paths = options.Value.AuthenticationPaths;

            Assert.Equal("a", paths.LogInPath);
            Assert.Equal("b", paths.LogInCallbackPath);
            Assert.Equal("c", paths.LogInFailedPath);
            Assert.Equal("d", paths.RegisterPath);
            Assert.Equal("e", paths.ProfilePath);
            Assert.Equal("f", paths.RemoteRegisterPath);
            Assert.Equal("g", paths.RemoteProfilePath);
            Assert.Equal("h", paths.LogOutPath);
            Assert.Equal("i", paths.LogOutCallbackPath);
            Assert.Equal("j", paths.LogOutFailedPath);
            Assert.Equal("k", paths.LogOutSucceededPath);

            var user = options.Value.UserOptions;
            Assert.Equal("l", user.AuthenticationType);
            Assert.Equal("m", user.ScopeClaim);
            Assert.Equal("n", user.RoleClaim);
            Assert.Equal("o", user.NameClaim);

            Assert.Equal("p", options.Value.ProviderOptions.ConfigurationEndpoint);
        }

        [Fact]
        public void OidcOptions_ConfigurationDefaultsGetApplied()
        {
            var builder = new WebAssemblyHostBuilder(new TestWebAssemblyJSRuntimeInvoker());
            builder.Services.Replace(ServiceDescriptor.Singleton<NavigationManager, TestNavigationManager>());
            builder.Services.AddOidcAuthentication(options => { });
            var host = builder.Build();

            var options = host.Services.GetRequiredService<IOptions<RemoteAuthenticationOptions<OidcProviderOptions>>>();

            var paths = options.Value.AuthenticationPaths;

            Assert.Equal("authentication/login", paths.LogInPath);
            Assert.Equal("authentication/login-callback", paths.LogInCallbackPath);
            Assert.Equal("authentication/login-failed", paths.LogInFailedPath);
            Assert.Equal("authentication/register", paths.RegisterPath);
            Assert.Equal("authentication/profile", paths.ProfilePath);
            Assert.Null(paths.RemoteRegisterPath);
            Assert.Null(paths.RemoteProfilePath);
            Assert.Equal("authentication/logout", paths.LogOutPath);
            Assert.Equal("authentication/logout-callback", paths.LogOutCallbackPath);
            Assert.Equal("authentication/logout-failed", paths.LogOutFailedPath);
            Assert.Equal("authentication/logged-out", paths.LogOutSucceededPath);

            var user = options.Value.UserOptions;
            Assert.Null(user.AuthenticationType);
            Assert.Null(user.ScopeClaim);
            Assert.Null(user.RoleClaim);
            Assert.Equal("name", user.NameClaim);

            var provider = options.Value.ProviderOptions;
            Assert.Null(provider.Authority);
            Assert.Null(provider.ClientId);
            Assert.Equal(new[] { "openid", "profile" }, provider.DefaultScopes);
            Assert.Equal("https://www.example.com/base/authentication/login-callback", provider.RedirectUri);
            Assert.Equal("https://www.example.com/base/authentication/logout-callback", provider.PostLogoutRedirectUri);
        }

        [Fact]
        public void OidcOptions_DefaultsCanBeOverriden()
        {
            var builder = new WebAssemblyHostBuilder(new TestWebAssemblyJSRuntimeInvoker());
            builder.Services.AddOidcAuthentication(options =>
            {
                options.AuthenticationPaths = new RemoteAuthenticationApplicationPathsOptions
                {
                    LogInPath = "a",
                    LogInCallbackPath = "b",
                    LogInFailedPath = "c",
                    RegisterPath = "d",
                    ProfilePath = "e",
                    RemoteRegisterPath = "f",
                    RemoteProfilePath = "g",
                    LogOutPath = "h",
                    LogOutCallbackPath = "i",
                    LogOutFailedPath = "j",
                    LogOutSucceededPath = "k",
                };
                options.UserOptions = new RemoteAuthenticationUserOptions
                {
                    AuthenticationType = "l",
                    ScopeClaim = "m",
                    RoleClaim = "n",
                    NameClaim = "o",
                };
                options.ProviderOptions = new OidcProviderOptions
                {
                    Authority = "p",
                    ClientId = "q",
                    DefaultScopes = Array.Empty<string>(),
                    RedirectUri = "https://www.example.com/base/custom-login",
                    PostLogoutRedirectUri = "https://www.example.com/base/custom-logout",
                };
            });

            var host = builder.Build();

            var options = host.Services.GetRequiredService<IOptions<RemoteAuthenticationOptions<OidcProviderOptions>>>();

            var paths = options.Value.AuthenticationPaths;

            Assert.Equal("a", paths.LogInPath);
            Assert.Equal("b", paths.LogInCallbackPath);
            Assert.Equal("c", paths.LogInFailedPath);
            Assert.Equal("d", paths.RegisterPath);
            Assert.Equal("e", paths.ProfilePath);
            Assert.Equal("f", paths.RemoteRegisterPath);
            Assert.Equal("g", paths.RemoteProfilePath);
            Assert.Equal("h", paths.LogOutPath);
            Assert.Equal("i", paths.LogOutCallbackPath);
            Assert.Equal("j", paths.LogOutFailedPath);
            Assert.Equal("k", paths.LogOutSucceededPath);

            var user = options.Value.UserOptions;
            Assert.Equal("l", user.AuthenticationType);
            Assert.Equal("m", user.ScopeClaim);
            Assert.Equal("n", user.RoleClaim);
            Assert.Equal("o", user.NameClaim);

            var provider = options.Value.ProviderOptions;
            Assert.Equal("p", provider.Authority);
            Assert.Equal("q", provider.ClientId);
            Assert.Equal(Array.Empty<string>(), provider.DefaultScopes);
            Assert.Equal("https://www.example.com/base/custom-login", provider.RedirectUri);
            Assert.Equal("https://www.example.com/base/custom-logout", provider.PostLogoutRedirectUri);
        }

        [Fact]
        public void AddOidc_ConfigurationGetsCalledOnce()
        {
            var builder = new WebAssemblyHostBuilder(new TestWebAssemblyJSRuntimeInvoker());
            var calls = 0;

            builder.Services.AddOidcAuthentication(options => calls++);
            builder.Services.Replace(ServiceDescriptor.Singleton(typeof(NavigationManager), new TestNavigationManager()));

            var host = builder.Build();

            var options = host.Services.GetRequiredService<IOptions<RemoteAuthenticationOptions<OidcProviderOptions>>>();
            Assert.Equal("name", options.Value.UserOptions.NameClaim);

            Assert.Equal(1, calls);
        }

        [Fact]
        public void AddOidc_CustomState_SetsUpConfiguration()
        {
            var builder = new WebAssemblyHostBuilder(new TestWebAssemblyJSRuntimeInvoker());
            var calls = 0;

            builder.Services.AddOidcAuthentication<TestAuthenticationState>(options => options.ProviderOptions.Authority = (++calls).ToString());
            builder.Services.Replace(ServiceDescriptor.Singleton(typeof(NavigationManager), new TestNavigationManager()));

            var host = builder.Build();

            var options = host.Services.GetRequiredService<IOptions<RemoteAuthenticationOptions<OidcProviderOptions>>>();
            // Make sure options are applied
            Assert.Equal("name", options.Value.UserOptions.NameClaim);

            Assert.Equal("1", options.Value.ProviderOptions.Authority);

            var authenticationService = host.Services.GetService<IRemoteAuthenticationService<TestAuthenticationState>>();
            Assert.NotNull(authenticationService);
            Assert.IsType<RemoteAuthenticationService<TestAuthenticationState, RemoteUserAccount, OidcProviderOptions>>(authenticationService);
        }

        [Fact]
        public void AddOidc_CustomStateAndAccount_SetsUpConfiguration()
        {
            var builder = new WebAssemblyHostBuilder(new TestWebAssemblyJSRuntimeInvoker());
            var calls = 0;

            builder.Services.AddOidcAuthentication<TestAuthenticationState, TestAccount>(options => options.ProviderOptions.Authority = (++calls).ToString());
            builder.Services.Replace(ServiceDescriptor.Singleton(typeof(NavigationManager), new TestNavigationManager()));

            var host = builder.Build();

            var options = host.Services.GetRequiredService<IOptions<RemoteAuthenticationOptions<OidcProviderOptions>>>();
            // Make sure options are applied
            Assert.Equal("name", options.Value.UserOptions.NameClaim);

            Assert.Equal("1", options.Value.ProviderOptions.Authority);

            var authenticationService = host.Services.GetService<IRemoteAuthenticationService<TestAuthenticationState>>();
            Assert.NotNull(authenticationService);
            Assert.IsType<RemoteAuthenticationService<TestAuthenticationState, TestAccount, OidcProviderOptions>>(authenticationService);
        }

        private class TestNavigationManager : NavigationManager
        {
            public TestNavigationManager()
            {
                Initialize("https://www.example.com/base/", "https://www.example.com/base/counter");
            }

            protected override void NavigateToCore(string uri, bool forceLoad) => throw new System.NotImplementedException();
        }

        private class TestAuthenticationState : RemoteAuthenticationState
        {
        }

        private class TestAccount : RemoteUserAccount
        {
        }
    }
}