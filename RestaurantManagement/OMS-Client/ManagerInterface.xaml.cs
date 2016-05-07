using OMS_Library;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OMS
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class ManagerInterface : UserControl
    {
        public ManagerInterface()
        {
            InitializeComponent();
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void listBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

		/// <summary>
		/// Select ticket for bill adjustment
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void button_Click(object sender, RoutedEventArgs e)
        {
			List<Cart> totalCart = new List<Cart>();
			decimal running_total = 0m;
			ticketNumber.Text = "N/A";
			authBy.Text = "Employee";
			foreach (client c in clientList)
			{
				if (c.name == Discounted_Tickets_List.SelectedValue.ToString())
				{
					try
					{
						string SQLConnectionString = "Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;";
						// Create an SqlConnection from the provided connection string.
						using (SqlConnection connection = new SqlConnection(SQLConnectionString))
						{
							// Formulate the command.
							SqlCommand command = new SqlCommand(@"SELECT * FROM dbo.Orders WHERE Client=@cli", connection);

							command.Parameters.AddWithValue("@cli", c.ip.ToString());
							connection.Open();
							SqlDataReader reader = command.ExecuteReader();

							// while not done reading the stuff returned from the query
							while (reader.Read())
							{
								Cart tempCart = new Cart();
								tempCart = (Cart)commHelper.ByteToObject((byte[])reader[1]);
								tempCart.Order_num = (int)reader[0];
								totalCart.Add(tempCart);
							}
						}
					}
					catch (Exception) { }

					foreach (Cart item in totalCart)
					{
						foreach (cartItem food in item.Items)
						{
							running_total += food.price;
						}
					}
					running_total = Math.Round(running_total, 2);
					originalAmt.Text = running_total.ToString();
					discountAmt.Text = c.adjustment.ToString();
				}
			}
			reason.Text = "N/A";
			
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void PullDailyReport_Click(object sender, RoutedEventArgs e)
        {
            TotalRevenue.Items.Clear();
            DailySummary.Items.Clear();
            decimal total = 0;
            // Create an SqlConnection from the provided connection string.
            using (SqlConnection connection = new SqlConnection("Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;"))
            {
                // Formulate the command.
                SqlCommand command = new SqlCommand(@"SELECT * FROM dbo.DailyRevenue", connection);
                // Open a connection to database.
                connection.Open();
                // Read data returned for the query.
                SqlDataReader reader = command.ExecuteReader();

                // while not done reading the stuff returned from the query
                while (reader.Read())
                {
                    decimal temp = (decimal)reader[0];

                    total += temp;
                    DailySummary.Items.Add(temp);
                }
            }

            TotalRevenue.Items.Add(total);
        }

		List<client> clientList = new List<client>();

		public void addBillAdjustment(string tableIP, decimal priceAdjustment)
		{
			client temp = new client() { ip = IPAddress.Parse(tableIP), name = getClientName(IPAddress.Parse(tableIP)), adjustment = priceAdjustment };
			clientList.Add(temp);

			Discounted_Tickets_List.Items.Add(temp.name);
		}

		class client
		{
			public IPAddress ip { get; set; }
			public string name { get; set; }
			public decimal adjustment { get; set; }
		}

		private string getClientName(IPAddress ip)
		{
			string machineName = string.Empty;
			try
			{
				IPHostEntry hostEntry = Dns.GetHostEntry(ip);

				machineName = hostEntry.HostName;
			}
			catch (Exception) { }
			return machineName;
		}

		private void approveBtn_Click(object sender, RoutedEventArgs e)
		{
			foreach(client c in clientList)
			{
				if (c.name == Discounted_Tickets_List.SelectedValue.ToString())
				{
					commHelper.functionSend("approveAdjustment");
					commHelper.functionSend(c.adjustment.ToString());
					commHelper.functionSend(c.ip.ToString());

					Discounted_Tickets_List.Items.Remove(c.name);
				}
			}
		}

		private void denyBtn_Click(object sender, RoutedEventArgs e)
		{
			Discounted_Tickets_List.Items.Remove(Discounted_Tickets_List.SelectedValue.ToString());
		}
	}
}
