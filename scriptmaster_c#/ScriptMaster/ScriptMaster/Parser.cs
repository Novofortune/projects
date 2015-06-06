using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;


namespace ScriptMaster
{
    public abstract class Parser1
    {
        public Dictionary<string, string> brackets { get; set; }
        public List<string> separators { get; set; }
        public List<BlockNode> BlockNodes { get; set; }
        public Parser1()
        {
            brackets = new Dictionary<string, string>();
            separators = new List<string>();
            BlockNodes = new List<BlockNode>();
        }
        public virtual void Parse(string source, ref CodeTreeView outputTree)
        {

        }
        public void AddBracketRule(string start,string end)
        {
            if (!brackets.ContainsKey(start)) { brackets.Add(start, end); }
        }
        public void AddSeparatorRule(string separator)
        {
            if (!separators.Contains(separator)) { separators.Add(separator); }
        }

        //public void FindBlock(BlockNode node, string RestNodeContetnt, int depth = 0)//This Method Divides the string into three parts: PreOrder, InOrder, PostOrder
        //{
        //    this.BlockNodes.Add(node);
        //    node.Depth = 0;
        //    string source = node.Content;
        //    int SourceOffset = node.Offset;

        //    int min = source.Length - 1;
        //    List<Match> results = new List<Match>();
        //    BlockNode substrings = new BlockNode();
        //    for (int i = 0; i < brackets.Count; i++)
        //    {
        //        Match match = RegexMatch(source, brackets.ElementAt(i).Key);
        //        if (match.Index < min)
        //        {
        //            min = match.Index;
        //            results.Add(match);
        //        }
        //    }
        //    if (results.Count > 0)
        //    {
        //        Match FirstMatch = results[results.Count - 1];

        //        string prev = "";
        //        string next = "";
        //        if (FirstMatch.Captures.Count != 0) // If there is the start match
        //        {
        //            next = source.Substring(FirstMatch.Index + 1);
        //            prev = source.Substring(0, FirstMatch.Index);
        //            if (brackets.ContainsKey(FirstMatch.Value))
        //            {
        //                Match LastMatch = RegexMatch(next, brackets[FirstMatch.Value]);
        //                if (LastMatch.Captures.Count != 0) //If there is the end match
        //                {
        //                    BlockNode child0 = new BlockNode();
        //                    child0.Offset = SourceOffset;
        //                    child0.Content = prev;
        //                    BlockNode child1 = new BlockNode();
        //                    child1.Offset = SourceOffset + FirstMatch.Index + FirstMatch.Length;
        //                    child1.Content = next.Substring(0, LastMatch.Index);
        //                    //BlockNode sibling = new BlockNode();
        //                    //sibling.Offset = SourceOffset + FirstMatch.Index + LastMatch.Index + FirstMatch.Length + LastMatch.Length;
        //                    RestContent = next.Substring(LastMatch.Index + 1);

        //                    if (child0.Content != "")
        //                    {
        //                        node.Children.Add(child0);
        //                        FindBlock(child0, RestNodeContetnt, depth);
        //                    }
        //                    if (child1.Content != "")
        //                    {
        //                        node.Children.Add(child1);
        //                        FindBlock(child1,RestNodeContetnt, depth);

        //                    }
        //                    if (child2.Content != "")
        //                    {
        //                        node.Children.Add(child2);
        //                        FindBlock(child2,RestNodeContetnt, depth);
        //                    }
        //                }
        //                else { }
        //            }
        //            else { }
        //        }
        //        else { }
        //    }
        //    else { }
        //}
        

        public static MatchCollection RegexMatches(string source,string pattern,bool if_reverse=false)
        {
            //RegexOptions opt = new RegexOptions();
            System.Text.RegularExpressions.Regex regex = new Regex(pattern);
          
            MatchCollection mc = regex.Matches(source);
            return mc;
        }
        public static Match RegexMatch(string source, string pattern)
        {
            System.Text.RegularExpressions.Regex regex = new Regex(pattern);
            Match m = regex.Match(source);
            return m;
        }
    }

    public class BlockNode
    {
        public int Offset { get; set; }
        public string Content { get; set; }
        public BlockNode Parent { get; set; }
        public List<BlockNode> Children { get; set; }
        public int Depth { get; set; }
        public BlockNode()
        {
            Children = new List<BlockNode>();
        }
    }
    //public class PhpParser:Parser
    //{
    //    public override void Parse(string source, ref CodeTreeView outputTree)
    //    {
    //        base.Parse( source, ref  outputTree);
    //    }

    //}
}
