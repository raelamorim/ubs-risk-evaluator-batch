
namespace UBS.Risk.Evaluator.Batch.Domain.Interfaces
{
    /// <summary>
    /// Interface that describes a trade in UBS company
    /// </summary>
    public interface ITrade
    {
		/// <summary>
		/// Indicates the transaction amount in dollars
		/// </summary>
		double Value { get;  }

		/// <summary>
		/// Indicates the client´s sector which can be "Public" or "Private" 
		/// </summary>
		string ClientSector { get; }

		/// <summary>
		/// Indicates when the next payment from the client to the bank is expected
		/// </summary>
		DateTime NextPaymentDate { get; }

		/// <summary>
		/// Indicates if a trade is valid ou not
		/// </summary>
		bool IsValid();
	}
}
