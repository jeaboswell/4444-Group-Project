using System;
using System.Linq;

namespace OMS
{
	class coupon
	{
		public string code;
		public DateTime expiration;

		public void generateCoupon()
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			var random = new Random();
			code =  new string(Enumerable.Repeat(chars, 5).Select(s => s[random.Next(s.Length)]).ToArray());
			expiration = DateTime.Now.AddMonths(3);
		}
	}
}
