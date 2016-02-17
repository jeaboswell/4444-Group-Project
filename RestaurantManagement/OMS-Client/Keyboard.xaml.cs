using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OMS
{
	/// <summary>
	/// Interaction logic for Keyboard.xaml
	/// </summary>
	public partial class Keyboard : UserControl
	{
		private string _result = "";
		public string Result
		{
			get { return _result; }
			private set { _result = value; this.OnPropertyChanged("Result"); }
		}

		public Keyboard()
		{
			InitializeComponent();
		}

		private void button_Click(object sender, RoutedEventArgs e)
		{
			Button button = sender as Button;
			switch (button.CommandParameter.ToString())
			{
				case "ENTER":
					// Add code here to check if phone number is in database
					break;

				case "BACKSPACE":
					if (Result.Length > 0)
					{
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

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}
	}
}
