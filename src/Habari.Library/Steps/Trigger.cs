using Habari.Library.Listeners;
using Habari.Library.Parameters;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Habari.Library.Steps
{
    public abstract class Trigger : IStep
    {
        public int Id { get; protected set; }

        public abstract string Code { get; }

        public abstract string Description { get; }

        public abstract string Name { get; }

        public InputParameters Inputs { get; protected set; } = new InputParameters();

        public OutputParameters Outputs { get; protected set; } = new OutputParameters();

        public StepStatus Status { get; protected set; }

        public IList<IStep> Steps { get; protected set; } = new List<IStep>();

        private IList<IStep> ResolveExcutionOrder()
        {

            Dictionary<IStep, List<IStep>> graph = new Dictionary<IStep, List<IStep>>();
            Dictionary<IStep, int> degree = new Dictionary<IStep, int>();

            // Initialize
            foreach (IStep step in Steps)
            {
                graph[step] = new List<IStep>();
                degree[step] = 0;
            }

            //Compute Degree
            foreach (IStep step in Steps)
            {
                foreach (IOutputParameter output in step.Outputs)
                {
                    foreach (IStep dependent in Steps.Where(s => s.Inputs.Values.Any(input => input.Source!.Equals(output))))
                    {
                        graph[step].Add(dependent);
                        degree[dependent]++;
                    }
                }
            }

            IList<IStep> sortedSteps = new List<IStep>();
            Queue<IStep> queue = new Queue<IStep>(degree.Where(d => d.Value == 0).Select(d => d.Key));

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

        private IList<IStep> ValidateRelations()
        {
            List<IStep> result = new List<IStep>();

            if (!Inputs.ValidateLink())
                result.Add(this);

            result.AddRange(Steps.Where(step => !step.Inputs.ValidateLink()));

            return result;
        }

        public abstract void HandleException(WorkflowContext context, Exception exception);

        public void Load(JsonObject config)
        {
            Id = config["id"]!.GetValue<int>();
            LoadCustomParameters(config);
            LoadSteps(config);
            LoadRelations(config);
        }

        public abstract void LoadCustomParameters(JsonObject config);

        public void Run(WorkflowContext context)
        {
            IList<IStep> stepMissingRelations = ValidateRelations();
            if (stepMissingRelations.Any())
            {
                throw new Exception("Missing required parameters.");
            }

            IList<IStep> sortedSteps = ResolveExcutionOrder();

            foreach (IStep step in sortedSteps)
            {
                step.Run(context);
            }
        }

        private void LoadSteps(JsonObject config)
        {
            foreach (JsonNode? stepConfig in config!["steps"]!.AsArray())
            {
                IStep? step = ConfigurationManager.Instance.GetStep(stepConfig!["code"]!.AsValue().GetValue<string>());
                if (step != null)
                {
                    step.Load(stepConfig.AsObject());
                    Steps.Add(step);
                }
            }
        }

        private void LoadRelations(JsonObject config)
        {
            foreach (JsonNode? relationConfig in config!["relations"]!.AsArray())
            {
                string[] from = relationConfig!["from"]!.AsValue().GetValue<string>().Split('.');
                string[] to = relationConfig!["to"]!.AsValue().GetValue<string>().Split('.');
                IStep? stepFrom = from[0].Equals("trigger", StringComparison.InvariantCultureIgnoreCase) ? this : Steps.FirstOrDefault(step => from[1].Equals(step.Id.ToString()));
                IStep? stepTo = to[0].Equals("trigger", StringComparison.InvariantCultureIgnoreCase) ? this : Steps.FirstOrDefault(step => to[1].Equals(step.Id.ToString()));
                if (stepFrom != null && stepTo != null)
                    stepTo.Inputs[to[2]].Link(stepFrom.Outputs[from[2]]);
            }
        }
    }
}
