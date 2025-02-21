using UBS.Risk.Evaluator.Batch.Domain.Interfaces;
using UBS.Risk.Evaluator.Batch.Domain.Models;

namespace UBS.Risk.Evaluator.Batch.Domain.ValueObjects
{
    public class TradeClassifier
    {
		private readonly List<ITradeCategory> _categories;
		public TradeClassifier()
		{
			_categories = new List<ITradeCategory>
			{
				new ExpiredCategory(),
				new HighRiskCategory(),
				new MediumRiskCategory()
			};
		}

		public string ClassifyTrade(ITrade trade, DateTime referenceDate)
		{
			return _categories.FirstOrDefault(category => category.Matches(trade, referenceDate))?.CategoryName ?? "UNCATEGORIZED";
		}
	}
}
