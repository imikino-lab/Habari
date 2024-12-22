using Habari.Library.Steps;
using Habari.Library.Listeners;
using Habari.Library.Workflows;
using System.Reflection;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;

namespace Habari.Library;

public sealed class ConfigurationManager
{
    private static readonly Lazy<ConfigurationManager> configurationManager = new Lazy<ConfigurationManager>(() => new ConfigurationManager());

    private ILogger? _logger { get; set; }

    public Dictionary<string, Type> AvailableListeners { get; private set; } = new Dictionary<string, Type>();

    public Dictionary<string, Type> AvailableSteps { get; private set; } = new Dictionary<string, Type>();

    public static ConfigurationManager Instance { get => configurationManager.Value; }

    private ConfigurationManager()
    {
        ILoggerFactory loggerFactory = LoggerFactory.Create(
            builder => builder
#if DEBUG
            .AddDebug()
            .SetMinimumLevel(LogLevel.Debug)
#else
            .SetMinimumLevel(LogLevel.Information)
#endif
        );

        _logger = loggerFactory.CreateLogger("Habari configuration");
    }

    public void ConfigureLogger(ILogger logger)
    {
        _logger = logger;
    }

    public IListener? GetListener(string listenerCode)
    {
        return Activator.CreateInstance(AvailableListeners[listenerCode]) as IListener;
    }

    public IStep? GetStep(string stepCode)
    {
        return Activator.CreateInstance(AvailableSteps[stepCode]) as IStep;
    }

    public void LoadConfiguration(string filename, out Workflow? workflow)
    {
        workflow = null;
        if (!File.Exists(filename))
        {
            throw new FileNotFoundException($"Configuration file not found.", filename);
        }

        JsonNode? config = JsonNode.Parse(File.ReadAllText(filename));

        if (config == null)
        {
            throw new Exception("Configuration file is not a json formatted file.");
        }

        _logger?.LogInformation($"Loading configuration from {filename}.");

        if (config!["stepsDirectories"] != null)
            foreach (JsonNode? workflowStepDirectory in config!["stepsDirectories"]!.AsArray())
            {
                string directory = workflowStepDirectory!.AsValue().ToString();
                Directory.CreateDirectory(directory);
                foreach (string actionFilename in Directory.GetFiles(directory))
                    foreach (IStep iWorkflowStep in LoadTypesFromAssembly<IStep>(LoadAssembly(actionFilename)))
                        AvailableSteps.Add(iWorkflowStep.Code, iWorkflowStep.GetType());
            }

        if (config!["listenersDirectories"] != null)
            foreach (JsonNode? listenersDirectory in config!["listenersDirectories"]!.AsArray())
            {
                string directory = listenersDirectory!.AsValue().ToString();
                Directory.CreateDirectory(directory);
                foreach (string listenerFilename in Directory.GetFiles(directory))
                    foreach (IListener iListener in LoadTypesFromAssembly<IListener>(LoadAssembly(listenerFilename)))
                        AvailableListeners.Add(iListener.Code, iListener.GetType());
            }

        if (config!["workflow"] != null)
        {
            string listenerCode = config!["workflow"]!["code"]!.AsValue().GetValue<string>();
            IListener? listener = GetListener(listenerCode);
            if (listener != null)
            {
                workflow = new Workflow(listener);
                workflow!.Load(config!["workflow"]!.AsObject());
            }
        }

        _logger?.LogInformation($"Configuration loaded.");
    }

    private Assembly? LoadAssembly(string assemblyPath)
    {
        if (!File.Exists(assemblyPath))
        {
            return null;
        }

        _logger?.LogInformation($"Loading library from: {assemblyPath}");
        DependencyLoadContext loadContext = new DependencyLoadContext(assemblyPath);
        return loadContext.LoadFromAssemblyName(AssemblyName.GetAssemblyName(assemblyPath));
    }

    private IEnumerable<I> LoadTypesFromAssembly<I>(Assembly? assembly) where I : class
    {
        if (assembly != null)
        {
            int count = 0;

            foreach (Type type in assembly!.GetTypes())
            {
                if (typeof(I).IsAssignableFrom(type))
                {
                    I? result = Activator.CreateInstance(type) as I;
                    if (result != null)
                    {
                        count++;
                        yield return result;
                    }
                }
            }
        }
    }
}
