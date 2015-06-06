using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;


namespace FileManager
{
    
    public class FileTreeNode:TreeNode
    {
        public FileNode fileNode;
        public static FileTreeNode Filter(FileTreeNode ftn, ConditionalExpression condition) {
            FileTreeNode treeNode = null;
            if (condition.Equals(true)) // Check the condition and give value to the node...
            {
                treeNode = ftn;
                if (ftn.Nodes != null)
                {
                    for (int i = 0; i < ftn.Nodes.Count; i++)
                    {
                        FileTreeNode childTreeNode = Filter(ftn.Nodes[i] as FileTreeNode,condition);
                        if (childTreeNode != null) // Check if it is null
                        {
                            treeNode.Nodes.Add(childTreeNode);
                        }
                    }
                }
            }
            return treeNode;
        }
        public static FileTreeNode LoadTreeView(FileNode fn)
        {
            FileTreeNode treeNode = new FileTreeNode();
            treeNode.Text = fn.name;
            //Console.WriteLine(fn.name);
            treeNode.fileNode = fn;
            if (fn.children != null)
            {
                for(int i = 0;i<fn.children.Count;i++){
                    FileTreeNode childTreeNode = LoadTreeView(fn.children[i]);
                    treeNode.Nodes.Add(childTreeNode);
                }
                
            }
            return treeNode;
        }
    }
    public class FileOperation
    {
        public static List<FileNode> GetFileTree(string path){
            List<FileNode> nodes = new List<FileNode>();
            FileNode fn = GetFileTreeLoop(path);
            GetFileTreeLoop(fn,ref nodes);
            return nodes;
        }
        private static void GetFileTreeLoop(FileNode node, ref List<FileNode> nodes) {
            nodes.Add(node);
            if (node.children!=null)
            {
                for (int i = 0; i < node.children.Count; i++)
                {
                    GetFileTreeLoop(node.children[i],ref nodes);
                }
            }
        }
        private static FileNode GetFileTreeLoop(string path) {
            FileNode node = new FileNode();
            DirectoryInfo dir = new DirectoryInfo(path); 
            Console.WriteLine(dir.Name);

           
                node.xpath = dir.FullName;
                node.isdir = dir.Attributes.HasFlag(FileAttributes.Directory); // Check if the path is a directory
                node.name = dir.Name;
                node.extension = dir.Extension;

                if (node.isdir)
                {
                    if (dir.Exists)
                    {
                        node.children = new List<FileNode>();
                        FileSystemInfo[] fsi = dir.GetFileSystemInfos();
                        for (int i = 0; i < fsi.Length; i++)
                        {
                            FileNode childNode = GetFileTreeLoop(fsi[i].FullName);
                            node.children.Add(childNode);
                            childNode.parent = node;
                        }
                    }
                }

            return node;
        }
        
    }
    public class FileNode{
        public FileNode parent;
        public List<FileNode> children;
        public string xpath;
        public bool isdir;
        public string name;
        public string extension;
    }
}
