#region Usings
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
using System.Windows.Threading;
#endregion

namespace OMS
{
	/// <summary>
	/// Interaction logic for tableInterface.xaml
	/// </summary>
	public partial class tableInterface : UserControl
	{
		#region Variables
		rewardMember currentMember = new rewardMember();
		List<object> sentOrders = new List<object>();
		List<object> myMenu = new List<object>();
		int funGames = 0, couponGames = 0;
		object order = new object();
		bool payFirst = true, tipApplied = false, couponApplied = false, adjustmentApplied = false;
		decimal tip = 0, adjustment = 0;
		#endregion

		#region Initialization
		/// <summary>
		/// Interface initilization
		/// </summary>
		public tableInterface()
		{
			InitializeComponent();
		}
		/// <summary>
		/// Initializes data for class to correct type
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			order = new Cart();
		}
		#endregion

		// Complete
		#region Menu Functions
		/// <summary>
		/// Download the menu from the database
		/// </summary>
		public void createMenu()
		{
			// Clear all affected lists
			myMenu.Clear();
			Dispatcher.Invoke(() =>
			{
				saladChoice.Items.Clear();
				sideChoice.Items.Clear();
			});
			try
			{
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
			}
			catch (Exception) { }

			try
			{
				Dispatcher.Invoke(() =>
				{
					saladChoice.Items.Add("None");
					sideChoice.Items.Add("None");
				});
			}
			catch (Exception) { }

			foreach (menuItem item in myMenu)
			{
				try
				{
					Dispatcher.Invoke(() =>
					{
						if (item.category == "salad" || item.category == "soup")
							saladChoice.Items.Add(item.name);
						else if (item.category == "side")
							sideChoice.Items.Add(item.name);
					});
				}
				catch (Exception) { }
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
                        menuPrice.Content = "$" + decimal.ToInt32(item.price).ToString(); //item.price.ToString("0.##");
                                                                   // the above commented code technically rounds the decimal number, but our values only ever hold two decimals so this is a non issue
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
								// Show addition alert
								overlay.Visibility = Visibility.Visible;
								addedAlert.Content = item.name + " added to cart.";
								addedAlert.Visibility = Visibility.Visible;
								Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
								// Add item to cart
								((Cart)order).Items.Add(new cartItem()
								{
									name = item.name,
									itemNumber = item.itemNumber,
									category = item.category,
									description = item.description,
									price = item.price,
									visible = item.visible
								});
								((Cart)order).Notes.Add("");
								Thread.Sleep(1000);
								// Remove addition alert
								addedAlert.Visibility = Visibility.Hidden;
								overlay.Visibility = Visibility.Hidden;
								break;
							case "appetizer":
								overlay.Visibility = Visibility.Visible;
								appetizerAddition.Visibility = Visibility.Visible;
								break;
							case "entree":
								overlay.Visibility = Visibility.Visible;
								entreeAddition.Visibility = Visibility.Visible;
								break;
							case "dessert":
								overlay.Visibility = Visibility.Visible;
								dessertAddition.Visibility = Visibility.Visible;
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
						addedAlert.Content = item.name + " added to cart.";
						addedAlert.Visibility = Visibility.Visible;
						Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
						// Add item to cart
						((Cart)order).Items.Add(new cartItem()
						{
							name = item.name,
							itemNumber = item.itemNumber,
							category = item.category,
							description = item.description,
							price = item.price,
							visible = item.visible
						});
						// Add notes with sides to cart
						string fullNotes = "Soup/Salad: " + saladChoice.SelectedValue.ToString() + Environment.NewLine + "Side Choice: " + sideChoice.SelectedValue.ToString() + Environment.NewLine + "Notes: " + entreeNotes.Text;
						((Cart)order).Notes.Add(fullNotes);
						// Reset form
						Thread.Sleep(1000);
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

		private void entreeNotes_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			defaultKeyboard.Visibility = Visibility.Visible;
			Thickness tempMargin = entreeAddition.Margin;
			tempMargin.Bottom += 300;
			tempMargin.Top -= 300;
			entreeAddition.Margin = tempMargin;
		}

		private void entreeNotes_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			defaultKeyboard.Visibility = Visibility.Hidden;
			Thickness tempMargin = new Thickness() { Top = 125, Bottom = 125, Left = 200, Right = 200 };
			entreeAddition.Margin = tempMargin;
			Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
		}
		#endregion

		#region |   Appetizer Addition   |
		/// <summary>
		/// Closes current appetizer addition overlay and resets fields
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cancelAppetizerAddition_Click(object sender, RoutedEventArgs e)
		{
			appetizerAddition.Visibility = Visibility.Hidden;
			overlay.Visibility = Visibility.Hidden;
			appetizerNotes.Text = "";
		}
		/// <summary>
		/// Adds item and notes to the cart
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void submitAppetizer_Click(object sender, RoutedEventArgs e)
		{
			foreach (menuItem item in myMenu)
			{
				try
				{
					if (item.name == menuList.SelectedValue.ToString())
					{
						// Show addition alert
						appetizerAddition.Visibility = Visibility.Hidden;
						addedAlert.Content = item.name + " added to cart.";
						addedAlert.Visibility = Visibility.Visible;
						Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
						// Add item to cart
						((Cart)order).Items.Add(new cartItem()
						{
							name = item.name,
							itemNumber = item.itemNumber,
							category = item.category,
							description = item.description,
							price = item.price,
							visible = item.visible
						});
						// Add notes with sides to cart
						((Cart)order).Notes.Add(appetizerNotes.Text);
						// Reset form
						Thread.Sleep(1000);
						addedAlert.Visibility = Visibility.Hidden;
						overlay.Visibility = Visibility.Hidden;
						appetizerNotes.Text = "";
					}
				}
				catch (Exception) { }
			}
		}

		private void appetizerNotes_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			defaultKeyboard.Visibility = Visibility.Visible;
			Thickness tempMargin = appetizerAddition.Margin;
			tempMargin.Bottom += 295;
			tempMargin.Top -= 185;
			appetizerAddition.Margin = tempMargin;
		}

		private void appetizerNotes_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			defaultKeyboard.Visibility = Visibility.Hidden;
			Thickness tempMargin = new Thickness() { Top = 125, Bottom = 125, Left = 200, Right = 200 };
			appetizerAddition.Margin = tempMargin;
			Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
		}
		#endregion

		#region |   Dessert Addition   |
		/// <summary>
		/// Closes current dessert addition overlay and resets fields
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cancelDessertAddition_Click(object sender, RoutedEventArgs e)
		{
			dessertAddition.Visibility = Visibility.Hidden;
			overlay.Visibility = Visibility.Hidden;
			dessertNotes.Text = "";
		}
		/// <summary>
		/// Addes item and notes to the cart
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void submitDessert_Click(object sender, RoutedEventArgs e)
		{
			foreach (menuItem item in myMenu)
			{
				try
				{
					if (item.name == menuList.SelectedValue.ToString())
					{
						// Show addition alert
						dessertAddition.Visibility = Visibility.Hidden;
						addedAlert.Content = item.name + " added to cart.";
						addedAlert.Visibility = Visibility.Visible;
						Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
						// Add item to cart
						((Cart)order).Items.Add(new cartItem()
						{
							name = item.name,
							itemNumber = item.itemNumber,
							category = item.category,
							description = item.description,
							price = item.price,
							visible = item.visible
						});
						// Add notes with sides to cart
						((Cart)order).Notes.Add(dessertNotes.Text);
						// Reset form
						Thread.Sleep(1000);
						addedAlert.Visibility = Visibility.Hidden;
						overlay.Visibility = Visibility.Hidden;
						dessertNotes.Text = "";
					}
				}
				catch (Exception) { }
			}
		}

		private void dessertNotes_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			defaultKeyboard.Visibility = Visibility.Visible;
			Thickness tempMargin = dessertAddition.Margin;
			tempMargin.Bottom += 295;
			tempMargin.Top -= 185;
			dessertAddition.Margin = tempMargin;
		}

		private void dessertNotes_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			defaultKeyboard.Visibility = Visibility.Hidden;
			Thickness tempMargin = new Thickness() { Top = 125, Bottom = 125, Left = 200, Right = 200 };
			dessertAddition.Margin = tempMargin;
			Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
		}
		#endregion
		#endregion

		// Complete
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
			sudokuBrowser.Navigate("http://blank.org/");
			solitareBrowser.Visibility = Visibility.Hidden;
			solitareBrowser.Navigate("http://blank.org/");
			mahjongBrowser.Visibility = Visibility.Hidden;
			mahjongBrowser.Navigate("http://blank.org/");
			flappyBrowser.Visibility = Visibility.Hidden;
			flappyBrowser.Navigate("http://blank.org/");
			spadesBrowser.Visibility = Visibility.Hidden;
			spadesBrowser.Navigate("http://blank.org/");
			blackjackBrowser.Visibility = Visibility.Hidden;
			blackjackBrowser.Navigate("http://blank.org/");
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
				if (currentMember.phoneNumber != null)
					newCoupon.generateCoupon(currentMember);
				else
					newCoupon.generateCoupon(null);
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

		// ToDo: Check member coupon string against DB
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
		private void DoneBtn_Click(object sender, RoutedEventArgs e)
		{
			#region Validate Fields
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
			#endregion

			#region Check if duplicate
			if (pass)
			{
				try
				{
					using (SqlConnection openCon = new SqlConnection("Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;"))
					{
						SqlCommand myCommand = new SqlCommand("SELECT * FROM dbo.Customers WHERE Phone = @phone", openCon);
						SqlDataAdapter sqlDa = new SqlDataAdapter(myCommand);

						myCommand.Parameters.AddWithValue("@phone", formatPhoneNumber(phoneNumber.Text));
						openCon.Open();
						SqlDataReader reader = myCommand.ExecuteReader();
						if(reader.HasRows)
						{
							pass = false;
							message = "Phone number already has account.";
						}
						openCon.Close();
					}
				}
				catch (Exception) { }
			}
			#endregion

			if (!pass)
			{
				MessageBox.Show(message);
			}
			else
			{
				rewardMember tempMember = new rewardMember();
				// Set data for current member
				tempMember.firstName = firstName.Text;
				tempMember.lastName = lastName.Text;
				tempMember.setBirthDate((int)monthBox.SelectedValue, (int)dayBox.SelectedValue, (int)yearBox.SelectedValue);
				tempMember.setPhoneNumber(phoneNumber.Text);
				tempMember.email = email.Text;
				tempMember.points += 1;
				// Send member to database
				try
				{
					using (SqlConnection openCon = new SqlConnection("Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;"))
					{
						string command = "INSERT into dbo.Customers (Phone, FName, LName, Birthday, Points, Discounts, Email) VALUES (@phone, @fname, @lname, @bday, @points, @discounts, @email)";

						using (SqlCommand querySave = new SqlCommand(command, openCon))
						{
							querySave.Parameters.AddWithValue("@phone", currentMember.phoneNumber);
							querySave.Parameters.AddWithValue("@fname", currentMember.firstName);
							querySave.Parameters.AddWithValue("@lname", currentMember.lastName);
							querySave.Parameters.AddWithValue("@bday", currentMember.birthDate);
							querySave.Parameters.AddWithValue("@points", currentMember.points);
							querySave.Parameters.AddWithValue("@discounts", 0);
							querySave.Parameters.AddWithValue("@email", currentMember.email);

							openCon.Open();
							querySave.ExecuteScalar();
							openCon.Close();
						}
					}
				}
				catch (Exception) { }
				setCurrentMember(tempMember);
				welcomeGrid.Visibility = Visibility.Visible;
				// Reset form
				firstName.Clear();
				lastName.Clear();
				monthBox.SelectedIndex = 0;
				dayBox.SelectedIndex = 0;
				yearBox.SelectedIndex = 0;
				phoneNumber.Clear();
				email.Clear();

				newAccountGrid.Visibility = Visibility.Hidden;
				welcomeGrid.Visibility = Visibility.Visible;
			}
		}
		#endregion
		/// <summary>
		/// Generate coupon if customer has 5 or more visits
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void redeemPoints_Click(object sender, RoutedEventArgs e)
		{
			coupon c = new coupon();
			c.generateCoupon(currentMember);
			memberCoupons.Items.Add(c.code);
			currentMember.points -= 5;

			if (currentMember.points < 5)
				redeemGrid.Visibility = Visibility.Hidden;

			try
			{
				using (SqlConnection openCon = new SqlConnection("Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;"))
				{
					SqlCommand myCommand = new SqlCommand("update dbo.Customers set Points = @points where Phone = @phone", openCon);

					myCommand.Parameters.AddWithValue("@phone", currentMember.phoneNumber);
					myCommand.Parameters.AddWithValue("@points", currentMember.points);
					openCon.Open();
					myCommand.ExecuteScalar();
					openCon.Close();
				}
			}
			catch (Exception) { }
		}
		/// <summary>
		/// Close the birthday popup overlay
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void birthdayOkay_Click(object sender, RoutedEventArgs e)
		{
			birthdayPopup.Visibility = Visibility.Hidden;
			overlay.Visibility = Visibility.Hidden;
		}
		#endregion

		// ToDo: Accept price adjustments
		//		 Add survey after payment
		//		 Submit payment (Remove orders from DB)
		#region Payment Functions
		/// <summary>
		/// Updates payment tab
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void homePage_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			TabItem temp = (TabItem)(((TabControl)sender).SelectedValue);
			if (temp.Header.ToString() == "Pay")
			{
				updateBill();
				defaultKeyboard.Visibility = Visibility.Hidden;
				closeKeyboard.Visibility = Visibility.Hidden;
			}
		}
		/// <summary>
		/// Returns a new grid separator for bill printing
		/// </summary>
		/// <returns></returns>
		Grid S()
		{
			Grid separator = new Grid()
			{
				Margin = new Thickness() { Left = 0, Right = 0 },
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Width = 573
			};
			separator.Children.Add(new Separator()
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Margin = new Thickness() { Left = 0, Right = 0 },
				Width = 573
			});

			return separator;
		}
		/// <summary>
		/// Add all items from submitted orders to the payment list
		/// </summary>
		private void updateBill()
		{
			paymentList.Items.Clear();
			decimal runningTotal = 0, tax;
			foreach (Cart oItem in sentOrders)
			{
				foreach (cartItem cItem in oItem.Items)
				{
					Grid billItem = new Grid()
					{
						Margin = new Thickness() { Left = 0, Right = 0 },
						HorizontalAlignment = HorizontalAlignment.Stretch,
						Width = 573
					};
					billItem.Children.Add(new Label()
					{
						Content = cItem.name,
						FontSize = 20,
						FontFamily = new FontFamily("Baskerville Old Face"),
						Margin = new Thickness() { Left = 0, Top = 0 },
						HorizontalAlignment = HorizontalAlignment.Left,
						Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"))
					});
					billItem.Children.Add(new Label()
					{
						Content = "$" + decimal.ToInt32(cItem.price).ToString(),
						FontSize = 20,
						FontFamily = new FontFamily("Baskerville Old Face"),
						Margin = new Thickness() { Right = 0, Top = 0 },
						HorizontalAlignment = HorizontalAlignment.Right,
						Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"))
					});
					runningTotal += decimal.ToInt32(cItem.price);
					paymentList.Items.Add(billItem);
				}
			}
			// Separator for coupon and tip section
			if (couponApplied || tipApplied)
				paymentList.Items.Add(S());
			// Print coupon discount
			if (couponApplied)
			{
				decimal discount = runningTotal * (decimal).1;
				discount = Math.Round(discount, 2);
				runningTotal -= discount;

				Grid billDiscount = new Grid()
				{
					Margin = new Thickness() { Left = 0, Right = 0 },
					HorizontalAlignment = HorizontalAlignment.Stretch,
					Width = 573
				};
				billDiscount.Children.Add(new Label()
				{
					Content = "Coupon (-10%)",
					FontSize = 20,
					FontFamily = new FontFamily("Baskerville Old Face"),
					Margin = new Thickness() { Left = 0, Top = 0 },
					HorizontalAlignment = HorizontalAlignment.Left,
					Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"))
				});
				billDiscount.Children.Add(new Label()
				{
					Content = "$" + discount.ToString(),
					FontSize = 20,
					FontFamily = new FontFamily("Baskerville Old Face"),
					Margin = new Thickness() { Right = 0, Top = 0 },
					HorizontalAlignment = HorizontalAlignment.Right,
					Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"))
				});
				paymentList.Items.Add(billDiscount);
			}
			// Calculate tax
			tax = runningTotal * (decimal).0625;
			tax = Math.Round(tax, 2);
			// Print tip
			if (tipApplied)
			{
				runningTotal += tip;

				Grid billTip = new Grid()
				{
					Margin = new Thickness() { Left = 0, Right = 0 },
					HorizontalAlignment = HorizontalAlignment.Stretch,
					Width = 573
				};
				billTip.Children.Add(new Label()
				{
					Content = "Tip",
					FontSize = 20,
					FontFamily = new FontFamily("Baskerville Old Face"),
					Margin = new Thickness() { Left = 0, Top = 0 },
					HorizontalAlignment = HorizontalAlignment.Left,
					Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"))
				});
				billTip.Children.Add(new Label()
				{
					Content = "$" + tip.ToString(),
					FontSize = 20,
					FontFamily = new FontFamily("Baskerville Old Face"),
					Margin = new Thickness() { Right = 0, Top = 0 },
					HorizontalAlignment = HorizontalAlignment.Right,
					Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"))
				});
				paymentList.Items.Add(billTip);
			}
			// Tax
			paymentList.Items.Add(S());
			runningTotal += tax;
			Grid billTax = new Grid()
			{
				Margin = new Thickness() { Left = 0, Right = 0 },
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Width = 573
			};
			billTax.Children.Add(new Label()
			{
				Content = "Tax (6.25%)",
				FontSize = 20,
				FontFamily = new FontFamily("Baskerville Old Face"),
				Margin = new Thickness() { Left = 0, Top = 0 },
				HorizontalAlignment = HorizontalAlignment.Left,
				Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"))
			});
			billTax.Children.Add(new Label()
			{
				Content = "$" + tax.ToString(),
				FontSize = 20,
				FontFamily = new FontFamily("Baskerville Old Face"),
				Margin = new Thickness() { Right = 0, Top = 0 },
				HorizontalAlignment = HorizontalAlignment.Right,
				Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"))
			});
			paymentList.Items.Add(billTax);
			// Total
			paymentList.Items.Add(S());
			Grid billTotal = new Grid()
			{
				Margin = new Thickness() { Left = 0, Right = 0 },
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Width = 573
			};
			billTotal.Children.Add(new Label()
			{
				Content = "Total",
				FontSize = 20,
				FontFamily = new FontFamily("Baskerville Old Face"),
				Margin = new Thickness() { Left = 0, Top = 0 },
				HorizontalAlignment = HorizontalAlignment.Left,
				Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"))
			});
			billTotal.Children.Add(new Label()
			{
				Content = "$" + runningTotal.ToString(),
				FontSize = 20,
				FontFamily = new FontFamily("Baskerville Old Face"),
				Margin = new Thickness() { Right = 0, Top = 0 },
				HorizontalAlignment = HorizontalAlignment.Right,
				Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"))
			});
			paymentList.Items.Add(billTotal);
		}

		#region |   Credit Card Fields   |
		/// <summary>
		/// Add expiration years to the credit card year comboBox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ccYear_Loaded(object sender, RoutedEventArgs e)
		{
			int year = DateTime.Now.Year;

			for (int i = 0; i < 4; i++)
			{
				ccYear.Items.Add(year);
				year++;
			}
		}
		/// <summary>
		/// Verify input for the credit card number
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ccNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			if (ccNumber.Text.Length != 19)
				validCC(ccNumber.Text + e.Text);

			if (ccNumber.Text.Length == 19 || !IsDigit(e.Text))
				e.Handled = true;
		}
		/// <summary>
		/// Verify input for the credit card cvv number
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ccCVV_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			if (ccCVV.Text.Length == 3 || !IsDigit(e.Text))
				e.Handled = true;
		}
		/// <summary>
		/// Fixes keyboard being open on first visit to the payment tab
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ccNumber_GotFocus(object sender, RoutedEventArgs e)
		{
			if (payFirst)
			{
				Keyboard.ClearFocus();
				payFirst = false;
				defaultKeyboard.Visibility = Visibility.Hidden;
				closeKeyboard.Visibility = Visibility.Hidden;
			}
		}
		/// <summary>
		/// Closes the keyboard
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void closeKeyboard_Click(object sender, RoutedEventArgs e)
		{
			defaultKeyboard.Visibility = Visibility.Hidden;
			closeKeyboard.Visibility = Visibility.Hidden;
		}
		/// <summary>
		/// Opens the keyboard
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ccField_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			defaultKeyboard.Visibility = Visibility.Visible;
			closeKeyboard.Visibility = Visibility.Visible;
		}
		/// <summary>
		/// Closes the keyboard
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ccField_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			defaultKeyboard.Visibility = Visibility.Hidden;
			closeKeyboard.Visibility = Visibility.Hidden;
		}
		/// <summary>
		/// Fixes keyboard being open on first visit to the payment tab
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void defaultKeyboard_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (!ccNumber.IsKeyboardFocused && ccNumber.IsFocused)
			{
				defaultKeyboard.Visibility = Visibility.Hidden;
			}
		}
		/// <summary>
		/// Fixes keyboard being open on first visit to the payment tab
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void closeKeyboard_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (!ccNumber.IsKeyboardFocused && ccNumber.IsFocused)
			{
				closeKeyboard.Visibility = Visibility.Hidden;
			}
		}
		#endregion

		#region |   Tip   |
		/// <summary>
		/// Open add tip overlay and submit given tip
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void addTip_Click(object sender, RoutedEventArgs e)
		{
			overlay.Visibility = Visibility.Visible;
			tipGrid.Visibility = Visibility.Visible;

			try
			{
				currentPercentage.Content = Convert.ToInt32(tipSlider.Value).ToString() + "%";
				currentTip.Content = "$" + (getTotal() * (18 * (decimal).01)).ToString();
			}
			catch (Exception)
			{
				currentPercentage.Content = "18%";
				currentTip.Content = "$" + (getTotal() * (18 * (decimal).01)).ToString();
			}
		}
		/// <summary>
		/// Returns the curent total of the bill
		/// </summary>
		/// <returns></returns>
		private decimal getTotal()
		{
			decimal runningTotal = 0;
			foreach (Cart oItem in sentOrders)
			{
				// Add all item prices
				foreach (cartItem cItem in oItem.Items)
				{
					runningTotal += decimal.ToInt32(cItem.price);
				}
				// Apply coupons
				if (couponApplied)
				{
					decimal discount = runningTotal * (decimal).1;
					discount = Math.Round(discount, 2);
					runningTotal -= discount;
				}
			}
			return runningTotal;
		}
		/// <summary>
		/// Adjust value of tip
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tipSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			try
			{
				currentPercentage.Content = Convert.ToInt32(tipSlider.Value).ToString() + "%";
				decimal temp = ((decimal)tipSlider.Value * (decimal).01);
				temp = Math.Round(temp, 2);
				currentTip.Content = "$" + (getTotal() * temp).ToString();
			}
			catch (Exception) { }
		}
		/// <summary>
		/// Close tip overlay
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cancelTip_Click(object sender, RoutedEventArgs e)
		{
			overlay.Visibility = Visibility.Hidden;
			tipGrid.Visibility = Visibility.Hidden;
		}
		/// <summary>
		/// Add tip to bill and close overlay
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void confirmTip_Click(object sender, RoutedEventArgs e)
		{
			decimal temp = ((decimal)tipSlider.Value * (decimal).01);
			temp = Math.Round(temp, 2);
			tip = (getTotal() * temp);
			tipApplied = true;
			overlay.Visibility = Visibility.Hidden;
			tipGrid.Visibility = Visibility.Hidden;
			updateBill();
		}
		#endregion

		#region |   Coupon   |
		/// <summary>
		/// Open add coupon overlay and adjust bill accordingly
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void addCoupon_Click(object sender, RoutedEventArgs e)
		{
			overlay.Visibility = Visibility.Visible;
			couponGrid.Visibility = Visibility.Visible;
		}
		/// <summary>
		/// Hide keyboard
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void billCoupon_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			defaultKeyboard.Visibility = Visibility.Hidden;
			cancelCouponAlternate.Visibility = Visibility.Hidden;
			Thickness tempMargin = new Thickness() { Top = 125, Bottom = 125, Left = 200, Right = 200 };
			couponGrid.Margin = tempMargin;
			Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
		}
		/// <summary>
		/// Display keyboard
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void billCoupon_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			defaultKeyboard.Visibility = Visibility.Visible;
			cancelCouponAlternate.Visibility = Visibility.Visible;
			Thickness tempMargin = couponGrid.Margin;
			tempMargin.Bottom += 300;
			tempMargin.Top -= 300;
			couponGrid.Margin = tempMargin;
		}
		/// <summary>
		/// WIP
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void billCoupon_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			if (billCoupon.Text.Length == 5)
				e.Handled = true;
		}
		/// <summary>
		/// WIP
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void billCoupon_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (billCoupon.Text.Length > 5)
				billCoupon.Text.Remove(5);
			e.Handled = true;
		}
		/// <summary>
		/// Applies coupon to the bill
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void applyCoupon_Click(object sender, RoutedEventArgs e)
		{
			bool valid = false;
			try
			{
				// Create an SqlConnection from the provided connection string.
				using (SqlConnection connection = new SqlConnection("Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;"))
				{
					// Formulate the command.
					SqlCommand command = new SqlCommand();
					command.Connection = connection;

					// Specify the query to be executed.
					command.CommandType = CommandType.Text;
					command.CommandText = @"
                    SELECT * FROM dbo.Coupons
                    ";
					// Open a connection to database.
					connection.Open();
					// Read data returned for the query.
					SqlDataReader reader = command.ExecuteReader();

					// while not done reading the stuff returned from the query
					while (reader.Read())
					{
						string temp = (string)reader[0];

						if (temp == billCoupon.Text.ToString())
							valid = true;
					}
					connection.Close();
				}

				if (valid == true)
				{
					using (SqlConnection connection = new SqlConnection("Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;"))
					{
						SqlCommand command = new SqlCommand(@"delete from dbo.Coupons where Code = @code", connection);
						command.Parameters.AddWithValue("@code", billCoupon.Text.ToString());
						
						connection.Open();
						command.ExecuteScalar();
						connection.Close();
					}

					couponApplied = true;
					addCoupon.IsEnabled = false;
					couponGrid.Visibility = Visibility.Hidden;
					overlay.Visibility = Visibility.Hidden;
					updateBill();
				}
				else
					MessageBox.Show("Invalid coupon code.");
			}
			catch (Exception) { }
		}
		/// <summary>
		/// Closes the coupon overlay
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cancelCoupon_Click(object sender, RoutedEventArgs e)
		{
			defaultKeyboard.Visibility = Visibility.Hidden;
			cancelCouponAlternate.Visibility = Visibility.Hidden;
			couponGrid.Visibility = Visibility.Hidden;
			overlay.Visibility = Visibility.Hidden;
		}
		#endregion

		#region |   Adjustment   |
		public void setAdjustment(decimal adj)
		{
			adjustment = adj;
			adjustmentApplied = true;
		}
		#endregion

		#region |   Payment   |
		/// <summary>
		/// Verify credit card and submit payment
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void submitPayment_Click(object sender, RoutedEventArgs e)
		{
			#region Validate fields
			string message = "";
			bool pass = true;
			// Verify the credit card number is valid
			if (validCC(ccNumber.Text))
			{
				message += "Invalid card number.\n";
				pass = false;
			}
			if (ccMonth.SelectedIndex == -1 || ccYear.SelectedIndex == -1)
			{
				message += "Invalid expiration date.\n";
				pass = false;
			}
			else if (ccYear.SelectedValue.ToString() == DateTime.Now.Year.ToString())
			{
				if (Convert.ToInt32(ccMonth.SelectedValue.ToString()) < DateTime.Now.Month)
				{
					message += "Invalid expiration date.\n";
					pass = false;
				}
			}
			if (ccCVV.Text.Length != 3)
			{
				message += "Invalid CVV code.\n";
				pass = false;
			}
			if (ccName.Text.Length < 3)
			{
				message += "Please enter your full name.\n";
				pass = false;
			}
			#endregion


		}
		/// <summary>
		/// Notify wait staff that customer wishes to pay with cash
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cashPayment_Click(object sender, RoutedEventArgs e)
		{
            commHelper.functionSend("cashPayment");
			waiterNotified.Visibility = Visibility.Visible;
        }
		/// <summary>
		/// Notify wait stagg that customer wishes to pay with a check
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void checkPayment_Click(object sender, RoutedEventArgs e)
		{
            commHelper.functionSend("checkPayment");
			waiterNotified.Visibility = Visibility.Visible;
		}

		public void markPaid()
		{

		}
		#endregion
		#endregion

		#region Service Dock
		#region |   Help Button  |
		/// <summary>
		/// Notify wait staff that the table has requested help
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
		#endregion

		#region |   Refill Button   |
		/// <summary>
		/// Display the refill overlay
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void refillButton_Click(object sender, RoutedEventArgs e)
		{
			addRefillItems();
			overlay.Visibility = Visibility.Visible;
			refillView.Visibility = Visibility.Visible;
		}
		/// <summary>
		/// Add all drinks in submitted orders to the refill list
		/// </summary>
		private void addRefillItems()
		{
			List<Refill> tempRefills = new List<Refill>();
			try
			{
				using (SqlConnection connection = new SqlConnection("Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;"))
				{
					// Formulate the command.
					SqlCommand command = new SqlCommand(@"SELECT * FROM dbo.Drinks where ip = @ip", connection);

					// Specify the query to be executed.
					command.CommandType = CommandType.Text;
					command.CommandText = @"SELECT * FROM dbo.Drinks";
					// Open a connection to database.
					connection.Open();
					// Read data returned for the query.
					SqlDataReader reader = command.ExecuteReader();

					// while not done reading the stuff returned from the query
					while (reader.Read())
					{   // fill up the Refill list object
						tempRefills.Add(new Refill
						{
							drink = (string)reader[0],
							ip = (string)reader[1]
						});
					}
				}
			}
			catch (Exception) { }

			refillList.Items.Clear();
			if (((Cart)order).Order_num != -1)
			{
				foreach (Cart oItem in sentOrders)
				{
					foreach (cartItem item in oItem.Items)
					{
						if (item.category == "drink")
						{
							bool requested = false;
							foreach (Refill r in tempRefills)
							{
								if (r.drink == item.name)
									requested = true;
							}
							Grid grid = new Grid()
							{
								Width = 832
							};
							// Drink Name
							grid.Children.Add(new Label()
							{
								Margin = new Thickness() { Left = 0, Top = 0 },
								HorizontalAlignment = HorizontalAlignment.Left,
								VerticalAlignment = VerticalAlignment.Top,
								FontFamily = new FontFamily("Baskerville Old Face"),
								FontSize = 20,
								Content = item.name,
								Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFACACAC"))
							});
							Button tempRequest = new Button();
							if (requested)
							{
								// Cancel Button
								tempRequest = (new Button()
								{
									Margin = new Thickness() { Right = 0 },
									Padding = new Thickness() { Right = 5, Left = 5 },
									HorizontalAlignment = HorizontalAlignment.Right,
									VerticalAlignment = VerticalAlignment.Center,
									FontFamily = new FontFamily("Baskerville Old Face"),
									FontSize = 20,
									Content = "Cancel Request",
									Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF701C1C")),
									BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30")),
									Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFACACAC"))
								});
							}
							else
							{
								// Request Button
								tempRequest = (new Button()
								{
									Margin = new Thickness() { Right = 0 },
									Padding = new Thickness() { Right = 5, Left = 5 },
									HorizontalAlignment = HorizontalAlignment.Right,
									VerticalAlignment = VerticalAlignment.Center,
									FontFamily = new FontFamily("Baskerville Old Face"),
									FontSize = 20,
									Content = "Request Refill",
									Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF701C1C")),
									BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30")),
									Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFACACAC"))
								});
							}
							tempRequest.Click += (sender, e) =>
							{
								Grid parent = GetAncestorOfType<Grid>(sender as Button);
								Label drinkName = parent.Children.OfType<Label>().FirstOrDefault();

								foreach (cartItem cItem in oItem.Items)
								{
									if (cItem.name == drinkName.Content.ToString())
									{
										if (((Button)sender).Content.ToString() == "Request Refill")
										{
											commHelper.functionSend("refillRequest");

											using (SqlConnection connection = new SqlConnection("Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;"))
											{
												SqlCommand command = new SqlCommand(@"insert into dbo.Drinks (Drinks, ip) values (@drink, @ip)", connection);
												command.Parameters.AddWithValue("@drink", cItem.name);
												command.Parameters.AddWithValue("@ip", Properties.Settings.Default.localIP);

												connection.Open();
												command.ExecuteScalar();
												connection.Close();
											}

											((Button)sender).Content = "Cancel Request";
										}
										else
										{
											commHelper.functionSend("cancelRefill");

											using (SqlConnection connection = new SqlConnection("Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;"))
											{
												SqlCommand command = new SqlCommand(@"delete from dbo.Drinks where Drinks = @drink and ip = @ip", connection);
												command.Parameters.AddWithValue("@drink", cItem.name);
												command.Parameters.AddWithValue("@ip", Properties.Settings.Default.localIP);

												connection.Open();
												command.ExecuteScalar();
												connection.Close();
											}

											((Button)sender).Content = "Request Refill";
										}
									}
								}
							};
							grid.Children.Add(tempRequest);

							refillList.Items.Add(grid);
						}
					}
				}
			}
		}
		/// <summary>
		/// Close the refill overlay
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void closeRefill_Click(object sender, RoutedEventArgs e)
		{
			refillView.Visibility = Visibility.Hidden;
			overlay.Visibility = Visibility.Hidden;
		}
		#endregion

		#region |   Cart Button   |
		/// <summary>
		/// Display the cart overlay
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cartButton_Click(object sender, RoutedEventArgs e)
		{
			addCartItems();
			overlay.Visibility = Visibility.Visible;
			cartView.Visibility = Visibility.Visible;
		}
		/// <summary>
		/// Adds all order items to cart list
		/// </summary>
		private void addCartItems()
		{
			cartList.Items.Clear();
			for (int i = 0; i < ((Cart)order).Items.Count; i++)
			{
				Grid item = new Grid()
				{
					Width = 730
				};
				// Item
				item.Children.Add(new Label()
				{
					Margin = new Thickness() { Left = 0, Top = 0 },
					HorizontalAlignment = HorizontalAlignment.Left,
					VerticalAlignment = VerticalAlignment.Top,
					FontFamily = new FontFamily("Baskerville Old Face"),
					FontSize = 20,
					Content = ((Cart)order).Items[i].name,
					Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFACACAC"))
				});

				if (((Cart)order).Items[i].category == "entree") // For entrees only
				{
					string[] lines = ((Cart)order).Notes[i].Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
					// Soup/Salad
					item.Children.Add(new Label()
					{
						Margin = new Thickness() { Left = 50, Top = 33 },
						HorizontalAlignment = HorizontalAlignment.Left,
						VerticalAlignment = VerticalAlignment.Top,
						FontFamily = new FontFamily("Baskerville Old Face"),
						FontSize = 20,
						Content = lines[0],
						Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFACACAC"))
					});
					// Side
					item.Children.Add(new Label()
					{
						Margin = new Thickness() { Left = 50, Top = 66 },
						HorizontalAlignment = HorizontalAlignment.Left,
						VerticalAlignment = VerticalAlignment.Top,
						FontFamily = new FontFamily("Baskerville Old Face"),
						FontSize = 20,
						Content = lines[1],
						Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFACACAC"))
					});
					// Notes
					if (lines[2].Length > 7)
					{
						item.Children.Add(new TextBlock()
						{
							Margin = new Thickness() { Left = 50, Top = 99 },
							Width = 520,
							HorizontalAlignment = HorizontalAlignment.Left,
							VerticalAlignment = VerticalAlignment.Top,
							FontFamily = new FontFamily("Baskerville Old Face"),
							FontSize = 20,
							Text = lines[2],
							Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFACACAC")),
							TextWrapping = TextWrapping.Wrap
						});
					}
				}
				else // For all other items
				{
					// Notes
					if (((Cart)order).Notes[i].Length > 7)
					{
						item.Children.Add(new TextBlock()
						{
							Margin = new Thickness() { Left = 50, Top = 33 },
							Width = 520,
							HorizontalAlignment = HorizontalAlignment.Left,
							VerticalAlignment = VerticalAlignment.Top,
							FontFamily = new FontFamily("Baskerville Old Face"),
							FontSize = 20,
							Text = ((Cart)order).Notes[i],
							Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFACACAC")),
							TextWrapping = TextWrapping.Wrap
						});
					}
				}
				// Remove button
				Button tempRemove = (new Button()
				{
					Margin = new Thickness() { Right = 0 },
					Padding = new Thickness() { Right = 5, Left = 5},
					HorizontalAlignment = HorizontalAlignment.Right,
					VerticalAlignment = VerticalAlignment.Center,
					FontFamily = new FontFamily("Baskerville Old Face"),
					FontSize = 20,
					Content = "Remove",
					Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF701C1C")),
					BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30")),
					Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFACACAC"))
				});
				tempRemove.Click += (sender, e) =>
				{
					Grid parent = GetAncestorOfType<Grid>(sender as Button);
					Label removeName  = parent.Children.OfType<Label>().FirstOrDefault();
					Label removeSalad = new Label(), removeSide = new Label(), removeNotes = new Label();
					string buildNotes = "";

					foreach (menuItem iter in myMenu)
					{
						if (iter.name == removeName.Content.ToString() && iter.category == "entree")
						{
							try
							{
								removeSalad = parent.Children.OfType<Label>().ElementAt(1);
								removeSide = parent.Children.OfType<Label>().ElementAt(2);
								removeNotes = parent.Children.OfType<Label>().ElementAt(3);
							}
							catch (Exception)
							{
								removeNotes.Content = "Notes: ";
							}

							buildNotes = removeSalad.Content.ToString() + Environment.NewLine + removeSide.Content.ToString() + Environment.NewLine + removeNotes.Content.ToString();
							break;
						}
							
					}

					for (int j = 0; j < ((Cart)order).Items.Count; j++)
					{
						if (removeName.Content.ToString() == ((Cart)order).Items[j].name && buildNotes == ((Cart)order).Notes[j])
						{
							((Cart)order).Items.RemoveAt(j);
							((Cart)order).Notes.RemoveAt(j);
							addCartItems();
							return;
						}
					}

				};
				item.Children.Add(tempRemove);

				cartList.Items.Add(item);
			}
			if (cartList.Items.Count != 0)
				submitCart.IsEnabled = true;
		}
		/// <summary>
		/// Close cart overlay
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void closeCart_Click(object sender, RoutedEventArgs e)
		{
			cartView.Visibility = Visibility.Hidden;
			overlay.Visibility = Visibility.Hidden;
		}
		/// <summary>
		/// Open overlay to verify customer wishes to submit the cart
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void submitCart_Click(object sender, RoutedEventArgs e)
		{
			verifyOrderSubmission.Visibility = Visibility.Visible;
		}
		/// <summary>
		/// Submit the cart to the database
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void yesSubmit_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				using (SqlConnection openCon = new SqlConnection("Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;"))
				{
					using (SqlCommand querySave = new SqlCommand("insert into dbo.Orders ([Order], Client) values (@order, @client) set @Id = SCOPE_IDENTITY()", openCon))
					{
						querySave.Parameters.AddWithValue("@order", ObjectToByteArray(order));
						querySave.Parameters.AddWithValue("@client", Properties.Settings.Default.localIP);
						querySave.Parameters.Add("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;

						openCon.Open();
						querySave.ExecuteScalar();
						((Cart)order).Order_num = (int)querySave.Parameters["@Id"].Value;
						openCon.Close();
						commHelper.functionSend("updateOrders");
						sentOrders.Add(order);
						order = new Cart();
						addCartItems();

						addedAlert.Content = "Order submitted to kitchen!";
						verifyOrderSubmission.Visibility = Visibility.Hidden;
						cartView.Visibility = Visibility.Hidden;
						submitCart.IsEnabled = false;
						addedAlert.Visibility = Visibility.Visible;
						Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
						Thread.Sleep(1000);
						addedAlert.Visibility = Visibility.Hidden;
						overlay.Visibility = Visibility.Hidden;
					}
				}
				commHelper.functionSend("orderSubmitted");
			}
			catch (Exception) { }

			updateBill();
		}
		/// <summary>
		/// Return to the cart overlay
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void noSubmit_Click(object sender, RoutedEventArgs e)
		{
			verifyOrderSubmission.Visibility = Visibility.Hidden;
		}
		#endregion
		#endregion

		#region Helper Functions
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
		/// <summary>
		/// Converts an object to a byte array
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		private byte[] ObjectToByteArray(object obj)
		{
			if (obj == null)
				return null;
			BinaryFormatter bf = new BinaryFormatter();
			using (MemoryStream ms = new MemoryStream())
			{
				bf.Serialize(ms, obj);
				ms.Position = 0;
				return ms.ToArray();
			}
		}
		/// <summary>
		/// Gets the a parent of specified type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="child"></param>
		/// <returns></returns>
		public T GetAncestorOfType<T>(FrameworkElement child) where T : FrameworkElement
		{
			var parent = VisualTreeHelper.GetParent(child);
			if (parent != null && !(parent is T))
				return GetAncestorOfType<T>((FrameworkElement)parent);
			return (T)parent;
		}
		/// <summary>
		/// Verify the provided string is a valid credit card number
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		private bool validCC(string number)
		{
			number.Replace(" ", string.Empty);
			try
			{
				ccLogo.Source = null;
				switch (number[0])
				{
					case '2': // enRoute
						if ((number.Substring(0, 4) == "2014" || number.Substring(0, 4) == "2149") && number.Length == 15)
							ccLogo.Source = new BitmapImage(new Uri("Resources/CreditCards/enroute.jpg", UriKind.Relative));
						break;
					case '3': // American Express, Diners Club, JCB
						if ((number[1] == '4' || number[1] == '7') && number.Length == 15)
							ccLogo.Source = new BitmapImage(new Uri("Resources/CreditCards/amex.jpg", UriKind.Relative));
						else if ((number.Substring(0, 4) == "3088" || number.Substring(0, 4) == "3096" || number.Substring(0, 4) == "3112" ||
								number.Substring(0, 4) == "3158" || number.Substring(0, 4) == "3337") && number.Length == 16)
							ccLogo.Source = new BitmapImage(new Uri("Resources/CreditCards/jcb.jpg", UriKind.Relative));
						else if ((number[1] == '0' || number[1] == '6' || number[1] == '8') && number.Length == 14)
							ccLogo.Source = new BitmapImage(new Uri("Resources/CreditCards/diners.jpg", UriKind.Relative));
						break;
					case '4': // Visa
						if (number.Length == 13 || number.Length == 16)
							ccLogo.Source = new BitmapImage(new Uri("Resources/CreditCards/visa.jpg", UriKind.Relative));
						break;
					case '5': // MasterCard
						if (Between(Convert.ToInt32(number.Substring(1, 1)), R(1, 5)) && number.Length == 16)
							ccLogo.Source = new BitmapImage(new Uri("Resources/CreditCards/mc.jpg", UriKind.Relative));
						break;
					case '6': // Discover
						if ((Between(Convert.ToInt32(number.Substring(0, 8)), R(60110000, 60119999)) ||
							Between(Convert.ToInt32(number.Substring(0, 8)), R(65000000, 65009999)) ||
							Between(Convert.ToInt32(number.Substring(0, 8)), R(62212600, 62292599))) &&
							number.Length == 16)
							ccLogo.Source = new BitmapImage(new Uri("Resources/CreditCards/discover.jpg", UriKind.Relative));
						break;
					default:
						break;
				}

				if (ccLogo.Source != null)
					return true;
			}
			catch (Exception) { return false; }

			return false;
		}
		// Represents a range of numbers
		class Range
		{
			public Range(int left, int right)
			{
				Left = left;
				Right = right;
			}

			public int Left { get; set; }
			public int Right { get; set; }
		}
		/// <summary>
		/// Returns a range with the provided bounds
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		Range R(int left, int right)
		{
			return new Range(left, right);
		}
		/// <summary>
		/// Checks if a given value is in the given range
		/// </summary>
		/// <param name="value"></param>
		/// <param name="range"></param>
		/// <returns></returns>
		private bool Between(int value, Range range)
		{
			if (value >= range.Left && value <= range.Right)
				return true;

			return false;
		}
		/// <summary>
		/// Formats provided phone number
		/// </summary>
		/// <param name="pass"></param>
		public string formatPhoneNumber(string pass)
		{
			return"(" + pass[0] + pass[1] + pass[2] + ") " + pass[3] + pass[4] + pass[5] + "-" + pass[6] + pass[7] + pass[8] + pass[9];
		}
		/// <summary>
		/// Sets current member and updates information on screen
		/// </summary>
		/// <param name="mem"></param>
		public void setCurrentMember(rewardMember mem)
		{
			currentMember = mem;
			welcomeName.Content = "Welcome, " + currentMember.firstName + "!";
			visitCount.Content = currentMember.points.ToString();
			try
			{
				string[] codes = currentMember.discountCodes.Split(',');
				memberCoupons.Items.Clear();
				foreach (string i in codes)
				{
					memberCoupons.Items.Add(i);
				}
			}
			catch (Exception) { }
		}
		#endregion
	}
}
