using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace OMS
{
	class coupon
	{
		public string code;
		public DateTime expiration;

		public void generateCoupon()
		{
			do
			{
				const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
				var random = new Random();
				code = new string(Enumerable.Repeat(chars, 5).Select(s => s[random.Next(s.Length)]).ToArray());
				expiration = DateTime.Now.AddMonths(3);
			} while (!valid());
			sendToDB();
		}

		private bool valid()
		{
			string SQLConnectionString = "Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;";
			// Create an SqlConnection from the provided connection string.
			using (SqlConnection connection = new SqlConnection(SQLConnectionString))
			{
				// Formulate the command.
				SqlCommand command = new SqlCommand();
				command.Connection = connection;

				// Specify the query to be executed.
				command.CommandType = CommandType.Text;
				command.CommandText = @"
                    SELECT * FROM dbo.Coupons
                    ";
				// Open a connection to database.
				connection.Open();
				// Read data returned for the query.
				SqlDataReader reader = command.ExecuteReader();

				// while not done reading the stuff returned from the query
				while (reader.Read())
				{
					string temp = (string)reader[1];

					if (temp == code)
						return false;
				}
			}
			return true;
		}

		private void sendToDB()
		{
			using (SqlConnection openCon = new SqlConnection("Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;"))
			{
				string saveCoupon = "INSERT into dbo.Coupons (Code, ExpDate) VALUES (@code,@expire)";

				using (SqlCommand querySave = new SqlCommand(saveCoupon))
				{
					querySave.Connection = openCon;
					querySave.Parameters.Add("@code", SqlDbType.NChar).Value = code;
					querySave.Parameters.Add("@expire", SqlDbType.Date).Value = expiration;
					
					openCon.Open();
					querySave.ExecuteScalar();
					openCon.Close();
				}
			}
		}
	}
}
