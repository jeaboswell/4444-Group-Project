using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
//using System.Windows.Forms;
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
	/// Interaction logic for Kitchen.xaml
	/// </summary>
	public partial class Kitchen : UserControl
	{
        List<menuItem> items = new List<menuItem>();
		public Kitchen()
		{
			InitializeComponent();
            menuItem sample = new menuItem();
            sample.name = "lol";
            items.Add(sample);

            itemCbox.DataContext = items;

            foreach (menuItem item in items)
            {

            }



		}

        private void backButton_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {

        }

        private void backHomeMenu(object sender, RoutedEventArgs e)
        {

        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
