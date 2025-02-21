namespace UBS.Risk.Evaluator.Batch.Domain.Exceptions
{
	public class InvalidTradeCountException : Exception
	{
		public InvalidTradeCountException(string message) : base(message)
		{
		}
	}
}
