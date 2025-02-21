namespace UBS.Risk.Evaluator.Batch.Domain.Exceptions
{
	using System;

	public class InvalidOutputFileException : Exception
	{
		public InvalidOutputFileException(string message) : base(message)
		{
		}
	}
}
