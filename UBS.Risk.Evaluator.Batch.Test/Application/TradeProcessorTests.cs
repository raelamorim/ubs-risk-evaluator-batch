using Moq;
using Microsoft.Extensions.Logging;
using UBS.Risk.Evaluator.Batch.Infrastructure.Interfaces;
using UBS.Risk.Evaluator.Batch.Domain.Exceptions;

namespace UBS.Risk.Evaluator.Batch.Test.Application;

public class TradeProcessorTests
{
	private readonly Mock<ILogger<TradeProcessor>> _mockLogger;
	private readonly Mock<IFileManager> _mockFileManager;
	private readonly Mock<StreamReader> _mockReader;
	private readonly Mock<StreamWriter> _mockWriter;
	private readonly TradeProcessor _tradeProcessor;

	public TradeProcessorTests()
	{
		_mockLogger = new Mock<ILogger<TradeProcessor>>();
		_mockReader = new Mock<StreamReader>(MockBehavior.Loose, new MemoryStream());
		_mockWriter = new Mock<StreamWriter>(MockBehavior.Loose, new MemoryStream());

		// Mock FileManager to return our mocked StreamReader and StreamWriter
		_mockFileManager = new Mock<IFileManager>();
		_mockFileManager.Setup(fm => fm.CreateReader(It.IsAny<string>())).Returns(_mockReader.Object);
		_mockFileManager.Setup(fm => fm.CreateWriter(It.IsAny<string>())).Returns(_mockWriter.Object);

		_tradeProcessor = new TradeProcessor(_mockLogger.Object, _mockFileManager.Object);
	}

	[Fact(DisplayName ="Should process trades sucessfully with valid data")]
	public void ProcessTrades_ValidData_ProcessesTradesSuccessfully()
	{
		// Arrange
		var tradeLines = new[]
		{
			"12/31/2024",
			"2",
			"1000.0 Public 01/01/2025",
			"2000.0 Private 02/01/2025"
		};

		_mockReader.SetupSequence(r => r.ReadLine())
			.Returns(() => tradeLines[0])
			.Returns(() => tradeLines[1])
			.Returns(() => tradeLines[2])
			.Returns(() => tradeLines[3])
			.Returns(() => null);

		_mockWriter.Setup(w => w.WriteLine(It.IsAny<string>()));

		// Act
		_tradeProcessor.ProcessTrades("input.txt", "output.txt");

		// Assert
		_mockWriter.Verify(writer => writer.WriteLine(It.IsAny<string>()), Times.Exactly(2));
	}

	[Fact(DisplayName = "Should finish process when input path file is invalid")]
	public void ProcessTrades_InvalidInputPath_ThrowsException()
	{
		// Act & Assert
		var exception = Assert.Throws<InvalidInputFileException>(() =>
		{
			_tradeProcessor.ProcessTrades(null, "output.txt");
		});

		// Assert
		_mockWriter.Verify(writer => writer.WriteLine(It.IsAny<string>()), Times.Never);
	}

	[Fact(DisplayName = "Should finish process when output path file is invalid")]
	public void ProcessTrades_InvalidOutputPath_ThrowsException()
	{
		// Act & Assert
		var exception = Assert.Throws<InvalidOutputFileException>(() =>
		{
			_tradeProcessor.ProcessTrades("input.txt", null);
		});

		// Assert
		_mockWriter.Verify(writer => writer.WriteLine(It.IsAny<string>()), Times.Never);
	}

	[Fact(DisplayName = "Should finish process when reference date is invalid")]
	public void ProcessTrades_InvalidReferenceDate_ThrowsException()
	{
		// Arrange
		var tradeLines = new[]
		{
			"InvalidDate",
            "2",
            "1000.0 Public 01/01/2025",
            "2000.0 Private 02/01/2025"
        };

		_mockReader.SetupSequence(r => r.ReadLine())
			.Returns(() => tradeLines[0])
			.Returns(() => tradeLines[1])
			.Returns(() => tradeLines[2])
			.Returns(() => tradeLines[3])
			.Returns(() => null);

		// Act & Assert
		var exception = Assert.Throws<InvalidReferencedDateException>(() =>
		{
			_tradeProcessor.ProcessTrades("input.txt", "output.txt");
		});

		// Assert
		_mockWriter.Verify(writer => writer.WriteLine(It.IsAny<string>()), Times.Never);
	}

	[Fact(DisplayName = "Should finish process when trading count is invalid")]
	public void ProcessTrades_InvalidTradeCount_ThrowsException()
	{
		// Arrange
		var tradeLines = new[]
		{
			"12/31/2024",
			"InvalidCount",
			"1000.0 Public 01/01/2025",
			"2000.0 Private 02/01/2025"
		};

		_mockReader.SetupSequence(r => r.ReadLine())
			.Returns(() => tradeLines[0])
			.Returns(() => tradeLines[1])
			.Returns(() => tradeLines[2])
			.Returns(() => tradeLines[3])
			.Returns(() => null);

		// Act & Assert
		var exception = Assert.Throws<InvalidTradeCountException>(() =>
		{
			_tradeProcessor.ProcessTrades("input.txt", "output.txt");
		});

		// Assert
		_mockWriter.Verify(writer => writer.WriteLine(It.IsAny<string>()), Times.Never);
	}

	[Fact(DisplayName = "Should logging warning and skip trade when first trade is invalid")]
	public void ProcessTrades_InvalidTradeFormat_LogsWarningAndSkips()
	{
		// Arrange
		var tradeLines = new[]
		{
			"12/31/2024",
            "2",
            "InvalidTrade",
            "2000.0 Private 02/01/2025"
        };

		_mockReader.SetupSequence(r => r.ReadLine())
			.Returns(() => tradeLines[0])
			.Returns(() => tradeLines[1])
			.Returns(() => tradeLines[2])
			.Returns(() => tradeLines[3])
			.Returns(() => null);

		// Act
		_tradeProcessor.ProcessTrades("input.txt", "output.txt");

		// Assert
		_mockWriter.Verify(writer => writer.WriteLine(It.IsAny<string>()), Times.Once);
	}

	[Fact(DisplayName = "Should logging warning and skip trade when trade value is invalid")]
	public void ProcessTrades_InvalidValueFormat_LogsWarningAndSkips()
	{
		// Arrange
		var tradeLines = new[]
		{
			"12/31/2024",
            "1",
            "Invalid Private 01/01/2025"
        };

		_mockReader.SetupSequence(r => r.ReadLine())
			.Returns(() => tradeLines[0])
			.Returns(() => tradeLines[1])
			.Returns(() => tradeLines[2])
			.Returns(() => null);

		// Act
		_tradeProcessor.ProcessTrades("input.txt", "output.txt");

		// Assert
		_mockWriter.Verify(writer => writer.WriteLine(It.IsAny<string>()), Times.Never);
		_mockLogger.Verify(
			x => x.Log(
				LogLevel.Warning,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Invalid value format on trade")),
				It.IsAny<Exception>(),
				It.IsAny<Func<It.IsAnyType, Exception, string>>()
			),
		Times.Once);
	}

	[Fact(DisplayName = "Should logging warning and skip trade when trade date is invalid")]
	public void ProcessTrades_InvalidDateFormat_LogsWarningAndSkips()
	{
		// Arrange
		var tradeLines = new[]
		{
			"12/31/2024",
            "1",
            "1000.0 A InvalidDate"
        };

		_mockReader.SetupSequence(r => r.ReadLine())
			.Returns(() => tradeLines[0])
			.Returns(() => tradeLines[1])
			.Returns(() => tradeLines[2])
			.Returns(() => null);

		// Act
		_tradeProcessor.ProcessTrades("input.txt", "output.txt");

		// Assert
		_mockWriter.Verify(writer => writer.WriteLine(It.IsAny<string>()), Times.Never);
		_mockLogger.Verify(
			x => x.Log(
				LogLevel.Warning,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Invalid date format on trade")),
				It.IsAny<Exception>(),
				It.IsAny<Func<It.IsAnyType, Exception, string>>()
			),
		Times.Once);
	}

	[Fact(DisplayName = "Should logging warning and skip trade when last trade is invalid")]
	public void ProcessTrades_InvalidTrade_LogsWarningAndSkips()
	{
		// Arrange
		var tradeLines = new[]
		{
			"12/31/2024",
            "2",
            "1000.0 Public 01/01/2025",
            "InvalidTrade"
        };

		_mockReader.SetupSequence(r => r.ReadLine())
			.Returns(() => tradeLines[0])
			.Returns(() => tradeLines[1])
			.Returns(() => tradeLines[2])
			.Returns(() => tradeLines[3])
			.Returns(() => null);

		_mockWriter.Setup(w => w.WriteLine(It.IsAny<string>()));

		// Act
		_tradeProcessor.ProcessTrades("input.txt", "output.txt");

		// Assert
		_mockWriter.Verify(writer => writer.WriteLine(It.IsAny<string>()), Times.Once);
		_mockLogger.Verify(
			x => x.Log(
				LogLevel.Warning,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Invalid data format on trade")),
				It.IsAny<Exception>(),
				It.IsAny<Func<It.IsAnyType, Exception, string>>()
			),
		Times.Once);
	}

	[Fact(DisplayName = "Should finish process when trade count is greater than actual lines count")]
	public void ProcessTrades_TradeCountGreaterThanActualLinesCount_ThrowsException()
	{
		// Arrange
		var tradeLines = new[]
		{
			"12/31/2024",
			"3",
			"1000.0 Public 01/01/2025",
			"2000.0 Private 02/01/2025"
		};

		_mockReader.SetupSequence(r => r.ReadLine())
			.Returns(() => tradeLines[0])
			.Returns(() => tradeLines[1])
			.Returns(() => tradeLines[2])
			.Returns(() => tradeLines[3])
			.Returns(() => null);

		_mockWriter.Setup(w => w.WriteLine(It.IsAny<string>()));

		// Act & Assert
		var exception = Assert.Throws<UnexpectEndOfFileException>(() =>
		{
			_tradeProcessor.ProcessTrades("input.txt", "output.txt");
		});

		// Assert
		_mockWriter.Verify(writer => writer.WriteLine(It.IsAny<string>()), Times.Exactly(2));
	}

	[Fact(DisplayName = "Should finish process when trade count is less than actual lines count")]
	public void ProcessTrades_TradeCountLessThanActualLinesCount_ThrowsException()
	{
		// Arrange
		var tradeLines = new[]
		{
			"12/31/2024",
			"2",
			"1000.0 Public 01/01/2025",
			"2000.0 Private 02/01/2025",
			"4000.0 Private 02/01/2025",
			"5000.0 Private 02/01/2025"
		};

		_mockReader.SetupSequence(r => r.ReadLine())
			.Returns(() => tradeLines[0])
			.Returns(() => tradeLines[1])
			.Returns(() => tradeLines[2])
			.Returns(() => tradeLines[4])
			.Returns(() => tradeLines[5])
			.Returns(() => null);

		_mockWriter.Setup(w => w.WriteLine(It.IsAny<string>()));

		// Act & Assert
		var exception = Assert.Throws<UnexpectedExtraLineException>(() =>
		{
			_tradeProcessor.ProcessTrades("input.txt", "output.txt");
		});

		// Assert
		_mockWriter.Verify(writer => writer.WriteLine(It.IsAny<string>()), Times.Exactly(2));
	}
}
