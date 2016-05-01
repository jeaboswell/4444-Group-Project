﻿using System;
using System.Collections.Generic;
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

        string removeItem = "";
        string removeOrder = "";

        public Kitchen()
		{
			InitializeComponent();
            createMenu();
            createOrder();
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
                orderList.Items.Add(item.Order_num);
            }
        }

        public void refreshMenu()
        {
            menuList.Items.Clear();
            foreach(menuItem item in myMenu)
            {
                if(item.visible == false)
                {
                    menuList.Items.Add("(REMOVED)" + item.name);
                }

                else
                {
                    menuList.Items.Add(item.name);
                }
            }
        }

        public void createMenu()
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

        /// <summary>
        /// Update the database to make it work nicer, and change the 
        /// </summary>
        public void createOrder()
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
                    // create menu items from the database               
                    myOrders.Add(new Cart
                    {
                        //Items = (List<menuItem>)reader[0],
                        //Order_num = (int)reader[1],
                        //Notes = (List<string>)reader[2]
                    });
                }

                connection.Close();
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            orderList.Visibility = Visibility.Visible;
            menuButton.Visibility = Visibility.Visible;
            doneButton.Visibility = Visibility.Visible;
        }

        private void menuButton_Click(object sender, RoutedEventArgs e)
        {
            orderList.Visibility = Visibility.Hidden;
            menuButton.Visibility = Visibility.Hidden;
            doneButton.Visibility = Visibility.Hidden;
        }

        private void doneButton_Click(object sender, RoutedEventArgs e)
        {

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
            }
            catch { }
        }

        private void orderBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            removeOrder = orderList.SelectedValue.ToString();
        }
    }
}
