using Habari.Library.Listeners;
using Habari.Library.Steps;
using Habari.Library.Workflows;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json.Nodes;

namespace Habari.Library;

public sealed class ConfigurationManager
{
    private static readonly Lazy<ConfigurationManager> configurationManager = new Lazy<ConfigurationManager>(() => new ConfigurationManager());

    private ILogger? _logger { get; set; }

    public Dictionary<string, Type> AvailableListeners { get; private set; } = new();

    public Dictionary<string, Type> AvailableSteps { get; private set; } = new();

    public Dictionary<string, Dictionary<string, Type>> AvailableTriggers { get; private set; } = new();

    public int ConfigurationPort { get; private set; } = 9000;

    public static ConfigurationManager Instance { get => configurationManager.Value; }

    public Dictionary<string, Workflow> Workflows { get; private set; } = new();

    public string WorkflowsDirectory { get; private set; } = string.Empty;

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

    public Listener? GetListener(string listenerCode)
    {
        return Activator.CreateInstance(AvailableListeners[listenerCode]) as Listener;
    }

    public Step? GetStep(string stepCode)
    {
        return Activator.CreateInstance(AvailableSteps[stepCode]) as Step;
    }

    public Trigger? GetTrigger(string listenerCode, string triggerCode)
    {
        return Activator.CreateInstance(AvailableTriggers[listenerCode][triggerCode]) as Trigger;
    }

    public void LoadConfiguration(string filename)
    {
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
                foreach (string actionFilename in Directory.GetFiles(directory).Where(actionFilename => Path.GetExtension(actionFilename).Equals(".dll", StringComparison.InvariantCultureIgnoreCase)))
                    foreach (Step step in LoadTypesFromAssembly<Step>(LoadAssembly(actionFilename)))
                        AvailableSteps.Add(step.Code, step.GetType());
            }

        if (config!["listenersDirectories"] != null)
            foreach (JsonNode? listenersDirectory in config!["listenersDirectories"]!.AsArray())
            {
                string directory = listenersDirectory!.AsValue().ToString();
                Directory.CreateDirectory(directory);
                foreach (string listenerFilename in Directory.GetFiles(directory).Where(listenerFilename => Path.GetExtension(listenerFilename).Equals(".dll", StringComparison.InvariantCultureIgnoreCase)))
                    foreach (Listener listener in LoadTypesFromAssembly<Listener>(LoadAssembly(listenerFilename)))
                    {
                        AvailableListeners.Add(listener.Code, listener.GetType());
                        AvailableTriggers.Add(listener.Code, new());
                        foreach (Trigger trigger in LoadTypesFromAssembly<Trigger>(LoadAssembly(listenerFilename)))
                            AvailableTriggers[listener.Code].Add(trigger.Code, trigger.GetType());
                    }
            }

        if (config!["configurationPort"] != null)
            ConfigurationPort = config!["configurationPort"]!.AsValue().GetValue<int>();

        if (config!["workflowsDirectory"] != null)
        {
            WorkflowsDirectory = config!["workflowsDirectory"]!.AsValue().ToString();
            Directory.CreateDirectory(WorkflowsDirectory);
            foreach (string workflowFilename in Directory.GetFiles(WorkflowsDirectory))
            {
                JsonNode? workflowConfig = JsonNode.Parse(File.ReadAllText(workflowFilename));
                string listenerCode = workflowConfig!["code"]!.AsValue().GetValue<string>();
                IListener? listener = GetListener(listenerCode);
                if (listener != null)
                {
                    Workflow workflow = new(listener);
                    workflow!.Load(workflowConfig!.AsObject());
                    workflow.Execute();
                    Workflows.Add(Path.GetFileNameWithoutExtension(workflowFilename), workflow);
                }
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
        DependencyLoadContext loadContext = new(assemblyPath);
        try
        {
            return loadContext.LoadFromAssemblyName(AssemblyName.GetAssemblyName(assemblyPath));
        }
        catch
        {
            return null;
        }
    }

    private IEnumerable<I> LoadTypesFromAssembly<I>(Assembly? assembly) where I : class
    {
        if (assembly != null)
        {
            int count = 0;

            foreach (Type type in assembly!.GetTypes())
            {
                if (typeof(I).IsAssignableFrom(type) && !type.IsAbstract)
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
