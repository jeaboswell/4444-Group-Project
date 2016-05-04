using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
using System.Xml.Serialization;
using D = System.Data;            // System.Data.dll
using C = System.Data.SqlClient;  // System.Data.dll
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using OMS_Library;

namespace OMS
{
    /// <summary>
    /// Interaction logic for Kitchen.xaml
    /// </summary>
    public partial class Kitchen : UserControl
    {
        List<menuItem> myMenu = new List<menuItem>();
        List<Cart> myOrders = new List<Cart>();
        List<cartItem> items = new List<cartItem>();

        string removeItem = "";
        string removeOrder = "";
        string order = "";

        public Kitchen()
        {
            InitializeComponent();
            createMenu();
            createOrders();
            foreach (menuItem item in myMenu)
            {
                if (item.visible == false)
                {
                    menuList.Items.Add("(REMOVED)" + item.name);
                }
                else
                {
                    menuList.Items.Add(item.name);
                }
            }

            foreach (Cart item in myOrders)
            {
                foreach (cartItem food in item.Items)
                {
                    order += (food.name + ", ");
                }
                orderList.Items.Add(item.Order_num + " " + order);
            }
        }

        public void refreshMenu()
        {
            menuList.Items.Clear();
            foreach (menuItem item in myMenu)
            {
                if (item.visible == false)
                {
                    menuList.Items.Add("(REMOVED)" + item.name);
                }

                else
                {
                    menuList.Items.Add(item.name);
                }
            }
        }

        public void refreshOrders()
        {
            orderList.Items.Clear();
            foreach (Cart item in myOrders)
            {
                foreach (cartItem food in item.Items)
                {
                    order += (food.name + ", ");
                }
                orderList.Items.Add(item.Order_num + " " + order);
            }
        }

        public void createMenu()
        {
			try
			{
				string SQLConnectionString = "Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;";
				// Create an SqlConnection from the provided connection string.
				using (C.SqlConnection connection = new C.SqlConnection(SQLConnectionString))
				{
					// Formulate the command.
					C.SqlCommand command = new C.SqlCommand();
					command.Connection = connection;

					// Specify the query to be executed.
					command.CommandType = D.CommandType.Text;
					command.CommandText = @"
                    SELECT * FROM dbo.Menu
                    ";
					// Open a connection to database.
					connection.Open();
					// Read data returned for the query.
					C.SqlDataReader reader = command.ExecuteReader();

					// while not done reading the stuff returned from the query
					while (reader.Read())
					{
						// create menu items from the database               
						myMenu.Add(new menuItem
						{
							itemNumber = (int)reader[0],
							name = (string)reader[1],
							description = (string)reader[2],
							imgSource = null,
							visible = (bool)reader[5],
							price = (decimal)reader[3],
							category = (string)reader[7]
						});
					}
					connection.Close();
				}
			}
			catch (Exception) { }
        }

        /// <summary>
        /// Update the database to make it work nicer, and change the 
        /// </summary>
        public void createOrders()
        {
			try
			{
				string SQLConnectionString = "Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;";
				// Create an SqlConnection from the provided connection string.
				using (C.SqlConnection connection = new C.SqlConnection(SQLConnectionString))
				{
					// Formulate the command.
					C.SqlCommand command = new C.SqlCommand();
					command.Connection = connection;

					// Specify the query to be executed.
					command.CommandType = D.CommandType.Text;
					command.CommandText = @"
                    SELECT * FROM dbo.Orders
                    WHERE Finished=0
                    ";
					// Open a connection to database.
					connection.Open();
					// Read data returned for the query.
					C.SqlDataReader reader = command.ExecuteReader();

					// while not done reading the stuff returned from the query
					while (reader.Read())
					{
                        Cart tempCart = new Cart();
						tempCart.Order_num = (int)reader[0];
						myOrders.Add(tempCart);
					}

					connection.Close();
				}
			}
			catch (Exception) { }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            orderList.Visibility = Visibility.Visible;
            menuButton.Visibility = Visibility.Visible;
            doneButton.Visibility = Visibility.Visible;
            addButton.Visibility = Visibility.Hidden;
        }

        private void menuButton_Click(object sender, RoutedEventArgs e)
        {
            orderList.Visibility = Visibility.Hidden;
            menuButton.Visibility = Visibility.Hidden;
            doneButton.Visibility = Visibility.Hidden;
            addButton.Visibility = Visibility.Visible;
        }

        private void doneButton_Click(object sender, RoutedEventArgs e)
        {
            using (C.SqlConnection openCon = new C.SqlConnection("Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;"))
            {
                string command = "update dbo.Orders set Finished = @true where Id = @id";

                using (C.SqlCommand querySave = new C.SqlCommand(command, openCon))
                {
                    querySave.Parameters.AddWithValue("@true", BitConverter.GetBytes(true));
                    querySave.Parameters.AddWithValue("@id", Int32.Parse(removeOrder));

                    openCon.Open();
                    querySave.ExecuteScalar();
                    openCon.Close();
                }
            }
            myOrders.Clear();
            createOrders();
            refreshOrders();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            using (C.SqlConnection openCon = new C.SqlConnection("Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;"))
            {
                string command = "update dbo.Menu set Available = @true where Id = @id";

                using (C.SqlCommand querySave = new C.SqlCommand(command, openCon))
                {
                    querySave.Parameters.AddWithValue("@true", BitConverter.GetBytes(true));
                    querySave.Parameters.AddWithValue("@id", getID(removeItem));

                    openCon.Open();
                    querySave.ExecuteScalar();
                    openCon.Close();
                }
            }
            myMenu.Clear();
            createMenu();
            refreshMenu();
        }

        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            using (C.SqlConnection openCon = new C.SqlConnection("Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;"))
            {
                string command = "update dbo.Menu set Available = @false where Id = @id";

                using (C.SqlCommand querySave = new C.SqlCommand(command, openCon))
                {
                    querySave.Parameters.AddWithValue("@false", BitConverter.GetBytes(0));
                    querySave.Parameters.AddWithValue("@id", getID(removeItem));

                    openCon.Open();
                    querySave.ExecuteScalar();
                    openCon.Close();
                }
            }
            myMenu.Clear();
            createMenu();
            refreshMenu();
        }

        private int getID(string item)
        {
            foreach (menuItem iter in myMenu)
            {
                if (iter.name == item)
                    return iter.itemNumber;
            }
            return -1;
        }

        private void menuBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                removeItem = menuList.SelectedValue.ToString();
                if (removeItem.StartsWith("("))
                {
                    removeItem = removeItem.Substring(9);
                }
            }
            catch { }
        }

        private void orderBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                removeOrder = orderList.SelectedValue.ToString();
            }
            catch { }
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
	}
}
