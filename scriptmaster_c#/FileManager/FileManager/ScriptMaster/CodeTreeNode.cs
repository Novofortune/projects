using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptMaster
{
    public class CodeTreeNode:System.Windows.Forms.TreeNode
    {
        public CodeSegment codeSegment { get; set; }
    }
    public class CodeTreeView : System.Windows.Forms.TreeView
    {
        public Dictionary<string, CodeTreeNode> allNodes { get; set; }
        public CodeTreeNode CurrentNode { get; set; }
        public void draw(CodeTreeNode node)// 显示出所有在此NODE下的树形结构
        {
            this.Nodes.Clear(); //刷新Nodes
            this.Nodes.Add(node); //重新加载ROOT NODE
            //重新展开到当前NODE
            List<CodeTreeNode> Ancestors = new List<CodeTreeNode>();
            LoopExpand(node, ref Ancestors); //寻找祖先
            for (int i = Ancestors.Count-1; i >=0 ;i-- ) //倒序寻找祖先
            {
                Ancestors[i].ExpandAll(); //按照祖先级别展开树形结构
            }
        }
        private void LoopExpand(CodeTreeNode node, ref List<CodeTreeNode> parents)
        {
            if(CurrentNode.Parent!=null)
            {
                parents.Add(CurrentNode.Parent as CodeTreeNode);
                LoopExpand(CurrentNode.Parent as CodeTreeNode,ref parents);
            }
        }

        public static CodeTreeView Extract(string source)
        {
            CodeTreeView codeTreeView = new CodeTreeView();

            return codeTreeView;
        }
    }


    
}
