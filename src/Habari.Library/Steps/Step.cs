using Habari.Library.Json;
using Habari.Library.Parameters;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Habari.Library.Steps
{
    public abstract class Step : IStep
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

        public float X { get; set; }

        public float Y { get; set; }

        public StepStatus Status { get; protected set; }

        public Step()
        {
            CreateConstants();
            CreateInputs();
            CreateOutputs();
        }

        public void Load(JsonObject config)
        {
            Id = config["id"]!.GetValue<int>();
            X = config["x"]!.GetValue<float>();
            Y = config["y"]!.GetValue<float>();
            LoadConstants(config);
        }

        private void CreateConstants()
        {
            var properties = GetType().GetProperties().Where(property => property.GetCustomAttributes(typeof(ConstantAttribute), false).Any());
            foreach (var property in properties)
            {
                var attribute = (ConstantAttribute)property.GetCustomAttributes(typeof(ConstantAttribute), false).First();
                Constants.Add(new Constant(this, attribute.Code, attribute.Name, attribute.IsRequired));
            }
        }

        private void CreateInputs()
        {
            var properties = GetType().GetProperties().Where(property => property.GetCustomAttributes(typeof(InputAttribute), false).Any());
            foreach (var property in properties)
            {
                var attribute = (InputAttribute)property.GetCustomAttributes(typeof(InputAttribute), false).First();
                Inputs.Add(new Input(this, attribute.Code, attribute.Name, attribute.IsRequired, attribute.Types));
            }
        }

        private void CreateOutputs()
        {
            var properties = GetType().GetProperties().Where(property => property.GetCustomAttributes(typeof(OutputAttribute), false).Any());
            foreach (var property in properties)
            {
                var attribute = (OutputAttribute)property.GetCustomAttributes(typeof(OutputAttribute), false).First();
                Outputs.Add(new Output(this, attribute.Code, attribute.Name, attribute.Types));
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

        public abstract Task RunAsync(WorkflowContext context);
    }
}
