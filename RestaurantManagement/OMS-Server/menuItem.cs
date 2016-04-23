using System;
using System.Windows.Media;

namespace OMS
{
    [Serializable]
	public class menuItem
	{
		public int itemNumber { get; set; }
		public string name { get; set; }
		public string description { get; set; }
		public ImageSource imgSource { get; set; }
		public int price { get; set; }
		public string category { get; set; }
		public bool visible { get; set; }
	}
}
