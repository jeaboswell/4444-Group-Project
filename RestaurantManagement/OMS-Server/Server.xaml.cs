using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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

namespace OMS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
	{
		private static List<IPAddress> clients = new List<IPAddress>();

		public MainWindow()
        {
            InitializeComponent();
		}

		public static void addClient(IPAddress ip)
		{
			if (clients == null || !clients.Exists(x => x == ip))
				clients.Add(ip);
		}

		Thread listner = new Thread(Listener.createListener);

		private void button_Click(object sender, RoutedEventArgs e)
        {
			//createListener();
			if (!listner.IsAlive)
			{
				Listener.setStop();
				listner.Start();
				button.Content = "Stop Listening";
			}
			else 
			{
				Listener.requestStop();
				button.Content = "Start Listening";
			}
        }
    }
}
