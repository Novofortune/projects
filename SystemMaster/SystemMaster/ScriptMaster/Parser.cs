using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using ScriptMaster;
using System.Windows.Forms;


namespace ScriptMaster
{
    public class Scanner
    {
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
            Dictionary<ASTNode, int> temp1 = new Dictionary<ASTNode, int>(); // used to count each ASTNode character occurance in the end , if the count does not equal its content length, then it was cancelled

            int maxindex = 0;
            foreach (ASTNode node in nodes)
            {
                if (temp.ContainsKey(node.Offset))
                {
                    // Replace the existing node
                    int length = node.Offset + node.Content.Length;
                    for (int i = node.Offset; i < length; i++)
                    {
                        temp[i] = node;
                    }
                }
                else
                {
                    int length = node.Offset+node.Content.Length;
                    for (int i = node.Offset; i < length; i++)
                    {
                        temp.Add(i, node);
                    }
                }


                if (node.Offset > maxindex)
                {
                    maxindex = node.Offset;
                }
            }
            int ii = 0;
            while (ii <= maxindex)
            {
                if (temp.ContainsKey(ii))
                {
                    if (temp1.ContainsKey(temp[ii]))
                    {
                        temp1[temp[ii]]++; // do the count
                    }
                    else
                    {
                        temp1.Add(temp[ii],1); // start the count
                    }
                }
                ii++;
            }

            foreach (KeyValuePair<ASTNode, int> ASTNodeKVP in temp1) {
                newnodes.Add(ASTNodeKVP.Key);
            }
            return newnodes;
        }
    }
    public class BlockParser
    {
        public List<ASTNode> data { get; set; }
        public GrammarNode rules { get; set; }
        public int curpos { get; set; }
        public ASTNodeBuilder ASTNodeBuilder { get; set; }
        public Stack<ASTNode> CurrentNode { get; set; }
        public Stack<string> CurrentState { get; set; }
        public BlockParser(GrammarNode rules, List<ASTNode> data)
        {
            reset(rules, data);
        }
        public void reset(){
            this.curpos = 0;
            this.CurrentNode = new Stack<ASTNode>();
            this.ASTNodeBuilder = new ASTNodeBuilder(this.CurrentNode);
            this.CurrentState = new Stack<string>();
        }
        public void reset(GrammarNode rules, List<ASTNode> data)
        {

            this.rules = rules;
            this.data = data;
            reset();
        }
        public ASTNode Parse()
        {
            ParseBegin();
            Parse(this.rules);
            return ParseEnd();
        }
        private void ParseBegin()
        {
            this.reset();
            //this.CurrentNode.Push(new ASTNode("root"));

            ASTNode block = new ASTNode("root");
            this.CurrentNode.Push(block); // push block

            //ASTNode sentence = new ASTNode("sentence");
            //this.CurrentNode.Push(sentence); // push sentence

            this.CurrentState.Push("none");
        }
        private ASTNode ParseEnd()
        {
            this.curpos = 0;
            this.CurrentState.Pop();
           
                //this.ASTNodeBuilder.Add(this.CurrentNode.Pop());
                return this.ASTNodeBuilder.Add(this.CurrentNode.Pop());
            
        }
        private void Parse(GrammarNode gn)
        {
            while (this.curpos < this.data.Count)
            {
                LoopInterpret(gn);
            }
        }
        private void LoopInterpret(GrammarNode gn)
        {
            string key = this.data[this.curpos].type;
            if (gn.childNodes.ContainsKey(key))
            {
                this.interpret(gn.childNodes[key].instructionrules); // perform instruction
                
                if (gn.childNodes[key].childNodes.Count == 0) // Check Terminal
                {
                    this.curpos++;
                }
                else
                {
                    this.curpos++;
                    LoopInterpret(gn.childNodes[key]); // perform instruction
                }
            }
            else
            {
                this.curpos++;
            }

        }
        private void interpret(Dictionary<string,string> instructionrules)
        {
            //Console.WriteLine(this.CurrentState.First());
            string instruction = "";

            if (instructionrules.ContainsKey(this.CurrentState.First()))
                {
                    instruction = instructionrules[this.CurrentState.First()];
                }
                else
                {

                }
            
            //Console.WriteLine(instruction);
            switch (instruction)
            {
                case "escape":
                    {
                        break;
                    }
                #region comment1
                case "pushcomment1":
                    {
                        ASTNode quote = new ASTNode();
                        quote.type = "comment1";
                        //this.CurrentNode.First().Children.Add(quote);
                        this.CurrentNode.Push(quote); // push quote
                        this.CurrentNode.First().Offset = this.data[this.curpos].Offset;
                        this.CurrentNode.First().Content = this.data[this.curpos].Content;
                        this.CurrentState.Push("comment1");
                        break;
                    } 
                case "ascomment1":
                    {
                       
                            this.CurrentNode.First().Content += this.data[this.curpos].Content; // Append content only
                       
                        break;
                    }
                case "popcomment1":
                    {
                            this.CurrentNode.First().Content += this.data[this.curpos].Content; // Append content
                            this.ASTNodeBuilder.Add(this.CurrentNode.Pop()); // pop quote
                            this.CurrentState.Pop();
                        break;
                    }
                #endregion
                #region comment2
                case "pushcomment2":
                    {
                        ASTNode quote = new ASTNode();
                        quote.type = "comment2";
                        //this.CurrentNode.First().Children.Add(quote);
                        this.CurrentNode.Push(quote); // push quote
                        this.CurrentNode.First().Offset = this.data[this.curpos].Offset;
                        this.CurrentNode.First().Content = this.data[this.curpos].Content;
                        this.CurrentState.Push("comment2");
                        break;
                    } 
                case "ascomment2":
                    {
                       
                            this.CurrentNode.First().Content += this.data[this.curpos].Content; // Append content only
                       
                        break;
                    }
                case "popcomment2":
                    {
                            this.CurrentNode.First().Content += this.data[this.curpos].Content; // Append content
                            this.ASTNodeBuilder.Add(this.CurrentNode.Pop()); // pop quote
                            this.CurrentState.Pop();
                        break;
                    }
                #endregion
                #region quote1
                case "pushquote1":
                    {
                     //   try
                        {
                            ASTNode quote = new ASTNode();
                            quote.type = "quote1";
                            //this.CurrentNode.First().Children.Add(quote);
                            this.CurrentNode.Push(quote); // push quote
                            this.CurrentNode.First().Offset = this.data[this.curpos].Offset;
                            this.CurrentNode.First().Content = this.data[this.curpos].Content;
                            this.CurrentState.Push("quote1");
                        }
                      //  catch { }
                        break;
                    }
                case "asquote1":
                    {
                       // try
                        {
                            this.CurrentNode.First().Content += this.data[this.curpos].Content; // Append content only
                        }
                       // catch { }
                        break;
                    }
                case "popquote1":
                    {
                      //  try
                        {
                            this.CurrentNode.First().Content += this.data[this.curpos].Content; // Append content
                            this.ASTNodeBuilder.Add(this.CurrentNode.Pop()); // pop quote
                            this.CurrentState.Pop();
                        }
                      //  catch { }
                        break;
                    }
                #endregion 
                    
                #region quote2
                case "pushquote2":
                    {
                        //   try
                        {
                            ASTNode quote = new ASTNode();
                            quote.type = "quote2";
                            //this.CurrentNode.First().Children.Add(quote);
                            this.CurrentNode.Push(quote); // push quote
                            this.CurrentNode.First().Offset = this.data[this.curpos].Offset;
                            this.CurrentNode.First().Content = this.data[this.curpos].Content;
                            this.CurrentState.Push("quote2");
                        }
                        //  catch { }
                        break;
                    }
                case "asquote2":
                    {
                        // try
                        {
                            this.CurrentNode.First().Content += this.data[this.curpos].Content; // Append content only
                        }
                        // catch { }
                        break;
                    }
                case "popquote2":
                    {
                        //  try
                        {
                            this.CurrentNode.First().Content += this.data[this.curpos].Content; // Append content
                            this.ASTNodeBuilder.Add(this.CurrentNode.Pop()); // pop quote
                            this.CurrentState.Pop();
                        }
                        //  catch { }
                        break;
                    }
                #endregion 
                #region block1
                case "pushblock1":
                    {
                            ASTNode block = new ASTNode("block1");
                            this.CurrentNode.Push(block); // push block

                            //ASTNode sentence = new ASTNode("sentence");
                            //this.CurrentNode.Push(sentence); // push sentence
                       
                        break;
                    }
                case "popblock1":
                    {
                            //this.ASTNodeBuilder.Add(this.CurrentNode.Pop());// pop sentence
                            this.ASTNodeBuilder.Add(this.CurrentNode.Pop());// pop block
                      
                        break;
                    }
                #endregion
                #region block2
                case "pushblock2":
                    {
                       
                            ASTNode block = new ASTNode("block2");
                            this.CurrentNode.Push(block); // push block

                            //ASTNode sentence = new ASTNode("sentence");
                            //this.CurrentNode.Push(sentence); // push sentence
                       
                        break;
                    }
                case "popblock2":
                    {
                      
                            //this.ASTNodeBuilder.Add(this.CurrentNode.Pop());// pop sentence
                            this.ASTNodeBuilder.Add(this.CurrentNode.Pop());// pop block
                      
                        break;
                    }
                #endregion
                #region block3
                case "pushblock3":
                    {
                      
                            ASTNode block = new ASTNode("block3");
                            this.CurrentNode.Push(block); // push block

                            //ASTNode sentence = new ASTNode("sentence");
                            //this.CurrentNode.Push(sentence); // push sentence
                        
                        break;
                    }
                case "popblock3":
                    {
                       
                            //this.ASTNodeBuilder.Add(this.CurrentNode.Pop());// pop sentence
                            this.ASTNodeBuilder.Add(this.CurrentNode.Pop());// pop block
                       
                        break;
                    }
                #endregion

                #region split
                case "split":
                    {
                        {
                            this.ASTNodeBuilder.Add(this.CurrentNode.Pop());// pop sentence
                            ASTNode sentence = new ASTNode("sentence");
                            //this.CurrentNode.First().Children.Add(sentence);
                            this.CurrentNode.Push(sentence);//push sentence
                        }
                        break;
                    }
                #endregion
                #region normal
                case "normal":
                    {
                            this.CurrentNode.First().Children.Add(this.data[this.curpos]); // Add the normal node to the current structural node children
                            this.data[this.curpos].ParentNode = this.CurrentNode.First(); // Add parent
                            break;
                    }
                #endregion
                default:
                    {
                        break;
                    }
            }
        }
    }

    public class ASTNodeProcessor
    {
        public List<ASTNode> data;
        public int curpos;
        public ASTNode ASTNode;
        public Dictionary<string, string> patterns;

        public ASTNodeProcessor(Dictionary<string, string> patterns, ASTNode ASTNode)
        {
            this.curpos = 0;
            this.patterns = patterns;
            List<ASTNode> nodes = new List<ASTNode>();
            ASTNode.LoopExpand(ASTNode, ref nodes);
            this.ASTNode = ASTNode;
            this.data = nodes;
        }

        public ASTNode Process()
        {
            while (curpos < data.Count)
            {
                foreach (KeyValuePair<string, string> pattern in patterns)
                {
                    Match m = Regex.Match(data[curpos].Content, pattern.Value);
                    if (this.data[curpos].Content.Length != 0)
                    {
                        if (m.Value.Length == this.data[curpos].Content.Length)
                        { //Check if the length of the matched string is the same as the node content string
                            this.data[curpos].type = pattern.Key;
                        }
                    }
                }
                curpos++;
            }
            curpos = 0;
            return this.ASTNode;
        }
        public ASTNode Remove(params string[] types)
        {
            Dictionary<string, string> typedic = new Dictionary<string, string>();
            foreach (string type in types) { typedic.Add(type, type); }

            while (curpos < data.Count)
            {
                if (typedic.ContainsKey(this.data[curpos].type))
                {
                    this.data[curpos].ParentNode.Children.Remove(this.data[curpos]);
                    //this.data[curpos].ParentNode = null;
                }
                curpos++;
            }
            curpos = 0;
            return this.ASTNode;
        }

    }
    //The Reverse Parser is used to supplement and verify SentenceParser
    public class ReverseParser {
        public List<ASTNode> data{get;set;}
        public ASTNode ASTNode{get;set;}
        public int curpos{get;set;}
        public GrammarNode rules { get; set; }
        public Stack<string> CurrentState { get; set; }
        public Stack<ASTNode> CurrentASTNode { get; set; }
        public List<ASTNode> MergeList { get; set; }

        public ReverseParser(GrammarNode gn, List<ASTNode> data, int curpos) {
            this.data = data;
            this.ASTNode = this.data[curpos];
            this.curpos = curpos;
            CurrentState = new Stack<string>();
            CurrentASTNode = new Stack<ScriptMaster.ASTNode>();
            MergeList = new List<ScriptMaster.ASTNode>();
            rules = gn;
        }

        public List<ASTNode> Parse()
        {
            ParseBegin();
            Parse(this.rules);
            return ParseEnd();
        }
        private void ParseBegin()
        {
            this.CurrentState.Push("none");
        }
        private List<ASTNode> ParseEnd()
        {
            this.curpos = 0;
            this.CurrentState.Pop();
            return this.MergeList;
        }
        private void Parse(GrammarNode gn)
        {
            while (this.curpos >0 )
            {
                LoopInterpret(gn);
            }
        }
        private void LoopInterpret(GrammarNode gn)
        {
            string key = this.data[this.curpos].type;
            if (gn.childNodes.ContainsKey(key))
            {
                this.interpret(gn.childNodes[key].instructionrules); // perform instruction

                if (gn.childNodes[key].childNodes.Count == 0) // Check Terminal
                {
                    this.curpos = 0;  //Stop parsing with only one result...

                    LoopInterpret(gn.childNodes[key]); // perform instruction

                }
                else
                {
                    this.curpos--;
                    LoopInterpret(gn.childNodes[key]); // perform instruction
                }
            }
            else
            {
                this.curpos--;
            }

        }
        private void interpret(Dictionary<string, string> instructionrules)
        {
            //Console.WriteLine(this.CurrentState.First());
            string instruction = "";

            if (instructionrules.ContainsKey(this.CurrentState.First()))
            {
                instruction = instructionrules[this.CurrentState.First()];
            }
            else
            {

            }

            //Console.WriteLine(instruction);
            switch (instruction)
            {
                case "premerge":
                    {
                        this.MergeList.Add(this.data[this.curpos]);
                        break;
                    }
               // case "merge":
                   // {
                   //     ASTNode.Merge(this.MergeList.ToArray());
                   //     this.MergeList.Clear();
                   //     break;
                   // }
                default:
                    {
                        break;
                    }
            }
        }
        
    }
    public class SentenceParser
    {
        public List<ASTNode> data { get; set; }
        public GrammarNode rules { get; set; }
        public int curpos { get; set; }
        public ASTNode ASTNode { get; set; }
        public ASTNode NewASTNode { get; set; }
        public Stack<ASTNode> CurrentNode { get; set; }
        public Stack<string> CurrentState { get; set; }
        public List<ASTNode> MergeList { get; set; }

        public Dictionary<string, GrammarNode> ReverseParseRules { get; set; }

        public SentenceParser(GrammarNode rules, ASTNode SourceASTNode, Dictionary<string, GrammarNode> ReverseParseRules = null)
        {
            reset(rules, SourceASTNode, ReverseParseRules);
        }
        private void reset()
        {
            this.curpos = 0;
            this.CurrentNode = new Stack<ASTNode>();
            //this.ASTNodeBuilder = new ASTNodeBuilder(this.CurrentNode);
            this.MergeList = new List<ASTNode>();
            this.CurrentState = new Stack<string>();
            NewASTNode = new ASTNode(this.ASTNode.type);
        }
        private void reset(GrammarNode rules, ASTNode SourceASTNode, Dictionary<string, GrammarNode> ReverseParseRules = null)
        {
            this.rules = rules;
            this.ASTNode = SourceASTNode;
            this.ReverseParseRules = ReverseParseRules;
            
            //List<ASTNode> nodes = new List<ScriptMaster.ASTNode>(); // Expand all ASTNode in the tree transverse sequence , give the parser a linear structure to follow
            //ASTNode.LoopExpand(SourceASTNode,ref nodes);
            
            this.data = ASTNode.ExpandBlock(this.ASTNode);

            reset();
        }
        public ASTNode Parse()
        {
            //this.reset();
            this.CurrentState.Push("none");
            
            while (this.curpos < this.data.Count)
            {
                LoopInterpret(this.rules);
            }
            this.curpos = 0;
            this.CurrentState.Pop();

            ASTNode node =  this.NewASTNode;

            //Start the Next Level 
          
            for (int i = 0; i < this.NewASTNode.Children.Count; i++)
            {
                if (this.NewASTNode.Children[i].Children.Count > 0)
                {
                    SentenceParser sp = new SentenceParser(this.rules, this.NewASTNode.Children[i]);
                    this.NewASTNode.Children[i] =  sp.Parse();
                }
               
            }
            return this.NewASTNode;
        }

        private void LoopInterpret(GrammarNode gn)
        {
            if (this.curpos < this.data.Count)
            {
                string key = this.data[this.curpos].type;
                if (gn.childNodes.ContainsKey(key))
                {
                    //Do ReverseRuleCheck 
                    if (this.ReverseParseRules != null)
                    {
                        if (this.ReverseParseRules.ContainsKey(key))
                        {
                            ReverseParser rp = new ReverseParser(this.ReverseParseRules[key], this.data, this.curpos);
                            List<ASTNode> ReverseMergeList = rp.Parse();
                            foreach (ASTNode node in ReverseMergeList)
                            {
                                this.MergeList = ReverseMergeList.Concat(this.MergeList) as List<ASTNode>;
                            }
                        }
                    }
                    //End ReverseRuleCheck 
                    this.interpret(gn.childNodes[key].instructionrules); // perform instruction
                    if (gn.childNodes[key].childNodes.Count == 0) // Check Terminal
                    {
                        this.curpos++;
                        //LoopInterpret(gn.childNodes[key]); // Check Next Element
                    }
                    else
                    {
                        this.curpos++;
                        LoopInterpret(gn.childNodes[key]); // Check Next Element
                    }
                }
                else
                {
                    this.MergeList.Clear();
                    NewASTNode.Children.Add(this.data[this.curpos]);
                    this.curpos++;
                }
            }

        }
        private void interpret(Dictionary<string, string> instructionrules)
        {
            //Console.WriteLine(this.CurrentState.First());
            string instruction = "";

            if (instructionrules.ContainsKey(this.CurrentState.First()))
            {
                instruction = instructionrules[this.CurrentState.First()];
            }
            else
            {

            }

            //Console.WriteLine(instruction);
            switch (instruction)
            {
                case "premerge": {
                    this.MergeList.Add(this.data[this.curpos]);
                    NewASTNode.Children.Add(this.data[this.curpos]);
                    break;
                }
                case "merge":
                    {
                        this.MergeList.Add(this.data[this.curpos]);
                        //ASTNode.Merge(this.MergeList.ToArray());


                        ASTNode newnode = null;
                        string newtype = null;
                        if (MergeList.Count > 0)
                        {
                            bool IfParentIsTheSame = true;
                            ASTNode parent = MergeList[0].ParentNode;
                            foreach (ASTNode node in MergeList)
                            {
                                if (node.ParentNode != parent) { IfParentIsTheSame = false; }
                            }
                            if (IfParentIsTheSame) // Start Main Logic
                            {
                                // Create a new type 
                                string[] types = new string[MergeList.Count];
                                for (int i = 0; i < MergeList.Count; i++)
                                {
                                    types[i] = MergeList[i].type;
                                }
                                 newtype = String.Join(".", types);
                                // End Create a new type

                                newnode = new ASTNode(newtype);
                               // int index = -1;
                                foreach (ASTNode node in MergeList)
                                {
                                    // link relation
                                    newnode.Children.Add(node); // adopting child
                                    node.ParentNode = newnode; // replacing parent

                                    //unlink relation
                                    //index = parent.Children.IndexOf(node);
                                    //parent.Children.RemoveAt(index); // discarding child //--------------------------------------------------This Step may cause efficiency issue-----------------------------------------------

                                }
                                //link relation 
                                //if (index != -1)
                               // {
                                   // parent.Children.Add(newnode); // adopting the newnode as a child
                                //}
                            } // End Main Logic
                        }

                        if (newtype == this.ASTNode.type && this.MergeList.Count == this.ASTNode.Children.Count) {
                            NewASTNode.Children.Add(this.data[this.curpos]);
                            break; } // This means it is the recursive recognition of previous Nodes

                        foreach(ASTNode mergeling in this.MergeList) // Remove all the premerge elements once the rule is successfully matched
                        {
                            NewASTNode.Children.Remove(mergeling);
                        }
                        NewASTNode.Children.Add(newnode);

                        this.MergeList.Clear();
                        break;
                    }
                default:
                    {
                        this.MergeList.Clear();
                        NewASTNode.Children.Add(this.data[this.curpos]);
                        break;
                    }
            }
        }
    }

    public class GrammarNode
    {
        public string type { get; set; }
        public Dictionary<string, string> instructionrules { get; set; }
        public Dictionary<string, string> staterules { get; set; }
        public GrammarNode parentNode { get; set; }
        public Dictionary<string, GrammarNode> childNodes { get; set; }
        public string ParseMode { get; set; }
        public GrammarNode()
        {
            this.childNodes = new Dictionary<string, GrammarNode>();
            this.instructionrules = new Dictionary<string, string>();
            this.staterules = new Dictionary<string, string>();
        }
        public GrammarNode(string type,GrammarNode parent = null)
        {
            this.childNodes = new Dictionary<string, GrammarNode>();
            this.instructionrules = new Dictionary<string, string>();
            this.staterules = new Dictionary<string, string>();
            this.type = type;
            if (parent != null)
            {
                this.parentNode = parent;
            }
        }
        public GrammarNode(string type, Dictionary<string,string> instructionrules, string parsemode = null) {
            this.childNodes = new Dictionary<string, GrammarNode>();
            this.instructionrules = new Dictionary<string, string>();
            this.staterules = new Dictionary<string, string>();
            this.type = type;
            this.ParseMode = parsemode;

            foreach (KeyValuePair<string, string> kvp in instructionrules) {
                this.AddInstruction(kvp.Key,kvp.Value);
            }
        }
        public static GrammarNode LinkGrammarNode(params GrammarNode[] gns)
        { // Normal Sequence
            GrammarNode gn = null;
            if (gns.Length == 0) { }
            else if (gns.Length == 1) { gn = gns[0]; }
            else
            {
                gn = gns[0];
                for (int i = 0; i < gns.Length - 1; i++)
                {
                    gns[i + 1].parentNode = gns[i];
                    gns[i].childNodes.Add(gns[i + 1].type, gns[i + 1]);
                }
            }
            return gn;
        }
        private static GrammarNode CreateGrammarNodeSequence(string state,string instruction, params string[] GrammarNodeTypeSequence)
        {
            List<GrammarNode> gns = new List<GrammarNode>();
            GrammarNode gn = null;
            for (int i = 0; i < GrammarNodeTypeSequence.Length;i++ )
            {
                if (i == 0) // if it is the first element
                {
                    gn = new GrammarNode(GrammarNodeTypeSequence[i]);
                    gns.Add( gn);
                }
                else if (i == GrammarNodeTypeSequence.Length - 1) // if it is the last element, add state and instruction
                {
                    gn = new GrammarNode(GrammarNodeTypeSequence[i], gns[i - 1]); 
                    gn.AddInstruction(state, instruction);
                    gns.Add(gn);
                }
                else
                {
                    gn = new GrammarNode(GrammarNodeTypeSequence[i], gns[i-1]);
                    gns.Add(  gn);
                }
            }
            return gn;
        }
        public static void AddGrammarNodeSequence( GrammarNode gn,  GrammarNode GrammarNodeSequence)
        {
                if (gn.childNodes.ContainsKey(GrammarNodeSequence.type))
                {
                    
                    if ( GrammarNodeSequence.childNodes.Count>0)
                    {
                        foreach(KeyValuePair<string,GrammarNode> kvp in GrammarNodeSequence.childNodes){
                            AddGrammarNodeSequence(GrammarNodeSequence, kvp.Value );
                        }
                    }
                }
                else
                {
                    gn.childNodes.Add(GrammarNodeSequence.type, GrammarNodeSequence);
                    gn.childNodes[GrammarNodeSequence.type].parentNode = gn;
                    //gn.childNodes[GrammarNodeSequence.type].childNodes = new Dictionary<string, GrammarNode>();
                    
                    //if (GrammarNodeSequence.childNodes.Count > 0)
                   // {
                     //   foreach (KeyValuePair<string, GrammarNode> kvp in GrammarNodeSequence.childNodes)
                      //  {
                      //      AddGrammarNodeSequence(GrammarNodeSequence, kvp.Value);
                      //  }
                   // }
                }
            
           
        }

        public void AddInstruction(string state,string instruction)
        {
                this.instructionrules.Add(state, instruction);
        }
    }
    public class ASTNode : TreeNode
    {
        public int Offset { get; set; }
        public string Content { get; set; }
        public ASTNode ParentNode { get; set; }
        public List<ASTNode> Children { get; set; }
        public ASTNode PreviousSibling { get; set; }
        public ASTNode NextSibling { get; set; }

        public int Depth { get; set; }
        public string type { get; set; }
       // public Dictionary<string, string> Property { get; set; }
        public ASTNode()
        {
           // Property = new Dictionary<string, string>();
            Children = new List<ASTNode>();
        }

        public ASTNode(string type)
        {
            this.type = type;
            this.Content = "";
           // Property = new Dictionary<string, string>();
            Children = new List<ASTNode>();
        }

        public static List<ASTNode> ExpandBlock(ASTNode block)
        {
            List<ASTNode> nodes = new List<ASTNode>();
            if (block.Children != null)
            {
                foreach (ASTNode node in block.Children)
                {
                    nodes.Add(node);
                }
            }
            return nodes;
        }



        public static void LoopExpand(ASTNode node, ref List<ASTNode> output)
        {
            output.Add(node);
           
                for (int i = 0; i < node.Children.Count; i++)
                {
                    if (i == node.Children.Count - 1)
                    {

                    }
                    else
                    {
                        node.Children[i].NextSibling = node.Children[i + 1];
                        node.Children[i + 1].PreviousSibling = node.Children[i];
                    }
                    LoopExpand(node.Children[i], ref output);
                }
            
        }
        public static void Visualize(ASTNode ThisNode)
        {
            ThisNode.Text = ThisNode.type;
            foreach (ASTNode child in ThisNode.Children)
            {
                ThisNode.Nodes.Add(child);
                Visualize(child);
            }
        }

        public static void OffsetAdjustToWinForm(ASTNode ThisNode,ref int backspaceCount)
        {
            ThisNode.Offset = ThisNode.Offset - backspaceCount;
            if (ThisNode.type == "enter")
            {
                backspaceCount++;
            }

            for (int i = 0; i < ThisNode.Nodes.Count;i++ ) {
                OffsetAdjustToWinForm(ThisNode.Nodes[i] as ASTNode,ref backspaceCount);
            }
        }




        public static ASTNode Merge(params ASTNode[] nodes) {
            ASTNode newnode = null;
            if (nodes.Length > 0)
            {
                bool IfParentIsTheSame = true;
                ASTNode parent = nodes[0].ParentNode;
                foreach (ASTNode node in nodes)
                {
                    if (node.ParentNode != parent) { IfParentIsTheSame = false; }
                }
                if (IfParentIsTheSame) // Start Main Logic
                {
                    // Create a new type 
                    string[] types = new string[nodes.Length];
                    for(int i=0;i<nodes.Length;i++){
                        types[i] = nodes[i].type;
                    }
                    string newtype = String.Join(".",types);
                    // End Create a new type

                    newnode = new ASTNode(newtype);
                    int index = -1;
                    foreach (ASTNode node in nodes)
                    {
                        // link relation
                        newnode.Children.Add(node); // adopting child
                        node.ParentNode = newnode; // replacing parent

                        //unlink relation
                         index = parent.Children.IndexOf(node);
                        parent.Children.RemoveAt(index); // discarding child //--------------------------------------------------This Step may cause efficiency issue-----------------------------------------------

                    } 
                    //link relation 
                    if (index != -1)
                    {
                        parent.Children.Insert(index, newnode); // adopting the newnode as a child
                    }
                } // End Main Logic
            }
            return newnode;
        }
       
    }
    public class ASTNodeBuilder
    {
        public Stack<ASTNode> CurrentNode;
       public ASTNodeBuilder(Stack<ASTNode> CurrentASTNode)
        {
            this.CurrentNode = CurrentASTNode;
        }
        public ASTNode Add(ASTNode node)
        {
            if (this.CurrentNode.Count != 0) //If no
            {
                this.CurrentNode.First().Children.Add(node);
                node.ParentNode = this.CurrentNode.First();
            }
            return node;
        }

    }
    public class SemanticNode : ASTNode
    {

    }

}
