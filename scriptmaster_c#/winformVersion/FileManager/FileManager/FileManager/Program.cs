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
        public static string curdir;
        public static List<FileNode> fns;
        public static FileTreeNode ftn;
        static void Main(string[] args)
        {

            //___________Testing__________
           // string dir = Directory.GetCurrentDirectory();
          // List<FileNode> fns =  FileOperation.GetFileTree(dir);
          // for (int i = 0; i < fns.Count; i++)
          // {
          //     if (fns[i].parent != null)
          //     {
          //         Console.WriteLine(fns[i].parent.name);
          //     }
         //  }
            //___________Testing__________



             curdir = Directory.GetCurrentDirectory();
             fns = FileOperation.GetFileTree(curdir);

            Form1 form = new Form1();
            form.FormClosed += form_FormClosed;
            form.textBox1.Text = curdir;
            ftn = FileTreeNode.LoadTreeView(fns[0]);
            form.treeView1.Nodes.Add(ftn);
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
