using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

enum token_type
{
    reserved,
    symobl,
    IDENTIFIER,
    NUMBER
};

public struct tokens
{
    public string type;
    public string value;
    public int x_axis;
    public int y_axis;
    public string node;
    public tokens(string t, string v, int x, int y, string node_in)
    {
        type = t;
        value = v;
        x_axis = x;
        y_axis = y;
        node = node_in;
    }
};

namespace scanner
{

    public partial class Form5 : Form
    {



        List<tokens> token_list = new List<tokens>();

        int error_checking = 0;

        string line;
        string[] reserved = { "if", "then", "else", "end", "repeat", "until", "write", "read" };

        // Data grid view
        public static Panel panels = new Panel();
        DataTable token_table = new DataTable();


        public Form5()
        {
            InitializeComponent();
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            // data grid view
            token_table.Columns.Add("Token Value", typeof(string));
            token_table.Columns.Add("Token Type", typeof(string));
            dataGridView1.DataSource = token_table;

        }
        string symbols(char word)
        {

            switch (word)
            {
                case '+':
                    return "PULS";
                case '-':
                    return "MINUS";
                case '*':
                    return "MULT";
                case '/':
                    return "DIV";
                case '<':
                    return "LESSTHAN";
                case '>':
                    return "GreaterThan";
                case '=':
                    return "Equal";
                case '[':
                    return "Left bracket";
                case ']':
                    return "OPENBRACKET";
                case '(':
                    return "CLOSEDBRACKET";
                case ')':
                    return "Right Parenthese";
                default:
                    return "other";
            }


        }
        //check if the word is a reserved kewword or not
        int Is_Reserved(string each_word_in_each_line)
        {
            int check = -1;
            for (int i = 0; i < reserved.Length; i++)
            {
                if (string.Equals(each_word_in_each_line, reserved[i]))
                {
                    check = i;
                    break;
                }
            }

            return check;

        }

        void check(string line)
        {
            char[] sperator2 = { ' ', '}' };
            string[] new_line = line.Split(sperator2, StringSplitOptions.RemoveEmptyEntries);
            string num = "", word = "";

            if (error_checking == 0)
            {
                foreach (string s in new_line)
                {
                    num = ""; word = "";

                    if (s[0] == '{')
                        break;

                    if (Is_Reserved(s) != -1)
                    {
                        token_table.Rows.Add(s, reserved[Is_Reserved(s)].ToUpper() + "   Reserved");
                        token_list.Add(new tokens(reserved[Is_Reserved(s)].ToUpper(), s, 0, 0, ""));

                        continue;
                    }

                    for (int i = 0; i < s.Length; i++)
                    {

                        if ((s[i] == ':') && (s[i + 1] == '='))
                        {
                            if (num != "")
                            {
                                token_table.Rows.Add(num, "NUMBER");
                                token_list.Add(new tokens("NUMBER", num.ToString(), 0, 0, ""));
                                num = "";
                            }
                            else if (word != "")
                            {
                                token_table.Rows.Add(word, "IDENTIFIER");
                                token_list.Add(new tokens("IDENTIFIER", word, 0, 0, ""));

                                word = "";
                            }
                            token_table.Rows.Add(":=", "ASSIGN");
                            token_list.Add(new tokens("ASSIGN", ":=", 0, 0, ""));


                            i++;
                        }
                        else if ((s[i] == ';'))
                        {
                            if (num != "")
                            {
                                token_table.Rows.Add(num, "NUMBER");

                                //new
                                token_list.Add(new tokens("NUMBER", num.ToString(), 0, 0, ""));
                                //end new

                                num = "";
                            }
                            else if (word != "")
                            {
                                token_table.Rows.Add(word, "IDENTIFIER");

                                //new
                                token_list.Add(new tokens("IDENTIFIER", word, 0, 0, ""));
                                //end new


                                word = "";
                            }
                            token_table.Rows.Add(";", "Separator");
                            token_list.Add(new tokens("SEMICOLON", ";", 0, 0, ""));


                        }
                        else if (symbols(s[i]) != "other")
                        {
                            if (num != "")
                            {
                                token_table.Rows.Add(num, "NUMBER");

                                //new
                                token_list.Add(new tokens("NUMBER", num, 0, 0, ""));
                                //end new

                                num = "";
                            }
                            else if (word != "")
                            {
                                token_table.Rows.Add(word, "IDENTIFIER");

                                //new
                                token_list.Add(new tokens("IDENTIFIER", word, 0, 0, ""));
                                //end new

                                word = "";
                            }

                            token_table.Rows.Add(s[i], symbols(s[i]));

                            //new
                            token_list.Add(new tokens(symbols(s[i]).ToString(), s[i].ToString(), 0, 0, ""));
                            //end new

                        }
                        else if (Char.IsDigit(s[i]))
                            num += s[i];

                        else if (Char.IsLetter(s[i]))
                            word += s[i];

                        if (num != "" && word != "")
                        {
                            var result = MessageBox.Show("Undefined Token..Do you want to edit the code", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            error_checking = 1;
                            token_table.Rows.Clear();

                            return;
                        }
                    }

                    if (num != "")
                    {

                        token_table.Rows.Add(num, "NUMBER");


                        token_list.Add(new tokens("NUMBER", num.ToString(), 0, 0, ""));

                    }
                    else if (word != "")
                    {

                        token_table.Rows.Add(word, "IDENTIFIER");

                        token_list.Add(new tokens("IDENTIFIER", word, 0, 0, ""));
                    }

                }
            }



        }


        private void button1_Click(object sender, EventArgs e)
        {
            char[] sperator = { '\n' };
            char[] sperator2 = { ' ', '}' };
            line = textBox1.Text;

            // separate each line of the code in a certain index in a string array
            string[] new_line = line.Split(sperator, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < new_line.Length; i++)
            {
                new_line[i] = new_line[i].Replace('\r', ' ');
            }


            foreach (string a in new_line)
            {
                check(a);
            }

            Form2 F2 = new Form2(token_list);
            F2.ShowDialog();


        }

        private void button2_Click(object sender, EventArgs e)
        {
            error_checking = 0;
            token_list.Clear();
            token_table.Rows.Clear();
            textBox1.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            error_checking = 0;
            token_table.Rows.Clear();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {

        }
    }


}
