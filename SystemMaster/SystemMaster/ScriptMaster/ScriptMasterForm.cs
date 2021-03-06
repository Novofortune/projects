﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ScriptMaster
{
    public partial class ScriptMasterForm : Form
    {
        public  string content;
        public  string program_version;
        public  string file_path;

        public int RichTextBoxSelectionStart;

        public BlockParser bp;
        public SentenceParser sp;
        public Scanner scanner;
        Dictionary<string, string> patterns;
        GrammarNode BlockParseGrammar; //AST Parse Rule
        Dictionary<string, string> patterns_for_text;
        Dictionary<string, string> patterns_for_other;
        GrammarNode SentenceParseGrammar; //Sentence Parse Rule
        //
        //disable double click expand
        //
        bool blnDoubleClick = false;
        public ScriptMasterForm()
        {
            InitializeComponent();
            #region Tokenize Rule Set
            patterns = new Dictionary<string, string>(); 
            patterns_for_other = new Dictionary<string,string>();
            patterns_for_text = new Dictionary<string,string>();
            /*
             * 按模式的顺序覆盖规则，后面的规则会覆盖前面的规则
             */
            //patterns.Add("indentifier", "[A-Z|a-z|_][A-Z|a-z|0-9|_]*");
            //patterns.Add("digital", "[0-9]*");
            //patterns.Add("escape",@"\.");
            patterns.Add("enter", @"\r\n|\r|\n");
            patterns.Add("space", @"(  *)");
            patterns.Add("other", @"([-+=<>@#$!%~`/\.\^\|\?\&\*])([-+=<>@#$!%~`/\.\^\|\?\&\*])*");
            patterns.Add("text", "[A-Z|a-z|0-9|_][A-Z|a-z|0-9|_]*");
            //patterns.Add("keyword", "using|System");
            patterns.Add("openingbracket1", "{");
            patterns.Add("closingbracket1", "}");
            patterns.Add("openingbracket2", "[[]");
            patterns.Add("closingbracket2", "[]]");
            patterns.Add("openingbracket3", "[(]");
            patterns.Add("closingbracket3", "[)]");
            patterns.Add("commentstart1", "//");
            patterns.Add("commentstart2", @"/\*");
            patterns.Add("commentend2", @"\*/");
            patterns.Add("quote1", "\"");
            patterns.Add("quote2", "\'");
            patterns.Add("colon", ":");
            patterns.Add("semicolon", ";");
            patterns.Add("comma", ",");
            //string json = FileOperation.ReadFromFile("PropertyInfo-771.json");
            //*******
            #endregion

            #region AST Parse Rule Set
            BlockParseGrammar = new GrammarNode("grammar");

            GrammarNode gn15 = new GrammarNode("commentstart1");
            gn15.AddInstruction("none", "pushcomment1");
            gn15.AddInstruction("comment1", "ascomment1");
            gn15.AddInstruction("comment2", "ascomment2");
            gn15.AddInstruction("quote1", "asquote1");
            gn15.AddInstruction("quote2", "asquote2");
            GrammarNode.AddGrammarNodeSequence(BlockParseGrammar, gn15);

            GrammarNode gn17 = new GrammarNode("commentstart2");
            gn17.AddInstruction("none", "pushcomment2");
            gn17.AddInstruction("comment1", "ascomment1");
            gn17.AddInstruction("comment2", "ascomment2");
            gn17.AddInstruction("quote1", "asquote1");
            gn17.AddInstruction("quote2", "asquote2");
            GrammarNode.AddGrammarNodeSequence(BlockParseGrammar, gn17);

            GrammarNode gn16 = new GrammarNode("commentend2");
            gn16.AddInstruction("none", "normal");
            gn16.AddInstruction("comment1", "ascomment1");
            gn16.AddInstruction("comment2", "popcomment2");
            gn16.AddInstruction("quote1", "asquote1");
            gn16.AddInstruction("quote2", "asquote2");
            GrammarNode.AddGrammarNodeSequence(BlockParseGrammar,  gn16);


            GrammarNode gn1 = new GrammarNode("quote1");
            gn1.AddInstruction("none", "pushquote1");
            gn1.AddInstruction("quote1", "popquote1");
            gn1.AddInstruction("quote2", "asquote2");
            gn1.AddInstruction("comment1", "ascomment1");
            gn1.AddInstruction("comment2", "ascomment2");
            GrammarNode.AddGrammarNodeSequence(BlockParseGrammar,  gn1);

            GrammarNode gn18 = new GrammarNode("quote2");
            gn18.AddInstruction("none", "pushquote2");
            gn18.AddInstruction("quote2", "popquote2");
            gn18.AddInstruction("quote1", "asquote1");
            gn18.AddInstruction("comment1", "ascomment1");
            gn18.AddInstruction("comment2", "ascomment2");
            GrammarNode.AddGrammarNodeSequence(BlockParseGrammar, gn18);

            GrammarNode gn2 = new GrammarNode("openingbracket1");
            gn2.AddInstruction("none", "pushblock1");
            gn2.AddInstruction("quote1", "asquote1");
            gn2.AddInstruction("comment1", "ascomment1");
            gn2.AddInstruction("comment2", "ascomment2");
            gn2.AddInstruction("quote2", "asquote2");
            GrammarNode.AddGrammarNodeSequence(BlockParseGrammar,  gn2);

            GrammarNode gn3 = new GrammarNode("closingbracket1");
            gn3.AddInstruction("none", "popblock1");
            gn3.AddInstruction("quote1", "asquote1");
            gn3.AddInstruction("comment1", "ascomment1");
            gn3.AddInstruction("comment2", "ascomment2");
            gn3.AddInstruction("quote2", "asquote2");
            GrammarNode.AddGrammarNodeSequence(BlockParseGrammar,  gn3);

            GrammarNode gn9 = new GrammarNode("openingbracket2");
            gn9.AddInstruction("none", "pushblock2");
            gn9.AddInstruction("quote1", "asquote1");
            gn9.AddInstruction("comment1", "ascomment1");
            gn9.AddInstruction("comment2", "ascomment2");
            gn9.AddInstruction("quote2", "asquote2");
            GrammarNode.AddGrammarNodeSequence(BlockParseGrammar,  gn9);

            GrammarNode gn10 = new GrammarNode("closingbracket2");
            gn10.AddInstruction("none", "popblock2");
            gn10.AddInstruction("quote1", "asquote1");
            gn10.AddInstruction("comment1", "ascomment1");
            gn10.AddInstruction("comment2", "ascomment2");
            gn10.AddInstruction("quote2", "asquote2");
            GrammarNode.AddGrammarNodeSequence(BlockParseGrammar,  gn10);

            GrammarNode gn11 = new GrammarNode("openingbracket3");
            gn11.AddInstruction("none", "pushblock3");
            gn11.AddInstruction("quote1", "asquote1");
            gn11.AddInstruction("comment1", "ascomment1");
            gn11.AddInstruction("comment2", "ascomment2");
            gn11.AddInstruction("quote2", "asquote2");
            GrammarNode.AddGrammarNodeSequence(BlockParseGrammar,  gn11);

            GrammarNode gn12 = new GrammarNode("closingbracket3");
            gn12.AddInstruction("none", "popblock3");
            gn12.AddInstruction("quote1", "asquote1");
            gn12.AddInstruction("comment1", "ascomment1");
            gn12.AddInstruction("comment2", "ascomment2");
            gn12.AddInstruction("quote2", "asquote2");
            GrammarNode.AddGrammarNodeSequence(BlockParseGrammar,  gn12);

            //GrammarNode gn4 = new GrammarNode("semicolon");
            //gn4.AddInstruction("none", "split");
            //gn4.AddInstruction("quote1", "asquote1");
            //gn4.AddInstruction("comment1", "ascomment1");
            //gn4.AddInstruction("comment2", "ascomment2");
            //gn4.AddInstruction("quote2", "asquote2");
            //GrammarNode.AddGrammarNodeSequence(BlockParseGrammar, 0, gn4);

            GrammarNode gn6 = new GrammarNode("text");
            gn6.AddInstruction("none", "normal");
            gn6.AddInstruction("quote1", "asquote1");
            gn6.AddInstruction("comment1", "ascomment1");
            gn6.AddInstruction("comment2", "ascomment2");
            gn6.AddInstruction("quote2", "asquote2");
            GrammarNode.AddGrammarNodeSequence(BlockParseGrammar,  gn6);

            //GrammarNode gn17 = new GrammarNode("keyword");
            //gn17.AddInstruction("none", "normal");
            //gn17.AddInstruction("quote1", "asquote1");
            //gn17.AddInstruction("comment1", "ascomment1");
            //GrammarNode.AddASTNodeSequence(gn, 0, gn17);

            GrammarNode gn7 = new GrammarNode("enter");
            gn7.AddInstruction("none", "normal");
            gn7.AddInstruction("quote1", "asquote1");
            gn7.AddInstruction("comment1", "popcomment1"); // use enter as the end of a comment1
            gn7.AddInstruction("comment2", "ascomment2");
            gn7.AddInstruction("quote2", "asquote2");
            GrammarNode.AddGrammarNodeSequence(BlockParseGrammar, gn7);

            GrammarNode gn14 = new GrammarNode("space");
            gn14.AddInstruction("none", "normal");
            gn14.AddInstruction("quote1", "asquote1");
            gn14.AddInstruction("comment1", "ascomment1");
            gn14.AddInstruction("comment2", "ascomment2");
            gn14.AddInstruction("quote2", "asquote2");
            GrammarNode.AddGrammarNodeSequence(BlockParseGrammar, gn14);

            GrammarNode gn8 = new GrammarNode("other");
            gn8.AddInstruction("none", "normal");
            gn8.AddInstruction("quote1", "asquote1");
            gn8.AddInstruction("comment1", "ascomment1");
            gn8.AddInstruction("comment2", "ascomment2");
            gn8.AddInstruction("quote2", "asquote2");
            GrammarNode.AddGrammarNodeSequence(BlockParseGrammar,  gn8);

            GrammarNode gn13 = new GrammarNode("escape");
            gn13.AddInstruction("none", "normal");
            gn13.AddInstruction("quote1", "asquote1");
            gn13.AddInstruction("comment1", "ascomment1");
            gn13.AddInstruction("comment2", "ascomment2");
            gn13.AddInstruction("quote2", "asquote2");
            GrammarNode.AddGrammarNodeSequence(BlockParseGrammar, gn13);

            GrammarNode gn19 = new GrammarNode("comma");
            gn19.AddInstruction("none", "normal");
            gn19.AddInstruction("quote2", "asquote2");
            gn19.AddInstruction("quote1", "asquote1");
            gn19.AddInstruction("comment1", "ascomment1");
            gn19.AddInstruction("comment2", "ascomment2");
            GrammarNode.AddGrammarNodeSequence(BlockParseGrammar,  gn19);
            GrammarNode gn20 = new GrammarNode("semicolon");
            gn20.AddInstruction("none", "normal");
            gn20.AddInstruction("quote2", "asquote2");
            gn20.AddInstruction("quote1", "asquote1");
            gn20.AddInstruction("comment1", "ascomment1");
            gn20.AddInstruction("comment2", "ascomment2");
            GrammarNode.AddGrammarNodeSequence(BlockParseGrammar, gn20);
            GrammarNode gn21 = new GrammarNode("colon");
            gn21.AddInstruction("none", "normal");
            gn21.AddInstruction("quote2", "asquote2");
            gn21.AddInstruction("quote1", "asquote1");
            gn21.AddInstruction("comment1", "ascomment1");
            gn21.AddInstruction("comment2", "ascomment2");
            GrammarNode.AddGrammarNodeSequence(BlockParseGrammar,  gn21);
            #endregion

            #region Sentence Parse Rule Set

            SentenceParseGrammar = new GrammarNode("grammar");

            Dictionary<string, string> tempdic = new Dictionary<string, string>();
            //******************create a grammar branch ****************************
            tempdic.Add("none","premerge");
            GrammarNode gn1001 = new GrammarNode("class", tempdic);
            GrammarNode gn1002 = new GrammarNode("identifier", tempdic);
            tempdic.Clear();
            tempdic.Add("none", "merge");
            GrammarNode gn1003 = new GrammarNode("block1", tempdic);
            GrammarNode gn1004 = GrammarNode.LinkGrammarNode(gn1001,gn1002,gn1003);
            GrammarNode.AddGrammarNodeSequence(SentenceParseGrammar, gn1004);
            //*********************End Create a grammar branch******************************
            tempdic.Clear();
            tempdic.Add("none", "premerge");
            GrammarNode gn1005 = new GrammarNode("identifier", tempdic);
            GrammarNode gn1006 = new GrammarNode("identifier", tempdic);
            GrammarNode gn1007 = new GrammarNode("block3", tempdic);
            tempdic.Clear();
            tempdic.Add("none", "merge");
            GrammarNode gn1008 = new GrammarNode("block1", tempdic);
            GrammarNode gn1009 = GrammarNode.LinkGrammarNode(gn1005, gn1006, gn1007,gn1008);
            GrammarNode.AddGrammarNodeSequence(SentenceParseGrammar, gn1009);
            //*********************End Add Branch to Grammar Tree***************************

            #endregion
            #region Text ASTNode Pattern Interpret Rule 
            // Ascending order of recognization priority
            patterns_for_text.Add("identifier", @"[A-Z|a-z|_][A-Z|a-z|0-9|_]*");
            patterns_for_text.Add("class", @"class");
            patterns_for_text.Add("using", @"using");
            patterns_for_text.Add("namespace", @"namespace");
            patterns_for_text.Add("function", @"function");
            patterns_for_text.Add("modifier", @"public|private|protected|internal|static|virtual|sealed");
            patterns_for_text.Add("new", @"new");

            patterns_for_text.Add("digital", @"[0-9][0-9]*");
            patterns_for_text.Add("dot", @"\.");
            //Operators
            patterns_for_text.Add("equal", @"=");
            patterns_for_text.Add("add", @"\+");
            patterns_for_text.Add("minus", @"-");
            patterns_for_text.Add("multiply", @"\*");
            patterns_for_text.Add("divide", @"\/");
            patterns_for_text.Add("power",@"\^");
            patterns_for_text.Add("and", @"\&\&|\&");
            patterns_for_text.Add("or", @"\|\||\|");
            patterns_for_text.Add("remainder", @"\|\||\|");
            patterns_for_text.Add("greaterthan", @">");
            patterns_for_text.Add("not", @"!");
            patterns_for_text.Add("lessthan", @"<");
            //End Operators
            #endregion
        }
        public void parse_content()
        {
            scanner = new Scanner(patterns, content);
            List<ASTNode> nodes = scanner.ScanAll(); // With Sorted Order

            foreach (ASTNode node in nodes)
            {
                Console.WriteLine(node.type+" "+node.Offset+" "+node.Content.Length);
            }

            bp = new BlockParser(BlockParseGrammar, nodes);//识别括号引号
            ASTNode root = bp.Parse();

            
            ASTNodeProcessor ap = new ASTNodeProcessor(patterns_for_text,root);
            ap.Process();
            ap.Remove("space","enter","comment1","comment2");

            //ASTNode.Visualize(root);
            sp = new SentenceParser(SentenceParseGrammar, root);
            root = sp.Parse();


            ASTNode.Visualize(root);
            //text box processing to prevent bug
            int backspaceCount = 0;
            ASTNode.OffsetAdjustToWinForm(root, ref backspaceCount);
            //End text box processing to prevent bug 

            this.fileTreeView.Nodes.Clear();
            this.fileTreeView.Nodes.Add(root);
            this.fileTreeView.ExpandAll();

            List<ASTNode> list = new List<ASTNode>();
            ASTNode.LoopExpand(root, ref list);
            Console.WriteLine(list.Count);

            //show labels
            #region Show TextBox----------------
            //RichTextBoxSelectionStart = this.richTextBox1.SelectionStart;
            //foreach (ASTNode node in nodes)
            //{
            //    switch (node.type)
            //    {
            //        case "openingbracket1":
            //            {
            //                richTextBox1.Select(node.Offset, node.Content.Length);
            //                richTextBox1.SelectionColor = Color.Red;
            //                break;
            //            }
            //        case "closingbracket1":
            //            {
            //                richTextBox1.Select(node.Offset, node.Content.Length);
            //                richTextBox1.SelectionColor = Color.Red;
            //                break;
            //            }

            //        case "openingbracket2":
            //            {
            //                richTextBox1.Select(node.Offset, node.Content.Length);
            //                richTextBox1.SelectionColor = Color.Green;
            //                break;
            //            }
            //        case "closingbracket2":
            //            {
            //                richTextBox1.Select(node.Offset, node.Content.Length);
            //                richTextBox1.SelectionColor = Color.Green;
            //                break;
            //            }

            //        case "text":
            //            {
            //                richTextBox1.Select(node.Offset, node.Content.Length);
            //                richTextBox1.SelectionColor = Color.Blue;
            //                break;
            //            }
            //        case "quote":
            //            {
            //                richTextBox1.Select(node.Offset, node.Content.Length);
            //                richTextBox1.SelectionColor = Color.Brown;
            //                break;
            //            }
            //        case "space":
            //            {
            //                richTextBox1.Select(node.Offset, node.Content.Length);
            //                richTextBox1.SelectionBackColor = Color.Blue;
            //                break;
            //            }
            //        case "other":
            //            {
            //                richTextBox1.Select(node.Offset, node.Content.Length);
            //                richTextBox1.SelectionBackColor = Color.Red;
            //                break;
            //            }
            //        case "escape":
            //            {
            //                richTextBox1.Select(node.Offset, node.Content.Length);
            //                richTextBox1.SelectionBackColor = Color.Brown;
            //                break;
            //            }
            //        case "enter":
            //            {
            //               // richTextBox1.Select(node.Offset, node.Content.Length);
            //               // richTextBox1.SelectionBackColor = Color.Black;
            //                break;
            //            }
            //    }
            //}
            //this.richTextBox1.SelectionStart = RichTextBoxSelectionStart;
            //this.richTextBox1.ScrollToCaret();
            #endregion
        }
        public void load(string FilePath)
        {
            if (System.IO.File.Exists(FilePath))
            {
                System.IO.Stream fileStream = new FileStream(FilePath, FileMode.Open);

                using (System.IO.StreamReader reader = new System.IO.StreamReader(fileStream))
                {
                    // Read the first line from the file and write it the textbox.
                    this.file_path = FilePath;
                    this.Text = FilePath;
                    this.richTextBox1.Enabled = true;
                    this.content = reader.ReadToEnd();
                }
                fileStream.Close();

                this.program_version = "ScriptMaster 1.0";
                this.setEvents();
                this.Text = this.program_version;
                this.richTextBox1.Text = this.content;
                    parse_content();
            }
        }
     
        private void setEvents()
        {
            this.FormClosed += Form1_FormClosed;
            this.richTextBox1.TextChanged += textBox1_TextChanged;
        }
        void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.content = this.richTextBox1.Text;
        }

        void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Environment.Exit(0);
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.file_path = Directory.GetCurrentDirectory() + "\\new file";
            this.Text = "new file";
            this.richTextBox1.Enabled = true;
            this.content = "Please Enter Your Content Here...";

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

          //  try
            {
                // Open the selected file to read.
                string FileName = this.file_path;
                DialogResult res1 = DialogResult.No;
                if (!File.Exists(FileName))
                {
                    res1 = MessageBox.Show("File Does Not Exist, Create?", "Warning", MessageBoxButtons.YesNo);
                }


                if (res1 == DialogResult.Yes||File.Exists(FileName))
                {
                    System.IO.Stream fileStream = new FileStream(FileName, FileMode.Create);

                    using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fileStream))
                    {
                        // Read the first line from the file and write it the textbox.
                        writer.Write(this.content);
                    }
                    fileStream.Close();
                }
            }


            //catch
            {
              //  throw new Exception();
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            // Create an instance of the open file dialog box.
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            // Set filter options and filter index.
            saveFileDialog1.Filter = "All Files (*.*)|*.*|Text Files (.txt)|*.txt";
            saveFileDialog1.FilterIndex = 1;

           // saveFileDialog1.Multiselect = true;

            // Call the ShowDialog method to show the dialog box.
            DialogResult? userClickedOK = saveFileDialog1.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK.HasValue == true)
            {
                if (userClickedOK.Value.ToString() == "Cancel")
                {
                    return;
                }
                try
                {
                    // Open the selected file to read.
                    string FileName = saveFileDialog1.FileName;
                    DialogResult res1 = DialogResult.No;
                    if (saveFileDialog1.CheckFileExists)
                    {
                        res1 = MessageBox.Show("File Exists, Overwrite?", "Warning", MessageBoxButtons.YesNo);
                    }

                    if (res1 == DialogResult.Yes || !saveFileDialog1.CheckFileExists)
                    {
                        System.IO.Stream fileStream = new FileStream(FileName, FileMode.Create);

                        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fileStream))
                        {
                            // Read the first line from the file and write it the textbox.
                            writer.Write(this.content);
                        }
                        fileStream.Close();
                    }
                    else if (res1 == DialogResult.No) { }
                }


                catch
                {
                    throw new Exception();
                }
            }
        }

        private void closeFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.file_path != null)
            {
                this.file_path = null;
                this.content = "";
                this.richTextBox1.Enabled = false;
                this.Text = this.program_version;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create an instance of the open file dialog box.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "All Files (*.*)|*.*|Text Files (.txt)|*.txt";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = true;

            // Call the ShowDialog method to show the dialog box.
            DialogResult? userClickedOK = openFileDialog1.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK.HasValue == true)
            {
              // Console.WriteLine( userClickedOK.Value.ToString());
                if (userClickedOK.Value.ToString() == "Cancel")
                {
                    return;
                }
              //  try
                {
                    // Open the selected file to read.
                    string FileName = openFileDialog1.FileName;
                    System.IO.Stream fileStream = new FileStream(FileName, FileMode.Open);

                    using (System.IO.StreamReader reader = new System.IO.StreamReader(fileStream))
                    {
                        // Read the first line from the file and write it the textbox.
                        this.file_path = FileName;
                        this.Text = FileName;
                        this.richTextBox1.Enabled = true;
                        this.content = reader.ReadToEnd();
                    }
                    fileStream.Close();
                }
                //catch
                {
                   // throw new Exception();
                }
            }
        }

        private void fileTreeView_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (blnDoubleClick == true && e.Action == TreeViewAction.Collapse)
                e.Cancel = true;
        }

        private void fileTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (blnDoubleClick == true && e.Action == TreeViewAction.Expand)
                e.Cancel = true;
        }

        private void fileTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Clicks > 1)
                blnDoubleClick = true;
            else
                blnDoubleClick = false;
        }

        private void fileTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            ASTNode newNode;
            newNode = (ASTNode)e.Node;
            
                //ScriptMaster.ScriptMasterForm smf = new ScriptMaster.ScriptMasterForm();
                //smf.load(newNode.FullPath);
                //smf.Show();
            typeBox.Text = newNode.type;
            contentBox.Text = newNode.Content;
        }
        
    }
}
