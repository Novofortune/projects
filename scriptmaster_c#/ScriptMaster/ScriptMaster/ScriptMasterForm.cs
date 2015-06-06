using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Irony.Parsing;
using Irony.Ast;

namespace ScriptMaster
{
    public partial class ScriptMasterForm : Form
    {
        public ScriptMasterForm()
        {
            InitializeComponent();
            this.FormClosed += Form1_FormClosed;
            this.Text = Program.program_version;
            this.textBox1.TextChanged += textBox1_TextChanged;
            this.textBox1.Enabled = false;
        }

        public void ShowParseTree(ParseTree _parseTree)
        {
            treeView1.Nodes.Clear();
            if (_parseTree == null) return;
            AddParseNodeRec(null, _parseTree.Root);
        }
        private void AddParseNodeRec(TreeNode parent, ParseTreeNode node)
        {
            if (node == null) return;
            string txt = node.ToString();
            TreeNode tvNode = (parent == null ? treeView1.Nodes.Add(txt) : parent.Nodes.Add(txt));
            tvNode.Tag = node;
            foreach (var child in node.ChildNodes)
                AddParseNodeRec(tvNode, child);
        }
        public void ShowAstTree(ParseTree _parseTree)
        {
            codeTreeView1.Nodes.Clear();
            if (_parseTree == null || _parseTree.Root == null || _parseTree.Root.AstNode == null) return;
            AddAstNodeRec(null, _parseTree.Root.AstNode);
        }

        private void AddAstNodeRec(TreeNode parent, object astNode)
        {
            if (astNode == null) return;
            string txt = astNode.ToString();
            TreeNode newNode = (parent == null ?
              codeTreeView1.Nodes.Add(txt) : parent.Nodes.Add(txt));
            newNode.Tag = astNode;
            var iBrowsable = astNode as IBrowsableAstNode;
            if (iBrowsable == null) return;
            var childList = iBrowsable.GetChildNodes();
            foreach (var child in childList)
                AddAstNodeRec(newNode, child);
        }
        void textBox1_TextChanged(object sender, EventArgs e)
        {
            Program.content = this.textBox1.Text;
        }

        void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
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
            Program.file_path = Directory.GetCurrentDirectory() + "\\new file";
            this.Text = "new file";
            this.textBox1.Enabled = true;
            Program.content = "Please Enter Your Content Here...";

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

          //  try
            {
                // Open the selected file to read.
                string FileName = Program.file_path;
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
                        writer.Write(Program.content);
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
                            writer.Write(Program.content);
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
            if (Program.file_path != null)
            {
                Program.file_path = null;
                Program.content = "";
                this.textBox1.Enabled = false;
                this.Text = Program.program_version;
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
                        Program.file_path = FileName;
                        this.Text = FileName;
                        this.textBox1.Enabled = true;
                        Program.content = reader.ReadToEnd();
                    }
                    fileStream.Close();
                }
                //catch
                {
                   // throw new Exception();
                }
            }
        }
        
    }
}
