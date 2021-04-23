using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AccountInformation.Client.Common
{
    public class AuthCodeHostedService : IHostedService
    {
        private readonly IServiceProvider serviceProvider;
        public AuthCodeHostedService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var url = string.Format(@"https://api-sandbox.sebgroup.com/mga/sps/oauth/oauth20/authorize?scope=psd2_accounts&scope=psd2_payments&client_id=CoL0wYTLDj9zDPfEC50O&redirect_uri={0}&response_type=code",
                    HttpUtility.UrlEncode(@"https://localhost:5001"));

                var ps = new ProcessStartInfo(url)
                {
                    UseShellExecute = true
                };
                Process.Start(ps);
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
