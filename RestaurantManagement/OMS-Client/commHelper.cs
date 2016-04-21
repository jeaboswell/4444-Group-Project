using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace OMS
{
	class commHelper
	{
		/// <summary>
		/// Sends a command to the server
		/// </summary>
		/// <param name="command"></param>
		public static void functionSend(string command)
		{
			try
			{
				IPAddress serverIp = IPAddress.Parse(Properties.Settings.Default.serverIP);
				IPEndPoint server = new IPEndPoint(serverIp, 44445);
				UdpClient connection = new UdpClient();

				byte[] sendCmd = Encoding.ASCII.GetBytes(command);

				connection.Send(sendCmd, sendCmd.Length, server);

				connection.Close();
			}
			catch (Exception ex) { }
		}
	}
}
