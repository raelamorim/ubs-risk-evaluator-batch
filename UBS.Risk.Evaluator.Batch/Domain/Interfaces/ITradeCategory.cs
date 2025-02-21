namespace UBS.Risk.Evaluator.Batch.Domain.Interfaces
{
	/// <summary>
	/// Interface that describes and matches trade category
	/// </summary>
    public interface ITradeCategory
    {
		/// <summary>
		/// Match the correct implementation of category given trade and reference date
		/// </summary>
		/// <param name="trade"></param>
		/// <param name="referenceDate"></param>
		/// <returns></returns>
		bool Matches(ITrade trade, DateTime referenceDate);

		/// <summary>
		/// Return the name of a category
		/// </summary>
		string CategoryName { get; }
	}
}
