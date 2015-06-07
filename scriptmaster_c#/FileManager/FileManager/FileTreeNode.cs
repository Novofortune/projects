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
        public static FileNode GetFilesByExtension(FileNode fn, string extension)
        {
            if (extension != "dir"&&extension!="")
            {
                FileNode root = new FileNode();
                root.isdir = true;
                root.extension = "";
                root.xpath = "";
                root.name = extension + " files";
                root.children = new List<FileNode>();
                GetFilesByExtensionLoop(fn, extension, ref root);
                return root;
            }
            else
            {
               return  ExtensionFilter( fn, extension);
            }
        }
        private static void GetFilesByExtensionLoop(FileNode fn, string extension,ref FileNode root)
        {
            if ((extension==""||fn.extension == extension) && !fn.isdir)
            {
                FileNode newfn = FileNode.CloneNoRelation(fn);
                root.children.Add(newfn);
                newfn.parent = root;
            }
            if (fn.children != null)
            {
                foreach (FileNode child in fn.children)
                {
                    GetFilesByExtensionLoop(child, extension, ref root);
                }
            }
            
        }
        public static FileNode ExtensionFilter(FileNode fn, string extension)
        {
            if (extension != "")
            {
                FileNode newfn = new FileNode();
                newfn.isdir = fn.isdir;
                newfn.extension = fn.extension;
                newfn.xpath = fn.xpath;
                newfn.name = fn.name;
                newfn.children = new List<FileNode>();
                if (fn.children != null)
                {
                    foreach (FileNode child in fn.children)
                    {
                        if (child.extension == extension || child.isdir)
                        {
                            FileNode newchild = FileTreeNode.ExtensionFilter(child, extension);
                            newfn.children.Add(newchild);
                            newchild.parent = newfn;
                        }
                    }
                }
                return newfn;
            }
            else
            {
                return fn;
            }
        }
      
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
        private static FileTreeNode LoadTreeView(FileNode fn)
        {
            FileTreeNode treeNode = new FileTreeNode();
            treeNode.Text = fn.name;
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
        public static FileTreeNode LoadTreeView(FileNode fn,ref List<FileTreeNode> nodes )
        {
            FileTreeNode treeNode = new FileTreeNode();
            nodes.Add(treeNode); //Add all node to a collection;
            treeNode.Text = fn.name;
            treeNode.fileNode = fn;
            if (fn.children != null)
            {
                for (int i = 0; i < fn.children.Count; i++)
                {
                    FileTreeNode childTreeNode = LoadTreeView(fn.children[i], ref nodes);
                    treeNode.Nodes.Add(childTreeNode);
                }
            }
            return treeNode;
        }
    }
    [Serializable]
    public class FileNode{
        public FileNode parent;
        public List<FileNode> children;
        public string xpath;
        public bool isdir;
        public string name;
        public string extension;
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
            //Console.WriteLine(dir.Name);

           
                node.xpath = dir.FullName;
                node.isdir = dir.Attributes.HasFlag(FileAttributes.Directory); // Check if the path is a directory
                node.name = dir.Name;
                node.extension = dir.Extension;

                if (node.isdir)
                {
                    if (dir.Exists)
                    {
                        try
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
                        catch { }
                    }
                }

            return node;
        }
        public static FileNode Clone(FileNode fn)
        {
            FileNode newfn = new FileNode();
            newfn.isdir = fn.isdir;
            newfn.extension = fn.extension;
            newfn.xpath = fn.xpath;
            newfn.name = fn.name;
            newfn.children = new List<FileNode>();
            if (fn.children != null)
            {
                for (int i = 0; i < fn.children.Count; i++)
                {
                    FileNode newchild = FileNode.Clone(fn.children[i]);
                    newfn.children.Add(newchild);
                    newchild.parent = newfn;
                }
            }
            return newfn;
        }
        public static FileNode CloneNoRelation(FileNode fn)
        {
            FileNode newfn = new FileNode();
            newfn.isdir = fn.isdir;
            newfn.extension = fn.extension;
            newfn.xpath = fn.xpath;
            newfn.name = fn.name;
            newfn.children = new List<FileNode>();
            return newfn;
        }
        public static void ToList(FileNode fn, ref List<FileNode> fns)
        {
            fns.Add(fn);
            if(fn.children!=null){
                foreach (FileNode child in fn.children)
                {
                    ToList(child,ref fns);
                }
            }
            
        }
    }
}
