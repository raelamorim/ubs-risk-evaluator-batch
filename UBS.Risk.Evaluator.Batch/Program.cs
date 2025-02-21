using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UBS.Risk.Evaluator.Batch.Application.Interfaces;
using UBS.Risk.Evaluator.Batch.Insfrastructure;
using UBS.Risk.Evaluator.Batch.Insfrastructure.Interfaces;

// Configure dependency injection
var serviceProvider = new ServiceCollection()
	.AddLogging(configure => configure.AddConsole())
	.AddSingleton<IFileManager, FileManager>()
	.AddSingleton<ITradeProcessor, TradeProcessor>()
	.BuildServiceProvider();

var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
var tradeProcessor = serviceProvider.GetRequiredService<ITradeProcessor>();

// Load configuration
var configuration = new ConfigurationManager();

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
var configurationFileName = environment == "Development" ? "appsettings_local.json" : "appsettings.json";
configuration.AddJsonFile(configurationFileName, optional: false, reloadOnChange: true);

string? inputFilePath = configuration["InputFilePath"];
string? outputFilePath = configuration["OutputFilePath"];

// Process trades
try 
{ 
	tradeProcessor.ProcessTrades(inputFilePath, outputFilePath);
}
catch (Exception ex)
{
	logger.LogError(ex, "Unexpected error when processing trades");
	throw;
}
