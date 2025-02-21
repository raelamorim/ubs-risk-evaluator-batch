using UBS.Risk.Evaluator.Batch.Domain.Interfaces;

namespace UBS.Risk.Evaluator.Batch.Domain.Models
{
	public class HighRiskCategory : ITradeCategory
	{
		public string CategoryName => "HIGHRISK";

		bool ITradeCategory.Matches(ITrade trade, DateTime referenceDate)
		{
			return trade.Value > 1000000 && trade.ClientSector == "Private";
		}
	}
}
