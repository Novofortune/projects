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
        #region resize
        int pathBoxOffset;
        int fileCountOffset;
        int filterOffset;
        int createOffsetWidth;
        int scriptOffsetWidth;
        int Offset = 20;
        int fileTreeOffsetWidth, fileTreeOffsetHeight;
        #endregion
        bool blnDoubleClick = false;
        //public FileTreeNode filter_ftn;
        public FileExplorerForm()
        {
            InitializeComponent();
            //-----------------------------offset---------
            pathBoxOffset = this.Width - PathBox.Width;
            fileCountOffset = this.Width - FileCountBox.Width;
            filterOffset = this.Width - FilterBox.Width;

            fileTreeOffsetWidth = this.Width - fileTreeView.Width;
            fileTreeOffsetHeight = this.Height - fileTreeView.Height;
            //--------------------end offset---------------------
            //--------------------Events---------------------------------
            setEvents();
            //--------------------End Events---------------------------------

        }

        public void firstloadTreeView()
        {
            this.ftns = new List<FileTreeNode>();
            this.fileTreeView.Nodes.Clear();
            this.curdir = this.PathBox.Text;
            this.fns = FileNode.GetFileTree(this.curdir);

            if (this.fns.Count > 0)
            {
                this.ftns.Clear();
                this.ftn = this.loadTreeView(this.fns[0], ref ftns);
                this.fileTreeView.Nodes.Add(this.ftn);
                this.fileTreeView.ExpandAll();
                this.FileCountBox.Text = "File Count:" + ftns.Count.ToString();
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
            PathBox.KeyDown += textBox1_KeyDown;
            this.fileTreeView.ItemDrag += treeView1_ItemDrag;
            this.fileTreeView.DragDrop += treeView2_DragDrop;
            this.fileTreeView.DragEnter += treeView2_DragEnter;
            this.fileTreeView.NodeMouseDoubleClick += fileTreeView_NodeMouseDoubleClick;

            this.FilterBox.GotFocus += textBox3_GotFocus;
            this.FilterBox.LostFocus += textBox3_LostFocus;
            this.FilterBox.KeyDown += textBox3_KeyDown;
            
        }

      

        void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string ext = this.FilterBox.Text;
                this.curdir = this.PathBox.Text;
                this.fns = FileNode.GetFileTree(this.curdir);

                this.ftns.Clear();
                //FileTreeNode NewTreeNode = FileTreeNode.LoadTreeView(FileTreeNode.ExtensionFilter(fns[0], ext),ref ftns);
                FileTreeNode NewTreeNode = loadTreeView(FileTreeNode.GetFilesByExtension(fns[0], ext), ref ftns);
               
                //this.filter_ftn = NewTreeNode;
                this.fileTreeView.Nodes.Clear();
                if (NewTreeNode != null)
                {
                    this.fileTreeView.Nodes.Add(NewTreeNode);
                    this.fileTreeView.ExpandAll();
                }
                this.FileCountBox.Text = "File Count:"+ftns.Count.ToString();
            }
        }

        #region Events
        void textBox3_LostFocus(object sender, EventArgs e)
        {
            //this.textBox3.Text = "Extension Filter";
        }

        void textBox3_GotFocus(object sender, EventArgs e)
        {
            this.FilterBox.Text = "";
        }

        void scrpitLabel_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        void scriptLabel_DragDrop(object sender, DragEventArgs e)
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

        void createLabel_DragDrop(object sender, DragEventArgs e)
        {
            //Console.WriteLine("One time");
            FileTreeNode NewNode;
            if (e.Data.GetDataPresent("FileManager.FileTreeNode", false))
            {
                NewNode = (FileTreeNode)e.Data.GetData("FileManager.FileTreeNode");
                FileExplorerForm form = new FileExplorerForm();

                form.PathBox.Text = NewNode.fileNode.xpath;
                form.firstloadTreeView();
                form.Show();
            }
        }
          void fileTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
               FileTreeNode NewNode;
               
               NewNode = (FileTreeNode)e.Node;
               if (File.Exists(NewNode.fileNode.xpath))
               {
                   ScriptMaster.ScriptMasterForm smf = new ScriptMaster.ScriptMasterForm();
                   smf.load(NewNode.fileNode.xpath);
                   smf.Show();
               }
               else
               {
                   FileExplorerForm form = new FileExplorerForm();

                   form.PathBox.Text = NewNode.fileNode.xpath;
                   form.firstloadTreeView();
                   form.Show();
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
        void createLabel_DragEnter(object sender, DragEventArgs e)
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
            this.fileTreeView.DoDragDrop(e.Item,DragDropEffects.Move);
            //Console.WriteLine(e.Item);
        }
      
        //void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        //{
        //    //Console.WriteLine(e.Node);
        //     FileTreeNode NewNode;
         
        //}

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ScriptMaster.ScriptMasterForm form = new ScriptMaster.ScriptMasterForm();
            form.program_version = "ScriptMaster 1.0";
            form.Show();
        }

        void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                firstloadTreeView();
            }
        }
        //private void button1_Click(object sender, EventArgs e)
        //{
        //    firstloadTreeView();
        //}
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
                        this.fileTreeView.Nodes.Clear();
                        this.fileTreeView.Nodes.Add(ftn);
                        this.curdir = fns[0].xpath;
                        this.PathBox.Text = this.curdir;
                        this.fileTreeView.ExpandAll();
                    }
                    fileStream.Close();
                }
                //catch
                {
                    // throw new Exception();
                }
            }
        }

        private void FileExplorerForm_Resize(object sender, EventArgs e)
        {
            PathBox.Width = this.Width - pathBoxOffset;
            FileCountBox.Location = new Point(fileTreeView.Location.X+fileTreeView.Width+5, FileCountBox.Location.Y);
            FileCountBox.Width = this.Width - fileCountOffset;
            FilterBox.Location = new Point(FileCountBox.Location.X, FilterBox.Location.Y);
            FilterBox.Width = this.Width - filterOffset;
            fileTreeView.Width = this.Width - fileTreeOffsetWidth;
            fileTreeView.Height = this.Height - fileTreeOffsetHeight;
            formSize.Text = "(" + this.Width + "," + this.Height + ")" + "(" + fileTreeView.Width + "," + fileTreeView.Height + ")";

            if (this.WindowState == FormWindowState.Maximized)
            {
                FileCountBox.Location = new Point(fileTreeView.Location.X + fileTreeView.Width + 5, FileCountBox.Location.Y);
                FileCountBox.Width = this.Width - fileCountOffset;
                FilterBox.Location = new Point(FileCountBox.Location.X, FilterBox.Location.Y);
                FilterBox.Width = this.Width - filterOffset;
                fileTreeView.Width = this.Width - fileTreeOffsetWidth;
                fileTreeView.Height = this.Height - fileTreeOffsetHeight;
            }
            else if (this.WindowState == FormWindowState.Minimized)
            {

                FileCountBox.Location = new Point(308, FileCountBox.Location.Y);
                FileCountBox.Width = this.Width - fileCountOffset;
                FilterBox.Location = new Point(FileCountBox.Location.X, FilterBox.Location.Y);
                FilterBox.Width = this.Width - filterOffset;
                fileTreeView.Width = this.Width - fileTreeOffsetWidth;
                fileTreeView.Height = this.Height - fileTreeOffsetHeight;
            }
        }
        private int getNewboxheight()
        {
            return this.Height / 3 - Offset;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            string tempPath;
            int length;
            int tempIndex = -1;
            tempPath = PathBox.Text;
            length=tempPath.Length;
            tempIndex = tempPath.LastIndexOf('\\');
            if (tempIndex > 0)
            {
                tempPath = tempPath.Substring(0, tempIndex);
                PathBox.Text = tempPath;
                firstloadTreeView();
            }
            
        }

       

       

     

        

  
   
    }
}
