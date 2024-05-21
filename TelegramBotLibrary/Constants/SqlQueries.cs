namespace TelegramBotLibrary.Constants
{
	internal class SqlQueries
	{
		public const string InsertBuyer = "INSERT INTO public.\"Buyer\" (\"Username\", \"ChannelId\") " +
								  "VALUES (@Username, (" +
								  "    SELECT c.\"Id\" " +
								  "    FROM public.\"Channel\" c " +
								  "    WHERE c.\"Name\" LIKE @ChannelName))";

		public const string UpdateBuyer = "UPDATE public.\"Buyer\" b " +
										  "SET \"ChannelId\" = (" +
										  "    SELECT c.\"Id\" " +
										  "    FROM public.\"Channel\" c " +
										  "    WHERE c.\"Name\" LIKE @ChannelName) " +
										  "WHERE b.\"Username\" LIKE @Username";

		public const string SelectBuyer = "SELECT b.* " +
										  "FROM public.\"Buyer\" b " +
										  "WHERE b.\"Username\" LIKE @Username";

		public const string SelectCostChannelByUsername = "SELECT c.\"Cost\" " +
														   "FROM public.\"Channel\" c " +
														   "WHERE c.\"Id\" = (" +
														   "    SELECT b.\"ChannelId\" " +
														   "    FROM public.\"Buyer\" b " +
														   "    WHERE b.\"Username\" LIKE @Username)";

		public const string UpdateBuyerInvoicedIdByUsername = "UPDATE public.\"Buyer\" " +
															   "SET \"InvoiceId\" = @InvoiceId " +
															   "WHERE \"Username\" LIKE @Username";

		public const string SelectInvoiceIdByUsername = "SELECT \"InvoiceId\" " +
														 "FROM public.\"Buyer\" " +
														 "WHERE \"Username\" LIKE @Username";

		public const string SelectChannelUrl = "SELECT c.\"Url\" " +
											   "FROM public.\"Channel\" c " +
											   "WHERE c.\"Id\" = (" +
											   "    SELECT b.\"ChannelId\" " +
											   "    FROM public.\"Buyer\" b " +
											   "    WHERE b.\"Username\" LIKE @Username)";

	}
}
