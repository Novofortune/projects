<?php
require 'phpmailer/PHPMailerAutoload.php';
require 'HtmlParser.php';
require 'MySQLAdapter.php';
require 'property.php';

ini_set('max_execution_time', 300);

$path='.';
$current_dir = opendir($path);    
while(($file = readdir($current_dir)) !== false) {    //Get one item in current_dir 
	$sub_dir = $path . DIRECTORY_SEPARATOR . $file;    //Get the path of the item
	if($file == '.' || $file == '..') { //Determine if it is empty
		continue;
	} else if(is_dir($sub_dir)) {    //Determine if it is a folder
		$file1 = opendir($sub_dir);
		while(($value = readdir($file1)) !== false) { 
			$dot_pos = strrchr($value,".");
			$ext = substr($dot_pos,1,strlen($dot_pos));
			if($ext=='xml'){ // for each xml file, extract their information into separate arrays and put them all in the all_data array
			$XMLString = ExtractXMLString($sub_dir."/".$value);
			//echo $XMLString;
			$result = update($XMLString);
				if($result)
				{ //Return the result of update operation; re-organise files if success
					if(file_exists($sub_dir."/temp/")){
						$myfile = fopen($sub_dir."/temp/".$value, "w") or die("Unable to open file!");
						fwrite($myfile,$XMLString);
						fclose($myfile);
						//unlink (  $sub_dir."/".$value );
					}else{
						mkdir($sub_dir."/temp/",0777,true);
						$myfile = fopen($sub_dir."/temp/".$value, "w") or die("Unable to open file!");
						fwrite($myfile,$XMLString);
						fclose($myfile);
						//unlink (  $sub_dir."/".$value );
					}	
				}
			}
		}
	}
}




function update($xml_data){
	$result = false;
	$dom=new DOMDocument();
	$dom->loadXML($xml_data);
        //print_r($dom->documentElement);
	$interpret = new interpretor();
        $interpret->node_state_property_name = "nodeName";
        $interpret->children_property_name = "childrenNodes";
        $interpret->rule_forest["#document"] = array();
        loopread($dom->documentElement);
        //$interpret-> rule_forest["#document"][""]= 
	return $result;
}

function ExtractXMLString($path){
$myfile = fopen($path, "r") or die("Unable to open file!");
		$xml_data = fread($myfile,filesize($path));
fclose($myfile);
return $xml_data;
}

function loopread($node){
    echo $node->nodeName."<br/>";
    if($node->childNodes!=null){
    foreach($node->childNodes as $child){
        loopread($child);
    }
    }
}

class interpretor{
	public  $tasks;
	public  $rule_forest;
	public $node_state_property_name;
	public $children_property_name;
	
	public function perform($Parse_Tree_Node,$depth=0){
		$current_state = $Parse_Tree_Node->{$this->node_state_property_name};
		foreach($this->tasks as $task){ // Checking Existing Tasks
			$task_states=$task->get_states();
			if(array_key_exists($current_state,$task->next_possible_states)){
				$task->add_state( $current_state);
				$task->update_rule($task_states ,$this->rule_forest);
                                $task-> perform_rule();
			}
		}
		
		if(array_key_exists($current_state,$this->rule_forest)){ // Check if the current node is the Starting State in the rule
					$task1 = new task(); // Start a Task with the Starting State
					$task1->add_state( $current_state);
					$task1->update_rule($task1->get_states() ,$this->rule_forest);
					$this->tasks[] = $task1;
		}
			
			
		if(count($Parse_Tree_Root->{$children_property_name})>0){ //Check if it is a Terminal
			foreach($Parse_Tree_Root->{$children_property_name} as $child){ // Transverse Children
                                $depth++;
				$this->perform($child,$depth);
			}
		}else{ // Get the Terminal Node 
			
		}
	}
}

class task{
	public $states;
        public $depth;
	public $next_possible_states;
	
	public function __construct(){
		$this->states = array();
	}
	
	public function get_states(){
		return $this->states;
	}
	public function add_state($state,$depth){
            if($this->depth+1 == $depth){ // Chekck if the depth is correct
            	$this->states[] = $state;
                $this->depth = $depth;
            }
	}
	public function update_rule(array $states,$rule_forest){
		
		$next_states = array();
		if(count($states)>0){
			if(array_key_exists($states[0],$rule_forest)){
					$next_states = $rule_forest[$states[0]];
				
                                
                            foreach($states as $state){
				if($states[0]!==$state){ // Exclude the first element
					if(array_key_exists($state,$next_states)){
						$next_states = $rule_forest[$state];
					}
				}
                            }
			}
		}
                if(count($next_states)==0){
                    
                }
		$this->next_possible_states = $next_states;
	}
	
	public function perform_rule(){
		if
	}
	public function clear(){
		$this->states = array();
	}
}

?>