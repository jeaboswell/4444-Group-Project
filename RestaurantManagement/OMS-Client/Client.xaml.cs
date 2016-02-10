using System;
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
			TcpClient server = new TcpClient();
			int serverPort = 44445;

			foreach (NetworkInterface netwIntrf in NetworkInterface.GetAllNetworkInterfaces())
			{
				Console.WriteLine("Inteface working: {0}", netwIntrf.OperationalStatus == OperationalStatus.Up);

				// If the current interface doesn't have an IP, skip it
				if (!(netwIntrf.GetIPProperties().GatewayAddresses.Count > 0))
				{
					continue;
				}

				// Get current IP Address(es)
				foreach (UnicastIPAddressInformation currentIpInfo in netwIntrf.GetIPProperties().UnicastAddresses)
				{
					if (currentIpInfo.Address.AddressFamily == AddressFamily.InterNetwork)
					{
						// Get the subnet mask and the IP address as bytes
						byte[] subnetMask = currentIpInfo.IPv4Mask.GetAddressBytes();
						byte[] ipAddr = currentIpInfo.Address.GetAddressBytes();

						// Reverse the byte-array if we are dealing with little endian.
						if (BitConverter.IsLittleEndian)
						{
							Array.Reverse(subnetMask);
							Array.Reverse(ipAddr);
						}

						// Convert the subnet mask to uint
						uint maskAsInt = BitConverter.ToUInt32(subnetMask, 0);

						// Convert the ip address to uint
						uint ipAsInt = BitConverter.ToUInt32(ipAddr, 0);

						// Use subnet to determine the maximum number of hosts possible in this subnet
						uint validHostsEndingMax = ~BitConverter.ToUInt32(subnetMask, 0);

						// Convert the start of the ip address to uint 
						uint validHostsStart = BitConverter.ToUInt32(ipAddr, 0) & BitConverter.ToUInt32(subnetMask, 0);

						// Increment the startIp to the number of maximum valid hosts in this subnet 
						// and check each to see if the intended port is listening
						for (uint i = 1; i <= validHostsEndingMax; i++)
						{
							uint host = validHostsStart + i;
							byte[] hostBytes = BitConverter.GetBytes(host);
							if (BitConverter.IsLittleEndian)
							{
								Array.Reverse(hostBytes);
							}

							// Convert current IP candidate to string 
							String ipCandidate = Convert.ToString(hostBytes[0]) + "." + Convert.ToString(hostBytes[1]) + "." + Convert.ToString(hostBytes[2]) + "." + Convert.ToString(hostBytes[3]);

							// For debug purposes only
							textBox.Text += "Trying: " + ipCandidate + "\n";
							Console.WriteLine("Trying: " + ipCandidate);

							Ping pingSender = new Ping();
							PingReply reply = pingSender.Send(ipCandidate);

							if (reply.Status == IPStatus.Success)
							{
								try
								{
									// Ty to connect to current candidate
									server.Connect(ipCandidate, serverPort);
									if (server.Connected == true)  // if succesful => something is listening on this port
									{
										NetworkStream stream = server.GetStream();
										byte[] greeting = Encoding.ASCII.GetBytes("Permission Request");
										byte[] response = new byte[256];
										stream.Write(greeting, 0, greeting.Length);
										int bytes = stream.Read(response, 0, response.Length);

										// For debug purposes only
										MessageBox.Show("Response: " + Encoding.ASCII.GetString(response, 0, bytes));
										textBox.Text += "\tIt worked at " + ipCandidate + "\n";

										server.Close();
										return;
									}
									//else goes to exception
								}
								catch (Exception ex)
								{
									// For debug purposes only
									textBox.Text += "\tDIDN'T work at " + ipCandidate + "\n";
								}
							}
						}
					}
				}
				Console.ReadLine();
			}
			server.Close();
		}

		private void button_Click(object sender, RoutedEventArgs e)
		{
			Connect();
		}
	}
}