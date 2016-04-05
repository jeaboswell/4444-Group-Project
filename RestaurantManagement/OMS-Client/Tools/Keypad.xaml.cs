using System;
using System.ComponentModel;
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
			// Ask server if Phone has account
			bool valid = true;

			if (valid)
			{
				parent.checkInGrid.Visibility = Visibility.Hidden;
				parent.welcomeGrid.Visibility = Visibility.Visible;
			}


		}
	}
}
