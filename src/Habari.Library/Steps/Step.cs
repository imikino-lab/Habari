using Habari.Library.Parameters;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Habari.Library.Steps
{
    public abstract class Step : IStep
    {
        public int Id { get; private set; }

        public abstract string Code { get; }

        public Constants Constants { get; } = new();

        public abstract string Description { get; }

        public Inputs Inputs { get; } = new();

        public abstract string Name { get; }

        public Outputs Outputs { get; } = new();

        public float X { get; set; }

        public float Y { get; set; }

        public StepStatus Status { get; protected set; }

        public void Load(JsonObject config)
        {
            Id = config["id"]!.GetValue<int>();
            X = config["x"]!.GetValue<float>();
            Y = config["y"]!.GetValue<float>();
            LoadConstants(config);
            LoadInputs();
            LoadOutputs();
        }

        private void LoadConstants(JsonObject config)
        {
            var properties = GetType().GetProperties().Where(property => property.GetCustomAttributes(typeof(ConstantAttribute), false).Any());
            foreach (var property in properties)
            {
                var attribute = (ConstantAttribute)property.GetCustomAttributes(typeof(ConstantAttribute), false).First();
                Constants.Add(new Constant(this, attribute.Code, attribute.Name, attribute.IsRequired));
                property.SetValue(this, JsonSerializer.Deserialize(config[attribute.Code], property.PropertyType));
            }
        }

        private void LoadInputs()
        {
            var properties = GetType().GetProperties().Where(property => property.GetCustomAttributes(typeof(InputAttribute), false).Any());
            foreach (var property in properties)
            {
                var attribute = (InputAttribute)property.GetCustomAttributes(typeof(InputAttribute), false).First();
                Inputs.Add(new Input(this, attribute.Code, attribute.Name, attribute.IsRequired, attribute.Types));
            }
        }

        private void LoadOutputs()
        {
            var properties = GetType().GetProperties().Where(property => property.GetCustomAttributes(typeof(OutputAttribute), false).Any());
            foreach (var property in properties)
            {
                var attribute = (OutputAttribute)property.GetCustomAttributes(typeof(OutputAttribute), false).First();
                Outputs.Add(new Output(this, attribute.Code, attribute.Name, attribute.Types));
            }
        }

        public abstract Task RunAsync(WorkflowContext context);
    }
}
