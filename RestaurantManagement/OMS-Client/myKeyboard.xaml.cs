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
					catch (Exception ex) { }
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
					catch (Exception ex) { }
					break;
			}
		}

		private Key getKey(string letter)
		{
			switch (letter)
			{
				case "A":
					return Key.A;
				case "B":
					return Key.B;
				case "C":
					return Key.C;
				case "D":
					return Key.D;
				case "E":
					return Key.E;
				case "F":
					return Key.F;
				case "G":
					return Key.G;
				case "H":
					return Key.H;
				case "I":
					return Key.I;
				case "J":
					return Key.J;
				case "K":
					return Key.K;
				case "L":
					return Key.L;
				case "M":
					return Key.M;
				case "N":
					return Key.N;
				case "O":
					return Key.O;
				case "P":
					return Key.P;
				case "Q":
					return Key.Q;
				case "R":
					return Key.R;
				case "S":
					return Key.S;
				case "T":
					return Key.T;
				case "U":
					return Key.U;
				case "V":
					return Key.V;
				case "W":
					return Key.W;
				case "X":
					return Key.X;
				case "Y":
					return Key.Y;
				case "Z":
					return Key.Z;
				case ",":
					return Key.OemComma;
				case ".":
					return Key.OemPeriod;
				case "0":
					return Key.NumPad0;
				case "1":
					return Key.NumPad1;
				case "2":
					return Key.NumPad2;
				case "3":
					return Key.NumPad3;
				case "4":
					return Key.NumPad4;
				case "5":
					return Key.NumPad5;
				case "6":
					return Key.NumPad6;
				case "7":
					return Key.NumPad7;
				case "8":
					return Key.NumPad8;
				case "9":
					return Key.NumPad9;
				case "BACKSPACE":
					return Key.Back;
				default:
					return Key.Space;
			}
		}
	}
}
