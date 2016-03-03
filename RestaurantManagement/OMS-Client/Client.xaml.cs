using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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
	public partial class Client : Window
	{
		Thread listener;
		private volatile bool stop;
		IPAddress serverIp;
		public Client()
		{
			InitializeComponent();
		}

		private void main_Loaded(object sender, RoutedEventArgs e)
		{
			Thread findServer = new Thread(Connect);
			findServer.IsBackground = true;
			findServer.Start();
		}

		private void main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			stop = true;
		}

		#region Server Communication
		void Connect()
		{
			UdpClient server = new UdpClient();
			byte[] RequestData = Encoding.ASCII.GetBytes("OMS-Client");
			IPEndPoint ServerEp = new IPEndPoint(IPAddress.Any, 0);

			server.EnableBroadcast = true;
			server.Client.SendTimeout = 1000;
			server.Client.ReceiveTimeout = 1000;

			bool success = false;
			while (!success)
			{
				server.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, 44445));
				try
				{
					byte[] ServerResponseData = server.Receive(ref ServerEp);
					string ServerResponse = Encoding.ASCII.GetString(ServerResponseData);
					Console.WriteLine("Recived {0} from {1}", ServerResponse, ServerEp.Address.ToString());
					serverIp = ServerEp.Address;
					success = true;
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error: No Server Found");    // For debugging
				}
			}
			server.Close();

			listener = new Thread(commandListener);
			listener.IsBackground = true;
			stop = false;
			listener.Start();
		}

		private void commandListener()
		{
			IPEndPoint serverEp = new IPEndPoint(serverIp, 0);
			UdpClient server = new UdpClient(44446);

			while (!stop)
			{
				try
				{
					byte[] command = server.Receive(ref serverEp);
					
					switch (Encoding.ASCII.GetString(command))
					{
						case "setPermission":
							command = server.Receive(ref serverEp);
							this.Dispatcher.Invoke((Action)(() =>
							{
								setPermission(Encoding.ASCII.GetString(command));
							}));
							break;
						default:
							break;
					}
				}
				catch (Exception ex) { }
			}
			server.Close();
		}
		#endregion

		#region Functions (Server)
		private void setPermission(string permission)
		{
			switch (permission)
			{
				case "Manager":
					permLabel.Content = permission;
					//tableGrid.Visibility = Visibility.Hidden;
					break;
				case "Server":
					permLabel.Content = permission;
					//tableGrid.Visibility = Visibility.Hidden;
					break;
				case "Kitchen":
					permLabel.Content = permission;
					//.Visibility = Visibility.Hidden;
					break;
				case "Reception":
					permLabel.Content = permission;
					//tableGrid.Visibility = Visibility.Hidden;
					break;
				case "Table":
					permLabel.Content = permission;
					//tableGrid.Visibility = Visibility.Visible;
					break;
				default:
					break;
			}
		}
		#endregion

		
	}
}