using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
using OMS_Library;

namespace OMS
{
	/// <summary>
	/// Interaction logic for tableInterface.xaml
	/// </summary>
	public partial class tableInterface : UserControl
	{
		#region Variables
		rewardMember currentMember = new rewardMember();
		List<menuItem> myMenu = new List<menuItem>();
		int funGames = 0, couponGames = 0;
		Cart order = new Cart();
		#endregion
		/// <summary>
		/// Interface initilization
		/// </summary>
		public tableInterface()
		{
			InitializeComponent();
		}

		#region Menu Functions
		/// <summary>
		/// Download the menu from the database
		/// </summary>
		public void createMenu()
		{
			// Clear all affected lists
			myMenu.Clear();
			Dispatcher.Invoke((Action)(() =>
			{
				saladChoice.Items.Clear();
				sideChoice.Items.Clear();
			}));

			string SQLConnectionString = "Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;";
			// Create an SqlConnection from the provided connection string.
			using (SqlConnection connection = new SqlConnection(SQLConnectionString))
			{
				// Formulate the command.
				SqlCommand command = new SqlCommand();
				command.Connection = connection;

				// Specify the query to be executed.
				command.CommandType = CommandType.Text;
				command.CommandText = @"
                    SELECT * FROM dbo.Menu
                    WHERE Available=1
                    ";
				// Open a connection to database.
				connection.Open();
				// Read data returned for the query.
				SqlDataReader reader = command.ExecuteReader();

				// while not done reading the stuff returned from the query
				while (reader.Read())
				{
					ImageSource tempSrc;

					try
					{
						tempSrc = LoadImage((byte[])reader[4]);
					}
					catch (Exception)
					{
						tempSrc = new BitmapImage(new Uri("Resources/noImage.jpg", UriKind.Relative));
					}
					// create menu items from the database               
					myMenu.Add(new menuItem
					{
						itemNumber = (int)reader[0],
						name = (string)reader[1],
						description = (string)reader[2],
						imgSource = tempSrc,
						price = (decimal)reader[3],
						category = (string)reader[7]
					});
				}
			}

			Dispatcher.Invoke((Action)(() =>
			{
				saladChoice.Items.Add("None");
				sideChoice.Items.Add("None");
			}));

			foreach (menuItem item in myMenu)
			{
				Dispatcher.Invoke((Action)(() =>
				{
					if (item.category == "salad" || item.category == "soup")
						saladChoice.Items.Add(item.name);
					else if (item.category == "side")
						sideChoice.Items.Add(item.name);
				}));
			}
		}
		/// <summary>
		/// Change data shown in menu screen when selected item is changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
                        menuPrice.Content = "$" + item.price.ToString("0.##");//Decimal.ToInt32(item.price).ToString();
                        // the above technically rounds the decimal number, but our values only ever hold two decimals so this is a non issue
					}
				}
				catch (Exception) { }
			}
		}
		/// <summary>
		/// Return to the main menu
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void backHomeMenu(object sender, RoutedEventArgs e)
		{
			homePage.Visibility = Visibility.Visible;
			menuGrid.Visibility = Visibility.Hidden;
		}
		/// <summary>
		/// Display the drink menu
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
		/// <summary>
		/// Display the appetizer menu
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
		/// <summary>
		/// Display the entree menu
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
		/// <summary>
		/// Display the dessert menu
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
		/// <summary>
		/// Send order to database
		/// </summary>
		private void submitOrder()
		{
			using (MemoryStream ms = new MemoryStream())
			{
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(ms, order);

				ms.Position = 0;
				byte[] orderData = ms.ToArray();

				using (SqlConnection openCon = new SqlConnection("Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;"))
				{
					string command = "INSERT into dbo.Orders (Order, Client) VALUES (@order, @client)";

					using (SqlCommand querySave = new SqlCommand(command, openCon))
					{
						querySave.Parameters.AddWithValue("@order", orderData);
						querySave.Parameters.AddWithValue("@client", Dns.GetHostAddresses(Dns.GetHostName()));

						openCon.Open();
						querySave.ExecuteScalar();
						openCon.Close();
					}
				}
			}
		}
		/// <summary>
		/// Function to add item to cart
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void addToCart_Click(object sender, RoutedEventArgs e)
		{
			foreach (menuItem item in myMenu)
			{
				try
				{
					if (item.name == menuList.SelectedValue.ToString())
					{
						switch (item.category)
						{
							case "drink":
								order.Items.Add(item);
								order.Notes.Add("");
								// Show addition alert
								overlay.Visibility = Visibility.Visible;
								addedAlert.Content = item.name + "added to cart.";
								addedAlert.Visibility = Visibility.Visible;
								Thread.Sleep(1000);
								addedAlert.Visibility = Visibility.Hidden;
								overlay.Visibility = Visibility.Hidden;
								break;
							case "appetizer":
								//
								// Show add to cart interface to allow notes to be added.
								//
								break;
							case "entree":
								overlay.Visibility = Visibility.Visible;
								entreeAddition.Visibility = Visibility.Visible;
								break;
							case "dessert":
								//
								// Show add to cart interface to allow notes to be added.
								//
								break;
							default:
								break;
						}
					}
				}
				catch (Exception) { }
			}
		}

		#region |   Entree Addition   |
		/// <summary>
		/// Closes current entree addition overlay and resets fields
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cancelEntreeAddition_Click(object sender, RoutedEventArgs e)
		{
			entreeAddition.Visibility = Visibility.Hidden;
			overlay.Visibility = Visibility.Hidden;
			saladChoice.SelectedIndex = -1;
			sideChoice.SelectedIndex = -1;
			entreeNotes.Text = "";
		}
		/// <summary>
		/// Verifies all fields are filled and adds item to cart
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void submitEntree_Click(object sender, RoutedEventArgs e)
		{
			bool complete = false;
			string errorMsg = "";

			if (saladChoice.SelectedValue == null)
				errorMsg += "Please select a salad/soup.\n";
			if (sideChoice.SelectedValue == null)
				errorMsg += "Pleses select a side dish.\n";
			if (errorMsg != "")
				MessageBox.Show(errorMsg);
			else
				complete = true;

			if (!complete)
				return;

			foreach (menuItem item in myMenu)
			{
				try
				{
					if (item.name == menuList.SelectedValue.ToString())
					{
						// Show addition alert
						entreeAddition.Visibility = Visibility.Hidden;
						addedAlert.Content = item.name + "added to cart.";
						addedAlert.Visibility = Visibility.Visible;
						// Add item to cart
						order.Items.Add(item);
						// Add notes with sides to cart
						string fullNotes = "Soup/Salad: " + saladChoice.SelectedValue.ToString() + "\n\nSide Choice: " + sideChoice.SelectedValue.ToString() + "\n\nNotes:\n" + entreeNotes.Text;
						order.Notes.Add(fullNotes);
						// Reset form
						//Thread.Sleep(1000);
						addedAlert.Visibility = Visibility.Hidden;
						overlay.Visibility = Visibility.Hidden;
						saladChoice.SelectedIndex = -1;
						sideChoice.SelectedIndex = -1;
						entreeNotes.Text = "";
					}
				}
				catch (Exception) {	}
			}
		}
		#endregion
		#endregion

		#region Game Functions
		/// <summary>
		/// Return to games home page
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void backFromGame_Click(object sender, RoutedEventArgs e)
		{
			backFromGame.Visibility = Visibility.Hidden;
			sudokuBrowser.Visibility = Visibility.Hidden;
			solitareBrowser.Visibility = Visibility.Hidden;
			mahjongBrowser.Visibility = Visibility.Hidden;
			flappyBrowser.Visibility = Visibility.Hidden;
			spadesBrowser.Visibility = Visibility.Hidden;
			blackjackBrowser.Visibility = Visibility.Hidden;
			couponGame.Visibility = Visibility.Hidden;
			gamesHome.Visibility = Visibility.Visible;
		}
		/// <summary>
		/// Open coupon game
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void couponBtn_Click(object sender, RoutedEventArgs e)
		{
			if (couponGames < 2)
			{
				gamesHome.Visibility = Visibility.Hidden;
				couponGame.Visibility = Visibility.Visible;
				backFromGame.Visibility = Visibility.Visible;
			}
			else
				MessageBox.Show("Play limit reached.");
		}
		/// <summary>
		/// Open sudoku game
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void sudokuBtn_Click(object sender, RoutedEventArgs e)
		{
			if (funGames < 2)
			{
				sudokuBrowser.Navigate("http://www.247sudoku.com/sudoku8.swf");
				gamesHome.Visibility = Visibility.Hidden;
				sudokuBrowser.Visibility = Visibility.Visible;
				backFromGame.Visibility = Visibility.Visible;
				funGames++;
			}
			else
				MessageBox.Show("Game limit reached.");
		}
		/// <summary>
		/// Open solitare game
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void solitareBtn_Click(object sender, RoutedEventArgs e)
		{
			if (funGames < 2)
			{
				solitareBrowser.Navigate("http://www.247solitaire.com/solitaire2.swf");
				gamesHome.Visibility = Visibility.Hidden;
				solitareBrowser.Visibility = Visibility.Visible;
				backFromGame.Visibility = Visibility.Visible;
				funGames++;
			}
			else
				MessageBox.Show("Game limit reached.");
		}
		/// <summary>
		/// Open mahjong game
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mahjongBtn_Click(object sender, RoutedEventArgs e)
		{
			if (funGames < 2)
			{
				mahjongBrowser.Navigate("http://www.247mahjong.com/mahjong10.swf");
				gamesHome.Visibility = Visibility.Hidden;
				mahjongBrowser.Visibility = Visibility.Visible;
				backFromGame.Visibility = Visibility.Visible;
				funGames++;
			}
			else
				MessageBox.Show("Game limit reached.");
		}
		/// <summary>
		/// Open flappy beak game
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void flappyBtn_Click(object sender, RoutedEventArgs e)
		{
			if (funGames < 2)
			{
				flappyBrowser.Navigate("http://www.flappybeak.com/flappy3.swf");
				gamesHome.Visibility = Visibility.Hidden;
				flappyBrowser.Visibility = Visibility.Visible;
				backFromGame.Visibility = Visibility.Visible;
				funGames++;
			}
			else
				MessageBox.Show("Game limit reached.");
		}
		/// <summary>
		/// Open spades game
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void spadesBtn_Click(object sender, RoutedEventArgs e)
		{
			if (funGames < 2)
			{
				spadesBrowser.Navigate("http://www.247spades.com/spades.swf");
				gamesHome.Visibility = Visibility.Hidden;
				spadesBrowser.Visibility = Visibility.Visible;
				backFromGame.Visibility = Visibility.Visible;
				funGames++;
			}
			else
				MessageBox.Show("Game limit reached.");
		}
		/// <summary>
		/// Open blackjack game
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void blackjackBtn_Click(object sender, RoutedEventArgs e)
		{
			if (funGames < 2)
			{
				blackjackBrowser.Navigate("http://www.247blackjack.com/blackjack.swf");
				gamesHome.Visibility = Visibility.Hidden;
				blackjackBrowser.Visibility = Visibility.Visible;
				backFromGame.Visibility = Visibility.Visible;
				funGames++;
			}
			else
				MessageBox.Show("Game limit reached.");
		}
		/// <summary>
		/// Determine if customer wins a coupon code.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void determineWin(object sender, RoutedEventArgs e)
		{
			Button temp = sender as Button;
			int selection = Convert.ToInt32(temp.Content.ToString());
			Random rng = new Random();
			int winningNumber = rng.Next(1, 5);
			if (selection == winningNumber)
			{
				coupon newCoupon = new coupon();
				newCoupon.generateCoupon();
				MessageBox.Show("Congratulations! Your coupon code is " + newCoupon.code);
			}
			else
				MessageBox.Show("Sorry, that is not the winning number. Thank you for playing!");
			couponGame.Visibility = Visibility.Hidden;
			backFromGame.Visibility = Visibility.Hidden;
			gamesHome.Visibility = Visibility.Visible;
			couponGames++;
		}
		#endregion

		#region eClub Functions
		/// <summary>
		/// Close the check in interface
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void closeCheckIn(object sender, RoutedEventArgs e)
		{
			checkInGrid.Visibility = Visibility.Hidden;
			eClubHome.Visibility = Visibility.Visible;
		}
		/// <summary>
		/// Open the check in interface
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void checkInBtn_Click(object sender, RoutedEventArgs e)
		{
			eClubHome.Visibility = Visibility.Hidden;
			checkInGrid.Visibility = Visibility.Visible;
		}
		/// <summary>
		/// Open the account creation interface
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void createAcntBtn_Click(object sender, RoutedEventArgs e)
		{
			eClubHome.Visibility = Visibility.Hidden;
			newAccountGrid.Visibility = Visibility.Visible;
		}
		/// <summary>
		/// Cancel the account creation interface
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cancelCreation_Click(object sender, RoutedEventArgs e)
		{
			eClubHome.Visibility = Visibility.Visible;
			newAccountGrid.Visibility = Visibility.Hidden;
		}

		#region	|   Account Creation Initialization   |
		/// <summary>
		/// Initialize the month selector in account creation
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void monthBox_Initialized(object sender, EventArgs e)
		{
			for (int i = 1; i <= 12; i++)
			{
				monthBox.Items.Add(i);
			}
		}
		/// <summary>
		/// Initialized day selector in account creation based on month selection
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
			dayBox.Items.Clear();
			for (int i = 1; i <= days; i++)
			{
				dayBox.Items.Add(i);
			}
		}
		/// <summary>
		/// Initialize the year selector in account creation
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void yearBox_Initialized(object sender, EventArgs e)
		{
			for (int i = DateTime.Now.Year; i != (DateTime.Now.Year - 120); i--)
			{
				yearBox.Items.Add(i);
			}
		}
		/// <summary>
		/// Check if string is a number
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		private bool IsDigit(string s)
		{
			Regex r = new Regex(@"^[0-9]+$");

			return r.IsMatch(s);
		}
		/// <summary>
		/// Validate phone number
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void phoneNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			if (phoneNumber.Text.Length == 10 || !IsDigit(e.Text))
				e.Handled = true;
		}
		/// <summary>
		/// Check fields for completion of new member
		/// Create account and send to database
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DoneBtn_Click(object sender, RoutedEventArgs e) // WIP
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
				//
				// Add code here to store account info in server
				//
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
		#endregion

		#region Service Dock
		private void helpButton_Click(object sender, RoutedEventArgs e)
		{
			if (helpButton.Background.ToString() == "#FF2D2D30")
			{
				helpButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF701C1C"));
				commHelper.functionSend("requestHelp");
			}
			else
			{
				helpButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30"));
				commHelper.functionSend("cancelHelp");
			}
		}

		private void refillButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void cartButton_Click(object sender, RoutedEventArgs e)
		{

		}
		#endregion

		#region Helper Functions
		/// <summary>
		/// Convert byte array to imageSource
		/// </summary>
		/// <param name="imageData"></param>
		/// <returns></returns>
		private static BitmapImage LoadImage(byte[] imageData)
		{
			if (imageData == null || imageData.Length == 0) return null;
			var image = new BitmapImage();
			using (var mem = new MemoryStream(imageData))
			{
				mem.Position = 0;
				image.BeginInit();
				image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
				image.CacheOption = BitmapCacheOption.OnLoad;
				image.UriSource = null;
				image.StreamSource = mem;
				image.EndInit();
			}
			image.Freeze();
			return image;
		}
		#endregion
	}
}
