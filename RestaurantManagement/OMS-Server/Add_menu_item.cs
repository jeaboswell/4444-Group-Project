using System;
using System.Windows.Forms;
using System.Data.SqlClient;  // System.Data.dll
using System.IO;

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
                string command = "INSERT INTO dbo.Menu (ItemName, Description, Price, category, Photo)";
                command += "VALUES (@ItemName, @Description, @Price, @category, @Photo)";

                using (SqlCommand myCommand = new SqlCommand(command, openCon))
                {
                    myCommand.Parameters.AddWithValue("@ItemName", textBox1.Text);
                    myCommand.Parameters.AddWithValue("@Price", textBox2.Text);
                    myCommand.Parameters.AddWithValue("@Photo", File.ReadAllBytes(textBox4.Text)); // can't set Null like this, will have to add photo after the query i guess
                    myCommand.Parameters.AddWithValue("@category", textBox5.Text);
                    myCommand.Parameters.AddWithValue("@Description", textBox3.Text);
                    openCon.Open();
                    myCommand.ExecuteNonQuery();
                    openCon.Close();
                }
            }

            MessageBox.Show("Item added!");
			//MessageBox.Show(textBox1.Text);
			Close();
        }
	}
}
