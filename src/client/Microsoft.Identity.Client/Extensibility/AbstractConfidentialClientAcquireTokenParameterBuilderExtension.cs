// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Identity.Client.Extensibility
{
    /// <summary>
    /// 
    /// </summary>
    public static class AbstractConfidentialClientAcquireTokenParameterBuilderExtension
    {
        /// <summary>
        /// Configures an async delegate that creates a client assertion. See https://aka.ms/msal-net-client-assertion
        /// </summary>
        /// <param name="clientAssertionAsyncDelegate">An async delegate computing the client assertion used to prove the identity of the application to Azure AD.
        /// This delegate should return a list a of name / value pairs which are to be sent to AAD as part of the token request. </param>
        /// <returns>The ConfidentialClientApplicationBuilder to chain more .With methods</returns>
        /// <remarks> Callers can use this mechanism to cache their assertions. Experimental method.</remarks>
        public static T WithClientAssertion<T>(
            this AbstractConfidentialClientAcquireTokenParameterBuilder<T> builder,
            Func<string, CancellationToken, Task<IReadOnlyList<KeyValuePair<string, string>>>> clientAssertionAsyncDelegate)
            where T : AbstractConfidentialClientAcquireTokenParameterBuilder<T>
        {
            if (clientAssertionAsyncDelegate == null)
            {
                throw new ArgumentNullException(nameof(clientAssertionAsyncDelegate));
            }

            builder.ValidateUseOfExperimentalFeature();
            builder.CommonParameters.ClientAssertionOverrideDelegate = clientAssertionAsyncDelegate;

            return (T)builder;
        }
    }
}
