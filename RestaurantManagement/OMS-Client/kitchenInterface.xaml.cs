﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                menuList.Items.Add(item.name);
            }

            foreach(Cart item in myOrders)
            {
                orderList.Items.Add(item.Order_num);
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
                    WHERE Available=1
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
            string SQLConnectionString = "Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;";
            // Create an SqlConnection from the provided connection string.
            using (C.SqlConnection connection = new C.SqlConnection(SQLConnectionString))
            {
                try {
                    // Formulate the command.
                    C.SqlCommand command = new C.SqlCommand();
                    command.Connection = connection;

                    // Specify the query to be executed.
                    command.CommandType = D.CommandType.Text;
                    command.CommandText = @"UPDATE * FROM dbo.Menu SET Available = 0x00 WHERE ItemName = 'Sprite'";
                    // Open a connection to database.
                    connection.Open();
                    command.ExecuteScalar(); }
                catch{ };
            }
        }

        private void menuBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            removeItem = menuList.SelectedValue.ToString();
        }

        private void orderBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            removeOrder = orderList.SelectedValue.ToString();
        }
    }
}
