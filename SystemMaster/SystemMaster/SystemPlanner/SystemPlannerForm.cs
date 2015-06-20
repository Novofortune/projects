using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SystemMaster.SystemPlanner
{
    public partial class SystemPlannerForm : Form
    {
        private Point MouseDownLocation;
        private object ReadyForConnect;
        public Cursor currentCursor;
        public SystemPlannerForm()
        {
            InitializeComponent();
            this.MouseClick += SystemPlannerForm_MouseClick;
            KeyDown += SystemPlannerForm_KeyDown;
            currentCursor = Cursors.Default;
        }

        void SystemPlannerForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.C)
            {
                if (currentCursor == Cursors.Default)
                {
                    currentCursor = Cursors.Hand;
                }
                else if (currentCursor == Cursors.Hand)
                {
                    currentCursor = Cursors.Default;
                }
               
            }
        }

        void SystemPlannerForm_MouseClick(object sender, MouseEventArgs e)
        {
            this.Focus();
            if (e.Button == MouseButtons.Right)
            {
                var child = this.GetChildAtPoint(e.Location);
                if (child != null)
                {

                }
                else
                {
                    TableLayoutPanel p = new TableLayoutPanel();
                    p.Padding = new Padding(0,10,0,0);
                    p.ColumnCount = 2;

                    p.BackColor = Color.Gray;
                    p.SetBounds(e.X, e.Y, 150, 150);
                    p.Location = e.Location;
                    p.BringToFront();
                    p.MouseUp += p_MouseUp;
                    p.MouseDown += p_MouseDown;
                    p.MouseMove += p_MouseMove;
                    p.PreviewKeyDown += p_PreviewKeyDown;
                    p.AutoSize = true;
                    p.AllowDrop = true;
                    p.BorderStyle = BorderStyle.Fixed3D;
                    p.GotFocus += p_GotFocus;
                    this.Controls.Add(p);
                    Label l = new Label();
                    l.AutoSize = true;
                    l.Text = "Type";
                    p.Controls.Add(l);
                    TextBox tb1 = new TextBox();
                    tb1.KeyDown += tb1_KeyDown;
                    p.Controls.Add(tb1);
                    
                    
                }
            }else if(e.Button==MouseButtons.Left){
                
            }
           
        }

        void p_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.C)
            {
                Cursor.Current = Cursors.Arrow;
            }
        }

        void tb1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) {
                (sender as TextBox).Enabled = false;
            }
        }

        void p_GotFocus(object sender, EventArgs e)
        {
            (sender as Panel).BringToFront();
        }

        void p_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                (sender as Panel).Left = e.X + (sender as Panel).Left - MouseDownLocation.X;
                (sender as Panel).Top = e.Y + (sender as Panel).Top - MouseDownLocation.Y;

            }
        }

        void p_MouseDown(object sender, MouseEventArgs e)
        {
            (sender as Panel).Focus();
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                MouseDownLocation = e.Location;
            }
        }

        void p_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                TextBox tb = new TextBox();
                tb.Location = e.Location;
                tb.MouseDown += tb_MouseDown;
                tb.KeyDown += tb_KeyDown;
                (sender as Panel).Controls.Add(tb);
                tb.Focus();
                tb.LostFocus += tb_LostFocus;
            }
            else if (e.Button == MouseButtons.Left)
            {
               // (sender as Panel).DoDragDrop(sender,DragDropEffects.Move);
            }
        }

        void tb_LostFocus(object sender, EventArgs e)
        {
            if ((sender as TextBox).Text != "")
            {
                Label l = new Label();
                l.Text = (sender as TextBox).Text;
                l.BackColor = Color.Green;
                l.Location = (sender as TextBox).Location;
                l.AutoSize = true;
                l.MouseDown += l_MouseDown;
                l.MouseMove += l_MouseMove;
                l.MouseUp += l_MouseUp;
                l.PreviewKeyDown += l_PreviewKeyDown;
                (sender as TextBox).Parent.Controls.Add(l);
                (sender as TextBox).Dispose();
            }
            else
            {
                (sender as TextBox).Dispose();
            }
        }

        void l_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.C)
            {
                Cursor.Current = Cursors.Arrow;
            }
        }

        void tb_MouseDown(object sender, MouseEventArgs e)
        {
            //(sender as TextBox).Location = e.Location;
        }

        void tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
               // (sender as TextBox).Enabled=false;
                if ((sender as TextBox).Text != "")
                {
                    Label l = new Label();
                    l.Text = (sender as TextBox).Text;
                    l.BackColor = Color.Green;
                    l.Location = (sender as TextBox).Location;
                    l.AutoSize = true;
                    l.MouseDown += l_MouseDown;
                    l.MouseMove += l_MouseMove;
                    l.MouseUp += l_MouseUp;
                    (sender as TextBox).Parent.Controls.Add(l);
                    (sender as TextBox).LostFocus-=tb_LostFocus;
                    (sender as TextBox).Dispose();
                }
            }
            if (e.KeyCode == Keys.Escape)
            {
                (sender as TextBox).LostFocus -= tb_LostFocus;
                (sender as TextBox).Dispose();
            }
        }

        void l_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TextBox ltb = new TextBox();
                ltb.Location = e.Location;
                ltb.KeyDown += ltb_KeyDown;
                ltb.Tag = (sender as Label);
                (sender as Label).Parent.Controls.Add(ltb);
                ltb.BringToFront();
                ltb.Focus();
                ltb.LostFocus += ltb_LostFocus;
            }
        }

        void ltb_LostFocus(object sender, EventArgs e)
        {
            if ((sender as TextBox).Text != "")
            {
                Label l = (sender as TextBox).Tag as Label;
                l.Text = (sender as TextBox).Text;
                (sender as TextBox).Dispose();
            }
            else
            {
                //Label l = (sender as TextBox).Tag as Label;
                //l.Dispose();
                (sender as TextBox).Dispose();
            }
        }

        void ltb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // (sender as TextBox).Enabled=false;
                if ((sender as TextBox).Text != "")
                {
                    Label l = (sender as TextBox).Tag as Label;
                    l.Text = (sender as TextBox).Text;
                    (sender as TextBox).Dispose();
                }
                else
                {
                    Label l = (sender as TextBox).Tag as Label;
                    l.Dispose();
                    (sender as TextBox).LostFocus -= ltb_LostFocus;
                    (sender as TextBox).Dispose();
                }
            }
            if (e.KeyCode == Keys.Escape)
            {
                (sender as TextBox).LostFocus -= ltb_LostFocus;
                (sender as TextBox).Dispose();
            }
        }

        void l_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                (sender as Label).Left = e.X + (sender as Label).Left - MouseDownLocation.X;
                (sender as Label).Top = e.Y + (sender as Label).Top - MouseDownLocation.Y;
            }
        }

        void l_MouseDown(object sender, MouseEventArgs e)
        {
            (sender as Label).Focus();
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                MouseDownLocation = e.Location;
            }
        }

        private void SystemPlannerForm_Load(object sender, EventArgs e)
        {

        }

    }
}
