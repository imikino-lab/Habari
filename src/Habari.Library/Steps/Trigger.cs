using Habari.Library.Base;
using Habari.Library.Json;
using Habari.Library.Parameters;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Habari.Library.Steps;

public abstract class Trigger : ITrigger
{
    public int Id { get; protected set; }

    public abstract string Code { get; }

    [JsonConverter(typeof(ConstantsJsonConverter))]
    public Constants Constants { get; } = new();

    public abstract string Description { get; }

    [JsonConverter(typeof(InputsJsonConverter))]
    public Inputs Inputs { get; } = new();

    public abstract string Name { get; }

    [JsonConverter(typeof(OutputsJsonConverter))]
    public Outputs Outputs { get; } = new();

    public int X { get; set; }

    public int Y { get; set; }

    public StepStatus Status { get; protected set; }

    public List<IStep> Steps { get; protected set; } = new ();

    private List<IStep> ResolveExcutionOrder()
    {

        Dictionary<IStep, List<IStep>> graph = new ();
        Dictionary<IStep, int> degree = new ();

        // Initialize
        foreach (IStep step in Steps)
        {
            graph[step] = new ();
            degree[step] = 0;
        }

        //Compute Degree
        foreach (IStep step in Steps)
        {
            foreach (IOutput output in step.Outputs)
            {
                foreach (IStep dependent in Steps.Where(s => s.Inputs.Values.Any(input => input.Source!.Equals(output))))
                {
                    graph[step].Add(dependent);
                    degree[dependent]++;
                }
            }
        }

        List<IStep> sortedSteps = new ();
        Queue<IStep> queue = new (degree.Where(d => d.Value == 0).Select(d => d.Key));

        while (queue.Any())
        {
            IStep currentStep = queue.Dequeue();
            sortedSteps.Add(currentStep);
            foreach (IStep neighbor in graph[currentStep])
            {
                degree[neighbor]--;
                if (degree[neighbor] == 0)
                {
                    queue.Enqueue(neighbor);
                }
            }
        }

        if (sortedSteps.Count != Steps.Count)
        {
            throw new InvalidOperationException("Cyclic dependancy detected in workflow.");
        }

        return sortedSteps;
    }

    private List<IBase> ValidateRelations()
    {
        List<IBase> result = new ();

        if (!Inputs.ValidateLink())
            result.Add(this);

        result.AddRange(Steps.Where(step => !step.Inputs.ValidateLink()));

        return result;
    }

    public abstract void HandleException(WorkflowContext context, Exception exception);

    public Trigger()
    {
        CreateConstants();
        CreateInputs();
        CreateOutputs();
    }

    public void Load(JsonObject config)
    {
        Id = config["id"]!.GetValue<int>();
        X = config["x"]!.GetValue<int>();
        Y = config["y"]!.GetValue<int>();
        LoadConstants(config);
        LoadStepsAndRelations(config);
    }

    public async Task RunAsync(WorkflowContext context)
    {
        List<IBase> stepMissingRelations = ValidateRelations();
        if (stepMissingRelations.Any())
        {
            throw new Exception("Missing required parameters.");
        }

        List<IStep> sortedSteps = ResolveExcutionOrder();

        foreach (IStep step in sortedSteps)
        {
            await step.RunAsync(context);
        }
    }

    private void CreateConstants()
    {
        var properties = GetType().GetProperties().Where(property => property.GetCustomAttributes(typeof(ConstantAttribute), false).Any());
        foreach (var property in properties)
        {
            var attribute = (ConstantAttribute)property.GetCustomAttributes(typeof(ConstantAttribute), false).First();
            Constants.Add(new Constant(this, attribute.Code, attribute.Name, attribute.ParameterType, attribute.IsRequired));
        }
    }

    private void CreateInputs()
    {
        var properties = GetType().GetProperties().Where(property => property.GetCustomAttributes(typeof(InputAttribute), false).Any());
        foreach (var property in properties)
        {
            var attribute = (InputAttribute)property.GetCustomAttributes(typeof(InputAttribute), false).First();
            Inputs.Add(new Input(this, attribute.Code, attribute.Name, attribute.ParameterType, attribute.IsRequired, attribute.Types));
        }
    }

    private void CreateOutputs()
    {
        var properties = GetType().GetProperties().Where(property => property.GetCustomAttributes(typeof(OutputAttribute), false).Any());
        foreach (var property in properties)
        {
            var attribute = (OutputAttribute)property.GetCustomAttributes(typeof(OutputAttribute), false).First();
            Outputs.Add(new Output(this, attribute.Code, attribute.Name, attribute.ParameterType, attribute.Types));
        }
    }

    private void LoadConstants(JsonObject config)
    {
        var properties = GetType().GetProperties().Where(property => property.GetCustomAttributes(typeof(ConstantAttribute), false).Any());
        foreach (var property in properties)
        {
            var attribute = (ConstantAttribute)property.GetCustomAttributes(typeof(ConstantAttribute), false).First();
            property.SetValue(this, JsonSerializer.Deserialize(config[attribute.Code], property.PropertyType));
        }
    }

    private void LoadStepsAndRelations(JsonObject config)
    {
        Steps.Clear();
        foreach (JsonNode? stepConfig in config!["steps"]!.AsArray())
        {
            Step? step = ConfigurationManager.Instance.GetStep(stepConfig!["code"]!.AsValue().GetValue<string>());
            if (step != null)
            {
                step.Load(stepConfig.AsObject());
                Steps.Add(step);
            }
        }

        foreach (JsonNode? relationConfig in config!["relations"]!.AsArray())
        {
            string[] from = relationConfig!["from"]!.AsValue().GetValue<string>().Split('.');
            string[] to = relationConfig!["to"]!.AsValue().GetValue<string>().Split('.');
            IBase? stepFrom = from[0].Equals("trigger", StringComparison.InvariantCultureIgnoreCase) ? this : Steps.FirstOrDefault(step => from[1].Equals(step.Id.ToString()));
            IBase? stepTo = to[0].Equals("trigger", StringComparison.InvariantCultureIgnoreCase) ? this : Steps.FirstOrDefault(step => to[1].Equals(step.Id.ToString()));
            if (stepFrom != null && stepTo != null)
                stepTo.Inputs[to[2]].Link(stepFrom.Outputs[from[2]]);
        }
    }
}
