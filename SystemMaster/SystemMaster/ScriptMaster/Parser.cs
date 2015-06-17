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

            ASTNode block = new ASTNode("block");
            this.CurrentNode.Push(block); // push block

            ASTNode sentence = new ASTNode("sentence");
            this.CurrentNode.Push(sentence); // push sentence

            this.CurrentState.Push("none");
        }
        private ASTNode ParseEnd()
        {
            this.curpos = 0;
            this.CurrentState.Pop();
            //try
            {
                this.ASTNodeBuilder.Add(this.CurrentNode.Pop());
                return this.ASTNodeBuilder.Add(this.CurrentNode.Pop());
            }
            //catch { return null; }
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
                       // try
                        {
                            ASTNode block = new ASTNode("block");
                            //this.CurrentNode.First().Children.Add(block);
                            this.CurrentNode.Push(block); // push block

                            ASTNode sentence = new ASTNode("sentence");
                            //this.CurrentNode.First().Children.Add(sentence);
                            this.CurrentNode.Push(sentence); // push sentence
                        }
                       // catch { }
                        break;
                    }
                case "popblock1":
                    {
                       // try
                        {
                            this.ASTNodeBuilder.Add(this.CurrentNode.Pop());// pop sentence
                            this.ASTNodeBuilder.Add(this.CurrentNode.Pop());// pop block
                        }
                       // catch { }
                        break;
                    }
                #endregion
                #region block2
                case "pushblock2":
                    {
                       // try
                        {
                            ASTNode block = new ASTNode("block");
                            //this.CurrentNode.First().Children.Add(block);
                            this.CurrentNode.Push(block); // push block

                            ASTNode sentence = new ASTNode("sentence");
                            //this.CurrentNode.First().Children.Add(sentence);
                            this.CurrentNode.Push(sentence); // push sentence
                        }
                       // catch { }
                        break;
                    }
                case "popblock2":
                    {
                       // try
                        {
                            this.ASTNodeBuilder.Add(this.CurrentNode.Pop());// pop sentence
                            this.ASTNodeBuilder.Add(this.CurrentNode.Pop());// pop block
                        }
                      //  catch { }
                        break;
                    }
                #endregion
                #region block3
                case "pushblock3":
                    {
                       // try
                        {
                            ASTNode block = new ASTNode("block");
                            //this.CurrentNode.First().Children.Add(block);
                            this.CurrentNode.Push(block); // push block

                            ASTNode sentence = new ASTNode("sentence");
                            //this.CurrentNode.First().Children.Add(sentence);
                            this.CurrentNode.Push(sentence); // push sentence
                        }
                       // catch { }
                        break;
                    }
                case "popblock3":
                    {
                        //try
                        {
                            this.ASTNodeBuilder.Add(this.CurrentNode.Pop());// pop sentence
                            this.ASTNodeBuilder.Add(this.CurrentNode.Pop());// pop block
                        }
                       // catch { }
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

    public class SentenceParser
    {
        public List<ASTNode> data { get; set; }
        public GrammarNode rules { get; set; }
        public int curpos { get; set; }
        public ASTNodeBuilder ASTNodeBuilder { get; set; }
        public Stack<ASTNode> CurrentNode { get; set; }
        public Stack<string> CurrentState { get; set; }
        public SentenceParser(GrammarNode rules, List<ASTNode> data)
        {
            reset(rules, data);
        }
        public void reset()
        {
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

            ASTNode block = new ASTNode("block");
            this.CurrentNode.Push(block); // push block

            //ASTNode sentence = new ASTNode("sentence");
            //this.CurrentNode.Push(sentence); // push sentence

            this.CurrentState.Push("none");
        }
        private ASTNode ParseEnd()
        {
            this.curpos = 0;
            this.CurrentState.Pop();
            //try
            //{
                //this.ASTNodeBuilder.Add(this.CurrentNode.Pop());
                return this.ASTNodeBuilder.Add(this.CurrentNode.Pop());
            //}
            //catch { return null; }
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
                case "pushsentence":
                    {
                        break;
                    }
                case "popsentence":
                    {
                        break;
                    }
                #region split
                case "split":
                    {
                        
                            this.ASTNodeBuilder.Add(this.CurrentNode.Pop());// pop sentence
                            ASTNode sentence = new ASTNode("sentence");
                            this.CurrentNode.Push(sentence);//push sentence
                        
                        break;
                    }
                #endregion
                #region normal
                case "normal":
                    {
                        this.CurrentNode.First().Children.Add(this.data[this.curpos]); // Add the normal node to the current structural node children

                        break;
                    }
                #endregion
                default:
                    {
                        break;
                    }
            }
        }

        public static List<ASTNode> ExpandBlock(ASTNode block) 
        {
            List<ASTNode> nodes = new List<ASTNode>();
            if (block.Children != null) {
                foreach (ASTNode node in block.Children)
                {
                    nodes.Add(node);
                }
            }
            return nodes;
        }
    }

    public class GrammarNode
    {
        public string type { get; set; }
        public Dictionary<string, string> instructionrules { get; set; }
        public Dictionary<string, string> staterules { get; set; }
        public GrammarNode parentNode { get; set; }
        public Dictionary<string, GrammarNode> childNodes { get; set; }
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
        public static GrammarNode CreateGrammarNodeSequence(string state,string instruction, params string[] GrammarNodeTypeSequence)
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
        public static void AddGrammarNodeSequence( GrammarNode gn,int i= 0, params GrammarNode[] GrammarNodeSequence)
        {
                if (gn.childNodes.ContainsKey(GrammarNodeSequence[i].type))
                {
                    i++;
                    while (i < GrammarNodeSequence.Length)
                    {
                        AddGrammarNodeSequence(gn.childNodes[GrammarNodeSequence[i].type], i, GrammarNodeSequence);
                    }
                }
                else
                {
                    gn.childNodes.Add(GrammarNodeSequence[i].type, GrammarNodeSequence[i]);
                    gn.childNodes[GrammarNodeSequence[i].type].parentNode = gn;
                    gn.childNodes[GrammarNodeSequence[i].type].childNodes = new Dictionary<string, GrammarNode>(); 
                    i++;
                    while (i < GrammarNodeSequence.Length)
                    {
                        
                        AddGrammarNodeSequence(gn.childNodes[GrammarNodeSequence[i].type], i, GrammarNodeSequence);
                    }
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
        public int Depth { get; set; }
        public string type { get; set; }
        public ASTNode()
        {
            Children = new List<ASTNode>();
        }

        public ASTNode(string type)
        {
            this.type = type;
            Children = new List<ASTNode>();
        }
        public static void LoopRead(ASTNode node, ref List<ASTNode> output)
        {
            output.Add(node);
            foreach (ASTNode child in node.Children)
            {
                LoopRead(child, ref output);
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
