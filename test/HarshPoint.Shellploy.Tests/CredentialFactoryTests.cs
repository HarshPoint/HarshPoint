﻿using HarshPoint.Tests;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Shellploy.Tests
{
    public class CredentialFactoryTests : SeriloggedTest
    {
        public CredentialFactoryTests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public void CredentialFactory_returns_null_for_default()
        {
            var result = CredentialFactory.CreateCredentials(CredentialType.Default, null, null, null);
            Assert.Null(result);
        }

        [Fact]
        public void CredentialFactory_returns_windows_credential()
        {
            var result = CredentialFactory.CreateCredentials(CredentialType.Windows, "user", "pwd", new Uri("https://localhost/site"));
            Assert.IsType<NetworkCredential>(result);
        }

        [Fact]
        public void CredentialFactory_returns_sp_online_credential()
        {
            var result = CredentialFactory.CreateCredentials(CredentialType.SharePointOnline, "user@example.org", "pwd", new Uri("https://localhost/site"));
            Assert.IsType<SharePointOnlineCredentials>(result);
        }

        [Theory]
        [InlineData(CredentialType.Default)]
        [InlineData(CredentialType.SharePointOnline)]
        public void CredentialFactory_returns_sp_online_credential_default(CredentialType type)
        {
            var result = CredentialFactory.CreateCredentials(type, "user@example.org", "pwd", new Uri("https://test.SharePoint.com/site"));
            Assert.IsType<SharePointOnlineCredentials>(result);
        }

        [Fact]
        public void CredentialFactory_returns_windows_credential_with_sp_online_url()
        {
            var result = CredentialFactory.CreateCredentials(CredentialType.Windows, "user", "pwd", new Uri("https://test.SharePoint.com/site"));
            Assert.IsType<NetworkCredential>(result);
        }

    }
}
