using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileManager
{
    public partial class TreeViewStorageForm : Form
    {
        public List<FileNode> fns;
        public List<FileTreeNode> ftns;
        public FileTreeNode ftn;
        private string curdir;
        public TreeViewStorageForm()
        {
            InitializeComponent();
            firstloadTreeView();
            
        }


        public void firstloadTreeView()
        {
            this.ftns = new List<FileTreeNode>();
            this.treeView1.Nodes.Clear();
            this.curdir = "treeviews";
            this.fns = FileNode.GetFileTree(this.curdir);

            if (this.fns.Count > 0)
            {
                this.ftns.Clear();
                this.ftn = this.loadTreeView(this.fns[0], ref ftns);
                this.treeView1.Nodes.Add(this.ftn);
                this.treeView1.ExpandAll();
                //this.textBox2.Text = "File Count:" + ftns.Count.ToString();
            }
        }
        public FileTreeNode loadTreeView(FileNode fn, ref List<FileTreeNode> ftns)
        {
            FileTreeNode ftn = FileTreeNode.LoadTreeView(fn, ref ftns);
            ColorLabel(ftn);
            return ftn;
        }


        public void ColorLabel(FileTreeNode node)
        {
            if (node.fileNode.isdir)
            {
                node.BackColor = Color.Yellow;
            }

            if (node.Nodes.Count > 0)
            {
                foreach (FileTreeNode child in node.Nodes)
                    ColorLabel(child);
            }
        }

        private void setEvents()
        {
            this.treeView1.DragDrop += treeView1_DragDrop;
            this.treeView1.DragEnter += treeView1_DragEnter;
            this.treeView1.ItemDrag += treeView1_ItemDrag;
        }

        void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            this.treeView1.DoDragDrop(e.Item, DragDropEffects.Move);
        }

        void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        void treeView1_DragDrop(object sender, DragEventArgs e)
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
                    //if (DestinationNode.TreeView != NewNode.TreeView)
                    {
                        if (!DestinationNode.fileNode.isdir)
                        {

                        }
                        else
                        {
                           // DestinationNode.fileNode.xpath;

                            DestinationNode.Nodes.Add((TreeNode)NewNode.Clone());
                            DestinationNode.Expand();
                        }

                        //Remove Original Node
                        //NewNode.Remove();
                    }
                }
            }
        }
    }
}
