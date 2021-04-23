using System;
namespace AccountInformation.Client.Common
{
    public class AppConfig
    {
        public AppConfig()
        {
        }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
        public string GrantType { get; set; }
    }
}
