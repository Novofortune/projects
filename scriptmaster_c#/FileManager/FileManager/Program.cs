using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Windows.Input;

namespace FileManager
{
    class Program
    {
        public static bool running;

        static void Main(string[] args)
        {
            Form1 form = new Form1();
            form.FormClosed += form_FormClosed;
            form.curdir = Directory.GetCurrentDirectory();
            form.fns = FileOperation.GetFileTree(form.curdir);

            form.textBox1.Text = form.curdir;
            form.ftn = FileTreeNode.LoadTreeView(form.fns[0]);
            form.treeView1.Nodes.Add(form.ftn);
            form.Show();

            running = true;
            while (running)
            {
                Application.DoEvents();
                Thread.Sleep(20);
            }
        }

        static void form_FormClosed(object sender, FormClosedEventArgs e)
        {
            running = false;
        }
    }
}
