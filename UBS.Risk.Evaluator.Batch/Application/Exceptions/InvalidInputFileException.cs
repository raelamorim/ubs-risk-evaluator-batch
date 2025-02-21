namespace UBS.Risk.Evaluator.Batch.Domain.Exceptions
{
	using System;

	public class InvalidInputFileException : Exception
	{
		public InvalidInputFileException(string message) : base(message)
		{
		}
	}
}
