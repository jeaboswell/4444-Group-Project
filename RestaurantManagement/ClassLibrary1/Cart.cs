using System;
using System.Collections.Generic;

namespace OMS_Library
{
	[Serializable]
	public class Cart
	{
		public List<menuItem> Items { get; set; }

		public int Order_num { get; set; }

		public List<string> Notes { get; set; }

		public Cart()
		{
			Items = new List<menuItem>();
			Notes = new List<string>();
		}
	}
}
