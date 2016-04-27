using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

[Serializable]

class ClientInfo
{
    public IPAddress IP { get; set; }

    public string Name { get; set; }

    public List<string> permissionList { get; set; } = new List<string>() { "None", "Manager", "Waiter", "Kitchen", "Table" };

    public string selectedPermission { get; set; }
}

namespace OMS
{
    /// <summary>
    /// Interaction logic for EmployeeInterface.xaml
    /// </summary>
    public partial class EmployeeInterface : UserControl
    {
        public EmployeeInterface()
        {
            InitializeComponent();
        }

        private void updateTables()
        {
            commHelper.functionSend("getTables");

            IPAddress tempAdr = IPAddress.Parse(Properties.Settings.Default.serverIP);
            IPEndPoint serverEp = new IPEndPoint(tempAdr, 0);
            UdpClient server = new UdpClient(44446);

            
            
                try
                {
                    byte[] command = server.Receive(ref serverEp);

                }
                catch (Exception ex) { }
            }
            server.Close();
        
        }
}

