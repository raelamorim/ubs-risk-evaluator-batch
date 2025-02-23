using System;
using System.Collections.Generic;
using UBS.Risk.Evaluator.Batch.Domain.Models;
using Xunit;
using FluentAssertions;

namespace UBS.Risk.Evaluator.Batch.Domain.Tests
{
	public class TradeTests
	{
		[Fact(DisplayName = "Should be valid when set properties correctly")]
		public void Constructor_ValidTrade_ShouldSetPropertiesCorrectly()
		{
			// Arrange
			double value = 100;
			string clientSector = "Private";
			DateTime nextPaymentDate = DateTime.UtcNow.AddDays(30);

			// Act
			Trade trade = new Trade(value, clientSector, nextPaymentDate);

			// Assert
			trade.Value.Should().Be(value);
			trade.ClientSector.Should().Be(clientSector);
			trade.NextPaymentDate.Should().Be(nextPaymentDate);
			trade.Notifications.Should().BeEmpty();
			trade.IsValid().Should().BeTrue();
		}

		[Fact(DisplayName = "Should be invalid when value was negative")]
		public void Constructor_NegativeValue_ShouldAddNotification()
		{
			// Arrange
			double value = -50;
			string clientSector = "Public";
			DateTime nextPaymentDate = DateTime.UtcNow.AddDays(30);

			// Act
			Trade trade = new Trade(value, clientSector, nextPaymentDate);

			// Assert
			trade.Notifications.Should().ContainSingle()
				.And.Contain("Value must be greater than zero.");
			trade.IsValid().Should().BeFalse();
		}

		[Fact(DisplayName = "Should be invalid when value was zero")]
		public void Constructor_ZeroValue_ShouldAddNotification()
		{
			// Arrange
			double value = 0;
			string clientSector = "Private";
			DateTime nextPaymentDate = DateTime.UtcNow.AddDays(30);

			// Act
			Trade trade = new Trade(value, clientSector, nextPaymentDate);

			// Assert
			trade.Notifications.Should().ContainSingle()
				.And.Contain("Value must be greater than zero.");
			trade.IsValid().Should().BeFalse();
		}

		[Fact(DisplayName = "Should be invalid when client sector is uncategorized")]
		public void Constructor_InvalidClientSector_ShouldAddNotification()
		{
			// Arrange
			double value = 100;
			string clientSector = "Unknown";
			DateTime nextPaymentDate = DateTime.UtcNow.AddDays(30);

			// Act
			Trade trade = new Trade(value, clientSector, nextPaymentDate);

			// Assert
			trade.Notifications.Should().ContainSingle()
				.And.Contain("ClientSector must be 'Private' or 'Public'.");
			trade.IsValid().Should().BeFalse();
		}

		[Fact(DisplayName = "Should be invalid and have multiple errors when properties was not set properly")]
		public void Constructor_MultipleErrors_ShouldAddMultipleNotifications()
		{
			// Arrange
			double value = -10;
			string clientSector = "Unknown";
			DateTime nextPaymentDate = DateTime.UtcNow.AddDays(30);

			// Act
			Trade trade = new Trade(value, clientSector, nextPaymentDate);

			// Assert
			trade.Notifications.Should().HaveCount(2)
				.And.Contain("Value must be greater than zero.")
				.And.Contain("ClientSector must be 'Private' or 'Public'.");
			trade.IsValid().Should().BeFalse();
		}

		[Fact(DisplayName = "Should be valid when client sector was Public")]
		public void Constructor_BorderlineValidClientSector_ShouldWork()
		{
			// Arrange
			double value = 50;
			string clientSector = "Public";
			DateTime nextPaymentDate = DateTime.UtcNow.AddDays(10);

			// Act
			Trade trade = new Trade(value, clientSector, nextPaymentDate);

			// Assert
			trade.Notifications.Should().BeEmpty();
			trade.IsValid().Should().BeTrue();
		}

		[Fact(DisplayName = "Should be invalid when client sector was null")]
		public void Constructor_NullOrEmptyClientSector_ShouldAddNotification()
		{
			// Arrange
			double value = 100;
			DateTime nextPaymentDate = DateTime.UtcNow.AddDays(30);

			// Act
			Trade trade = new Trade(value, null, nextPaymentDate);

			// Assert
			trade.Notifications.Should().ContainSingle()
				.And.Contain("ClientSector is required.");
			trade.IsValid().Should().BeFalse();
		}

	}
}
