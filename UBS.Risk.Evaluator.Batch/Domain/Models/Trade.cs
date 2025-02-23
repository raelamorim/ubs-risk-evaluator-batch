using UBS.Risk.Evaluator.Batch.Domain.Interfaces;

namespace UBS.Risk.Evaluator.Batch.Domain.Models
{
	/// <summary>
	/// Class that describes a trade in UBS company
	/// </summary>
    public class Trade : ITrade
    {
		/// <summary>
		/// Indicates the transaction amount in dollars
		/// </summary>
		public double Value { get; }

		/// <summary>
		/// Indicates the client´s sector which can be "Public" or "Private" 
		/// </summary>
		public string ClientSector { get; }

		/// <summary>
		/// Indicates when the next payment from the client to the bank is expected
		/// </summary>
		public DateTime NextPaymentDate { get; }

		/// <summary>
		/// Holds any validation errors for the trade.
		/// </summary>
		public List<string> Notifications { get; } = new List<string>();

		public Trade(double value, string clientSector, DateTime nextPaymentDate)
		{
			// Validate Value
			if (value <= 0)
			{
				Notifications.Add("Value must be greater than zero.");
			}

			// Validate ClientSector
			if (string.IsNullOrWhiteSpace(clientSector))
			{
				Notifications.Add("ClientSector is required.");
			}
			else
			{
				if (clientSector != "Private" && clientSector != "Public")
				{
					Notifications.Add("ClientSector must be 'Private' or 'Public'.");
				}
			}

			// If there are no errors, assign values
			if (Notifications.Count == 0)
			{
				Value = value;
				ClientSector = clientSector;
				NextPaymentDate = nextPaymentDate;
			}
		}

		/// <summary>
		/// Returns whether the trade is valid based on notifications
		/// </summary>
		public bool IsValid()
		{
			return Notifications.Count == 0;
		}
	}
}
