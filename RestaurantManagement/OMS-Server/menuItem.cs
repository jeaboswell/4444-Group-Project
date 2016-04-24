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
        public decimal price { get; set; }          // It's very important that any number you store here must have an 'm' after it like so price = 69.69m  <------
        public string category { get; set; }
		public bool visible { get; set; }
	}
}
