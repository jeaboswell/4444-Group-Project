using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OMS
{
	/// <summary>
	/// Interaction logic for Keypad.xaml
	/// </summary>
	public partial class Keypad : UserControl
	{
		#region Variables/Declarations
		public event PropertyChangedEventHandler PropertyChanged;
		private string _result = "";
		#endregion

		public string Result
		{
			get { return _result; }
			private set { _result = value; this.OnPropertyChanged("Result"); }
		}

		public Keypad()
		{
			InitializeComponent();
		}

		private void button_Click(object sender, RoutedEventArgs e)
		{
			Button button = sender as Button;
			switch (button.CommandParameter.ToString())
			{
				case "BACKSPACE":
					if (Result.Length > 0)
					{
						if (Result.Length == 2)
							Result = Result.Remove(Result.Length - 1);
						else if (Result.Length == 6)
						{
							Result = Result.Remove(Result.Length - 1);
							Result = Result.Remove(Result.Length - 1);
						}
						else if (Result.Length == 10)
							Result = Result.Remove(Result.Length - 1);
						Result = Result.Remove(Result.Length - 1);
					}
					break;

				default:
					if (Result.Length == 0)
						Result += "(";
					if (Result.Length != 14)
						Result += button.Content.ToString();
					if (Result.Length == 4)
						Result += ") ";
					else if (Result.Length == 9)
						Result += "-";
					break;
			}
		}

		private void OnPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
			phoneNumber.Content = Result;
		}

		public T GetAncestorOfType<T>(FrameworkElement child) where T : FrameworkElement
		{
			var parent = VisualTreeHelper.GetParent(child);
			if (parent.ToString().Contains("ContainerVisual"))
			{
				ContainerVisual temp = parent as ContainerVisual;
				parent = temp.Parent;
			}

			if (parent != null && !(parent is T))
				return (T)GetAncestorOfType<T>((FrameworkElement)parent);
			return (T)parent;
		}

		private void submitBtn_Click(object sender, RoutedEventArgs e)
		{
			tableInterface parent = GetAncestorOfType<tableInterface>(this);
			bool valid = true;
			string message = "";
			rewardMember member = new rewardMember();
			// Check databse for phone number
			try
			{
				using (SqlConnection openCon = new SqlConnection("Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;"))
				{
					SqlCommand myCommand = new SqlCommand("SELECT * FROM dbo.Customers WHERE Phone = @phone", openCon);
					SqlDataAdapter sqlDa = new SqlDataAdapter(myCommand);

					myCommand.Parameters.AddWithValue("@phone", phoneNumber.Content.ToString());
					openCon.Open();
					SqlDataReader reader = myCommand.ExecuteReader();
					if (!reader.HasRows)
					{
						valid = false;
						message = "Phone number not found.";
					}
					else
					{
						member.phoneNumber = (string)reader[0];
						member.firstName = (string)reader[1];
						member.lastName = (string)reader[2];
						member.birthDate = (DateTime)reader[3];
						member.points = (int)reader[5];
						member.email = (string)reader[7];
					}
					openCon.Close();
				}
			}
			catch (Exception) { }

			if (valid)
			{
				parent.setCurrentMember(member);
				parent.checkInGrid.Visibility = Visibility.Hidden;
				parent.welcomeGrid.Visibility = Visibility.Visible;
			}
			else
			{
				MessageBox.Show(message);
			}


		}
	}
}
