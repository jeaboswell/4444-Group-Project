using System;
using System.Collections.Generic;

namespace OMS_Library
{
	[Serializable]
	public class Cart
	{
		public List<cartItem> Items { get; set; }

		public int Order_num { get; set; }

		public List<string> Notes { get; set; }

		public Cart()
		{
			Items = new List<cartItem>();
			Notes = new List<string>();
		}
	}

	[Serializable]
	public class cartItem
	{
		public int itemNumber { get; set; }
		public string name { get; set; }
		public string description { get; set; }
		public decimal price { get; set; }          // It's very important that any number you store here must have an 'm' after it like so price = 69.69m  <------
		public string category { get; set; }
		public bool visible { get; set; }
	}
}
