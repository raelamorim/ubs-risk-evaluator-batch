namespace UBS.Risk.Evaluator.Batch.Domain.Exceptions
{
	public class UnexpectEndOfFileException : Exception
	{
		public UnexpectEndOfFileException(string message) : base(message)
		{
		}
	}
}
