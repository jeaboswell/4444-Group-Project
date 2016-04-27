using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OMS
{
	/// <summary>
	/// Interaction logic for Keyboard.xaml
	/// </summary>
	public partial class myKeyboard : UserControl
	{
		public myKeyboard()
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

				case "NEXT":
					KeyEventArgs args = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Tab);
					args.RoutedEvent = Keyboard.KeyDownEvent;
					InputManager.Current.ProcessInput(args);
					break;

				case "BACKSPACE":
					try
					{
						TextBox target = (TextBox)Keyboard.FocusedElement;
						if (target.Text.Length > 0)
						{
							int tempIndex = target.CaretIndex;
							if (tempIndex != 0)
								target.Text = target.Text.Remove(tempIndex - 1, 1);
							target.CaretIndex = tempIndex - 1;
						}
					}
					catch (Exception) { }
					break;

				default:
					try
					{
						TextBox target = (TextBox)Keyboard.FocusedElement;
						if (target.Name == "phoneNumber" && target.Text.Length != 10)
						{
							switch (button.Content.ToString())
							{
								case "0": case "1": case "2": case "3": case "4":
								case "5": case "6": case "7": case "8": case "9":
									target.Text += button.Content.ToString();
									target.CaretIndex = target.Text.Length;
									break;
								default:
									break;
							}
						}
						else if (target.Name != "phoneNumber")
						{
							target.Text += button.Content.ToString();
							target.CaretIndex = target.Text.Length;
						}
					}
					catch (Exception) { }
					break;
			}
		}
	}
}
