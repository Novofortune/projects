<?php
$hm = new HtmlParser('example.html');
//print_r($hm->root);
foreach($hm->nodes as $node){
	echo $node->type.$node->depth."-->";
	foreach($node->children as $child){
		echo $child->type.$child->depth." ";
	}
	echo "<br/>";
}

if( $hm->root->parent == null){
	echo "YEAH";
}
 

foreach($hm->pairs as $pair){
	print_r( $pair->start_tag->start."-". $pair->start_tag->end."***".$pair->end_tag->start."-".$pair->end_tag->end."=".$pair->start_tag->type." <<< ".$pair->parent_pair->start_tag->start."-".$pair->parent_pair->start_tag->end."***".$pair->parent_pair->end_tag->start."-".$pair->parent_pair->end_tag->end."=".$pair->parent_pair->start_tag->type."<br/>");
}
echo '<hr/><h3>function write()<h3/>';
//echo "<textarea style = 'height:800;width:600px'>".$hm->write(0,1,"inserted")."</textarea>";


$newdata = $hm->create_html_node($hm->getElementById('td'),null,'MyTag',false,array('id'=>'MyTag'),$innerhtml="MyContent");
//print_r($hm->getElementById('MyTag')->type);
$newdata = $hm->edit_html_node($hm->getElementById('MyTag'),'MyTag',false, array('id'=>'newid'), 'new content');
$newdata = $hm->delete_html_node($hm->getElementById('newid'));
foreach($hm->nodes as $node){
	if($node->parent!=null){
		print_r( $node->start."-". $node->inner_start."***".$node->inner_end."-".$node->end."=".$node->type." <<< ".$node->parent->start."-".$node->parent->inner_start."***".$node->parent->inner_end."-".$node->parent->end."=".$node->parent->type."<br/>");
	}
	else{
		print_r( $node->start."-". $node->inner_start."***".$node->inner_end."-".$node->end."=".$node->type." <<< <br/>");
	}
}

echo "<textarea style = 'height:800;width:600px'>".$newdata."</textarea>";


class HtmlParser{
	public $data;
	public $tags = array();
	public $pairs = array();
	public $root;
	public $nodes = array();
	
	public function __construct($html_file){
		$html_data = $this->ExtractFileString($html_file);
		$this->data = $html_data;
		$this->root = new html_node();
		//Basic Procedures to Analyse the Html File...
		$this->find_tags('<','>'); 
		$this->pair_tags();
		$this->find_parents();
		$this->construct_html_nodes();
		//Once call this function, all information are processed and ready for retrieve...
	}
	function refresh(){
		$this->find_tags('<','>'); 
		$this->pair_tags();
		$this->find_parents();
		$this->construct_html_nodes();
	}
	function find_all($needle){
		$poss = array();
		$pos = 0;
		while(true){
			$pos = strpos($this->data,$needle,$pos);
			if($pos===false){
				break;
			}else{
				$poss[]= $pos;
				$pos = $pos+1;
			}
		}
		return $poss;
	}
	public function find_tags($tag_start,$tag_end){
		$starts = $this->find_all($tag_start);
		$ends = array();
		for($i=0;$i<count($starts);$i++){
			$end = strpos($this->data,$tag_end,$starts[$i]);
			$ends[] = $end;
			if($i==count($starts)-1){ // determine if the search reaches the last element, if so there is no need to check exceeding, otherwise there is still a need to check exceeding 
			}else{
				if($end>=$starts[$i+1]){ //determine if the end exceed the next start, if so return an exception
					return false;
				}
			}
		}
		if(count($starts)==count($ends)){ // determine if the counts of starts and ends are consistent, if not return false;
			$tags = array();
			for($i=0;$i<count($starts);$i++){
				$tag = new tag();
				$tag->start = $starts[$i];
				$tag->end = $ends[$i];
				$tag->content = substr($this->data,$starts[$i],$ends[$i]-$starts[$i]+1);
				$tag->type = $tag->get_type();
				$tag->property_string = $tag->get_property_string();
				$tags[] = $tag;
			}
			$this->tags = $tags;
			return $tags;
		}else{
			return false;
		}
	}
	function pair_tags(){
		$tags = &$this->tags;
		$pairs = array();
		
		// Define a virtual Root pair
		$root_pair = new pair(); 
		$root_pair->start_tag = new tag();
		$root_pair->end_tag = new tag();
		$root_pair->start_tag->start = -2;
		$root_pair->start_tag->end = -1;
		$root_pair->end_tag->start = strlen($this->data)+1;
		$root_pair->end_tag->end = strlen($this->data)+2;
		$root_pair->start_tag->type = "root";
		$root_pair->end_tag->type = "/root";
		$root_pair->parent_pair = &$root_pair;
		$pairs[] = $root_pair;
		
		for($i=count($tags)-1;$i>=0;$i--){ // Reversely Transverse the tags
			$type = $tags[$i]->type;
			$pair_type = null;
			$pair = new pair();
			preg_match("/\/(.*)/",$type,$match);
			if(count($match)>1){
				$pair_type = $match[1];
			}
			if($pair_type!=null){
				for($j=$i;$j>=0;$j--){ // Reversely search for the pairing tag
					if($tags[$j]->type==$pair_type){
						$pair->end_tag = &$tags[$i];
						$pair->start_tag = &$tags[$j];
						$pairs[] = $pair;
						break;
					}
				}
			}else{
				//echo "detecting single tag... ";
				preg_match("/<(.*)\/(.*)>/",$tags[$i]->content,$match);
				if(count($match)>2){ // determine if the tag is single tag
					//echo "single tag found! ";
					$pair->end_tag = &$tags[$i];
					$pair->start_tag = &$tags[$i];
					$pairs[] = $pair;
				}
				//echo "No Pair Found! ";
			}
		}
		$this->pairs = $pairs;
		return $pairs;
	}
	public function find_parents(){
		$pairs = &$this->pairs;
		for($i=0;$i<count($pairs)-1;$i++){
			if($pairs[$i+1]->start_tag->start > $pairs[$i]->start_tag->end){ // determine if the previous pair is parent
				$pairs[$i+1]->parent_pair = &$pairs[$i];
			}else{
				for($j=$i;$j>=0;$j--){ // Reversely transverse back to find which previous pair is the current one's parent
					if($pairs[$i+1]->start_tag->start > $pairs[$j]->start_tag->end){
						$pairs[$i+1]->parent_pair = &$pairs[$j];
						break;
					}
				}
			}
		}
	}
	public function construct_html_nodes(){
		$pairs = &$this->pairs;
		$tags = &$this->tags;
		$data = &$this->data;
		$root = &$this->root;
		$nodes = &$this->nodes;
		
		for($i=count($pairs)-1;$i>=0;$i--){
			$node = new html_node();
			$node->type = $pairs[$i]->start_tag->type; 
			$node->start = $pairs[$i]->start_tag->start;
			$node->inner_start = $pairs[$i]->start_tag->end;
			$node->inner_end = $pairs[$i]->end_tag->start;
			$node->end = $pairs[$i]->end_tag->end;
			$node->property = $this->get_property($pairs[$i]->start_tag->property_string); // ending tag does not have any property
			$node->innerhtml = substr($data,$node->inner_start+1,$node->inner_end-$node->inner_start-1);
			if($node->inner_start == $node->end)
			{
				$node->if_single = true;
			}else{
				$node->if_single = false;
			}
			$pairs[$i]->html_node = $node;
			$nodes[] = $node;
		}
		for($i=count($pairs)-1;$i>=0;$i--){ // Linking parent and children
			$pairs[$i]->html_node->parent = &$pairs[$i]->parent_pair->html_node;
			$children = &$pairs[$i]->parent_pair->html_node->children;
			$children[] = &$pairs[$i]->html_node;
		}
		for($i=0;$i<count($nodes);$i++){ // Finding the Root
			if($nodes[$i]->type == "root"){
				$root = $nodes[$i];
			}
		}
		unset($root->children[count($root->children)-1]); // Remove Root Recursive...
		$root->parent = null; // Remove Root Recursive...
		$this->assign_depth($root,0); // Assignin the depth of each html_node
		$nodes = array();
		$this->sort_order($root,$nodes);
		$this->nodes = $nodes;
	}
	private function assign_depth($node,$depth){
		$node->depth = $depth;
		if(count($node->children)>0){
			foreach($node->children as $child){
				if($child->type!='root'){
					$this->assign_depth($child,$depth+1);
				}
			}
		}
	}
	function sort_order(&$node,array &$nodes){	
		$nodes[] = $node;
		if(count($node->children)>0){
			foreach($node->children as $child){
				if($child->type!='root'){
					$this->sort_order($child,$nodes);
				}
			}
		}
	}
	
	private function get_property($property_string){
		$property = array();
		$property_string = str_replace(" ","",$property_string); // Make all character lower case AND leaves no space....Need to be adjusted...
		preg_match_all("/(.*?)=[\"\'](.*?)[\"\']/",$property_string,$match);
		for($i=0;$i<count($match[0]);$i++){
			$property[$match[1][$i]] = $match[2][$i];
		}
		return $property;
	}
	public function ExtractFileString($path){
		$fh = fopen($path, "r") or die("Unable to open file!");
		$data = fread($fh,filesize($path));
		fclose($fh);
		return $data;
	}
	public function WriteToFile($str,$path){
		$fh = fopen($path, "w") or die("Unable to write to file");
		fwrite($fh,$str);
		fclose($fh);
	}	
	
	function getElementByProperty($property_name,$property_value){ // This is more general way to get elements
		foreach($this->nodes as $node){
			if(array_key_exists($property_name,$node->property)){
				if($node->property[$property_name]==$id){
					return $node; // Returning the REFERENCE of the node 
				}
			}
		}
	}
	function getElementById($id){ // This is one of the traditional way to get elements
		foreach($this->nodes as $node){
			if(array_key_exists('id',$node->property)){
				if($node->property['id']==$id){
					return $node; // Returning the REFERENCE of the node 
				}
			}
		}
	}
	function write($start,$end,$string){
		return substr($this->data,0,$start).$string.substr($this->data,$end);
	}
	function create_html_node(&$parent,$append_index,$type,$if_single,array $property=null,$innerhtml=""){ // if append_index is null then append in the end, otherwise append before the element
		$property_string = "";
		if($property!=null){
			//$property_string = $property_string." "; // Make a space first
			foreach($property as $key=>$value){
				$property_string = $property_string." ".$key."=\"".$value."\"";
			}
		}
		$string = '<'.$type.(($property_string)?$property_string:"").($if_single?'/>':'>'.$innerhtml.'</'.$type.'>');
		
		$children_count = count($parent->children);
		if($children_count>0){
			//echo $children_count;
			if($append_index!==null){
				if(array_key_exists($append_index,$parent->children)){
				echo 'correct';
				echo "childnum".count($parent->children);
				$this->data = $this->write($parent->children[$append_index]->start,$parent->children[$append_index]->start,$string);
				$this->refresh();
				}else{ // invalid index, No changes are made 
					echo 'invalid index, No changes are made ';
					return $this->data;
				}
			}else{ // If there is no append index is indicated, then append in the end of all the other elements
				$this->data = $this->write($parent->inner_end,$parent->inner_end,$string);
				$this->refresh();
			}
		}else{ //if there is no other children in the parent node, then just insert without thinking
			$this->data = $this->write($parent->inner_end,$parent->inner_end,$string);
			$this->refresh();
		}
		return $this->data;
	}
	function edit_html_node(&$node,$type = null,$if_single = null, array $property=null,$innerhtml = null){
		$property_string = "";
		if($property!=null){
			//$property_string = $property_string." "; // Make a space first
			foreach($property as $key=>$value){
				$property_string = $property_string." ".$key."=\"".$value."\"";
			}
		}else{
			foreach($node->property as $key=>$value){ // Use previous property to construct property_string
				$property_string = $property_string." ".$key."=\"".$value."\"";
			}
		}
		$newtype;
		if($type!==null){ // If there is any change of $type 
			$newtype = $type;
		}else{
			$newtype=$node->type;
		}
		if($if_single===null){
			$if_single = $node->if_single;
		}
		if($innerhtml===null){
			$innerhtml = $node->innerhtml;
		}
		
		$string = '<'.$newtype.(($property_string)?$property_string:"").($if_single?'/>':'>'.$innerhtml.'</'.$newtype.'>');
		
		$this->data = $this->write($node->start,$node->end+1,$string); // The end will need to be adjusted..
		$this->refresh();
		
		//echo $node->innerhtml;
		return $this->data;
	}
	function delete_html_node(&$node){
		
		$this->data = $this->write($node->start,$node->end+1,''); // The end will need to be adjusted..
		$this->refresh();
		
		return $this->data;
	}
}


class tag{
	public $start;
	public $end;
	public $content;
	public $type;
	public $property_string;
	
	public function __construct(){
		$start = 0;
		$end = 0;
		$content = "";
		$type = "";
		$property_string = "";
	}
	
	public function get_type(){
		$tag_content = $this->content;
		$match = array();
		preg_match("/<(.*?)[ >]/",$tag_content,$match);
		if(count($match)>1){ // determine if any tag type is found
			if($match[1]=='/'||$match[1]=='!'){ // determine if the tag type is separated by space, this case is rare, but possible in case of non-standard html grammar
				preg_match("/<(.*?)\s+(.*?)[ >]/",$tag_content,$match);
				if(count($match)>2){
					return $match[1].$match[2];
				}
			}
			preg_match("/\w+\//",$match[1],$match1);
			if(count($match1)>0){
				preg_match("/(.*)\//",$match[1],$match1);
				if(count($match1)>1){
					$match[1] = $match1[1];
				}
			}
			return $match[1];
		}else{
			return 'No Type Found! ';
		}
	}
	public function get_property_string(){
		$tag_content = $this->content;
		//echo $this->type;
		$type_string = str_replace("/","",str_replace("!","",$this->type)); // get rid of ! and /
		//echo $type_string;
		preg_match("/".$type_string."(.*?)[\/>]/",$tag_content,$match);
		if(count($match)>1){ // determine if any tag with property_string is found
			//echo "|| ";
			//echo $match[1];
			return $match[1];
		}else{
			return '';
		}
	}
}
class pair{
	public $start_tag;
	public $end_tag;
	public $parent_pair;
	public $html_node;
	
	public function __construct(){
		$start_tag = new tag();
		$end_tag = new tag();
		$parent_pair = null;
	}
}
class html_node{
	public $parent;
	public $children;
	
	public $type;
	public $if_single;
	public $property;
	public $innerhtml;
	
	public $start;
	public $end;
	public $inner_start;
	public $inner_end;
	
	public $depth;
	
	public function __construct(){
		 $this->children = array();
	}
	//The Following Functions are still waiting to be implemented in the future....
	function getKin($node_li,$min_depth,$max_depth){ // Positive: children; negative: parents;
		
	}
	function getChildren($node_li,$child_list,$min_depth,$max_depth){
		for($i=0;$i<count($this->children);$i++){
			$depth = $this->children[$i]->depth;
			if($depth>=$min_depth&&$depth<=$max_depth){ // Determine if the children is within the range
				$child_list[] = $this->children[$i];
				getChildren($this->children[$i]);
			}else if($depth<$min_depth){ //Determine if the children is still not within the range
				getChildren($this->children[$i]);
			}else{ //Determine if the children are exceeding the range
				
			}
		}
	}
	function getParents($node_li,$min_depth,$max_depth){
		
	}
	
	
}
?>