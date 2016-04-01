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

class ClientInfo
{
	public IPAddress IP { get; set; }
	public string Name { get; set; }
	public List<string> permissionList { get; set; } = new List<string>() { "None", "Manager", "Server", "Kitchen", "Table" };
	public string selectedPermission { get; set; }
}

namespace OMS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
	{
		private List<IPAddress> clients = new List<IPAddress>();
		public Thread listener;
		public object selectedPermission { get; set; }

		public MainWindow()
        {
            InitializeComponent();

			Thread temp = new Thread(commandListener);
			listener = temp;
			listener.IsBackground = true;
			listener.Start();
		}

		#region Listener
		private void commandListener()
		{
			UdpClient client = new UdpClient(44445);
			byte[] ResponseData = Encoding.ASCII.GetBytes("OMS-Server");

			while (!stop)
			{
				IPEndPoint ClientEp = new IPEndPoint(IPAddress.Any, 44446);
				try
				{
					byte[] clientRequest = client.Receive(ref ClientEp);

					switch (Encoding.ASCII.GetString(clientRequest))
					{
						case "OMS-Client":
							addClient(ClientEp.Address);
							Console.WriteLine("Recived {0} from {1}, sending response", clientRequest, ClientEp.Address.ToString());
							client.Send(ResponseData, ResponseData.Length, ClientEp);
							break;
						case "clientClosed":
							clientClosed(ClientEp.Address);
							break;
						default:
							break;
					}
				}
				catch (Exception ex) { }
			}
			client.Close();
		}

		public void requestStop()
		{
			stop = true;
		}

		public void setStop()
		{
			stop = false;
		}

		private volatile bool stop;
		#endregion

		#region Client Add/Remove
		public void addClient(IPAddress ip)
		{
			if (clients == null || !clients.Exists(x => x.Equals(ip)))
				clients.Add(ip);
			updateClientList();
		}

		private void clientClosed(IPAddress ip)
		{
			clients.Remove(ip);
			updateClientList();
		}

		private void updateClientList()
		{
			this.Dispatcher.Invoke((Action)(() =>
			{
				clientList.Items.Clear();
				foreach (IPAddress client in clients)
					clientList.Items.Add(new ClientInfo { IP = client, Name = getClientName(client) });
			}));

		}

		private string getClientName(IPAddress ip)
		{
			string machineName = string.Empty;
			try
			{
				IPHostEntry hostEntry = Dns.GetHostEntry(ip);

				machineName = hostEntry.HostName;
			}
			catch (Exception ex) { }
			return machineName;
		}
		#endregion

		#region Grant Permissions
		public T GetAncestorOfType<T>(FrameworkElement child) where T : FrameworkElement
		{
			var parent = VisualTreeHelper.GetParent(child);
			if (parent != null && !(parent is T))
				return (T)GetAncestorOfType<T>((FrameworkElement)parent);
			return (T)parent;
		}

		private void syncClient_Click(object sender, RoutedEventArgs e)
		{
			requestStop();
			ClientInfo client = (ClientInfo)GetAncestorOfType<ListViewItem>(sender as Button).Content;
			if (client.selectedPermission == null)
				return;
			IPEndPoint clientIP = new IPEndPoint(client.IP, 44446);
			UdpClient connection = new UdpClient();

			string command = "setPermission";
			byte[] sendCmd = Encoding.ASCII.GetBytes(command);

			connection.Send(sendCmd, sendCmd.Length, clientIP);

			command = client.selectedPermission;
			sendCmd = Encoding.ASCII.GetBytes(command);

			connection.Send(sendCmd, sendCmd.Length, clientIP);
			
			connection.Close();
			setStop();
		}
		#endregion
	}
}
