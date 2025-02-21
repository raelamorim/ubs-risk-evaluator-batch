using Microsoft.Extensions.Logging;
using System.Globalization;
using UBS.Risk.Evaluator.Batch.Application.Interfaces;
using UBS.Risk.Evaluator.Batch.Domain.Exceptions;
using UBS.Risk.Evaluator.Batch.Domain.Interfaces;
using UBS.Risk.Evaluator.Batch.Domain.Models;
using UBS.Risk.Evaluator.Batch.Domain.ValueObjects;
using UBS.Risk.Evaluator.Batch.Infrastructure.Interfaces;

public class TradeProcessor : ITradeProcessor
{
	private readonly ILogger<TradeProcessor> _logger;
	private readonly IFileManager _fileManager;

	public TradeProcessor(ILogger<TradeProcessor> logger, IFileManager fileManager)
	{
		_logger = logger;
		_fileManager = fileManager;
	}

	public void ProcessTrades(string? inputFilePath, string? outputFilePath)
	{
		if (string.IsNullOrEmpty(inputFilePath))
		{
			throw new InvalidInputFileException("Input file path is not properly configured in appsettings.json.");
		}

		if (string.IsNullOrEmpty(outputFilePath))
		{
			throw new InvalidOutputFileException("Output file path is not properly configured in appsettings.json.");
		}

		using (StreamReader reader = _fileManager.CreateReader(inputFilePath))
		using (StreamWriter writer = _fileManager.CreateWriter(outputFilePath))
		{
			TryGetReferenceDate(reader, out DateTime referenceDate);
			TryGetTradeCount(reader, out int tradeCount);

			_logger.LogInformation("Starting trade processing. Total trades: {0}", tradeCount);

			int readCount = 0;
			int writtenCount = 0;
			int warningCount = 0;

			TradeClassifier classifier = new TradeClassifier();

			for (int i = 0; i < tradeCount; i++)
			{
				string? tradeLine = reader.ReadLine()?.Trim();
				readCount++;
				if (tradeLine == null)
				{
					throw new UnexpectEndOfFileException($"Unexpected end of file while reading trade {i + 1}.");
				}

				ITrade? trade = ParseTrade(tradeLine, i, ref warningCount);
				if (trade == null) continue;

				string classification = classifier.ClassifyTrade(trade, referenceDate);
				_logger.LogDebug("Trade {0} classified as: {1}", i + 1, classification);
				writer.WriteLine(classification);
				writtenCount++;
			}

			string? remainingLine;
			while ((remainingLine = reader.ReadLine()) != null)
			{
				throw new UnexpectedExtraLineException($"Unexpected extra line after trade {tradeCount + 1}: {remainingLine.Trim()}.");
			}

			_logger.LogInformation("Processing completed. Output written to {0}", outputFilePath);
			_logger.LogInformation("Summary: Trades Read: {0}, Trades Written: {1}, Warnings: {2}", readCount, writtenCount, warningCount);
		}
	}

	private void TryGetReferenceDate(StreamReader reader, out DateTime referenceDate)
	{
		string? firstLine = reader.ReadLine()?.Trim();
		if (firstLine == null || !DateTime.TryParseExact(firstLine, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out referenceDate))
		{
			throw new InvalidReferencedDateException("Invalid reference date format.");
		}
	}

	private void TryGetTradeCount(StreamReader reader, out int tradeCount)
	{
		string? secondLine = reader.ReadLine()?.Trim();
		if (secondLine == null || !int.TryParse(secondLine, out tradeCount))
		{
			throw new InvalidTradeCountException("Invalid trade count.");
		}
	}

	private ITrade? ParseTrade(string tradeLine, int tradeIndex, ref int warningCount)
	{
		string[] tradeData = tradeLine.Split(' ');

		if (tradeData.Length != 3)
		{
			_logger.LogWarning("Invalid data format on trade {0}. Skipping.", tradeIndex + 1);
			warningCount++;
			return null;
		}

		if (!double.TryParse(tradeData[0], out double value))
		{
			_logger.LogWarning("Invalid value format on trade {0}. Skipping.", tradeIndex + 1);
			warningCount++;
			return null;
		}

		string clientSector = tradeData[1];

		if (!DateTime.TryParseExact(tradeData[2], "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime nextPaymentDate))
		{
			_logger.LogWarning("Invalid date format on trade {0}. Skipping.", tradeIndex + 1);
			warningCount++;
			return null;
		}

		Trade trade = new Trade(value, clientSector, nextPaymentDate);

		if (!trade.IsValid())
		{
			_logger.LogWarning("Trade {0} is invalid. Skipping.", tradeIndex + 1);
			warningCount++;
			return null;
		}

		return trade;
	}
}
