﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
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
        List<ClientInfo> TableList = new List<ClientInfo>;

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
                TableList = (List<ClientInfo>)ByteToObject(command);
            }
            catch (Exception) { }
        
            server.Close();

            foreach (ClientInfo iter in TableList)
            {
                Button tmpButton = new Button();
                tmpButton.Content = iter.Name;
                tmpButton.Click += (s, e) => { /*Add code here to determine what happens when table button is clicked*/};
                tmpButton.Height = 100;
                tmpButton.Width = 100;
                Table_Grid.Children.Add(tmpButton);
            }

        }



        private object ByteToObject(byte[] byteArray)
        {
            try
            {
                MemoryStream ms = new MemoryStream(byteArray);
                BinaryFormatter bf = new BinaryFormatter();
                ms.Position = 0;

                return bf.Deserialize(ms);
            }
            catch (Exception) { }

            return null;
        }
    }
}

