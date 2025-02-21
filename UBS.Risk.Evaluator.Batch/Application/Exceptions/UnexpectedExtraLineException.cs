using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UBS.Risk.Evaluator.Batch.Domain.Exceptions
{
	public class UnexpectedExtraLineException : Exception
	{
		public UnexpectedExtraLineException(string message) : base(message)
		{
		}
	}
}
