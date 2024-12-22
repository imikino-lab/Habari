﻿using Habari.Library;
using Habari.Library.Steps;
using Habari.Library.Workflows;
using Microsoft.Extensions.Logging;

namespace Habari;

class Program
{
    static void Main(string[] args)
    {
        ILoggerFactory loggerFactory = LoggerFactory.Create(
            builder => builder
            .AddConsole()
#if DEBUG
            .AddDebug()
            .SetMinimumLevel(LogLevel.Debug)
#else
            .SetMinimumLevel(LogLevel.Information)
#endif
        );

        ILogger logger = loggerFactory.CreateLogger("Habari");
        ConfigurationManager.Instance.ConfigureLogger(logger);

        try
        {
            Workflow? workflow;
            var context = new WorkflowContext();
            ConfigurationManager.Instance.LoadConfiguration(args.Any() ? args[0] : "conf.json", out workflow);
            workflow!.Execute(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
        }
        finally
        {
            Console.ReadLine();
        }
    }
}
