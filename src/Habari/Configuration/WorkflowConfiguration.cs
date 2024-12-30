using Habari.Library;
using Microsoft.Extensions.Logging;
using System.Resources;
using System.Text.Json;
using WatsonWebserver;
using WatsonWebserver.Core;
using HttpMethod = WatsonWebserver.Core.HttpMethod;

namespace Habari.Configuration
{
    internal class WorkflowConfiguration
    {
        private ILogger? _logger { get; set; }

        private Webserver? _webserver { get; set; } = null;

        public WorkflowConfiguration(ILogger logger)
        {
            _logger = logger;
        }

        public void Start()
        {
            ResourceManager resourceManager = new ("Habari.Properties.Configuration", typeof(WorkflowConfiguration).Assembly);

            WebserverSettings webserverSettings = new ("127.0.0.1", ConfigurationManager.Instance.ConfigurationPort, false);
            byte[]? indexHtml = (byte[]?)resourceManager.GetObject("index.html");
            byte[]? habariCss = (byte[]?)resourceManager.GetObject("habari.css");
            byte[]? robotoCss = (byte[]?)resourceManager.GetObject("roboto.css");
            byte[]? robotoWoff2 = (byte[]?)resourceManager.GetObject("roboto.woff2");
            byte[]? drawflowMinCss = (byte[]?)resourceManager.GetObject("drawflow.min.css");
            byte[]? drawflowMinJs = (byte[]?)resourceManager.GetObject("drawflow.min.js");
            byte[]? sweetalert2Js = (byte[]?)resourceManager.GetObject("sweetalert2.js");
            byte[]? drawflowStyleJs = (byte[]?)resourceManager.GetObject("drawflow.style.js");

            _webserver = new (webserverSettings, async (HttpContextBase contextBase) =>
            {
                await contextBase.Response.Send(indexHtml); 
            });

            _webserver.Routes.PreAuthentication.Parameter.Add(HttpMethod.GET, "/css/drawflow.min.css", async (HttpContextBase contextBase) =>
            {
                await contextBase.Response.Send(drawflowMinCss);
            });

            _webserver.Routes.PreAuthentication.Parameter.Add(HttpMethod.GET, "/css/roboto.css", async (HttpContextBase contextBase) =>
            {
                await contextBase.Response.Send(robotoCss);
            });

            _webserver.Routes.PreAuthentication.Parameter.Add(HttpMethod.GET, "/css/habari.css", async (HttpContextBase contextBase) =>
            {
                await contextBase.Response.Send(habariCss);
            });

            _webserver.Routes.PreAuthentication.Parameter.Add(HttpMethod.GET, "/font/roboto.woff2", async (HttpContextBase contextBase) =>
            {
                await contextBase.Response.Send(robotoWoff2);
            });

            _webserver.Routes.PreAuthentication.Parameter.Add(HttpMethod.GET, "/js/drawflow.min.js", async (HttpContextBase contextBase) =>
            {
                await contextBase.Response.Send(drawflowMinJs);
            });

            _webserver.Routes.PreAuthentication.Parameter.Add(HttpMethod.GET, "/js/sweetalert2.js", async (HttpContextBase contextBase) =>
            {
                await contextBase.Response.Send(sweetalert2Js);
            });

            _webserver.Routes.PreAuthentication.Parameter.Add(HttpMethod.GET, "/js/drawflow.style.js", async (HttpContextBase contextBase) =>
            {
                await contextBase.Response.Send(drawflowStyleJs);
            });

            _webserver.Routes.PreAuthentication.Parameter.Add(HttpMethod.GET, "/api/listeners", async (HttpContextBase contextBase) =>
            {
                await contextBase.Response.Send(JsonSerializer.Serialize(ConfigurationManager.Instance.AvailableListeners.Values.Select(value => new Listener(value))));
            });

            _webserver.Routes.PreAuthentication.Parameter.Add(HttpMethod.GET, "/api/steps", async (HttpContextBase contextBase) =>
            {
                await contextBase.Response.Send(JsonSerializer.Serialize(ConfigurationManager.Instance.AvailableSteps.Values.Select(value => new Step(value))));
            });

            _webserver.Start();
        }

        public void Stop()
        {
            _webserver!.Stop();
            _webserver = null;
            GC.Collect();
        }
    }
}
