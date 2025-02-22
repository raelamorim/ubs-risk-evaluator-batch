using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Elasticsearch;
using UBS.Risk.Evaluator.Batch.Application.Interfaces;
using UBS.Risk.Evaluator.Batch.Infrastructure;
using UBS.Risk.Evaluator.Batch.Infrastructure.Interfaces;

// Load configuration
var configuration = new ConfigurationManager();

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
var configurationFileName = environment == "Development" ? "appsettings_local.json" : "appsettings.json";
configuration.AddJsonFile(configurationFileName, optional: false, reloadOnChange: true);

string? inputFilePath = configuration["InputFilePath"];
string? outputFilePath = configuration["OutputFilePath"];

// Configure logging
GetLogger(configuration, environment);

// Configure dependency injection
var serviceProvider = new ServiceCollection()
	.AddLogging(configure => configure.AddSerilog())
	.AddSingleton<IFileManager, FileManager>()
	.AddSingleton<ITradeProcessor, TradeProcessor>()
	.BuildServiceProvider();

var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
var tradeProcessor = serviceProvider.GetRequiredService<ITradeProcessor>();

// Process trades
try
{
	Log.Information("Iniciando o processamento de trades.");
	tradeProcessor.ProcessTrades(inputFilePath, outputFilePath);
	Log.Information("Processamento de trades concluído com sucesso.");
}
catch (Exception ex)
{
	Log.Error(ex, "Erro inesperado ao processar trades");
	throw;
}
finally
{
	Log.CloseAndFlush();
}

static void GetLogger(IConfiguration configuration, string environment)
{
	var loggerConfiguration = new LoggerConfiguration();


	if (environment == "Development")
	{
		loggerConfiguration.WriteTo.Console();
	}
	else
	{
		string? logsDir = configuration["LogDir"];

		if (string.IsNullOrEmpty(logsDir))
		{
			throw new Exception("Log file path is not properly configured in appsettings.json");
		}


		TimeZoneInfo brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById(Environment.GetEnvironmentVariable("TZ") ?? "America/Sao_Paulo");
		var timestamp = TimeZoneInfo.ConvertTime(DateTime.Now, brazilTimeZone).ToString("yyyy-MM-dd HH.mm.ss");
		var logFileName = Path.Combine(logsDir, string.Concat("log_", timestamp, ".json"));

		loggerConfiguration
			.WriteTo.File(new ElasticsearchJsonFormatter(), logFileName)
			.Enrich.FromLogContext()
			.Enrich.WithEnvironmentName()
			.Enrich.WithMachineName()
			.Enrich.WithThreadId()
			.Enrich.WithCorrelationId()
			.Enrich.WithProperty("Application", "UBS.Risk.Evaluator.Batch")
			.Enrich.WithProperty("Environment", environment); ;
	}

	Log.Logger = loggerConfiguration.CreateLogger();
}
