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
    public partial class Form2 : Form
    {
        List<tokens> list_token = new List<tokens>();
        Graphics s;
        int index = 0;
        static tokens temp;
        int x = 50, y = 10;
        int i = 0;
        int flag = 1;
        int invalid_rule_flag = 0;
        Brush a1 = new SolidBrush(Color.BlueViolet);
        Brush a2 = new SolidBrush(Color.White);
        Font Fo1 = new Font("Tahoma", 8);

        Stack<int> stack_else_x = new Stack<int>();
        Stack<int> stack_else_y = new Stack<int>();

        Stack<int> stack_end_x = new Stack<int>();
        Stack<int> stack_end_y = new Stack<int>();

        Stack<int> stack_until_x = new Stack<int>();
        Stack<int> stack_until_y = new Stack<int>();

        public Form2(List<tokens> input_token)
        {
            InitializeComponent();
            list_token = input_token;
        }

        void match(string exp)
        {
            temp = list_token[index];
            if (temp.type == exp)
            {
                textBox1.Text = temp.type + "   " + temp.value + "\n";
                if (list_token.Count > index + 1)
                    index++;

                temp = list_token[index];
            }
            else
            {
                flag = 0;
                var result = MessageBox.Show("Invalid Rule!");
                invalid_rule_flag = 1;
                if(result.ToString() == "OK")
                {
                    
                    Form1 F1 = new Form1();
                    F1.Show();
                    this.Hide();
                }

            }
            
            

        }
        public void stmt_sequence()
        {       
            statement();
            temp = list_token[index];
            while (temp.value == ";")
            {
                match(temp.type);
                statement();

            }
        }
        public void statement()
        {
            temp = list_token[index];
            switch (temp.type)
            {
                case "IF":
                    if_stmt();
                    break;
                case "REPEAT":
                    repeat();
                    break;
                case "IDENTIFIER":
                    assign();
                    break;
                case "READ":
                    read();
                    break;
                case "WRITE":
                    write();
                    break;
                default:
                    var result = MessageBox.Show("Invalid Rule!");
                    invalid_rule_flag = 1;
                    if (result.ToString() == "OK")
                    {
                        this.Close();
                        Form1 F1 = new Form1();
                        F1.ShowDialog();
                        this.Hide();
                    }
                    break;
            }


        }
        void if_stmt()
        {
            temp = list_token[index];
            match("IF");
            exp();
            temp = list_token[index];
            match("THEN");
            stmt_sequence();
            if (temp.type == "ELSE")
            {
                match(temp.type);
                stmt_sequence();
            }
            match("END");
        }

        void exp()
        {
            temp = list_token[index];
            simple_exp();

            if (temp.value == "<" || temp.value == ">" || temp.value == "=")
            {
                match(temp.type);
                simple_exp();
            }

        }


        public void repeat()
        {
            match("REPEAT");
            stmt_sequence();
            match("UNTIL");
            exp();

        }
        public void read()
        {
            match("READ");
            match("IDENTIFIER");


        }

        public void write()
        {
            match("WRITE");
            exp();
        }
        public void assign()
        {
            match("IDENTIFIER");
            match("ASSIGN");
            exp();
        }

        void simple_exp()
        {
            temp = list_token[index];
            term();

            while (temp.value == "+" || temp.value == "-")
            {
                match(temp.type);
                term();
            }
        }

        void term()
        {
            temp = list_token[index];
            factor();

            while (temp.value == "*" || temp.value == "/")
            {
                match(temp.type);
                factor();
            }
        }

        void factor()
        {
            temp = list_token[index];
            if (temp.type == "Left Parentheses")
            {
                match(temp.type);
                exp();
                match("Right Parenthese");
            }
            else if (temp.type == "NUMBER")
                match("NUMBER");
            else if (temp.type == "IDENTIFIER")
                match("IDENTIFIER");
        }





        //---------------------------------------------Drawing functions---------------------------------------------------------
        void draw_statement(int x, int y, string info) // rectangle
        {
            s = panel2.CreateGraphics();            
            s.FillRectangle(a1, x, y, 50, 50);
            s.DrawString(info, Fo1, a2, (x+5), y);          
        }

        void draw_expression(int x1, int y1, string info, int r1 = 50) // oval
        {
            s = panel2.CreateGraphics();
            s.FillEllipse(a1, x1, y1, r1 + 20 , r1);
            s.DrawString(info, new Font("Tahoma", 10), Brushes.Black, x1 + 5, y1 + 5);           
        }
        void draw_line(int x1, int y1, int x2, int y2)
        {
            Pen p = new Pen(Color.PaleGoldenrod, 5);
            s = panel2.CreateGraphics();
            s.DrawLine(p, x1, y1, x2, y2);
        }


        void P_Read()
        {
            int d = i+1;

            draw_statement(x, y, "READ" + Environment.NewLine + list_token[++i].value);
            if (list_token.Count > i + 1 && list_token[++d].value != "end" && list_token[d].value != "else" && list_token[d].value != "until")
                draw_line((x + 50), (y + 25), (x + 200), (y + 25)); // length of line =50
            x = x + 200; // update x value
        }

        void P_WRITE()
        {           
            draw_statement(x, y, "WRITE");
            draw_line((x + 25), (y + 50), (x + 25), (y + 75)); // length of line =25
            draw_expression(x, (y + 75), list_token[++i].type + Environment.NewLine + list_token[i].value);
            int d = i+1;
            if (list_token.Count > i + 1 && list_token[d].value != "end" && list_token[d].value != "else" && list_token[d].value != "until")
                draw_line((x + 50), (y + 25), (x + 200), (y + 25)); // length of line =50
            x = x + 150; // update x value
        }

        void P_OP()
        {
            tokens go = list_token[i + 1];

            draw_expression((x - 10), (y + 75), "OP" + Environment.NewLine + list_token[i + 2].value);

            //get the left part of expration

            draw_line((x + 25), (y + 125), (x - 25), (y + 160)); // length of line =25
            draw_expression((x - 60), (y + 160), list_token[++i].type + Environment.NewLine + list_token[i].value);

            i++;//skeap operation

            //get the right part of expration
            if (list_token.Count > i + 2)
            {
                go = list_token[i + 2]; //to check if we have an operator or not                
            }
            if (go.value == "+" || go.value == "-" || go.value == "/" || go.value == "*")
            {
                draw_line((x + 25), (y + 125), (x + 65), (y + 160)); // length of line =25
                draw_expression((x + 40), (y + 160), "OP" + Environment.NewLine + list_token[i + 2].value);

                draw_line((x + 65), (y + 185), (x + 30), (y + 230)); // length of line =25
                draw_expression((x + 30), (y + 230), list_token[++i].type + Environment.NewLine + list_token[i].value);

                i++;

                draw_line((x + 65), (y + 185), (x + 100), (y + 230)); // length of line =25
                draw_expression((x + 100), (y + 230), list_token[++i].type + Environment.NewLine + list_token[i++].value);

            }
            else
            {
                draw_line((x + 25), (y + 125), (x + 65), (y + 160)); // length of line =25
                draw_expression((x + 40), (y + 160), list_token[++i].type + Environment.NewLine + list_token[i++].value);
            }
        }

      
        void P_ASSIGN()
        {
            tokens go = list_token[i + 1];          
            draw_statement(x, y, "ASSIGN" + Environment.NewLine + list_token[i].value);
            draw_line((x + 25), (y + 50), (x + 25), (y + 100)); // length of line =50
            i++; //to pass ":=" and get next token
            if (list_token.Count > i + 2)
            {
                go = list_token[i + 2]; //to check if we have an operator or not                
            }
            if (go.value == "+" || go.value == "-" || go.value == "/" || go.value == "*")
            {
                P_OP();
            }
            else
            {
                //  draw_line((x + 25), (y + 125), (x - 25), (y + 160));  // length of line =25
                draw_expression(x, (y + 100), list_token[++i].type + Environment.NewLine + list_token[i++].value);
            }

            int d = i;
            if (list_token.Count > i + 1 && list_token[d].value != "end" && list_token[d].value != "else" && list_token[d].value != "until")
                draw_line((x + 50), (y + 25), (x + 200), (y + 25)); // length of line =50

            x = x + 200;
        }

        void P_IF()
        {         
            stack_else_x.Push(x);
            stack_end_x.Push(x);
            stack_else_y.Push(y);
            stack_end_y.Push(y);

            draw_statement(x, y, "IF");
            draw_line((x + 25), (y + 50), (x - 20), (y + 75)); // length of line =25

            /////******** make test part of IF********** ////////////
            x = x - 50;
            // y = y + 50;
            P_OP();
            draw_line((x + 80), (y + 50), (x + 120 + 100), (y + 80)); // length of line =25

            x = x + 120 + 100;
            y = y + 70;
        }

        void P_ELSE()
        {
            int Xsave = stack_else_x.Pop();
            int YSave = stack_else_y.Pop();
            draw_line((Xsave + 20), (YSave + 50), (x), (y)); // length of line =25
        }

        void P_END()
        {
            int Xsave = stack_end_x.Pop();
            int YSave = stack_end_y.Pop();

            x = x + 100;
            y = y - 70;
            int d = i+1;
            if (list_token.Count > i + 1 && list_token[d].value != "end" && list_token[d].value != "else" && list_token[d].value != "until")
                draw_line(Xsave + 50, YSave + 25, x, y + 25); // length of line =50
        }       

        private void Form2_Load(object sender, EventArgs e)
        {
            stmt_sequence();
        }
        
        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            if (flag == 1)
            {
                s = panel2.CreateGraphics();
                panel2.Width  = 10000;
                panel2.Height = 10000;
                x = 100;
                y = 10;
                i = 0;
                tokens now;
                tokens now_ass;
                for (; i < list_token.Count; i++)
                {

                    now = list_token[i];
                    now_ass = list_token[i];
                    if (list_token.Count > i + 1)
                        now_ass = list_token[i + 1];

                    if (now.type == "READ")
                        P_Read();

                    if (now.type == "WRITE")
                        P_WRITE();

                    if (now_ass.type == "ASSIGN")
                    {
                        P_ASSIGN();
                        i--;
                    }

                    if (now.type == "REPEAT")
                    {
                        stack_until_x.Push(x);
                        stack_until_y.Push(y);
                        draw_statement(x, y, "REPEAT");
                        draw_line((x + 25), (y + 50), (x + 25), (y + 250)); // length of line =50
                        y += 250;
                        continue;
                    }

                    if (now.type == "UNTIL")
                    {
                        y -= 200;
                        int Xsave = stack_until_x.Pop() ;
                        int YSave = stack_until_y.Pop() ;
                        draw_line(Xsave+20, YSave+50, (x), (y + 80)); // length of line =25
                        P_OP();
                        y -= 50;
                        x += 200;
                        int d = i;
                        if (list_token.Count > i + 1 && list_token[d].value != "end" && list_token[d].value != "else" && list_token[d].value != "until")
                            draw_line(Xsave+50, YSave+25, x, y+25); // length of line =50
                    }

                    if (now.type == "IF")
                        P_IF();
                    if (now.type == "ELSE")
                        P_ELSE();
                    if (now.type == "END")
                        P_END();
                }
            }
        }
     
        

    }

       
    }