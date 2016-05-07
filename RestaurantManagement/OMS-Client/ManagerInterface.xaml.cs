using OMS_Library;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {

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

        private void button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void button_Click_1(object sender, RoutedEventArgs e)
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
    }
}
