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
        public Scanner scanner;
        Dictionary<string, string> patterns;
        GrammarNode gn;
        //
        //disable double click expand
        //
        bool blnDoubleClick = false;
        public ScriptMasterForm()
        {
            InitializeComponent();
            patterns = new Dictionary<string, string>(); 
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
            patterns.Add("openingbracket1", "{");
            patterns.Add("closingbracket1", "}");
            patterns.Add("openingbracket2", "[[]");
            patterns.Add("closingbracket2", "[]]");
            patterns.Add("openingbracket3", "[(]");
            patterns.Add("closingbracket3", "[)]");
            patterns.Add("quote1", "\"");
            patterns.Add("quote2", "\'");
            patterns.Add("colon", ":");
            patterns.Add("semicolon", ";");
            patterns.Add("comma", ",");
            //string json = FileOperation.ReadFromFile("PropertyInfo-771.json");
            
            gn = new GrammarNode("grammar");
            GrammarNode gn1 = new GrammarNode("quote1");
            gn1.AddInstruction("^quote", "pushquote");
            gn1.AddInstruction("quote", "popquote");
            GrammarNode.AddASTNodeSequence(gn, 0, gn1);

            GrammarNode gn2 = new GrammarNode("openingbracket1");
            gn2.AddInstruction("^quote", "pushblock1");
            gn2.AddInstruction("quote", "asquote");
            GrammarNode.AddASTNodeSequence(gn, 0, gn2);

            GrammarNode gn3 = new GrammarNode("closingbracket1");
            gn3.AddInstruction("^quote", "popblock1");
            gn3.AddInstruction("quote", "asquote");
            GrammarNode.AddASTNodeSequence(gn, 0, gn3);

            GrammarNode gn9 = new GrammarNode("openingbracket2");
            gn9.AddInstruction("^quote", "pushblock2");
            gn9.AddInstruction("quote", "asquote");
            GrammarNode.AddASTNodeSequence(gn, 0, gn9);

            GrammarNode gn10 = new GrammarNode("closingbracket2");
            gn10.AddInstruction("^quote", "popblock2");
            gn10.AddInstruction("quote", "asquote");
            GrammarNode.AddASTNodeSequence(gn, 0, gn10);

            GrammarNode gn11 = new GrammarNode("openingbracket3");
            gn11.AddInstruction("^quote", "pushblock3");
            gn11.AddInstruction("quote", "asquote");
            GrammarNode.AddASTNodeSequence(gn, 0, gn11);

            GrammarNode gn12 = new GrammarNode("closingbracket3");
            gn12.AddInstruction("^quote", "popblock3");
            gn12.AddInstruction("quote", "asquote");
            GrammarNode.AddASTNodeSequence(gn, 0, gn12);

            GrammarNode gn4 = new GrammarNode("semicolon");
            gn4.AddInstruction("^quote", "split");
            gn4.AddInstruction("quote", "asquote");
            GrammarNode.AddASTNodeSequence(gn, 0, gn4);

            GrammarNode gn6 = new GrammarNode("text");
            gn6.AddInstruction("^quote", "normal");
            gn6.AddInstruction("quote", "asquote");
            GrammarNode.AddASTNodeSequence(gn, 0, gn6);

            GrammarNode gn7 = new GrammarNode("enter");
            gn7.AddInstruction("^quote", "normal");
            gn7.AddInstruction("quote", "asquote");
            GrammarNode.AddASTNodeSequence(gn, 0, gn7);

            GrammarNode gn14 = new GrammarNode("space");
            gn14.AddInstruction("^quote", "normal");
            gn14.AddInstruction("quote", "asquote");
            GrammarNode.AddASTNodeSequence(gn, 0, gn14);

            GrammarNode gn8 = new GrammarNode("other");
            gn8.AddInstruction("^quote", "normal");
            gn8.AddInstruction("quote", "asquote");
            GrammarNode.AddASTNodeSequence(gn, 0, gn8);

            GrammarNode gn13 = new GrammarNode("escape");
            gn13.AddInstruction("^quote", "normal");
            gn13.AddInstruction("quote", "asquote");
            GrammarNode.AddASTNodeSequence(gn, 0, gn13);
        }
        public void parse_content()
        {
            scanner = new Scanner(patterns, content);
            List<ASTNode> nodes = scanner.ScanAll(); // With Sorted Order

            foreach (ASTNode node in nodes)
            {
                Console.WriteLine(node.type+" "+node.Offset+" "+node.Content.Length);
            }

            bp = new BlockParser(gn, nodes);//识别括号引号
            ASTNode root = bp.Parse();
            ASTNode.Visualize(root);

            //text box processing to prevent bug
            int backspaceCount = 0;
            ASTNode.OffsetAdjustToWinForm(root, ref backspaceCount);
            //End text box processing to prevent bug 

            this.fileTreeView.Nodes.Clear();
            this.fileTreeView.Nodes.Add(root);
            this.fileTreeView.ExpandAll();

            List<ASTNode> list = new List<ASTNode>();
            ASTNode.LoopRead(root, ref list);
            Console.WriteLine(list.Count);

            //show labels
            #region Show TextBox----------------
            RichTextBoxSelectionStart = this.richTextBox1.SelectionStart;
            foreach (ASTNode node in list)
            {
                switch (node.type)
                {
                    case "openingbracket1":
                        {
                            richTextBox1.Select(node.Offset, node.Content.Length);
                            richTextBox1.SelectionColor = Color.Red;
                            break;
                        }
                    case "closingbracket1":
                        {
                            richTextBox1.Select(node.Offset, node.Content.Length);
                            richTextBox1.SelectionColor = Color.Red;
                            break;
                        }

                    case "openingbracket2":
                        {
                            richTextBox1.Select(node.Offset, node.Content.Length);
                            richTextBox1.SelectionColor = Color.Green;
                            break;
                        }
                    case "closingbracket2":
                        {
                            richTextBox1.Select(node.Offset, node.Content.Length);
                            richTextBox1.SelectionColor = Color.Green;
                            break;
                        }

                    case "text":
                        {
                            richTextBox1.Select(node.Offset, node.Content.Length);
                            richTextBox1.SelectionColor = Color.Blue;
                            break;
                        }
                    case "quote":
                        {
                            richTextBox1.Select(node.Offset, node.Content.Length);
                            richTextBox1.SelectionColor = Color.Brown;
                            break;
                        }
                    case "space":
                        {
                            richTextBox1.Select(node.Offset, node.Content.Length);
                            richTextBox1.SelectionBackColor = Color.Blue;
                            break;
                        }
                    case "other":
                        {
                            richTextBox1.Select(node.Offset, node.Content.Length);
                            richTextBox1.SelectionBackColor = Color.Red;
                            break;
                        }
                    case "escape":
                        {
                            richTextBox1.Select(node.Offset, node.Content.Length);
                            richTextBox1.SelectionBackColor = Color.Brown;
                            break;
                        }
                    case "enter":
                        {
                           // richTextBox1.Select(node.Offset, node.Content.Length);
                           // richTextBox1.SelectionBackColor = Color.Black;
                            break;
                        }
                }
            }
            this.richTextBox1.SelectionStart = RichTextBoxSelectionStart;
            this.richTextBox1.ScrollToCaret();
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
                try
                {
                    parse_content();
                }
                catch { }
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
