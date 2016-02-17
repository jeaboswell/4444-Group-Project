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
				case "SHOWNUMBERS":
					letters.Visibility = Visibility.Hidden;
					numbers.Visibility = Visibility.Visible;
					break;

				case "SHOWLETTERS":
					letters.Visibility = Visibility.Visible;
					numbers.Visibility = Visibility.Hidden;
					break;

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
					var key = Key.A;                    // Key to send
					var target = Keyboard.FocusedElement;    // Target element
					var routedEvent = Keyboard.KeyDownEvent; // Event to send

					target.RaiseEvent(new System.Windows.Input.KeyEventArgs(Keyboard.PrimaryDevice,
					 System.Windows.PresentationSource.FromVisual((Visual)target), 0, key)
					{ RoutedEvent = routedEvent });
					Result += button.Content.ToString();
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
