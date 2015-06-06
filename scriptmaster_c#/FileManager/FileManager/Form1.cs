using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileManager
{
    public partial class Form1 : Form
    {
        public  string curdir;
        public  List<FileNode> fns;
        public  FileTreeNode ftn;
        public Form1()
        {
            InitializeComponent();
            textBox1.KeyDown += textBox1_KeyDown;
        }

        void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) {

                this.treeView1.Nodes.Clear();
                this.curdir = this.textBox1.Text;
                this.fns = FileOperation.GetFileTree(this.curdir);

                if (this.fns.Count > 0)
                {
                    this.ftn = FileTreeNode.LoadTreeView(this.fns[0]);
                    this.treeView1.Nodes.Add(this.ftn);
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.treeView1.Nodes.Clear();
            this.curdir = this.textBox1.Text;
            this.fns = FileOperation.GetFileTree(this.curdir);

            if (this.fns.Count > 0)
            {
                this.ftn = FileTreeNode.LoadTreeView(this.fns[0]);
                this.treeView1.Nodes.Add(this.ftn);
            }
        }
    }
}
