using OMS_Library;
using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Interaction logic for EmployeeInterface.xaml
    /// </summary>
    public partial class EmployeeInterface : UserControl
    {
        List<ClientInfo> TableList = new List<ClientInfo>();

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
                    currentTableName.Content = ((Button)sender).Content;
                    currentTableStatus.Content = iter.status;
                    tableOptions.Visibility = Visibility.Visible;          
                };
                tmpButton.Height = 100;
                tmpButton.Width = 100;
				switch (iter.status)
				{
					case "Open":
						tmpButton.Background = new SolidColorBrush(ToMediaColor(DColor.Lime));
						break;
					case "Help Requested":
						tmpButton.Background = new SolidColorBrush(ToMediaColor(DColor.Red));
						break;
					case "Reading Menu":
					case "Placed order and waiting on food":
					case "Eating":
						tmpButton.Background = new SolidColorBrush(ToMediaColor(DColor.Yellow));
						break;
					default:
						break;
				}
                Table_Grid.Children.Add(tmpButton);
            }

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
        }

        private void createOrder_Click(object sender, RoutedEventArgs e)
        {
      
        }

        private void payWithCash_Click(object sender, RoutedEventArgs e)
        {
            currentTableStatus.Content = "Paid with Cash";
        }

        private void payWithCheck_Click(object sender, RoutedEventArgs e)
        {
            currentTableStatus.Content = "Paid with Check";
        }
    
        private void ticketAdjustment_Click(object sender, RoutedEventArgs e)
        {
        
        }

        private void cleanTable_Click(object sender, RoutedEventArgs e)
        {
      
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
	}
}

