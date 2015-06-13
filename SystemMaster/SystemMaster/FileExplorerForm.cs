using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
//using Irony;

namespace FileManager
{
    public partial class FileExplorerForm : Form
    {
        public  string curdir;
        public  List<FileNode> fns;
        public FileTreeNode ftn;
        public List<FileTreeNode> ftns;
        //public FileTreeNode filter_ftn;
        public FileExplorerForm()
        {
            InitializeComponent();
            //--------------------Events---------------------------------
            setEvents();
            //--------------------End Events---------------------------------

        }

        public void firstloadTreeView()
        {
            this.ftns = new List<FileTreeNode>();
            this.treeView1.Nodes.Clear();
            this.curdir = this.textBox1.Text;
            this.fns = FileNode.GetFileTree(this.curdir);

            if (this.fns.Count > 0)
            {
                this.ftns.Clear();
                this.ftn = this.loadTreeView(this.fns[0], ref ftns);
                this.treeView1.Nodes.Add(this.ftn);
                this.treeView1.ExpandAll();
                this.textBox2.Text = "File Count:" + ftns.Count.ToString();
            }
        }
        public FileTreeNode loadTreeView(FileNode fn, ref List<FileTreeNode> ftns)
        {
            FileTreeNode ftn =  FileTreeNode.LoadTreeView(fn, ref ftns);
            ColorLabel(ftn);
            return ftn;
        }
        public void ColorLabel(FileTreeNode node)
        {
            if (node.fileNode.isdir)
            {
                node.BackColor = Color.Yellow;
            }
            
            if(node.Nodes.Count>0){
                foreach(FileTreeNode child in node.Nodes)
                    ColorLabel(child);
            }
        }
        public void setEvents()
        {
            textBox1.KeyDown += textBox1_KeyDown;
            //this.treeView1.NodeMouseClick += treeView1_NodeMouseClick;
            this.treeView1.ItemDrag += treeView1_ItemDrag;
            this.treeView1.DragDrop += treeView2_DragDrop;
            this.treeView1.DragEnter += treeView2_DragEnter;

            this.label2.DragEnter += label2_DragEnter;
            this.label2.DragDrop += label2_DragDrop;
            this.label3.DragDrop += label3_DragDrop;
            this.label3.DragEnter += label3_DragEnter;
            this.textBox3.GotFocus += textBox3_GotFocus;
            this.textBox3.LostFocus += textBox3_LostFocus;
            this.textBox3.KeyDown += textBox3_KeyDown;
        }

        void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string ext = this.textBox3.Text;
                this.curdir = this.textBox1.Text;
                this.fns = FileNode.GetFileTree(this.curdir);

                this.ftns.Clear();
                //FileTreeNode NewTreeNode = FileTreeNode.LoadTreeView(FileTreeNode.ExtensionFilter(fns[0], ext),ref ftns);
                FileTreeNode NewTreeNode = loadTreeView(FileTreeNode.GetFilesByExtension(fns[0], ext), ref ftns);
               
                //this.filter_ftn = NewTreeNode;
                this.treeView1.Nodes.Clear();
                if (NewTreeNode != null)
                {
                    this.treeView1.Nodes.Add(NewTreeNode);
                    this.treeView1.ExpandAll();
                }
                this.textBox2.Text = "File Count:"+ftns.Count.ToString();
            }
        }

        #region Events
        void textBox3_LostFocus(object sender, EventArgs e)
        {
            //this.textBox3.Text = "Extension Filter";
        }

        void textBox3_GotFocus(object sender, EventArgs e)
        {
            this.textBox3.Text = "";
        }

        void label3_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        void label3_DragDrop(object sender, DragEventArgs e)
        {
            FileTreeNode NewNode;
            if (e.Data.GetDataPresent("FileManager.FileTreeNode", false))
            {
                NewNode = (FileTreeNode)e.Data.GetData("FileManager.FileTreeNode");
                if (File.Exists(NewNode.fileNode.xpath))
                {
                    ScriptMaster.ScriptMasterForm smf = new ScriptMaster.ScriptMasterForm();
                    smf.load(NewNode.fileNode.xpath);
                    smf.Show();
                }
            }
        }

        void label2_DragDrop(object sender, DragEventArgs e)
        {
            //Console.WriteLine("One time");
            FileTreeNode NewNode;
            if (e.Data.GetDataPresent("FileManager.FileTreeNode", false))
            {
                NewNode = (FileTreeNode)e.Data.GetData("FileManager.FileTreeNode");
                FileExplorerForm form = new FileExplorerForm();

                form.textBox1.Text = NewNode.fileNode.xpath;
                form.firstloadTreeView();
                form.Show();
            }
        }

        void label2_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        void treeView2_DragDrop(object sender, DragEventArgs e)
        {
            FileTreeNode NewNode;

            if (e.Data.GetDataPresent("FileManager.FileTreeNode", false))
            {
                Point pt = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
                FileTreeNode DestinationNode = ((TreeView)sender).GetNodeAt(pt) as FileTreeNode;
                NewNode = (FileTreeNode)e.Data.GetData("FileManager.FileTreeNode");

                //Console.WriteLine(NewNode.Name);
                if (DestinationNode != null)
                {
                    if (DestinationNode.TreeView != NewNode.TreeView)
                    {
                        DestinationNode.Nodes.Add((TreeNode)NewNode.Clone());
                        DestinationNode.Expand();
                        //Remove Original Node
                        NewNode.Remove();
                    }
                }
            }
        }

      
        void treeView2_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

       

        

        void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            this.treeView1.DoDragDrop(e.Item,DragDropEffects.Move);
            //Console.WriteLine(e.Item);
        }

        void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //Console.WriteLine(e.Node);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            ScriptMaster.ScriptMasterForm form = new ScriptMaster.ScriptMasterForm();
            form.program_version = "ScriptMaster 1.0";
            form.CodeTreeViews = new List<ScriptMaster.CodeTreeView>();
            form.codeTreeView = new ScriptMaster.CodeTreeView();
            form.CodeTreeViews.Add(form.treeView1); 
            form.Show();
        }

        void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                firstloadTreeView();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            firstloadTreeView();
        }
        #endregion 

        private void exportTreeViewToolStripMenuItem_Click(object sender, EventArgs e)
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
                            BinaryFormatter bf = new BinaryFormatter();
                            bf.Serialize(fileStream, this.fns[0]);
                            //writer.Write( System.Web.Script.Serialization. this.fns[0]);
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

        private void importTreeViewToolStripMenuItem_Click(object sender, EventArgs e)
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
                        //this.file_path = FileName;
                        //this.Text = FileName;
                        //this.textBox1.Enabled = true;
                        //this.content = reader.ReadToEnd();
                        BinaryFormatter bf = new BinaryFormatter();
                        this.fns[0] = bf.Deserialize(fileStream) as FileNode;
                        this.ftn = this.loadTreeView(this.fns[0],ref this.ftns);
                        this.treeView1.Nodes.Clear();
                        this.treeView1.Nodes.Add(ftn);
                        this.curdir = fns[0].xpath;
                        this.textBox1.Text = this.curdir;
                        this.treeView1.ExpandAll();
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
