using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AccountInformation.Client.Common;
using Microsoft.AspNetCore.Http;

namespace AccountInformation.Client.HttpHandlers
{
    public class HttpHeaderHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        //private TokenManager tokenManager;

        public HttpHeaderHandler(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            //this.tokenManager = tokenManager;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var httpContext = httpContextAccessor.HttpContext;
            var accessToken = httpContext.Request.Cookies[AppConstant.ACCESS_TOKEN];

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                /* expires_in is not taken into account to validate access token
                   in the server side, hence commenting the following code to
                   to retrieve access token using refresh token.
                 */

                //var expiresAt = httpContext.Request.Cookies["expires_in"];

                //var expiresAtAsDateTimeOffset =
                //    DateTimeOffset.Parse(expiresAt, CultureInfo.InvariantCulture);
                ////Console.WriteLine("Access token Expires at : " + expiresAtAsDateTimeOffset);

                //if ((expiresAtAsDateTimeOffset.AddSeconds(-60)).ToUniversalTime() < DateTime.UtcNow)
                //{
                //    var refreshTokenInfo = await this.tokenManager.GetRefreshTokenInfoAsync(
                //            httpContext.Request.Cookies["refresh_token"]
                //        );
                //}

                request.Headers.Add(AppConstant.AUTHORIZATION, $"Bearer {accessToken}");
                request.Headers.Add(AppConstant.X_REQUEST_ID, Guid.NewGuid().ToString());
                request.Headers.Add(AppConstant.PSU_IP_ADDRESS, "192.168.1.101");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
