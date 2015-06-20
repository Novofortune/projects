using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeEdit
{
    public partial class Main : Form
    {
        private Pen p=new Pen(Color.Red);
        Graphics gra;
        private Panel cur;
        private Point MouseDownLocation;
        public Main()
        {
            InitializeComponent();
            //this.KeyDown += Main_KeyDown;
            this.MouseDown += Main_MouseDown;
            this.MouseUp += Main_MouseUp;
            
        }

        void Main_MouseUp(object sender, MouseEventArgs e)
        {

             gra.DrawLine(p, MouseDownLocation.X, MouseDownLocation.Y, e.X, e.Y);
         
        }

        void Main_MouseDown(object sender, MouseEventArgs e)
        {
            gra = this.CreateGraphics();
            MouseDownLocation.X = e.X;
            MouseDownLocation.Y = e.Y;
        }

        void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.N)
            {
                //create panel
                Panel p = new Panel();
                p.BackColor = Color.Pink;
                p.Bounds = new Rectangle(50,50,150,150);
                //create class name
                TextBox tb = new TextBox();
                tb.Bounds = new Rectangle(0, 0, 75, 10);
                //ed
                //create table
                DataGridView dgv = new DataGridView();
                dgv.Bounds = new Rectangle(0, 20, 150, 130);
                dgv.ColumnCount = 2;
                dgv.Columns[0].Name = "name";
                dgv.Columns[0].Width = 50;
                dgv.Columns[1].Name = "type";
                dgv.Columns[1].Width = 50;
                //ed
                //ed

                //add to panel
                p.Controls.Add(tb);
                p.Controls.Add(dgv);
                this.Controls.Add(p);
                p.MouseMove += p_MouseMove;
                p.MouseDown += p_MouseDown;
            }
        }


        void p_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                (sender as Panel).Left = e.X + (sender as Panel).Left - MouseDownLocation.X;
                (sender as Panel).Top = e.Y + (sender as Panel).Top - MouseDownLocation.Y;
            }
            if (e.Button == MouseButtons.Right)
            {
 
            }
        }
        void p_MouseDown(object sender, MouseEventArgs e)
        {
            //(sender as Panel).Focus();
            if (e.Button == MouseButtons.Left)
            {
                cur = (sender as Panel);
                cur.BackColor = Color.Blue;
                foreach (Control obj in this.Controls)
                {
                    if (obj is Panel && obj != cur)
                    {
                        obj.BackColor = Color.Pink;
                    }
                }
                MouseDownLocation = e.Location;
            }
            if (e.Button == MouseButtons.Right)
            {
 
            }
        }

        private void addNePanelCtrlNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Panel p = new Panel();
            p.BackColor = Color.Pink;
            p.Bounds = new Rectangle(50, 50, 100, 100);
            this.Controls.Add(p);
            p.MouseMove += p_MouseMove;
            p.MouseDown += p_MouseDown;
        }
    }
}
