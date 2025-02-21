using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UBS.Risk.Evaluator.Batch.Domain.Interfaces;
using UBS.Risk.Evaluator.Batch.Domain.Models;
using UBS.Risk.Evaluator.Batch.Domain.ValueObjects;

namespace UBS.Risk.Evaluator.Batch.Test.Domain.ValueObjects
{
    public class TradeClassifierTests
    {
		private readonly TradeClassifier _classifier;

		public TradeClassifierTests()
		{
			_classifier = new TradeClassifier();
		}

		[Theory(DisplayName ="Should return correct trade category given")]
		[InlineData(2000000, "Private", "12/29/2025", "HIGHRISK")]
		[InlineData(400000, "Public", "07/01/2020", "EXPIRED")]
		[InlineData(400000, "Public", "11/10/2020", "EXPIRED")]
		[InlineData(2000000, "Private", "11/11/2020", "HIGHRISK")]
		[InlineData(5000000, "Public", "01/02/2024", "MEDIUMRISK")]
		[InlineData(3000000, "Public", "10/26/2023", "MEDIUMRISK")]
		[InlineData(1000000.01, "Public", "10/26/2023", "MEDIUMRISK")]
		[InlineData(1000000.01, "Private", "10/26/2023", "HIGHRISK")]
		[InlineData(1000000, "Public", "10/26/2023", "UNCATEGORIZED")]
		[InlineData(1000000, "Private", "10/26/2023", "UNCATEGORIZED")]
		public void ClassifyTrade_ShouldReturnCorrectCategory(double value, string clientSector, string nextPaymentDate, string expectedCategory)
		{
			DateTime referenceDate = new DateTime(2020, 12, 11);
			DateTime parsedNextPaymentDate = DateTime.ParseExact(nextPaymentDate, "MM/dd/yyyy", null);
			ITrade trade = new Trade(value, clientSector, parsedNextPaymentDate);

			string category = _classifier.ClassifyTrade(trade, referenceDate);

			Assert.Equal(expectedCategory, category);
		}
	}
}
