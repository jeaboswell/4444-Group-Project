using System;

namespace OMS
{
	[Serializable]
	public class rewardMember
	{
		public string firstName { get; set; }
		public string lastName { get; set; }
		public DateTime birthDate { get; set; } // setBirthDate()
		public string phoneNumber { get; set; } // setPhoneNumber()
		public string email { get; set; }
		public int points { get; set; }

		public void setBirthDate(int month, int day, int year)
		{
			birthDate = birthDate.AddMonths(month);
			birthDate = birthDate.AddDays(day);
			birthDate = birthDate.AddYears(year);
		}

		public void setPhoneNumber(string pass)
		{
			phoneNumber += "(" + pass[0] + pass[1] + pass[2] + ") " + pass[3] + pass[4] + pass[5] + "-" + pass[6] + pass[7] + pass[8] + pass[9];
		}
	}
}
