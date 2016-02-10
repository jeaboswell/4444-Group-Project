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

        void Connect(string serverIP, string message)
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            int wantedPort = 44445;    //this is the port you want

            byte[] msg = Encoding.ASCII.GetBytes("Are you my server?");


            foreach (NetworkInterface netwIntrf in NetworkInterface.GetAllNetworkInterfaces())
            {
                textBox.Text += "Interface name: " + netwIntrf.Name + "\n";
                // Console.WriteLine("Interface name: " + netwIntrf.Name);

                //textBox.Text += ("Inteface working: {0}", netwIntrf.OperationalStatus == OperationalStatus.Up);
                Console.WriteLine("Inteface working: {0}", netwIntrf.OperationalStatus == OperationalStatus.Up);

                //if the current interface doesn't have an IP, skip it
                if (!(netwIntrf.GetIPProperties().GatewayAddresses.Count > 0))
                {
                    break;
                }

                //Console.WriteLine("IP Address(es):");

                //get current IP Address(es)
                foreach (UnicastIPAddressInformation currentIpInfo in netwIntrf.GetIPProperties().UnicastAddresses)
                {
                    if (currentIpInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        //get the subnet mask and the IP address as bytes
                        byte[] subnetMask = currentIpInfo.IPv4Mask.GetAddressBytes();
                        byte[] ipAddr = currentIpInfo.Address.GetAddressBytes();//localIP.GetAddressBytes(); ; //uniIpInfo.Address.GetAddressBytes();

                        // we reverse the byte-array if we are dealing with littl endian.
                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(subnetMask);
                            Array.Reverse(ipAddr);
                        }

                        //we convert the subnet mask as uint (just for didactic purposes (to check everything is ok now and next - use thecalculator in programmer mode)
                        uint maskAsInt = BitConverter.ToUInt32(subnetMask, 0);
                        //Console.WriteLine("\t subnet={0}", Convert.ToString(maskAsInt, 2));

                        //we convert the ip addres as uint (just for didactic purposes (to check everything is ok now and next - use thecalculator in programmer mode)
                        uint ipAsInt = BitConverter.ToUInt32(ipAddr, 0);
                        //Console.WriteLine("\t ip={0}", Convert.ToString(ipAsInt, 2));

                        //we negate the subnet to determine the maximum number of host possible in this subnet
                        uint validHostsEndingMax = ~BitConverter.ToUInt32(subnetMask, 0);
                        //Console.WriteLine("\t !subnet={0}", Convert.ToString(validHostsEndingMax, 2));

                        //we convert the start of the ip addres as uint (the part that is fixed wrt the subnet mask - from here we calculate each new address by incrementing with 1 and converting to byte[] afterwards 
                        uint validHostsStart = BitConverter.ToUInt32(ipAddr, 0) & BitConverter.ToUInt32(subnetMask, 0);
                        //Console.WriteLine("\t IP & subnet={0}", Convert.ToString(validHostsStart, 2));

                        //we increment the startIp to the number of maximum valid hosts in this subnet and for each we check the intended port (refactoring needed)
                        for (uint i = 1; i <= validHostsEndingMax; i++)
                        {
                            uint host = validHostsStart + i;
                            //byte[] hostAsBytes = BitConverter.GetBytes(host);
                            byte[] hostBytes = BitConverter.GetBytes(host);
                            if (BitConverter.IsLittleEndian)
                            {
                                Array.Reverse(hostBytes);
                            }

                            //this is the candidate IP address in "readable format" 
                            String ipCandidate = Convert.ToString(hostBytes[0]) + "." + Convert.ToString(hostBytes[1]) + "." + Convert.ToString(hostBytes[2]) + "." + Convert.ToString(hostBytes[3]);
                            Console.WriteLine("Trying: " + ipCandidate);
                            textBox.Text += "Trying: " + ipCandidate + "\n";

                            try
                            {
                                //try to connect
                                sock.Connect(ipCandidate, wantedPort);
                                if (sock.Connected == true)  // if succesful => something is listening on this port
                                {

                                    textBox.Text += "\tIt worked at " + ipCandidate + "\n";
                                    //Console.WriteLine("\tIt worked at " + ipCandidate);
                                    sock.Close();
                                    sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                }
                                //else -. goes to exception
                            }
                            catch (SocketException ex)
                            {
                                //TODO: if you want, do smth here
                                textBox.Text += "\tDIDN'T work at " + ipCandidate + "\n";
                                //Console.WriteLine("\tDIDN'T work at " + ipCandidate);
                            }
                        }
                    }
                }
                Console.ReadLine();
            }
            sock.Close();
        }









		void NewConnect()
		{
			TcpClient server = new TcpClient(); //Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			int wantedPort = 44445;    //this is the port you want

			byte[] msg = Encoding.ASCII.GetBytes("Are you my server?");


			foreach (NetworkInterface netwIntrf in NetworkInterface.GetAllNetworkInterfaces())
			{
				textBox.Text += "Interface name: " + netwIntrf.Name + "\n";
				// Console.WriteLine("Interface name: " + netwIntrf.Name);

				//textBox.Text += ("Inteface working: {0}", netwIntrf.OperationalStatus == OperationalStatus.Up);
				Console.WriteLine("Inteface working: {0}", netwIntrf.OperationalStatus == OperationalStatus.Up);

				//if the current interface doesn't have an IP, skip it
				if (!(netwIntrf.GetIPProperties().GatewayAddresses.Count > 0))
				{
					break;
				}

				//Console.WriteLine("IP Address(es):");

				//get current IP Address(es)
				foreach (UnicastIPAddressInformation currentIpInfo in netwIntrf.GetIPProperties().UnicastAddresses)
				{
					if (currentIpInfo.Address.AddressFamily == AddressFamily.InterNetwork)
					{
						//get the subnet mask and the IP address as bytes
						byte[] subnetMask = currentIpInfo.IPv4Mask.GetAddressBytes();
						byte[] ipAddr = currentIpInfo.Address.GetAddressBytes();//localIP.GetAddressBytes(); ; //uniIpInfo.Address.GetAddressBytes();

						// we reverse the byte-array if we are dealing with littl endian.
						if (BitConverter.IsLittleEndian)
						{
							Array.Reverse(subnetMask);
							Array.Reverse(ipAddr);
						}

						//we convert the subnet mask as uint (just for didactic purposes (to check everything is ok now and next - use thecalculator in programmer mode)
						uint maskAsInt = BitConverter.ToUInt32(subnetMask, 0);
						//Console.WriteLine("\t subnet={0}", Convert.ToString(maskAsInt, 2));

						//we convert the ip addres as uint (just for didactic purposes (to check everything is ok now and next - use thecalculator in programmer mode)
						uint ipAsInt = BitConverter.ToUInt32(ipAddr, 0);
						//Console.WriteLine("\t ip={0}", Convert.ToString(ipAsInt, 2));

						//we negate the subnet to determine the maximum number of host possible in this subnet
						uint validHostsEndingMax = ~BitConverter.ToUInt32(subnetMask, 0);
						//Console.WriteLine("\t !subnet={0}", Convert.ToString(validHostsEndingMax, 2));

						//we convert the start of the ip addres as uint (the part that is fixed wrt the subnet mask - from here we calculate each new address by incrementing with 1 and converting to byte[] afterwards 
						uint validHostsStart = BitConverter.ToUInt32(ipAddr, 0) & BitConverter.ToUInt32(subnetMask, 0);
						//Console.WriteLine("\t IP & subnet={0}", Convert.ToString(validHostsStart, 2));

						//we increment the startIp to the number of maximum valid hosts in this subnet and for each we check the intended port (refactoring needed)
						for (uint i = 1; i <= validHostsEndingMax; i++)
						{
							uint host = validHostsStart + i;
							//byte[] hostAsBytes = BitConverter.GetBytes(host);
							byte[] hostBytes = BitConverter.GetBytes(host);
							if (BitConverter.IsLittleEndian)
							{
								Array.Reverse(hostBytes);
							}

							//this is the candidate IP address in "readable format" 
							String ipCandidate = Convert.ToString(hostBytes[0]) + "." + Convert.ToString(hostBytes[1]) + "." + Convert.ToString(hostBytes[2]) + "." + Convert.ToString(hostBytes[3]);
							Console.WriteLine("Trying: " + ipCandidate);
							textBox.Text += "Trying: " + ipCandidate + "\n";

							try
							{
								//try to connect
								server.Connect(ipCandidate, wantedPort);
								if (server.Connected == true)  // if succesful => something is listening on this port
								{
									NetworkStream stream = server.GetStream();
									byte[] greeting = Encoding.ASCII.GetBytes("Permission Request");
									byte[] response = new byte[256];
									stream.Write(greeting, 0, greeting.Length);
									int bytes = stream.Read(response, 0, response.Length);
									MessageBox.Show("Response: " + Encoding.ASCII.GetString(response, 0, bytes));

									textBox.Text += "\tIt worked at " + ipCandidate + "\n";
									//Console.WriteLine("\tIt worked at " + ipCandidate);
									server.Close();
									return;
									//sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
								}
								//else -. goes to exception
							}
							catch (SocketException ex)
							{
								//TODO: if you want, do smth here
								textBox.Text += "\tDIDN'T work at " + ipCandidate + "\n";
								//Console.WriteLine("\tDIDN'T work at " + ipCandidate);
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
            
            // In this code example, use a hard-coded 
            // IP address and message. 
            string serverIP = "localhost";
            string message = "Hello";
			//Connect(serverIP, message);
			NewConnect();
            /*
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

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
                //try to connect
                sock.Connect(localIP, 4444);
                if (sock.Connected == true)  // if succesful => something is listening on this port
                {
                    textBox.Text += "\tIt worked at " + localIP + "\n";
                    //Console.WriteLine("\tIt worked at " + ipCandidate);
                    sock.Close();
                    sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }
                //else -. goes to exception
            }
            catch (SocketException ex)
            {
                //TODO: if you want, do smth here
                textBox.Text += "\tDIDN'T work at " + localIP + "\n";
                //Console.WriteLine("\tDIDN'T work at " + ipCandidate);
            }
			*/
        }
    }
}
