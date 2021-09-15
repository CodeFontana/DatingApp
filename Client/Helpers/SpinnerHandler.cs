using Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Client.Helpers
{
    public class SpinnerHandler : DelegatingHandler
    {
        private readonly SpinnerService _spinnerService;

        public SpinnerHandler(SpinnerService spinnerService)
        {
            _spinnerService = spinnerService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _spinnerService.Show();
            //await Task.Delay(3000); // artificial delay for testing
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            _spinnerService.Hide();
            return response;
        }
    }
}
