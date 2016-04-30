#region Usings
using OMS_Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
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
#endregion

namespace OMS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
	{
		#region Variables
		private volatile bool stop;
		private List<IPAddress> clients = new List<IPAddress>();
        private List<menuItem> myList = new List<menuItem>();
		public Thread listener;
		BackgroundWorker menu_load;
		#endregion
		/// <summary>
		/// Initializes application
		/// Loads menu
		/// Starts command listener
		/// </summary>
		public MainWindow()
        {
            InitializeComponent();
			
			menu_load= new BackgroundWorker();
			menu_load.DoWork += new DoWorkEventHandler(menu_load_DoWork);
			menu_load.RunWorkerAsync();
			listener = new Thread(commandListener); ;
			listener.IsBackground = true;
			listener.Start();
		}
		/// <summary>
		/// When application attempts to close verify no clients are connected.
		/// Request all threads stop when closing
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void main_Closing(object sender, CancelEventArgs e)
		{
			if (clientList.HasItems)
			{
				e.Cancel = true;
				MessageBox.Show("Unable to close server while clients are connected.");
			}
			requestStop();
		}

		#region Listener
		/// <summary>
		/// Listens to the network for requests from clients
		/// </summary>
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
					Console.WriteLine(Encoding.ASCII.GetString(clientRequest));
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
						case "requestHelp":
							foreach (ClientInfo iter in clientList.Items)
							{
								if (iter.selectedPermission == "Waiter")
								{
									sendCommand(ClientEp.Address, "requestHelp");
								}
							}
							break;
						case "cancelHelp":
							foreach (ClientInfo iter in clientList.Items)
							{
								if (iter.selectedPermission == "Waiter")
								{
									sendCommand(ClientEp.Address, "cancelHelp");
								}
							}
							break;
						case "getTables":
							sendTables(ClientEp.Address);
							break;
						default:
							break;
					}
				}
				catch (Exception) { }
			}
			client.Close();
		}
		/// <summary>
		/// Request threads stop
		/// </summary>
		public void requestStop()
		{
			stop = true;
		}
		/// <summary>
		/// Allow threads to run
		/// </summary>
		public void setStop()
		{
			stop = false;
		}
		#endregion

		#region Client Add/Remove
		/// <summary>
		/// Adds client to local clients list
		/// Syncs client with sotred permission
		/// Sends tables to waiter clients when new table connects
		/// Updates clientList interface element
		/// </summary>
		/// <param name="ip"></param>
		public void addClient(IPAddress ip)
		{
			if (clients == null || !clients.Exists(x => x.Equals(ip)))
				clients.Add(ip);
			syncClientAuto(new ClientInfo { IP = ip, Name = getClientName(ip), selectedPermission = getClientPermission(ip) });
			if (getClientPermission(ip) == "Table")
			{
				foreach (ClientInfo c in clientList.Items)
				{
					if (c.selectedPermission == "Waiter")
						sendTables(c.IP);
				}
			}
			updateClientList();
		}

		private void clientClosed(IPAddress ip)
		{
			clients.Remove(ip);
			updateClientList();
		}

		private void updateClientList()
		{
			Dispatcher.Invoke((Action)(() =>
			{
				clientList.Items.Clear();
				foreach (IPAddress client in clients)
				{
					clientList.Items.Add(new ClientInfo { IP = client, Name = getClientName(client), selectedPermission = getClientPermission(client) });
				}
			}));
		}

		private string getClientPermission(IPAddress client)
		{
			string permission = "None";
			// Check database for permission
			using (SqlConnection connection = new SqlConnection("Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;"))
			{
				// Create command
				var sql = String.Format("select * from dbo.Clients where IPAddress = '{0}' and Name = '{1}'", client.ToString(), getClientName(client));
				SqlCommand command = new SqlCommand(sql);
				command.Connection = connection;
				// Specify the query to be executed.
				command.CommandType = CommandType.Text;
				// Open a connection to database.
				connection.Open();
				// Read data returned for the query.
				SqlDataReader reader = command.ExecuteReader();

				// while not done reading the stuff returned from the query
				while (reader.Read())
				{
					permission = (string)reader[2];
				}
			}
			return permission;
		}

		private string getClientName(IPAddress ip)
		{
			string machineName = string.Empty;
			try
			{
				IPHostEntry hostEntry = Dns.GetHostEntry(ip);

				machineName = hostEntry.HostName;
			}
			catch (Exception) { }
			return machineName;
		}
		#endregion

		#region Client Communication
		public T GetAncestorOfType<T>(FrameworkElement child) where T : FrameworkElement
		{
			var parent = VisualTreeHelper.GetParent(child);
			if (parent != null && !(parent is T))
				return (T)GetAncestorOfType<T>((FrameworkElement)parent);
			return (T)parent;
		}

		private void syncClientAuto(ClientInfo client)
		{
			requestStop();
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

			// Send client info to database
			sendClient(client);
		}

		private void sendCommand(IPAddress clientIP, string command)
		{
			try
			{
				IPEndPoint client = new IPEndPoint(clientIP, 44446);
				UdpClient connection = new UdpClient();

				byte[] sendCmd = Encoding.ASCII.GetBytes(command);

				connection.Send(sendCmd, sendCmd.Length, client);

				connection.Close();
			}
			catch (Exception) { }
		}

		private void sendTables(IPAddress clientIP)
		{
			UdpClient client = new UdpClient();
			IPEndPoint ClientEp = new IPEndPoint(clientIP, 44446);
			// Generate table list
			List<ClientInfo> tableList = new List<ClientInfo>();

			byte[] prepData = Encoding.ASCII.GetBytes("receiveTables");
			client.Send(prepData, prepData.Length, ClientEp);
			//tableList.Add(new ClientInfo { IP = IPAddress.Parse("1.1.1.1"), Name = "Table 1", selectedPermission = "Table" });
			//tableList.Add(new ClientInfo { IP = IPAddress.Parse("1.1.1.2"), Name = "Table 2", selectedPermission = "Table" });
			//tableList.Add(new ClientInfo { IP = IPAddress.Parse("1.1.1.3"), Name = "Table 3", selectedPermission = "Table" });
			//tableList.Add(new ClientInfo { IP = IPAddress.Parse("1.1.1.4"), Name = "Table 4", selectedPermission = "Table" });

			foreach (ClientInfo iter in clientList.Items)
			{
				if (iter.selectedPermission == "Table")
					tableList.Add(iter);
			}

			byte[] sendData = ObjectToByteArray(tableList);
			client.Send(sendData, sendData.Length, ClientEp);
		}
		#endregion

		#region Database Functions
		private void sendClient(ClientInfo client)
		{
			if (clientInDB(client))
			{
				using (SqlConnection openCon = new SqlConnection("Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;"))
				{
					using (SqlCommand querySave = new SqlCommand("update dbo.Clients set Permission = @permission where IPAddress = @ip and Name = @name", openCon))
					{
						querySave.Parameters.AddWithValue("@ip", client.IP.ToString());
						querySave.Parameters.AddWithValue("@name", client.Name);
						querySave.Parameters.AddWithValue("@permission", client.selectedPermission);

						openCon.Open();
						querySave.ExecuteScalar();
						openCon.Close();
					}
				}
			}
			else
			{
				using (SqlConnection openCon = new SqlConnection("Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;"))
				{
					string command = "INSERT into dbo.Clients (IPAddress, Name, Permission) VALUES (@ip, @name, @permission)";

					using (SqlCommand querySave = new SqlCommand(command, openCon))
					{
						querySave.Parameters.Add("@ip", SqlDbType.NVarChar).Value = client.IP.ToString();
						querySave.Parameters.Add("@name", SqlDbType.NVarChar).Value = client.Name;
						querySave.Parameters.Add("@permission", SqlDbType.NVarChar).Value = client.selectedPermission;

						openCon.Open();
						querySave.ExecuteScalar();
						openCon.Close();
					}
				}
			}
		}

		private bool clientInDB(ClientInfo client)
		{
			using (SqlConnection openCon = new SqlConnection("Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;"))
			{
				SqlCommand myCommand = new SqlCommand("SELECT IPAddress FROM dbo.Clients WHERE IPAddress = @ip and Name = @name", openCon);
				SqlDataAdapter sqlDa = new SqlDataAdapter(myCommand);

				myCommand.Parameters.AddWithValue("@ip", client.IP.ToString());
				myCommand.Parameters.AddWithValue("@name", client.Name);
				openCon.Open();
				SqlDataReader reader = myCommand.ExecuteReader();
				bool hasRows = reader.HasRows;
				openCon.Close();
				return hasRows;
			}
		}
		#endregion

		private byte[] ObjectToByteArray(object obj)
		{
			if (obj == null)
				return null;
			BinaryFormatter bf = new BinaryFormatter();
			using (MemoryStream ms = new MemoryStream())
			{
				bf.Serialize(ms, obj);
				ms.Position = 0;
				List<ClientInfo> temp = (List<ClientInfo>)bf.Deserialize(ms);
				foreach (ClientInfo c in temp)
				{
					Console.WriteLine("IP: " + c.IP.ToString() + " Name: " + c.Name + "\n");
				}
				return ms.ToArray();
			}
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

        #region Menu
        private void menu_load_DoWork(object sender, DoWorkEventArgs e)
        {
            menuLoader();
        }

        private void menuLoader()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                if(myList.Count != 0)
                {
                    myList.Clear();
                }
                if(menuList.Items.Count != 0)
                {
                    menuList.Items.Clear();
                }
                using (SqlConnection connection = new SqlConnection("Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;"))
                {
                    // Formulate the command.
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;

                    // Specify the query to be executed.
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"
                                            SELECT * FROM dbo.Menu
                                            WHERE Available=1
                                            ";
                    // Open a connection to database.
                    connection.Open();
                    // Read data returned for the query.
                    SqlDataReader reader = command.ExecuteReader();

                    // while not done reading the stuff returned from the query
                    while (reader.Read())
                    {
                        menuItem temp = new menuItem
                        {
                            itemNumber = (int)reader[0],
                            name = (string)reader[1],
                            description = (string)reader[2],
                            price = (decimal)reader[3],
                            category = (string)reader[7]
                        };
                        myList.Add(temp);
                        menuList.Items.Add(temp.name);
                    }
                }

            }));
        }
        private void more_info_Click(object sender, RoutedEventArgs e)
        {
            if(menuList.SelectedIndex != -1)
            {
                int position = menuList.SelectedIndex;
                menuItem temp = myList[position];
                MessageBox.Show("Price: " + temp.price + "\nCategory: " + temp.category + "\nDescription: " + temp.description);
            }
        }
        #endregion

        private void updateMenu_Click(object sender, RoutedEventArgs e)
        {
            BackgroundWorker menu_REload;
            menu_REload = new BackgroundWorker();
            menu_REload.DoWork += new DoWorkEventHandler(menu_load_DoWork);
            menu_REload.RunWorkerAsync();
        }
	}
}
