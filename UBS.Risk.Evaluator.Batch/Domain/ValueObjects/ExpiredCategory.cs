using UBS.Risk.Evaluator.Batch.Domain.Interfaces;

namespace UBS.Risk.Evaluator.Batch.Domain.Models
{
	public class ExpiredCategory : ITradeCategory
	{
		public string CategoryName => "EXPIRED";

		bool ITradeCategory.Matches(ITrade trade, DateTime referenceDate)
		{
			return (referenceDate - trade.NextPaymentDate).Days > 30;
		}
	}
}
