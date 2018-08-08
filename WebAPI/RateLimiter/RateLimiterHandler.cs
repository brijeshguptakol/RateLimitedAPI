using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace WebAPI.RateLimiter
{
    public class RateLimiterHandler : DelegatingHandler
    {
        private RateLimiter m_rateLimiter;

        public RateLimiterHandler(HttpConfiguration httpConfiguration, RateLimiter rateLimiter) : base()
        {

            if (rateLimiter == null)
                throw new ArgumentNullException("RateLimiter");

            InnerHandler = new HttpControllerDispatcher(httpConfiguration);

            m_rateLimiter = rateLimiter;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Task<HttpResponseMessage> response = null;

            if (m_rateLimiter.IsCallAllowed(request.RequestUri.AbsolutePath,DateTime.Now))
            {
                response = base.SendAsync(request, cancellationToken);
            }
            else
            {
                response = CreateResponse(request, HttpStatusCode.Forbidden,
                    "Too many request. Please try after some time");
            }

            return response;
        }

        private static Task<HttpResponseMessage> CreateResponse(HttpRequestMessage request, HttpStatusCode statusCode, string message)
        {
            var taskCompletionSource = new TaskCompletionSource<HttpResponseMessage>();

            var response = request.CreateResponse(statusCode);
            response.ReasonPhrase = message;
            response.Content = new StringContent(message);

            taskCompletionSource.SetResult(response);

            return taskCompletionSource.Task;
        }
    }
}