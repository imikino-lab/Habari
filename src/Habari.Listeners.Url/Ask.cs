using Habari.Library.Listeners;
using Habari.Library.Steps;
using Habari.Listeners.Url.Triggers;
using System.Text;
using System.Text.Json.Nodes;
using WatsonWebserver;
using WatsonWebserver.Core;

namespace Habari.Listeners.Url;

public class Ask : Listener
{
    public override string Code => "Habari.Listener.Url.Ask";

    public override string Description => "Occurs when a url is asked";

    //public Get DefaultGet { get; private set; } = new ();

    public string DefaultPageName { get; set; } = "index.html";

    public override string Name => "Url asked";

    public string Host { get; set; } = "127.0.0.1";

    public int Port { get; set; } = 80;

    public bool Ssl { get; set; }

    private Webserver? Webserver { get; set; } = null;

    public Ask()
    {
        Triggers.Add(new Get());
    }

    public override void Load(JsonObject config)
    {
        DefaultPageName = config["defaultPageName"]!.GetValue<string>();
        Host = config["host"]!.GetValue<string>();
        Port = config["port"]!.GetValue<int>();
        Ssl = config["ssl"]!.GetValue<bool>();
//        DefaultGet.Load(config["defaultGet"]!.AsObject());
    }

    public override void LoadTrigger(JsonObject config)
    {
        string code = config["code"]!.GetValue<string>();
        switch (code.ToLowerInvariant())
        {
            case "habari.listener.url.ask.get":
                Get get = new Get();
                get.Load(config);
                Triggers.Add(get);
                break;
            default:
                break;
        }
    }

    public override void Start(WorkflowContext context)
    {
        WebserverSettings webserverSettings = new (Host, Port, Ssl);
        Webserver = new (webserverSettings, async (HttpContextBase contextBase) =>
        {
            await RunRoute(contextBase, context, (UrlTrigger)Triggers.First());
        });

        foreach (IStep trigger in Triggers.Skip(1))
        {
            UrlTrigger urlTrigger = (UrlTrigger)trigger;
            Webserver.Routes.PreAuthentication.Parameter.Add(urlTrigger.HttpMethod, urlTrigger.Route, async (HttpContextBase contextBase) =>
            {
                await RunRoute(contextBase, context, urlTrigger);
            });
        }

        Webserver.Start();
    }

    public override void Stop()
    {
        Webserver!.Stop();
        Webserver = null;
        GC.Collect();
    }

    private async Task RunRoute(HttpContextBase contextBase, WorkflowContext context, UrlTrigger trigger)
    {
        string pathString = contextBase.Request.Url.RawWithQuery;
        if (string.IsNullOrWhiteSpace(Path.GetFileName(pathString)))
        {
            pathString += DefaultPageName;
        }
        byte[] pathByte = Encoding.Default.GetBytes(pathString);
        trigger.Context.SetValue(context, (typeof(HttpContextBase), contextBase));
        trigger.Path.SetValue(context, (typeof(string), pathString), (typeof(byte[]), pathByte));
        await trigger.RunAsync(context);
        return;
    }
}
