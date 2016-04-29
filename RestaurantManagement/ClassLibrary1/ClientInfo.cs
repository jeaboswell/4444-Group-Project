using System;
using System.Collections.Generic;
using System.Net;

namespace OMS_Library
{
	[Serializable]
    public class ClientInfo
    {
		public IPAddress IP { get; set; }
		public string Name { get; set; }
		public List<string> permissionList { get; set; } = new List<string>() { "None", "Manager", "Waiter", "Kitchen", "Table" };
		public string selectedPermission { get; set; }
	}
}
