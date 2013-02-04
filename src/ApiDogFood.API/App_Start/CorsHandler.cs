using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace ApiDogFood.API
{
    public class CorsHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Options)
            {
                var apiExplorer = request.GetConfiguration().Services.GetApiExplorer();

                var controllerRequested = request.GetRouteData().Values["controller"] as string;
                var supportedMethods = apiExplorer.ApiDescriptions
                    .Where(d =>
                    {
                        var controller = d.ActionDescriptor.ControllerDescriptor.ControllerName;
                        return string.Equals(
                            controller, controllerRequested, StringComparison.OrdinalIgnoreCase);
                    })
                    .Select(d => d.HttpMethod.Method)
                    .Distinct();

                if (!supportedMethods.Any())
                {
                    return Task.FromResult(request.CreateResponse(HttpStatusCode.NotFound));
                }

                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Access-Control-Allow-Methods", string.Join(",", supportedMethods));

                return Task.FromResult(response);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
