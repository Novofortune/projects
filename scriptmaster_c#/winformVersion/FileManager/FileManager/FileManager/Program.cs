using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace FileManager
{
    class Program
    {
        public static bool running; 
        static void Main(string[] args)
        {

            //___________Testing__________
          //  string dir = Directory.GetCurrentDirectory();
           //List<FileNode> fns =  FileOperation.GetFileTree(dir);
          // for (int i = 0; i < fns.Count; i++)
          // {
          //     if (fns[i].parent != null)
          //     {
          //         Console.WriteLine(fns[i].parent.name);
          //     }
         //  }
            //___________Testing__________



            string curdir = Directory.GetCurrentDirectory();
            List<FileNode> fns = FileOperation.GetFileTree(curdir);

            Form1 form = new Form1(); 
            form.FormClosed += form_FormClosed;
            form.textBox1.Text = curdir;
            FileTreeNode ftn = FileTreeNode.LoadTreeView(fns[0]);
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
