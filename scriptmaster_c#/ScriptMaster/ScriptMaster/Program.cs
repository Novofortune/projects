using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Irony;
using System.Reflection;
using Irony.Parsing;

namespace ScriptMaster
{
    class Program
    {
        public static ScriptMasterForm form;
        public static string content;
        public static string program_version;
        public static string file_path;

        public static List<CodeTreeView> CodeTreeViews;
        public static CodeTreeView codeTreeView;
        //public static PhpParser phpParser;
        [STAThread]
        static void Main(string[] args)
        {
            //初始化静态变量
            program_version = "ScriptMaster 1.0";
            CodeTreeViews = new List<CodeTreeView>();
            codeTreeView = new CodeTreeView();
           // phpParser = new PhpParser();
            //初始化form界面
            form = new ScriptMasterForm();
            form.Show();

            CodeTreeViews.Add(form.treeView1); //将form中的treeView加入到静态集合变量
            string source = FileOperation.ReadFromFile("PerfTest.py");//"bottles-v11-long.cs");//"source.txt");

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
            MiniPythonGrammar _CSharpGrammar = new MiniPythonGrammar();
            Parser _parser = new Parser(_CSharpGrammar);
            ParseTree _parseTree =  _parser.Parse(source);

            form.ShowParseTree(_parseTree);
            form.ShowAstTree(_parseTree);

            Console.WriteLine();
            while(true){
                UpdateLogic();
                Application.DoEvents();
            }
        }
        public static void UpdateLogic(){
            form.textBox1.Text = content;
            //form.textBox2.Text = ;
           
        }
        
    }
}
