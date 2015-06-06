using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace ScriptMaster
{
    class Program
    {
        public static ScriptMasterForm form;
        //public static PhpParser phpParser;
         void Main1(string[] args)
        {
            //初始化静态变量
           // phpParser = new PhpParser();
            //初始化form界面
            form = new ScriptMasterForm();
            form.program_version = "ScriptMaster 1.0";
            form.CodeTreeViews = new List<CodeTreeView>();
            form.codeTreeView = new CodeTreeView();
            form.Show();

            form.CodeTreeViews.Add(form.treeView1); //将form中的treeView加入到静态集合变量
            //string source = FileOperation.ReadFromFile("PerfTest.py");//"bottles-v11-long.cs");//"source.txt");

           // phpParser.AddBracketRule("<", ">");
           // phpParser.AddBracketRule("\"", "\"");
           // BlockNode node = new BlockNode();
           // node.Offset = 0;
           // node.Content = source;
            //phpParser.FindBlock(node);
          //  foreach (BlockNode n in phpParser.BlockNodes)
          //      Console.WriteLine(n.Content);
          //  Console.WriteLine(phpParser.BlockNodes.Count);
           // Irony.Parsing.Grammar _grammer = new Irony.Parsing.Grammar();

            //Console.WriteLine();
            while(true){
                UpdateLogic();
                Application.DoEvents();
            }
        }
        public static void UpdateLogic(){
            form.textBox1.Text = form.content;
            //form.textBox2.Text = ;
           
        }
        
    }
}
