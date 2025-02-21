namespace UBS.Risk.Evaluator.Batch.Domain.Exceptions
{
	public class InvalidReferencedDateException : Exception
	{
		public InvalidReferencedDateException(string message) : base(message)
		{
		}
	}
}
