using Habari.Library.Parameters;
using Habari.Library.Steps;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.Json;

namespace Habari.Actions.Database;

public class Select : Step
{
    public override string Code => "Habari.Action.Database.Select";

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
        dbCommand.CommandText = $"SELECT * FROM {ObjectName.GetValue<string>(context)}";
        DbDataReader dbDataReader = dbCommand.ExecuteReader();
        DataTable dataTable = dbDataReader.GetSchemaTable()!;
        List<Dictionary<string, object>> rows = new();
        while (dbDataReader.Read())
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Dictionary<string, object> rowValues = new();
                foreach (DataColumn column in dataTable.Columns)
                {
                    rowValues.Add(column.ColumnName, dbDataReader[column.ColumnName]);
                }
                rows.Add(rowValues);
            }
        }
        dbDataReader.Close();
        dbConnection.Close();
        string content = JsonSerializer.Serialize(rows);
        byte[] fileContentBytes = Encoding.Default.GetBytes(content);
        Content.SetValue(context, (typeof(byte[]), fileContentBytes), (typeof(string), content));
        return Task.CompletedTask;
    }
}
