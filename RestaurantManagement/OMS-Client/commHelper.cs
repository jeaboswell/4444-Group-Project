using OMS_Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
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
			catch (Exception) { }
		}

		public static void objectSend(object obj)
		{
			try
			{
				IPAddress serverIp = IPAddress.Parse(Properties.Settings.Default.serverIP);
				IPEndPoint server = new IPEndPoint(serverIp, 44445);
				UdpClient connection = new UdpClient();

				byte[] sendObj = ObjectToByteArray(obj);

				connection.Send(sendObj, sendObj.Length, server);

				connection.Close();
			}
			catch (Exception) { }
		}

		private static byte[] ObjectToByteArray(object obj)
		{
			if (obj == null)
				return null;
			BinaryFormatter bf = new BinaryFormatter();
			using (MemoryStream ms = new MemoryStream())
			{
				bf.Serialize(ms, obj);
				ms.Position = 0;
				return ms.ToArray();
			}
		}

		private static object ByteToObject(byte[] byteArray)
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
	}
}
