#region Usings
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
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

using OMS_Library;
using System.Reflection;
using System.Windows.Threading;
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
		BackgroundWorker menuLoader;
		private volatile bool stop;
		IPAddress serverIp;
		#endregion

		public Client()
		{
			//
			// Include OMS-Library.dll
			//
			AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
			{
				string resourceName = new AssemblyName(args.Name).Name + ".dll";
				string resource = Array.Find(GetType().Assembly.GetManifestResourceNames(), element => element.EndsWith(resourceName));

				using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
				{
					byte[] assemblyData = new byte[stream.Length];
					stream.Read(assemblyData, 0, assemblyData.Length);
					return Assembly.Load(assemblyData);
				}
			};

			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
			//
			// Initialize
			//
			InitializeComponent();
			Properties.Settings.Default.localIP = GetLocalIPAddress();
		}
		/// <summary>
		/// Links all embedded assemblies
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			return EmbeddedAssembly.Get(args.Name);
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
			menuLoader = new BackgroundWorker();
			menuLoader.DoWork += new DoWorkEventHandler(menuLoader_DoWork);
			menuLoader.RunWorkerAsync();
			
			setPermission(Properties.Settings.Default.savedPermission);
		}

		private void menuLoader_DoWork(object sender, DoWorkEventArgs e)
		{
			tableUI.createMenu();
			kitchenUI.createMenu();
			kitchenUI.createOrders();
			Connect();
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
				catch (Exception)
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
					Console.WriteLine(Encoding.ASCII.GetString(command));
					switch (Encoding.ASCII.GetString(command))
					{
						case "setPermission":
							command = server.Receive(ref serverEp);
							Dispatcher.Invoke(() =>
							{
								setPermission(Encoding.ASCII.GetString(command));
							});
							break;
						case "receiveTables":
							command = server.Receive(ref serverEp);
							employeeUI.getTableList((List<ClientInfo>)ByteToObject(command));
							break;
						case "requestHelp":
							command = server.Receive(ref serverEp);
							employeeUI.requestHelp(IPAddress.Parse(Encoding.ASCII.GetString(command)));
							break;
						case "cancelHelp":
							command = server.Receive(ref serverEp);
							employeeUI.cancelHelp(IPAddress.Parse(Encoding.ASCII.GetString(command)));
							break;
                        case "updateOrders":
                            kitchenUI.createOrders();
                            break;
                        case "updateRefills":
                            employeeUI.updateRefills();
                            break;
						case "paid":
							Dispatcher.Invoke(() =>
							{
								tableUI.markPaid();
							});
							break;
						case "ticketAdjusted":
							command = server.Receive(ref serverEp);
							decimal price = Convert.ToDecimal(Encoding.ASCII.GetString(command));
							price = Math.Round(price, 2);
							tableUI.setAdjustment(price);
							break;
						case "close":
                            Dispatcher.Invoke(() =>
                            {
								Close();
                            });
                            break;
                        default:
							break;
					}
				}
				catch (Exception) { }
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
					employeeUI.Visibility = Visibility.Hidden;
                    managerUI.Visibility = Visibility.Hidden;
					break;
				case "Manager":
					permLabel.Content = permission;
					tableUI.Visibility = Visibility.Hidden;
					kitchenUI.Visibility = Visibility.Hidden;
					employeeUI.Visibility = Visibility.Hidden;
                    managerUI.Visibility = Visibility.Visible;
                    break;
				case "Waiter":
					permLabel.Content = permission;
					tableUI.Visibility = Visibility.Hidden;
					kitchenUI.Visibility = Visibility.Hidden;
					employeeUI.Visibility = Visibility.Visible;
					commHelper.functionSend("getTables");
					break;
				case "Kitchen":
					permLabel.Content = permission;
					tableUI.Visibility = Visibility.Hidden;
					kitchenUI.Visibility = Visibility.Visible;
					employeeUI.Visibility = Visibility.Hidden;
                    managerUI.Visibility = Visibility.Hidden;
                    break;
				case "Table":
					permLabel.Content = permission;
					tableUI.Visibility = Visibility.Visible;
					kitchenUI.Visibility = Visibility.Hidden;
					employeeUI.Visibility = Visibility.Hidden;
                    managerUI.Visibility = Visibility.Hidden;
                    break;
				default:
					break;
			}
		}
		#endregion

		private void main_ContentRendered(object sender, EventArgs e)
		{
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

		public static string GetLocalIPAddress()
		{
			var host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (var ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					return ip.ToString();
				}
			}
			return "0";
		}
	}
}