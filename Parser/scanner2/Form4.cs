using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace scanner
{
    public partial class Form4 : Form
    {

        List<tokens> token_list = new List<tokens>();

        // Data grid view
        public static Panel panels = new Panel();
        DataTable token_table = new DataTable();

        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            // data grid view
            token_table.Columns.Add("Token Value", typeof(string));
            token_table.Columns.Add("Token Type", typeof(string));
            dataGridView1.DataSource = token_table;

        }

        private void button2_Click(object sender, EventArgs e)
        {

            var FD = new System.Windows.Forms.OpenFileDialog();
            if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                string fileToOpen = FD.FileName;

                // System.IO.FileInfo File = new System.IO.FileInfo(FD.FileName);
                try
                {
                    if (FD.OpenFile() != null)
                    {
                        string[] lines = System.IO.File.ReadAllLines(fileToOpen.ToString());
                        string[] words;
                        foreach (var line in lines)
                        {
                            words = line.Trim(' ', '\n', '\r').Split(',');

                            // note: trim is used here to remove the extra space from the token type
                            token_list.Add(new tokens(words[1].Trim(' '), words[0], 0, 0, ""));
                        }
                    }

                    Form2 F2 = new Form2(token_list);
                    F2.ShowDialog();
                }
                catch (Exception)
                {
                    MessageBox.Show("Error: Could not read file from disk.");
                }

            }

            
            
        }

        private void button3_Click(object sender, EventArgs e)
        {

            /*--------------------------------------------------------------------*/
            int rowCount = token_table.Rows.Count; // number of rows in data grid view

            for (int i = 0; i < rowCount; i++)
            {
                var val = dataGridView1.Rows[i].Cells[0].Value; // take value of cell
                string value;
                value = val.ToString(); // change it to string

                var ty = dataGridView1.Rows[i].Cells[1].Value; // take value of cell
                string type;
                type = ty.ToString(); // change it to string

                token_list.Add(new tokens(type, value, 0, 0, ""));
                
            }

            this.Close();

            if (rowCount > 0)
            {
                Form2 F2 = new Form2(token_list);
                F2.Show();
            }


        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        
    }
}
