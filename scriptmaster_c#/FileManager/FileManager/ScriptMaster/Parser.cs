using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;


namespace ScriptMaster
{


    public class JsonParser:Parser
    {
        
        public ASTNode parse(string data,VocabularyNode spelling,GrammarNode rule)
        {
            
            ASTNode gn = new ASTNode();
            this.root = gn;
            this.curpos = 0;
            while (this.curpos < data.Length)
            {
                //LoopScan(data, spelling);
                LoopParse(gn,data,rule);
            }
            return gn;
        }
        
       
        private void LoopParse(ASTNode gn,string data,GrammarNode rule)
        {
            //if(data[this.curpos]
            this.curpos++;
        }
    }



    public class MyScanner
    {
        public Parser parser { get; set; }
        public VocabularyNode spelling { get; set; }
        public int curpos { get; set; }
        public string data { get; set; }
        public string state { get; set; }
        public ASTNode CurrentASTNode { get; set; }
        public StringBuilder ASTNodeBuilder { get; set; }
        public MyScanner(VocabularyNode spelling,string data)
        {
            reset(spelling, data);
        }
        public void reset(){
            this.curpos = 0;
            this.state = "off";
            this.ASTNodeBuilder = new StringBuilder();
            this.CurrentASTNode = new ASTNode();
        }
        public void reset(VocabularyNode spelling,string data){
            
            this.spelling = spelling;
            this.data = data;
            
            reset();
        }
        public ASTNode Scan()
        {
                return LoopScan(data,spelling);
        }
        private ASTNode LoopScan(string data, VocabularyNode spelling)
        {
            if (this.curpos >= data.Length) { return null; }
            if (spelling.childNodes.ContainsKey('.')) // Match all
            {
                #region '.'
                if (this.state == "on") // Continuing
                {
                    this.ASTNodeBuilder.Append(data[this.curpos]);
                    if (spelling.childNodes['.'].childNodes.Count == 0) // Check success
                    {
                        this.CurrentASTNode = new ASTNode();
                        this.CurrentASTNode.Offset = this.curpos - this.ASTNodeBuilder.Length + 1;
                        this.CurrentASTNode.Content = this.ASTNodeBuilder.ToString();
                        this.CurrentASTNode.type = "token";
                        this.ASTNodeBuilder.Clear();
                        this.state = "off";
                        this.curpos++;
                        return this.CurrentASTNode;
                    }
                    this.curpos++;
                    return LoopScan(data, spelling.childNodes['.']);
                }
                else if (this.state == "off") // Starting 
                {
                    this.state = "on";
                    this.ASTNodeBuilder.Clear(); //refresh the ASTNode builder
                    this.ASTNodeBuilder.Append(data[this.curpos]);
                    if (spelling.childNodes['.'].childNodes.Count == 0) // Check success
                    {
                        this.CurrentASTNode = new ASTNode();
                        this.CurrentASTNode.Offset = this.curpos - this.ASTNodeBuilder.Length + 1;
                        this.CurrentASTNode.Content = this.ASTNodeBuilder.ToString();
                        this.CurrentASTNode.type = "token";
                        this.ASTNodeBuilder.Clear();
                        this.state = "off";
                        this.curpos++;
                        return this.CurrentASTNode;
                    }
                    this.curpos++;
                    return LoopScan(data, spelling.childNodes['.']);
                }
                else { return null; }
                #endregion
            }
            else if(spelling.childNodes.ContainsKey('+'))// Repeat the last until the next match appear 
            {
                return LoopScan(data, spelling.childNodes['+']);
            }
                else if (spelling.childNodes.ContainsKey(data[this.curpos])) // Precise Matching
                {
                    if (this.state == "on") // Continuing
                    {
                        this.ASTNodeBuilder.Append(data[this.curpos]);
                        if (spelling.childNodes[data[this.curpos]].childNodes.Count == 0) // Check success
                        {
                            this.CurrentASTNode = new ASTNode();
                            this.CurrentASTNode.Offset = this.curpos - this.ASTNodeBuilder.Length+1;
                            this.CurrentASTNode.Content = this.ASTNodeBuilder.ToString();
                            this.CurrentASTNode.type = "token";
                            this.ASTNodeBuilder.Clear();
                            this.state = "off";
                            this.curpos++;
                            return this.CurrentASTNode;
                        }
                        this.curpos++;
                        return LoopScan(data, spelling.childNodes[data[this.curpos - 1]]);
                    }
                    else if (this.state == "off") // Starting 
                    {
                        this.state = "on";
                        this.ASTNodeBuilder.Clear(); //refresh the ASTNode builder
                        this.ASTNodeBuilder.Append(data[this.curpos]);
                        if (spelling.childNodes[data[this.curpos]].childNodes.Count == 0) // Check success
                        {
                            this.CurrentASTNode = new ASTNode();
                            this.CurrentASTNode.Offset = this.curpos - this.ASTNodeBuilder.Length+1;
                            this.CurrentASTNode.Content = this.ASTNodeBuilder.ToString();
                            this.CurrentASTNode.type = "token";
                            this.ASTNodeBuilder.Clear();
                            this.state = "off";
                            this.curpos++;
                            return this.CurrentASTNode;
                        }
                        this.curpos++;
                        return LoopScan(data, spelling.childNodes[data[this.curpos - 1]]);
                    }
                    else { return null; }
                }
                else // This means the Vocabulary match fails
                {
                    if (this.state == "on")
                    {
                        if (spelling.parentNode != null)
                        {
                            if (spelling.parentNode.childNodes.ContainsKey('+')) // if it fails, then check if the parentNode has '*' rule, if so, run a second check
                            {
                                return LoopScan(data, spelling.parentNode.parentNode);
                            }
                            else
                            {
                                this.state = "off";
                                this.ASTNodeBuilder.Clear();
                                this.curpos++;
                                return LoopScan(data, this.spelling);
                            }
                        }
                        else
                        {
                            this.state = "off";
                            this.ASTNodeBuilder.Clear();
                            this.curpos++;
                            return LoopScan(data, this.spelling);
                        }
                    }
                    else if (this.state == "off")
                    {
                        this.curpos++;
                        return LoopScan(data, this.spelling);
                    }
                    else { return null; }
                }
            }
        
    }
    public class Scanner
    {
        public Parser parser { get; set; }
        public Dictionary<string,string> patterns { get; set; }
        public int curpos { get; set; }
        public string data { get; set; }
        public string state { get; set; }
        public ASTNode CurrentASTNode { get; set; }
        public StringBuilder ASTNodeBuilder { get; set; }
        public Scanner(Dictionary<string,string> patterns, string data)
        {
            reset(patterns, data);
        }
        public void reset(){
            this.curpos = 0;
            this.state = "off";
            this.ASTNodeBuilder = new StringBuilder();
            this.CurrentASTNode = new ASTNode();
        }
        public void reset(Dictionary<string,string> patterns, string data)
        {

            this.patterns = patterns;
            this.data = data;
            
            reset();
        }
        public List<ASTNode> ScanAll()
        {
            List<ASTNode> nodes = new List<ASTNode>();
            Dictionary<string,MatchCollection> mcs = new Dictionary<string,MatchCollection>();
            foreach (KeyValuePair<string,string> pattern in this.patterns)
            {
                MatchCollection tokens = Regex.Matches(data, pattern.Value, RegexOptions.Singleline);
                mcs.Add(pattern.Key,tokens);
            }
            foreach (KeyValuePair<string,MatchCollection> mc in mcs)
            {
                foreach (Match m in mc.Value)
                {
                    ASTNode node = new ASTNode();
                    node.Content = m.Value;
                    node.type = mc.Key;
                    node.Offset= m.Index;
                    nodes.Add(node);
                }
            }
            nodes = Sort(nodes);
            return nodes;
        }

        private static List<ASTNode> Sort(List<ASTNode> nodes)
        {
            List<ASTNode> newnodes = new List<ASTNode>();
            Dictionary<int, ASTNode> temp = new Dictionary<int, ASTNode>();

            int maxindex = 0;
            foreach (ASTNode node in nodes)
            {
                temp.Add(node.Offset,node);
                if(node.Offset>maxindex){
                    maxindex = node.Offset;
                }
            }
            int i = 0;
            while (i <= maxindex)
            {
                if (temp.ContainsKey(i))
                {
                    newnodes.Add(temp[i]);
                }
                i++;
            }
                return newnodes;
        }
    }
    public class Parser
    {
        public GrammarNode rule { get; set; }
        public int curpos { get; set; }
        public ASTNode root { get; set; }
       
    }

    public class VocabularyNode
    {
        public VocabularyNode parentNode { get; set; }
        public Dictionary<char, VocabularyNode> childNodes { get; set; }
        public static VocabularyNode Create(List<string> WordList)
        {
            VocabularyNode vn = new VocabularyNode();
            vn.childNodes = new Dictionary<char, VocabularyNode>();
            foreach (string s in WordList)
            {
                int i = 0;
                LoopCreate( s , vn,  i);
            }
            return vn;
        }
        public static void LoopCreate(string s ,VocabularyNode vn, int i)
        {
            if (i < s.Length)
            {
                if (vn.childNodes.ContainsKey(s[i]))
                {
                    LoopCreate(s, vn.childNodes[s[i]], i + 1);
                }
                else
                {
                    vn.childNodes.Add(s[i], new VocabularyNode());
                    vn.childNodes[s[i]].parentNode = vn;
                    vn.childNodes[s[i]].childNodes = new Dictionary<char, VocabularyNode>();
                    LoopCreate(s, vn.childNodes[s[i]], i + 1);
                }
            }
        }
    }
    public class GrammarNode{
        public Dictionary<char, GrammarNode> children { get; set; }
    }
    public class ASTNode
    {
        public int Offset { get; set; }
        public string Content { get; set; }
        public ASTNode Parent { get; set; }
        public List<ASTNode> Children { get; set; }
        public int Depth { get; set; }
        public string type { get; set; }
        public ASTNode()
        {
            Children = new List<ASTNode>();
        }
    }
}
