using System;
using System.Threading.Tasks;
using System.Web;
using AccountInformation.Client.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AccountInformation.Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppConfig appConfig;
        private readonly TokenManager tokenManager;
        private const string AUTHORITY = "https://api-sandbox.sebgroup.com/mga/sps/oauth/oauth20/authorize";

        public HomeController(IOptions<AppConfig> appConfig, TokenManager tokenManager)
        {
            this.appConfig = appConfig.Value;
            this.tokenManager = tokenManager;
        }

        public ActionResult Index()
        {
            var url = string.Format(@AUTHORITY +
                "?scope=psd2_accounts&scope=psd2_payments&client_id={0}&redirect_uri={1}&response_type=code",
                appConfig.ClientId, HttpUtility.UrlEncode(appConfig.RedirectUri));

            return Redirect(url);
        }

        [Route("callback")]
        public async Task<ActionResult> CallBack(string code)
        {
            var tokenInfo = await tokenManager.GetTokenInfoAsync(code);
            if (tokenInfo == null) throw new Exception("Acces token not found");

            var accessToken = tokenInfo[AppConstant.ACCESS_TOKEN];
            if (!string.IsNullOrEmpty(accessToken))
            {
                HttpContext.Response.Cookies.Append(AppConstant.ACCESS_TOKEN, accessToken);
                HttpContext.Response.Cookies.Append(AppConstant.REFRESH_TOKEN, tokenInfo[AppConstant.REFRESH_TOKEN]);
                HttpContext.Response.Cookies.Append(AppConstant.EXPIRES_IN, tokenInfo[AppConstant.EXPIRES_IN]);
            }
            
            return View();
        }

    }
}
