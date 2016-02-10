﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
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
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class Client : Window
	{
		public Client()
		{
			InitializeComponent();
		}

		void Connect()
		{
			UdpClient server = new UdpClient();
			byte[] RequestData = Encoding.ASCII.GetBytes("OMS-Client");
			IPEndPoint ServerEp = new IPEndPoint(IPAddress.Any, 0);

			server.EnableBroadcast = true;
			server.Client.SendTimeout = 1000;
			server.Client.ReceiveTimeout = 1000;
			server.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, 44445));

			try
			{
				byte[] ServerResponseData = server.Receive(ref ServerEp);
				string ServerResponse = Encoding.ASCII.GetString(ServerResponseData);
				Console.WriteLine("Recived {0} from {1}", ServerResponse, ServerEp.Address.ToString());
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: No Server Found");	// For debugging
			}
			server.Close();
		}

		private void button_Click(object sender, RoutedEventArgs e)
		{
			Connect();
		}
	}
}