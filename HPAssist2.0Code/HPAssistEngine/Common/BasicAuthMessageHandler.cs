#region namespaces
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using HPAssistEngine.HPBAL;
#endregion

namespace HPAssistEngine.Common
{
    public class BasicAuthMessageHandler : DelegatingHandler
    {
        #region VariableDeclarations
        private const string BasicAuthResponseHeader = "WWW-Authenticate";
        private const string BasicAuthResponseHeaderValue = "Basic";

        public IProvidePrincipal PrincipalProviderObject { get; set; }
        #endregion

        protected override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            AuthenticationHeaderValue authValue = request.Headers.Authorization;
            if (authValue != null && !String.IsNullOrWhiteSpace(authValue.Parameter))
            {
                Credentials parsedCredentials = XMLParser.ParseAuthorizationHeader(authValue.Parameter);
                if (parsedCredentials != null)
                {
                    Thread.CurrentPrincipal = PrincipalProviderObject
                        .CreatePrincipal(parsedCredentials.APIKey, parsedCredentials.CompanyCode, parsedCredentials.Username);

                }
            }
            return base.SendAsync(request, cancellationToken)
                .ContinueWith(task =>
                {
                    var response = task.Result;
                    if (response.StatusCode == HttpStatusCode.Unauthorized
                        && !response.Headers.Contains(BasicAuthResponseHeader))
                    {
                        string resultXml = "<Errors><Error><ErrorCode>EC001</ErrorCode><ErrorMessage>Missing Authorization token or Unauthorized user</ErrorMessage></Error></Errors>";

                        response = request.CreateResponse(HttpStatusCode.Unauthorized, resultXml, "application/xml");
                        response.Content = new StringContent(resultXml, Encoding.Unicode);
                        response.Headers.Add(BasicAuthResponseHeader
                                             , BasicAuthResponseHeaderValue);
                    }
                    return response;
                });
        }

    }
}