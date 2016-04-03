using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
	/// Interaction logic for tableInterface.xaml
	/// </summary>
	public partial class tableInterface : UserControl
	{
		rewardMember currentMember = new rewardMember();
		List<menuItem> myMenu = new List<menuItem>();

		public tableInterface()
		{
			InitializeComponent();
			createMenu();
		}

		// Temporary demo functions
		public void createMenu()
		{
			myMenu.Add(new menuItem
			{
				itemNumber = 3,
				name = "Truffled Pommes Frites",
				description = "Natural cut julienne fries topped with cracked pepper, parmesean cheese, and white truffle oil.",
				imgSource = new BitmapImage(new Uri("Resources/Temp/pommesFrites.jpg", UriKind.Relative)),
				price = 12,
				category = "appetizer"
			});
			myMenu.Add(new menuItem
			{
				itemNumber = 0,
				name = "Filet Mignon",
				description = "Our most tender steak! Signature Center-Cut Filet Mignon, perfectly lean, served thick & juciy. Served with a salad or soup, plus your choice of side.",
				imgSource = new BitmapImage(new Uri("Resources/Temp/filetMignon.jpg", UriKind.Relative)),
				price = 36,
				category = "entree"
			});
			myMenu.Add(new menuItem
			{
				itemNumber = 1,
				name = "Grilled Salmon",
				description = "Farm-raised Alaskan Salmon filet topped with lump crab, shrimp, and Béarnaise sauce over roasted garlic Parmesan mashed potatoes and sautéed vegetables.",
				imgSource = new BitmapImage(new Uri("Resources/Temp/grilledSalmon.jpg", UriKind.Relative)),
				price = 29,
				category = "entree"
			});
			myMenu.Add(new menuItem
			{
				itemNumber = 2,
				name = "Tiramisu",
				description = "The classic Italian dessert. A layer of creamy custard set atop espresso-soaked ladyfingers served with blueberries and raspberries.",
				imgSource = new BitmapImage(new Uri("Resources/Temp/tiramisu.jpg", UriKind.Relative)),
				price = 9,
				category = "dessert"
			});
			myMenu.Add(new menuItem
			{
				itemNumber = 4,
				name = "Crème Brûlée",
				description = "",
				imgSource = new BitmapImage(new Uri("Resources/Temp/cremeBrulee.jpg", UriKind.Relative)),
				price = 9,
				category = "dessert"
			});
			myMenu.Add(new menuItem
			{
				itemNumber = 5,
				name = "Coca-Cola",
				description = "",
				imgSource = new BitmapImage(new Uri("Resources/Temp/coke.jpg", UriKind.Relative)),
				price = 2,
				category = "drink"
			});
			myMenu.Add(new menuItem
			{
				itemNumber = 6,
				name = "Diet Coke",
				description = "",
				imgSource = new BitmapImage(new Uri("Resources/Temp/dietCoke.jpg", UriKind.Relative)),
				price = 2,
				category = "drink"
			});
			myMenu.Add(new menuItem
			{
				itemNumber = 7,
				name = "Coka-Cola Zero",
				description = "",
				imgSource = new BitmapImage(new Uri("Resources/Temp/cokeZero.jpg", UriKind.Relative)),
				price = 2,
				category = "drink"
			});
			myMenu.Add(new menuItem
			{
				itemNumber = 8,
				name = "Sprite",
				description = "",
				imgSource = new BitmapImage(new Uri("Resources/Temp/sprite.jpg", UriKind.Relative)),
				price = 2,
				category = "drink"
			});
			myMenu.Add(new menuItem
			{
				itemNumber = 9,
				name = "Orange Fanta",
				description = "",
				imgSource = new BitmapImage(new Uri("Resources/Temp/orangeFanta.jpg", UriKind.Relative)),
				price = 2,
				category = "drink"
			});
			myMenu.Add(new menuItem
			{
				itemNumber = 10,
				name = "Root Beer",
				description = "",
				imgSource = new BitmapImage(new Uri("Resources/Temp/rootBeer.png", UriKind.Relative)),
				price = 2,
				category = "drink"
			});
		}

		#region Menu Functions
		private void menuList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			foreach (menuItem item in myMenu)
			{
				try
				{
					if (item.name == menuList.SelectedValue.ToString())
					{
						menuImage.Source = item.imgSource;
						menuDescription.Text = item.description;
						menuPrice.Content = "$" + item.price.ToString();
					}
				}
				catch (Exception ex) { }
			}
		}

		private void backHomeMenu(object sender, RoutedEventArgs e)
		{
			homePage.Visibility = Visibility.Visible;
			menuGrid.Visibility = Visibility.Hidden;
		}

		private void drinkButton_Click(object sender, RoutedEventArgs e)
		{
			menuList.Items.Clear();
			homePage.Visibility = Visibility.Hidden;
			menuGrid.Visibility = Visibility.Visible;
			foreach (menuItem item in myMenu)
			{
				if (item.category == "drink")
					menuList.Items.Add(item.name);
			}
			menuList.SelectedIndex = 0;
		}

		private void appetizerButton_Click(object sender, RoutedEventArgs e)
		{
			menuList.Items.Clear();
			homePage.Visibility = Visibility.Hidden;
			menuGrid.Visibility = Visibility.Visible;
			foreach (menuItem item in myMenu)
			{
				if (item.category == "appetizer")
					menuList.Items.Add(item.name);
			}
			menuList.SelectedIndex = 0;
		}

		private void entreeButton_Click(object sender, RoutedEventArgs e)
		{
			menuList.Items.Clear();
			homePage.Visibility = Visibility.Hidden;
			menuGrid.Visibility = Visibility.Visible;
			foreach (menuItem item in myMenu)
			{
				if (item.category == "entree")
					menuList.Items.Add(item.name);
			}
			menuList.SelectedIndex = 0;
		}

		private void dessertButton_Click(object sender, RoutedEventArgs e)
		{
			menuList.Items.Clear();
			homePage.Visibility = Visibility.Hidden;
			menuGrid.Visibility = Visibility.Visible;
			foreach (menuItem item in myMenu)
			{
				if (item.category == "dessert")
					menuList.Items.Add(item.name);
			}
			menuList.SelectedIndex = 0;
		}
		#endregion

		#region Game Functions
		private bool getWin()
		{
			Random rng = new Random();
			int num = rng.Next(0, 100);

			if (num < 10)
				return true;

			return false;
		}
		#endregion

		#region eClub Functions
		private void closeCheckIn(object sender, RoutedEventArgs e)
		{
			checkInGrid.Visibility = Visibility.Hidden;
			eClubHome.Visibility = Visibility.Visible;
		}

		private void checkInBtn_Click(object sender, RoutedEventArgs e)
		{
			eClubHome.Visibility = Visibility.Hidden;
			checkInGrid.Visibility = Visibility.Visible;
		}

		private void createAcntBtn_Click(object sender, RoutedEventArgs e)
		{
			eClubHome.Visibility = Visibility.Hidden;
			newAccountGrid.Visibility = Visibility.Visible;
		}

		private void cancelCreation_Click(object sender, RoutedEventArgs e)
		{
			eClubHome.Visibility = Visibility.Visible;
			newAccountGrid.Visibility = Visibility.Hidden;
		}

		private void monthBox_Initialized(object sender, EventArgs e)
		{
			for (int i = 1; i <= 12; i++)
			{
				monthBox.Items.Add(i);
			}
		}

		private void monthBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			int days = 0;
			switch (monthBox.SelectedIndex)
			{
				case 0:  // January
				case 2:  // March
				case 4:  // May
				case 6:  // July
				case 7:  // August
				case 9:  // October
				case 11: // December
					days = 31;
					break;
				case 3:  // April
				case 5:  // June
				case 8:  // September
				case 10: // November
					days = 30;
					break;
				case 1:  // February
					days = 29;
					break;
				default:
					break;
			}
			for (int i = 1; i <= days; i++)
			{
				dayBox.Items.Add(i);
			}
		}

		private void yearBox_Initialized(object sender, EventArgs e)
		{
			for (int i = DateTime.Now.Year; i != (DateTime.Now.Year - 120); i--)
			{
				yearBox.Items.Add(i);
			}
		}

		private bool IsDigit(string s)
		{
			Regex r = new Regex(@"^[0-9]+$");

			return r.IsMatch(s);
		}

		private void phoneNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			if (phoneNumber.Text.Length == 10 || !IsDigit(e.Text))
				e.Handled = true;
		}

		private void DoneBtn_Click(object sender, RoutedEventArgs e) // Check fields for completion of new member
		{
			string message = "";
			bool pass = true;
			if (firstName.Text.Length == 0)
			{
				message += "Please enter your first name.\n";
				pass = false;
			}
			if (lastName.Text.Length == 0)
			{
				message += "Please enter your last name.\n";
				pass = false;
			}
			if (monthBox.SelectedIndex == -1 || dayBox.SelectedIndex == -1 || yearBox.SelectedIndex == -1)
			{
				message += "Please enter your birth date.\n";
				pass = false;
			}
			if (phoneNumber.Text.Length != 10)
			{
				message += "Please enter a valid 10 digit phone number.\n";
				pass = false;
			}
			if (email.Text.Length == 0)
			{
				message += "Please enter your email address.\n";
				pass = false;
			}
			else // Verify email
			{
				bool foundAt = false, valid = false;
				int atIndex = 0;

				for (int i = 0; i < email.Text.Length; i++)
				{
					if (email.Text[i] == '@')
					{
						foundAt = true;
						atIndex = i;
					}
					else if (email.Text[i] == '.' && foundAt && i != email.Text.Length - 1 && atIndex != i - 1)
					{
						valid = true;
					}
				}
				if (!valid)
				{
					message += "Please enter a valid email address.\n";
					pass = false;
				}
			}

			if (!pass)
			{
				MessageBox.Show(message);
			}
			else
			{

				// Set data for current member
				currentMember.firstName = firstName.Text;
				currentMember.lastName = lastName.Text;
				currentMember.setBirthDate((int)monthBox.SelectedValue, (int)dayBox.SelectedValue, (int)yearBox.SelectedValue);
				currentMember.setPhoneNumber(phoneNumber.Text);
				currentMember.email = email.Text;
				currentMember.points += 1;

				// Add code here to store account info in server

				// Reset form
				firstName.Clear();
				lastName.Clear();
				monthBox.SelectedIndex = 0;
				dayBox.SelectedIndex = 0;
				yearBox.SelectedIndex = 0;
				phoneNumber.Clear();
				email.Clear();

				newAccountGrid.Visibility = Visibility.Hidden;
				memberInfo.Visibility = Visibility.Visible;
			}
		}
		#endregion

		#region Service Dock
		private void helpButton_Click(object sender, RoutedEventArgs e)
		{
			if (helpButton.Background.ToString() == "#FF2D2D30")
				helpButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF701C1C"));
			else
				helpButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30"));
		}

		private void refillButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void cartButton_Click(object sender, RoutedEventArgs e)
		{

		}
		#endregion
	}
}
