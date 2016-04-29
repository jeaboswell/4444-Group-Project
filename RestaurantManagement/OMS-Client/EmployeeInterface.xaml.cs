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
using OMS_Library;

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
			Dispatcher.Invoke((Action)(() =>
			{
				updateTables();
			}));
		}

		private void updateTables()
        {
            foreach (ClientInfo iter in TableList)
            {
                Button tmpButton = new Button();
                tmpButton.Content = iter.Name;
                tmpButton.Click += (s, e) => { /*Add code here to determine what happens when table button is clicked*/};
                tmpButton.Height = 100;
                tmpButton.Width = 100;
                Table_Grid.Children.Add(tmpButton);
            }

        }
	}
}

