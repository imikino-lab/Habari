using Habari.Library.Parameters;
using Habari.Library.Steps;
using System.Data.Common;
using System.Text;

namespace Habari.Actions.Database;

public class Describe : Step
{
    public override string Code => "Habari.Action.Database.Describe";

    [Output("content", "File content", ParameterType.Text, typeof(byte[]), typeof(string))]
    public Output Content => Outputs["content"];

    [Input("dataSource", "Data source", ParameterType.DatabaseConnection, true, typeof(DbDataSource))]
    public Input DataSource => Inputs["dataSource"];

    public override string Description => "Describe an object";

    public override string Name => "Describe object";

    [Input("objectName", "Object Name", ParameterType.Text, true, typeof(byte[]), typeof(string))]
    public Input ObjectName => Inputs["objectName"];

    public override Task RunAsync(WorkflowContext context)
    {
        DbDataSource dbDataSource = DataSource.GetValue<DbDataSource>(context)!;
        DbConnection dbConnection = dbDataSource.CreateConnection();
        dbConnection.Open();
        DbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = $"DESCRIBE {ObjectName.GetValue<string>(context)}";
        DbDataReader dbDataReader = dbCommand.ExecuteReader();
        string content = string.Empty;
        while (dbDataReader.Read())
        {
            for (int i = 0; i < dbDataReader.FieldCount; i++)
            {
                content += dbDataReader.GetName(i) + " " + dbDataReader.GetDataTypeName(i) + "\n";
            }
        }
        dbDataReader.Close();
        dbConnection.Close();
        byte[] fileContentBytes = Encoding.Default.GetBytes(content);
        Content.SetValue(context, (typeof(byte[]), fileContentBytes), (typeof(string), content));
        return Task.CompletedTask;
    }
}
