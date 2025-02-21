namespace UBS.Risk.Evaluator.Batch.Application.Interfaces
{
	public interface ITradeProcessor
	{
		void ProcessTrades(string? inputFilePath, string? outputFilePath);
	}
}
