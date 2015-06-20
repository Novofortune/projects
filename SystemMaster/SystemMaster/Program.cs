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
        [STAThread]
        static void Main(string[] args)
        {
            FileExplorerForm form = new FileExplorerForm();
            form.PathBox.Text = Directory.GetCurrentDirectory();
            form.firstloadTreeView();
            //form.Show();
            SystemMaster.SystemPlanner.SystemPlannerForm form1 = new SystemMaster.SystemPlanner.SystemPlannerForm();
            form1.Show();

            running = true;
            while (running)
            {
                System.Windows.Forms.Application.DoEvents();
                System.Windows.Forms.Cursor.Current = form1.currentCursor;
                Thread.Sleep(20);
                ProgramExitCheck();
            }
        }

      
        static void ProgramExitCheck()
        {
            if (System.Windows.Forms.Application.OpenForms.Count == 0) { running = false; };
        }
    }
}
