using UBS.Risk.Evaluator.Batch.Domain.Interfaces;

namespace UBS.Risk.Evaluator.Batch.Domain.Models
{
	public class MediumRiskCategory : ITradeCategory
	{
		public string CategoryName => "MEDIUMRISK";

		bool ITradeCategory.Matches(ITrade trade, DateTime referenceDate)
		{
			return trade.Value > 1000000 && trade.ClientSector == "Public";
		}
	}
}
