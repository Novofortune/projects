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
            //patterns.Add("indentifier", "[A-Z|a-z|_][A-Z|a-z|0-9|_]*");
            //patterns.Add("digital", "[0-9]*");
            patterns.Add("text", "[A-Z|a-z|0-9|_][A-Z|a-z|0-9|_]*");
            patterns.Add("openingbracket1", "{");
            patterns.Add("closingbracket1", "}");
            patterns.Add("quote1", "[\"]");
            patterns.Add("quote2", "[\']");
            patterns.Add("colon", ":");
            patterns.Add("semicolon", ";");
            patterns.Add("comma", ",");
            string json = FileOperation.ReadFromFile("PropertyInfo-771.json");
            Scanner scanner = new Scanner(patterns, json);
            List<ASTNode> nodes = scanner.ScanAll(); // With Sorted Order

         
           GrammarNode gn = new GrammarNode("grammar");
           GrammarNode gn1 = new GrammarNode("quote1");
           gn1.AddInstruction("^quote", "pushquote");
           gn1.AddInstruction("quote","popquote");
           GrammarNode.AddASTNodeSequence(gn, 0, gn1);

           GrammarNode gn2 = new GrammarNode("openingbracket1");

           gn2.AddInstruction("^quote", "pushblock1");
           gn2.AddInstruction("quote", "asquote");
           GrammarNode.AddASTNodeSequence(gn, 0, gn2);

           GrammarNode gn3 = new GrammarNode("closingbracket1");

           gn3.AddInstruction("^quote", "popblock1");
           gn3.AddInstruction("quote", "asquote");
           GrammarNode.AddASTNodeSequence(gn, 0, gn3);

           GrammarNode gn4 = new GrammarNode("comma");
           gn4.AddInstruction("^quote", "split");
           gn4.AddInstruction("quote", "asquote");
           GrammarNode.AddASTNodeSequence(gn, 0, gn4);

           GrammarNode gn6 = new GrammarNode("text");
           gn6.AddInstruction("^quote", "normal");
           gn6.AddInstruction("quote", "asquote");
           GrammarNode.AddASTNodeSequence(gn, 0, gn6);

           BlockParser bp = new BlockParser(gn, nodes);
           ASTNode root = bp.Parse();
           ASTNode.Visualize(root);
           //ASTNode.ChangeVisualText(root);


            FileExplorerForm form = new FileExplorerForm();

            form.textBox1.Text = Directory.GetCurrentDirectory();
            //form.firstloadTreeView();
            form.treeView1.Nodes.Add(root);
            form.treeView1.ExpandAll();
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
