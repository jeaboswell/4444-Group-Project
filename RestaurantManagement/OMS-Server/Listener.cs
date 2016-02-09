using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace OMS
{
	class Listener
	{
		static string output = "";

		static public void createListener()
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
				MainWindow.addClient(remoteIpEndPoint.Address);

				// Read the data stream from the client. 
				byte[] bytes = new byte[256];
				NetworkStream stream = tcpClient.GetStream();
				stream.Read(bytes, 0, bytes.Length);
				//SocketHelper helper = new SocketHelper();
				//helper.processMsg(tcpClient, stream, bytes);
				string mstrMessage = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
				//MessageBox.Show(mstrMessage);
			}
		}

		static public void requestStop()
		{
			stop = true;
		}

		static public void setStop()
		{
			stop = false;
		}

		private static volatile bool stop;
	}
}
