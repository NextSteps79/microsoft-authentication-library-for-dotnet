﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Identity.Client.Internal;
using Microsoft.Identity.Client.Internal.Broker;
using Microsoft.Identity.Client.OAuth2;
using Microsoft.Identity.Client.PlatformsCommon.Shared;
using Microsoft.Identity.Client.TelemetryCore;
using Microsoft.Identity.Client.Utils;
using Microsoft.Identity.Json.Linq;
using Microsoft.Identity.Test.Common.Core.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Identity.Test.Unit.CoreTests.OAuth2Tests
{
    [TestClass]
    public class TokenResponseTests : TestBase
    {
        [TestMethod]
        public void JsonDeserializationTest()
        {
            using (var harness = CreateTestHarness())
            {
                harness.HttpManager.AddSuccessTokenResponseMockHandlerForPost(TestConstants.AuthorityCommonTenant);

                OAuth2Client client = new OAuth2Client(harness.ServiceBundle.ApplicationLogger, harness.HttpManager);

                Task<MsalTokenResponse> task = client.GetTokenAsync(
                    new Uri(TestConstants.AuthorityCommonTenant + "oauth2/v2.0/token"),
                    new RequestContext(harness.ServiceBundle, Guid.NewGuid()));
                MsalTokenResponse response = task.Result;
                Assert.IsNotNull(response);
            }
        }

        [TestMethod]
        public void AndroidBrokerTokenResponseParseTest()
        {
            string unixTimestamp = DateTimeHelpers.DateTimeToUnixTimestamp(DateTimeOffset.UtcNow + TimeSpan.FromMinutes(40));
            string androidBrokerResponse = TestConstants.AndroidBrokerResponse.Replace("1591196764", unixTimestamp); 
            string correlationId = Guid.NewGuid().ToString();
            // Act
            var mobileTokenResponse = MobileBrokerTokenResponse.CreateFromAndroidBrokerResponse(androidBrokerResponse, correlationId);

            // Assert
            Assert.AreEqual("secretAt", mobileTokenResponse.AccessToken);
            Assert.AreEqual(correlationId, mobileTokenResponse.CorrelationId);
            Assert.AreEqual("clientInfo", mobileTokenResponse.ClientInfo);
            Assert.AreEqual("idT", mobileTokenResponse.IdToken);
            Assert.AreEqual("User.Read openid offline_access profile", mobileTokenResponse.Scope);
            Assert.AreEqual("Bearer", mobileTokenResponse.TokenType);
            
            Assert.AreEqual("49f548d0-12b7-4169-a390-bb5304d24462", mobileTokenResponse.TenantId);
            Assert.AreEqual("some_user@contoso.com", mobileTokenResponse.Upn);
            Assert.AreEqual("ae821e4d-f408-451a-af82-882691148603", mobileTokenResponse.AccountUserId);
            Assert.AreEqual("https://login.microsoftonline.com/common", mobileTokenResponse.AuthorityUrl);

            Assert.IsNull(mobileTokenResponse.RefreshToken);
        }       
    }
}
