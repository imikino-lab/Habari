using Habari.Library;
using Microsoft.Extensions.Logging;
using System.Resources;
using System.Text.Json;
using System.Text.Json.Nodes;
using WatsonWebserver;
using WatsonWebserver.Core;
using HttpMethod = WatsonWebserver.Core.HttpMethod;

namespace Habari.Configuration;

internal class WorkflowConfiguration
{
    private ILogger? _logger { get; set; }

    private Webserver? _webserver { get; set; } = null;

    private JsonSerializerOptions jsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };

    public WorkflowConfiguration(ILogger logger)
    {
        _logger = logger;
    }

    public void Start()
    {
        ResourceManager resourceManager = new("Habari.Properties.Configuration", typeof(WorkflowConfiguration).Assembly);

        WebserverSettings webserverSettings = new("127.0.0.1", ConfigurationManager.Instance.ConfigurationPort, false);
        byte[]? indexHtml = (byte[]?)resourceManager.GetObject("index.html");
        byte[]? habariJs = (byte[]?)resourceManager.GetObject("habari.js");
        byte[]? habariCss = (byte[]?)resourceManager.GetObject("habari.css");
        byte[]? robotoCss = (byte[]?)resourceManager.GetObject("roboto.css");
        byte[]? robotoWoff2 = (byte[]?)resourceManager.GetObject("roboto.woff2");
        byte[]? sweetalert2Js = (byte[]?)resourceManager.GetObject("sweetalert2.js");

        _webserver = new(webserverSettings, async (HttpContextBase contextBase) =>
        {
            await contextBase.Response.Send(indexHtml);
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

        _webserver.Routes.PreAuthentication.Parameter.Add(HttpMethod.GET, "/js/habari.js", async (HttpContextBase contextBase) =>
        {
            await contextBase.Response.Send(habariJs);
        });

        _webserver.Routes.PreAuthentication.Parameter.Add(HttpMethod.GET, "/js/sweetalert2.js", async (HttpContextBase contextBase) =>
        {
            await contextBase.Response.Send(sweetalert2Js);
        });

        _webserver.Routes.PreAuthentication.Parameter.Add(HttpMethod.GET, "/api/workflows", async (HttpContextBase contextBase) =>
        {
            await contextBase.Response.Send(JsonSerializer.Serialize(Directory.GetFiles(ConfigurationManager.Instance.WorkflowsDirectory).Select(filename => new KeyValuePair<string, JsonNode?>(Path.GetFileNameWithoutExtension(filename), JsonNode.Parse(File.ReadAllText(filename)))).ToDictionary(), jsonSerializerOptions));
        });

        _webserver.Routes.PreAuthentication.Parameter.Add(HttpMethod.GET, "/api/listeners", async (HttpContextBase contextBase) =>
        {
            await contextBase.Response.Send(JsonSerializer.Serialize(ConfigurationManager.Instance.AvailableListeners.Values.Select(value => Activator.CreateInstance(value)), jsonSerializerOptions));
        });

        _webserver.Routes.PreAuthentication.Parameter.Add(HttpMethod.GET, "/api/steps", async (HttpContextBase contextBase) =>
        {
            await contextBase.Response.Send(JsonSerializer.Serialize(ConfigurationManager.Instance.AvailableSteps.Values.Select(value => Activator.CreateInstance(value)), jsonSerializerOptions));
        });

        _webserver.Routes.PreAuthentication.Parameter.Add(HttpMethod.GET, "/api/triggers", async (HttpContextBase contextBase) =>
        {
            await contextBase.Response.Send(JsonSerializer.Serialize(ConfigurationManager.Instance.AvailableTriggers.Values.SelectMany(trigger => trigger.Values.Select(value => Activator.CreateInstance(value))), jsonSerializerOptions));
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
