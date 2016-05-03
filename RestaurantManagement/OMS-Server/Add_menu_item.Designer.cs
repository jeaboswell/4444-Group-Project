namespace OMS
{
    partial class add_item
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.textBox4 = new System.Windows.Forms.TextBox();
			this.textBox5 = new System.Windows.Forms.TextBox();
			this.add_to_DB = new System.Windows.Forms.Button();
			this.item_name = new System.Windows.Forms.Label();
			this.item_price = new System.Windows.Forms.Label();
			this.item_cate = new System.Windows.Forms.Label();
			this.photo_loc = new System.Windows.Forms.Label();
			this.item_descr = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(70, 49);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(100, 20);
			this.textBox1.TabIndex = 0;
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(222, 49);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(100, 20);
			this.textBox2.TabIndex = 1;
			// 
			// textBox3
			// 
			this.textBox3.Location = new System.Drawing.Point(222, 107);
			this.textBox3.Name = "textBox3";
			this.textBox3.Size = new System.Drawing.Size(243, 20);
			this.textBox3.TabIndex = 4;
			// 
			// textBox4
			// 
			this.textBox4.Location = new System.Drawing.Point(70, 107);
			this.textBox4.Name = "textBox4";
			this.textBox4.Size = new System.Drawing.Size(100, 20);
			this.textBox4.TabIndex = 3;
			// 
			// textBox5
			// 
			this.textBox5.Location = new System.Drawing.Point(365, 49);
			this.textBox5.Name = "textBox5";
			this.textBox5.Size = new System.Drawing.Size(100, 20);
			this.textBox5.TabIndex = 2;
			// 
			// add_to_DB
			// 
			this.add_to_DB.Location = new System.Drawing.Point(222, 178);
			this.add_to_DB.Name = "add_to_DB";
			this.add_to_DB.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.add_to_DB.Size = new System.Drawing.Size(100, 44);
			this.add_to_DB.TabIndex = 5;
			this.add_to_DB.Text = "Add to Database";
			this.add_to_DB.UseVisualStyleBackColor = true;
			this.add_to_DB.Click += new System.EventHandler(this.add_to_DB_Click);
			// 
			// item_name
			// 
			this.item_name.AutoSize = true;
			this.item_name.Location = new System.Drawing.Point(67, 30);
			this.item_name.Name = "item_name";
			this.item_name.Size = new System.Drawing.Size(58, 13);
			this.item_name.TabIndex = 6;
			this.item_name.Text = "Item Name";
			// 
			// item_price
			// 
			this.item_price.AutoSize = true;
			this.item_price.Location = new System.Drawing.Point(219, 30);
			this.item_price.Name = "item_price";
			this.item_price.Size = new System.Drawing.Size(31, 13);
			this.item_price.TabIndex = 7;
			this.item_price.Text = "Price";
			// 
			// item_cate
			// 
			this.item_cate.AutoSize = true;
			this.item_cate.Location = new System.Drawing.Point(362, 30);
			this.item_cate.Name = "item_cate";
			this.item_cate.Size = new System.Drawing.Size(72, 13);
			this.item_cate.TabIndex = 8;
			this.item_cate.Text = "Item Category";
			// 
			// photo_loc
			// 
			this.photo_loc.AutoSize = true;
			this.photo_loc.Location = new System.Drawing.Point(67, 91);
			this.photo_loc.Name = "photo_loc";
			this.photo_loc.Size = new System.Drawing.Size(79, 13);
			this.photo_loc.TabIndex = 9;
			this.photo_loc.Text = "Photo Location";
			// 
			// item_descr
			// 
			this.item_descr.AutoSize = true;
			this.item_descr.Location = new System.Drawing.Point(219, 91);
			this.item_descr.Name = "item_descr";
			this.item_descr.Size = new System.Drawing.Size(83, 13);
			this.item_descr.TabIndex = 10;
			this.item_descr.Text = "Item Description";
			// 
			// add_item
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(540, 255);
			this.Controls.Add(this.item_descr);
			this.Controls.Add(this.photo_loc);
			this.Controls.Add(this.item_cate);
			this.Controls.Add(this.item_price);
			this.Controls.Add(this.item_name);
			this.Controls.Add(this.add_to_DB);
			this.Controls.Add(this.textBox5);
			this.Controls.Add(this.textBox4);
			this.Controls.Add(this.textBox3);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.textBox1);
			this.Name = "add_item";
			this.Text = "Add Menu Item";
			this.Load += new System.EventHandler(this.add_item_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Button add_to_DB;
        private System.Windows.Forms.Label item_name;
        private System.Windows.Forms.Label item_price;
        private System.Windows.Forms.Label item_cate;
        private System.Windows.Forms.Label photo_loc;
        private System.Windows.Forms.Label item_descr;
    }
}