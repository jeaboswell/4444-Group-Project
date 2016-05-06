using OMS_Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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
using DColor = System.Drawing.Color;
using MColor = System.Windows.Media.Color;

namespace OMS
{
    public class Refill
    {
        public string drink { get; set; }
        public string ip { get; set; }
    }
    /// <summary>
    /// Interaction logic for EmployeeInterface.xaml
    /// </summary>
    public partial class EmployeeInterface : UserControl
    {
        List<ClientInfo> TableList = new List<ClientInfo>();
        List<Refill> RefillList = new List<Refill>();
        public EmployeeInterface()
        {
			InitializeComponent();
            //testTable();
            updateTables();
        }

        private void testTable()
        {
            TableList.Add(new ClientInfo { Name = "Table 1" });
            TableList.Add(new ClientInfo { Name = "Table 2" });
        }

		public void getTableList(List<ClientInfo> list)
		{
			TableList = list;
			Dispatcher.Invoke(() =>
			{
				updateTables();
			});
		}

		private void updateTables()
        {
			Table_Grid.Children.Clear();
            foreach (ClientInfo iter in TableList)
            {
                Button tmpButton = new Button();
                tmpButton.Content = iter.Name;
                tmpButton.Click += (sender, e) => 
                {
                    updateRefills();
                    currentTableName.Content = ((Button)sender).Content;
                    currentTableStatus.Content = iter.status;
                    tableOptions.Visibility = Visibility.Visible;
                    foreach (ClientInfo c in TableList)
                    {
                        if (c.Name == currentTableName.Content.ToString())
                            getDrinks(c.IP.ToString());
                    }
                };
                tmpButton.Height = 100;
                tmpButton.Width = 100;
				switch (iter.status)
				{
					case "Open":
						tmpButton.Background = new SolidColorBrush(ToMediaColor(DColor.Lime));
						break;
					case "Help Requested":
					case "Waiting to pay with cash":
					case "Waiting to pay with check":
						tmpButton.Background = new SolidColorBrush(ToMediaColor(DColor.Red));
						break;
					case "Reading Menu":
					case "Placed order and waiting on food":
					case "Eating":
                    case "Table Needs Cleaning":
						tmpButton.Background = new SolidColorBrush(ToMediaColor(DColor.Yellow));
						break;
					default:
						break;
				}
				
				foreach (Refill thing in RefillList)
				{
					if (thing.ip == iter.IP.ToString())
					{
						tmpButton.Background = new SolidColorBrush(ToMediaColor(DColor.Red));
					}
				}

				Table_Grid.Children.Add(tmpButton);
            }

        }

        public void updateRefills()
        {
            RefillList.Clear();
            try
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
                    SELECT * FROM dbo.Drinks
                    ";
                    // Open a connection to database.
                    connection.Open();
                    // Read data returned for the query.
                    SqlDataReader reader = command.ExecuteReader();

                    // while not done reading the stuff returned from the query
                    while (reader.Read())
                    {   // fill up the Refill list object
                        RefillList.Add(new Refill
                        {
                            drink = (string)reader[0],
                            ip = (string)reader[1]
                        });
                    }
                }
            }
            catch (Exception) { }
        }

        public void requestHelp(IPAddress table)
        {
            foreach (ClientInfo itr in TableList)
            {
                if(itr.IP == table)
                {
                    itr.priorStatus = itr.status;
                    itr.status = "Help Requested";
					commHelper.functionSend("recieveClient");
					commHelper.objectSend(itr);
                }
            }    
        }

        public void cancelHelp(IPAddress table)
        {
            foreach (ClientInfo itr in TableList)
            {
                if (itr.IP == table)
                {
                    itr.status = itr.priorStatus;
					commHelper.functionSend("recieveClient");
					commHelper.objectSend(itr);
				}
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
			tableOptions.Visibility = Visibility.Hidden;

			foreach (ClientInfo itr in TableList)
			{
				if (itr.Name == currentTableName.Content.ToString())
				{
					if (itr.status != currentTableStatus.Content.ToString())
					{
						itr.status = currentTableStatus.Content.ToString();

						commHelper.functionSend("recieveClient");
						commHelper.objectSend(itr);
					}
				}
			}
            currentTableStatus.Content = "";
        }

		private void payWithCash_Click(object sender, RoutedEventArgs e)
        {
            foreach (ClientInfo itr in TableList)
            {
                if (itr.Name == currentTableName.Content.ToString())
                {
                    itr.priorStatus = itr.status;
                    itr.status = "Paid";
                    commHelper.functionSend("clientPaid");
                    commHelper.objectSend(itr);
                }
            }
            currentTableStatus.Content = "Paid";
        }

        private void payWithCheck_Click(object sender, RoutedEventArgs e)
        {
            foreach (ClientInfo itr in TableList)
            {
                if (itr.Name == currentTableName.Content.ToString())
                {
                    itr.priorStatus = itr.status;
                    itr.status = "Paid";
                    commHelper.functionSend("clientPaid");
                    commHelper.objectSend(itr);
                }
            }
            currentTableStatus.Content = "Paid";
        }
    
        private void ticketAdjustment_Click(object sender, RoutedEventArgs e)
        {
			List<Cart> temp = new List<Cart>();
            tick_ad.Visibility = Visibility.Visible;
			string ip = "";
			foreach (ClientInfo thing in TableList)
			{
				if (thing.Name == currentTableName.Content.ToString())
				{
					ip = thing.IP.ToString();
				}
			}
			try
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
                    SELECT * FROM dbo.Orders
					WHERE Client=@cli
                    ";
					command.Parameters.AddWithValue("@cli",ip );
					// Open a connection to database.
					connection.Open();
					// Read data returned for the query.
					SqlDataReader reader = command.ExecuteReader();

					// while not done reading the stuff returned from the query
					while (reader.Read())
					{
						Cart tempCart = new Cart();
						tempCart = (Cart)ByteToObject((byte[])reader[1]);
						tempCart.Order_num = (int)reader[0];
						temp.Add(tempCart);
					}
				}
			}
			catch (Exception) { }
			decimal running_total = 0m;

			foreach (Cart item in temp)
			{
				foreach (cartItem food in item.Items)
				{
					running_total += food.price;
				}
			}
			running_total = Math.Round(running_total,2);
			comp_item_price.Content = running_total.ToString();
		}

        private void cleanTable_Click(object sender, RoutedEventArgs e)
        {
            foreach (ClientInfo itr in TableList)
            {
                if (itr.IP == sender)
                {
                    itr.priorStatus = itr.status;
                    itr.status = "Table Needs Cleaning";
                    commHelper.functionSend("cleanRequest");
                    commHelper.objectSend(itr.IP.ToString());
                }
            }
            currentTableStatus.Content = "Table Needs Cleaning";
        }

        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            currentTableStatus.Content = "Open";
        }

        private void readingButton_Click(object sender, RoutedEventArgs e)
        {
            currentTableStatus.Content = "Reading Menu";
        }

        private void waitingButton_Click(object sender, RoutedEventArgs e)
        {
            currentTableStatus.Content = "Placed order and waiting on food";
        }

        private void eatingButton_Click(object sender, RoutedEventArgs e)
        {
            currentTableStatus.Content = "Eating";
        }

		public MColor ToMediaColor(DColor color)
		{
			return MColor.FromArgb(color.A, color.R, color.G, color.B);
		}


        /// <summary>
        /// PUll refill requests from the database 
        /// </summary>
        public void getDrinks(string clientIP)
        {
            refillBox.Items.Clear();
            foreach ( Refill thing in RefillList )
            {
                if (thing.ip == clientIP)
                {
                    refillBox.Items.Add(thing.drink);
                }
            }
        }

        private object ByteToObject(byte[] byteArray)
        {
            try
            {
                MemoryStream ms = new MemoryStream(byteArray);
                BinaryFormatter bf = new BinaryFormatter();
                ms.Position = 0;

                return bf.Deserialize(ms);
            }
            catch (Exception) { }

            return null;
        }

        private void tick_done_Click(object sender, RoutedEventArgs e)
        {
			List<Cart> temp = new List<Cart>();
			string ip = "";
			foreach (ClientInfo thing in TableList)
			{
				if (thing.Name == currentTableName.Content.ToString())
				{
					ip = thing.IP.ToString();
				}
			}
			try
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
                    SELECT * FROM dbo.Orders
					WHERE Client=@cli
                    ";
					command.Parameters.AddWithValue("@cli", ip);
					// Open a connection to database.
					connection.Open();
					// Read data returned for the query.
					SqlDataReader reader = command.ExecuteReader();

					// while not done reading the stuff returned from the query
					while (reader.Read())
					{
						Cart tempCart = new Cart();
						tempCart = (Cart)ByteToObject((byte[])reader[1]);
						tempCart.Order_num = (int)reader[0];
						temp.Add(tempCart);
					}
				}
			}
			catch (Exception) { }
			decimal running_total = 0m;

			foreach (Cart item in temp)
			{
				foreach (cartItem food in item.Items)
				{
					running_total += food.price;
				}
			}

			if (Convert.ToDecimal(comp.Text) > running_total)
			{
				MessageBox.Show("NO FREE FOOD");
			}
			else
			{
				commHelper.functionSend("ticketAdjusted");
				commHelper.functionSend(comp.Text);
				commHelper.functionSend(ip);
				tick_ad.Visibility = Visibility.Hidden;
			}
        }
    }
}

