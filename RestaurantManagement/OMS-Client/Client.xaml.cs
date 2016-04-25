#region Usings
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Xml;
#endregion

namespace OMS
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class Client : Window
	{
		#region Variables
		Thread listener;
		private volatile bool stop;
		IPAddress serverIp;
		SqlConnection database;
		#endregion

		public Client()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Begins searching for the Server
		/// Loads previiously granted permission
		/// Connects to the database
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void main_Loaded(object sender, RoutedEventArgs e)
		{
			Thread findServer = new Thread(Connect);
			findServer.IsBackground = true;
			findServer.Start();
			setPermission(Properties.Settings.Default.savedPermission);
			dbConnect();
		}

		/// <summary>
		/// Creates a database connection
		/// </summary>
		private void dbConnect()
		{
			using (database = new SqlConnection(Properties.Settings.Default.DatabasesConnectionString))
			{
				database.Open();
			}
		}

		/// <summary>
		/// Notifies server tha that the client is closed
		/// Saves all settings
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			commHelper.functionSend("clientClosed");
			Properties.Settings.Default.Save();
			stop = true;
		}

		#region Server Communication
		/// <summary>
		/// Searches for and will creates connection to the server
		/// </summary>
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
				try
				{

					server.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, 44445));
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
			Properties.Settings.Default.serverIP = serverIp.ToString();
		}

		/// <summary>
		/// Listens for commands from the server
		/// </summary>
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

		#region Functions (From Server)
		/// <summary>
		/// Sets the permission level of the client
		/// </summary>
		/// <param name="permission"></param>
		private void setPermission(string permission)
		{
			Properties.Settings.Default.savedPermission = permission;
			switch (permission)
			{
				case "None":
					permLabel.Content = "Waiting on server...";
					tableUI.Visibility = Visibility.Hidden;
					kitchenUI.Visibility = Visibility.Hidden;
					break;
				case "Manager":
					permLabel.Content = permission;
					tableUI.Visibility = Visibility.Hidden;
					kitchenUI.Visibility = Visibility.Hidden;
					break;
				case "Waiter":
					permLabel.Content = permission;
					tableUI.Visibility = Visibility.Hidden;
					kitchenUI.Visibility = Visibility.Hidden;
					break;
				case "Kitchen":
					permLabel.Content = permission;
					tableUI.Visibility = Visibility.Hidden;
					kitchenUI.Visibility = Visibility.Visible;
					break;
				case "Table":
					permLabel.Content = permission;
					tableUI.Visibility = Visibility.Visible;
					kitchenUI.Visibility = Visibility.Hidden;
					break;
				default:
					break;
			}
		}
		#endregion

		private void main_ContentRendered(object sender, EventArgs e)
		{
			tableUI.createMenu();
		}
	}
}