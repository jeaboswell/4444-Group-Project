using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;  // System.Data.dll

namespace OMS
{
    public partial class add_item : Form
    {
        public add_item()
        {
            InitializeComponent();
        }

        private void add_item_Load(object sender, EventArgs e)
        {

        }

        private void add_to_DB_Click(object sender, EventArgs e)
        {


            using (SqlConnection openCon = new SqlConnection("Server=tcp:omsdb.database.windows.net,1433;Database=OMSDB;User ID=csce4444@omsdb;Password=Pineapple!;"))
            {
                string command = "INSERT INTO dbo.Menu (ItemName, Description, Price, category)";
                command += "VALUES (@ItemName, @Description, @Price, @category)";

                using (SqlCommand myCommand = new SqlCommand(command, openCon))
                {
                    myCommand.Parameters.AddWithValue("@ItemName", textBox1.Text);
                    myCommand.Parameters.AddWithValue("@Price", textBox2.Text);
                    //myCommand.Parameters.AddWithValue("@Photo", DBNull.Value); // can't set Null like this, will have to add photo after the query i guess
                    myCommand.Parameters.AddWithValue("@category", textBox5.Text);
                    myCommand.Parameters.AddWithValue("@Description", textBox3.Text);
                    openCon.Open();
                    myCommand.ExecuteNonQuery();
                    openCon.Close();
                }
            }


            MessageBox.Show("Item added!");
            //MessageBox.Show(textBox1.Text);
            this.Close();
        }
    }
}
