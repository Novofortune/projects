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
using ScriptMaster;

namespace FileManager
{
    class Program
    {
        public static bool running;
        //public static string cmd;
        [STAThread]
        static void Main(string[] args)
        {
            //testing
         

            FileExplorerForm form = new FileExplorerForm();

            form.textBox1.Text = Directory.GetCurrentDirectory();
            form.firstloadTreeView();
            form.Show();

            running = true;
            while (running)
            {
                Application.DoEvents();
                Thread.Sleep(20);

                ProgramExitCheck();
            }
        }

      
        static void ProgramExitCheck()
        {
            if (Application.OpenForms.Count == 0) { running = false; };
        }
    }
}
