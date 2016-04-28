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

[Serializable]
class ClientInfo
{
	public IPAddress IP { get; set; }
	public string Name { get; set; }
	public List<string> permissionList { get; set; } = new List<string>() { "None", "Manager", "Waiter", "Kitchen", "Table" };
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
        private List<menuItem> myList = new List<menuItem>();
		public Thread listener;
		BackgroundWorker menu_load;
		public object selectedPermission { get; set; }

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
									//
									// Add code here to send help request to waiter interface
									//
								}
							}
							break;
						case "cancelHelp":
							foreach (ClientInfo iter in clientList.Items)
							{
								if (iter.selectedPermission == "Waiter")
								{
									//
									// Add code here to send cancel request to employee interface
									//
								}
							}
							break;
						case "getTables":
							IPEndPoint TempEp = new IPEndPoint(ClientEp.Address, 44446);
							// Generate table list
							List<ClientInfo> tableList = new List<ClientInfo>();
							byte[] prepData = Encoding.ASCII.GetBytes("receiveTables");
							client.Send(prepData, prepData.Length, TempEp);
							foreach (ClientInfo iter in clientList.Items)
							{
								if (iter.selectedPermission == "Table")
									tableList.Add(iter);
							}
							byte[] sendData = ObjectToByteArray(tableList);
							client.Send(sendData, sendData.Length, TempEp);
							break;
						default:
							break;
					}
				}
				catch (Exception) { }
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

					clientList.Items.Add(new ClientInfo { IP = client, Name = getClientName(client), selectedPermission = permission });
					syncClientAuto(new ClientInfo { IP = client, Name = getClientName(client), selectedPermission = permission });
				}
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
    }
}
