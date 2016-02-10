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

class Client
{
	public IPAddress IP { get; set; }

	public List<string> permissionList { get; set; } = new List<string>() { "Manager", "Server", "Kitchen", "Table" }; 
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

		public MainWindow()
        {
            InitializeComponent();

			Thread temp = new Thread(createListener);
			listener = temp;
			listener.IsBackground = true;
		}

		public void addClient(IPAddress ip)
		{
			if (clients == null || !clients.Exists(x => x == ip))
				clients.Add(ip);
			updateClientList();
		}

		private void updateClientList()
		{
			this.Dispatcher.Invoke((Action)(() =>
			{
				clientList.Items.Clear();
				foreach (IPAddress client in clients)
					clientList.Items.Add(new Client { IP = client });
			}));
			
		}

		private void button_Click(object sender, RoutedEventArgs e)
        {
			//createListener();
			if (!listener.IsAlive)
			{
				setStop();
				listener.Start();
				button.Content = "Stop Listening";
			}
			else 
			{
				requestStop();
				button.Content = "Start Listening";
			}
        }

		#region Listener
		string output = "";

		public void createListener()
		{
			// Create an instance of the TcpListener class.
			TcpListener tcpListener = null;
			IPAddress ipAddress = Dns.GetHostEntry("localhost").AddressList[0];

			IPHostEntry hostT;
			IPAddress localIP = null;
			hostT = Dns.GetHostEntry(Dns.GetHostName());
			foreach (IPAddress ip in hostT.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					localIP = ip;
				}
			}

			try
			{
				// Set the listener on the local IP address 
				// and specify the port.
				tcpListener = new TcpListener(localIP, 44445);
				tcpListener.Start();
				output = "Waiting for a connection...";
			}
			catch (Exception e)
			{
				output = "Error: " + e.ToString();
				MessageBox.Show(output);
			}
			while (!stop)
			{
				// Always use a Sleep call in a while(true) loop 
				// to avoid locking up your CPU.
				Thread.Sleep(10);
				// Create a TCP socket. 
				// If you ran this server on the desktop, you could use 
				// Socket socket = tcpListener.AcceptSocket() 
				// for greater flexibility.
				TcpClient tcpClient = tcpListener.AcceptTcpClient();

				IPEndPoint remoteIpEndPoint = tcpClient.Client.RemoteEndPoint as IPEndPoint;
				addClient(remoteIpEndPoint.Address);

				// Read the data stream from the client. 
				byte[] bytes = new byte[256];
				NetworkStream stream = tcpClient.GetStream();
				stream.Read(bytes, 0, bytes.Length);
				SocketHelper helper = new SocketHelper();
				helper.processMsg(tcpClient, stream, bytes);
				string mstrMessage = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
				MessageBox.Show(mstrMessage);
			}
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
	}
}
