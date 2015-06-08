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
            Dictionary<string, string> patterns = new Dictionary<string, string>();
            patterns.Add("indentifier", "[A-Z|a-z|_][A-Z|a-z|0-9|_]*");
            patterns.Add("openingbracket1", "{");
            patterns.Add("closingbracket1", "}");
            patterns.Add("quote1", "[\"]");
            patterns.Add("quote2", "[\']");
            patterns.Add("colon", ":");
            patterns.Add("comma", ",");
            Scanner scanner = new Scanner(patterns, "{\"abc\":{ \'aas\':\'cd\',\'a\':\'c\'} }");
            List<ASTNode> nodes = scanner.ScanAll(); // With Sorted Order

           Console.WriteLine( nodes.Count);
<<<<<<< HEAD
            foreach(ASTNode node in nodes){
                Console.WriteLine(node.Offset);
            }

=======
           foreach (ASTNode node in nodes)
           {
               Console.WriteLine(node.type+":"+node.Content);
           }
>>>>>>> origin/master
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
