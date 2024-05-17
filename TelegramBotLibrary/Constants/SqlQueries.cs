using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotLibrary.Constants
{
	internal class SqlQueries
	{
		public const string InsertBuyer = "INSERT " +
			"INTO Buyer (Username, ChannelId) " +
			"VALUES (@Username, (" +
			"	SELECT Id" +
			"	FROM Channel" +
			$"	WHERE Name LIKE @ChannelName))";

		public const string UpdateBuyer = "UPDATE Buyer " +
			"SET ChannelId = (" +
			"	SELECT Id" +
			"	FROM Channel" +
			"	WHERE Name LIKE @ChannelName) " +
			"WHERE Username LIKE @Username";

		public const string SelectBuyer = "SELECT * " +
			"FROM Buyer " +
			$"WHERE Username LIKE @Username";

		public const string SelectCostChannelByUsername = "SELECT Cost " +
			"FROM Channel " +
			"WHERE Id = (" +
			"	SELECT ChannelId" +
			"	FROM Buyer" +
			"	WHERE Username LIKE @Username)";

		public const string UpdateBuyerInvoicedIdByUsername = "UPDATE Buyer " +
			"SET InvoiceId = @InvoiceId " +
			"WHERE Username LIKE @Username";

		public const string SelectInvoiceIdByUsername = "SELECT InvoiceId " +
			"FROM Buyer " +
			"WHERE Username LIKE @Username";

		public const string SelectChannelUrl = "SELECT Url " +
			"FROM Channel " +
			"WHERE Id = (SELECT ChannelId " +
			"	FROM Buyer " +
			"	WHERE Username LIKE @Username)";
	}
}
