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
            ftns = new List<FileTreeNode>();
            //--------------------Events---------------------------------
            setEvents();
            //--------------------End Events---------------------------------

        }

        public void loadTreeView()
        {
            this.treeView1.Nodes.Clear();
            this.treeView2.Nodes.Clear();
            this.curdir = this.textBox1.Text;
            this.fns = FileNode.GetFileTree(this.curdir);

            if (this.fns.Count > 0)
            {
                this.ftns.Clear();
                this.ftn = FileTreeNode.LoadTreeView(this.fns[0],ref ftns);
                this.treeView1.Nodes.Add(this.ftn);
                this.treeView2.Nodes.Add(this.ftn.Clone() as FileTreeNode);
                this.treeView1.ExpandAll();
                this.treeView2.ExpandAll();
                this.textBox2.Text = "File Count:" + ftns.Count.ToString();
               
            }
        }
       
        public void setEvents()
        {
            textBox1.KeyDown += textBox1_KeyDown;
            //this.treeView1.NodeMouseClick += treeView1_NodeMouseClick;
            this.treeView1.ItemDrag += treeView1_ItemDrag;
            this.treeView2.DragDrop += treeView2_DragDrop;
            this.treeView2.DragEnter += treeView2_DragEnter;

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
                FileTreeNode NewTreeNode = FileTreeNode.LoadTreeView(FileTreeNode.ExtensionFilter(fns, ext),ref ftns);
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
            this.textBox3.Text = "Extension Filter";
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
                form.loadTreeView();
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
            Console.WriteLine(e.Node);
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
                loadTreeView();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            loadTreeView();
        }
        #endregion 
    }
}
