<?php
//Testing for tree_parser functions
require 'phpmailer/PHPMailerAutoload.php';
require 'HtmlParser.php';
require 'MySQLAdapter.php';
require 'property.php';

ini_set('max_execution_time', 300);

$path='.';
$current_dir = opendir($path);    
//agent_mail::$dictionary[4001] = "rutherfordjohnson@first2move.com";
//agent_mail::$dictionary["HHFTHM"] = "rutherfordjohnson@first2move.com";


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
			$output = parsexml($XMLString);
                        $result = update($output);
                        //echo "have";
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




function parsexml($xml_data){
	//$result = false;
	$dom=new DOMDocument();
	$dom->loadXML($xml_data);
        
        data_formatter::$dictionary = array('House'=>'1','Unit'=>'2','Flat'=>'2','Apartment'=>'2','Studio'=>'3','Townhouse'=>'4','Villa'=>'7','Vacant Land'=>'15');
        data_formatter::$dictionary["current"] = "A";
        data_formatter::$dictionary["sold"] = "D";
        data_formatter::$dictionary["leased"] = "D";
        data_formatter::$dictionary["withdrawn"] = "D";
        data_formatter::$dictionary["offmarket"] = "D";
        
	$interpret = new reverse_rule_tree_parser();
        $interpret->state_property = "nodeName";
        $interpret->parent_property = "parentNode";
        $interpret->children_property = "childNodes";
        
        //Specification for rules
        $interpret->rule_forest["rental"] = array("propertyList"=>'function1');
        $interpret->function_rules['function1'] = create_function('&$node, &$output',''
                . '$output["property"][]= new property() ;'
                . 'foreach($node->attributes as $attr){'
                . 'if($attr->name=="modTime"){'
                . '$output["property"][count($output["property"])-1]->mod_time = $attr->value;}'
                . 'if($attr->name=="status"){ '
                . '$output["property"][count($output["property"])-1]->status = data_formatter::$dictionary[$attr->value];}}'
                . '$property_pricing = new property_pricing();'
                . '$property_pricing->sale_method = "FR";'
                . '$output["property"][count($output["property"])-1]->property_pricings[] = $property_pricing;'
                . '$property_detail = new property_detail();'
                . '$output["property"][count($output["property"])-1]->property_details[] = $property_detail;'
                . '$property_description = new property_description();'
                . '$output["property"][count($output["property"])-1]->property_descriptions[] = $property_description;');
              //  . '$output["property"][count($output["property"])-1]->property_imgs = array();');
        $interpret->rule_forest["residential"] = array("propertyList"=>'function2');
        $interpret->function_rules['function2'] = create_function('&$node,  &$output','$output["property"][]= new property() ; '
                . 'foreach($node->attributes as $attr){'
                . 'if($attr->name=="modTime"){'
                . '$output["property"][count($output["property"])-1]->mod_time = $attr->value;}'
                . 'if($attr->name=="status"){'
                . '$output["property"][count($output["property"])-1]->status = $attr->value;}}'
                . '$property_pricing = new property_pricing();'
                . '$property_pricing->sale_method = "FS";'
                . '$output["property"][count($output["property"])-1]->property_pricings[] = $property_pricing;'
                . '$property_detail = new property_detail();'
                . '$output["property"][count($output["property"])-1]->property_details[] = $property_detail;'
                . '$property_description = new property_description();'
                . '$output["property"][count($output["property"])-1]->property_descriptions[] = $property_description;');
               // . '$output["property"][count($output["property"])-1]->property_imgs = array();');
        $interpret->rule_forest["streetNumber"] = array("address"=>"function3");
        $interpret->function_rules['function3'] = create_function('&$node,  &$output','$output["property"][count($output["property"])-1]->street_no = $node->nodeValue  ; ');
        
        $interpret->rule_forest["street"] = array("address"=>"function4");
        $interpret->function_rules['function4'] = create_function('&$node,  &$output','$output["property"][count($output["property"])-1]->street = $node->nodeValue  ; ');
        
        $interpret->rule_forest["suburb"] = array("address"=>"function5");
        $interpret->function_rules['function5'] = create_function('&$node,  &$output','$output["property"][count($output["property"])-1]->town = $node->nodeValue  ; ');
        
        $interpret->rule_forest["state"] = array("address"=>"function6");
        $interpret->function_rules['function6'] = create_function('&$node,  &$output','$output["property"][count($output["property"])-1]->state = $node->nodeValue  ; ');
        
        $interpret->rule_forest["postcode"] = array("address"=>"function7");
        $interpret->function_rules['function7'] = create_function('&$node,  &$output','$output["property"][count($output["property"])-1]->postcode = $node->nodeValue  ; ');
        //Listing Agent
        $interpret->rule_forest["listingAgent"] = "function8";
        $interpret->function_rules['function8'] = create_function('&$node,  &$output','$listing_agent = new listing_agent();'
                . '$output["property"][count($output["property"])-1]->listing_agents[] = $listing_agent  ; ');
        
        $interpret->rule_forest["name"] = array("listingAgent"=>"function9");
        $interpret->function_rules['function9'] = create_function('&$node,  &$output','$temp_property =&$output["property"][count($output["property"])-1]; '
                . '$names = explode(" ",$node->nodeValue);'
                . '$temp_property->listing_agents[count($temp_property->listing_agents)-1]->first_name = $names[0] ; '
                . '$temp_property->listing_agents[count($temp_property->listing_agents)-1]->last_name = $names[1] ;');
        
        $interpret->rule_forest["telephone"] = array("listingAgent"=>"function10");
        $interpret->function_rules['function10'] = create_function('&$node,  &$output','$temp_property =&$output["property"][count($output["property"])-1]; '
                . '$temp_property->listing_agents[count($temp_property->listing_agents)-1]->telephone = $node->nodeValue ;');
                
        $interpret->rule_forest["email"] = array("listingAgent"=>"function11");
        $interpret->function_rules['function11'] = create_function('&$node,  &$output','$temp_property =&$output["property"][count($output["property"])-1]; '
                . '$temp_property->listing_agents[count($temp_property->listing_agents)-1]->email = $node->nodeValue ;');
                
         //Property Details
        $interpret->rule_forest["category"] = "function12";
        $interpret->function_rules['function12'] = create_function('&$node,  &$output','$temp_property =&$output["property"][count($output["property"])-1];'
                 . 'if(count($node->attributes)>0){ '
                . 'foreach($node->attributes as $attr){'
                . 'if($attr->name=="name"){'
                . 'if(array_key_exists($attr->value,data_formatter::$dictionary)){'
                . '$temp_property->property_details[count($temp_property->property_details)-1]->type = data_formatter::$dictionary[$attr->value];}'
                . 'else{ $temp_property->property_details[count($temp_property->property_details)-1]->type = 0; }'
                . '}}}'
                . ' ');
        
        $interpret->rule_forest["bedrooms"] = array("features"=>"function13");
        $interpret->function_rules['function13'] = create_function('&$node,  &$output','$temp_property =&$output["property"][count($output["property"])-1]; '
                . '$temp_property->property_details[count($temp_property->property_details)-1]->num_bedroom = $node->nodeValue ;');
                
        $interpret->rule_forest["bathrooms"] = array("features"=>"function14");
        $interpret->function_rules['function14'] = create_function('&$node,  &$output','$temp_property =&$output["property"][count($output["property"])-1]; '
                . '$temp_property->property_details[count($temp_property->property_details)-1]->num_bathroom = $node->nodeValue ;');
                
        $interpret->rule_forest["garages"] = array("features"=>"function15");
        $interpret->function_rules['function15'] = create_function('&$node,  &$output','$temp_property =&$output["property"][count($output["property"])-1]; '
                . '$temp_property->property_details[count($temp_property->property_details)-1]->num_parking = $node->nodeValue ;');
                
        $interpret->rule_forest["energyRating"] = "function16";
        $interpret->function_rules['function16'] = create_function('&$node,  &$output','$temp_property =&$output["property"][count($output["property"])-1]; '
                . '$temp_property->property_details[count($temp_property->property_details)-1]->energy_rating = $node->nodeValue;');
        //Property Pricing
        $interpret->rule_forest["price"] = "function17";
        $interpret->function_rules['function17'] = create_function('&$node,  &$output','$temp_property =&$output["property"][count($output["property"])-1]; '
                . '$temp_property->property_pricings[count($temp_property->property_pricings)-1]->sale_price = $node->nodeValue;');
                
        $interpret->rule_forest["rent"] = "function18";
        $interpret->function_rules['function18'] = create_function('&$node,  &$output','$temp_property =&$output["property"][count($output["property"])-1]; '
                . '$temp_property->property_pricings[count($temp_property->property_pricings)-1]->sale_price = $node->nodeValue;');
        //Property Description
        $interpret->rule_forest["headline"] = "function19";
        $interpret->function_rules['function19'] = create_function('&$node,  &$output','$temp_property =&$output["property"][count($output["property"])-1]; '
                . '$temp_property->property_descriptions[count($temp_property->property_descriptions)-1]->headline = $node->nodeValue;');
                
        $interpret->rule_forest["description"] = "function20";
        $interpret->function_rules['function20'] = create_function('&$node,  &$output','$temp_property =&$output["property"][count($output["property"])-1]; '
                . '$temp_property->property_descriptions[count($temp_property->property_descriptions)-1]->description = $node->nodeValue;');
        //Property Images
        $interpret->rule_forest["img"] = array("objects"=>"function21");
        $interpret->function_rules['function21'] = create_function('&$node,  &$output','$property_img = new property_img();'
                . 'foreach($node->attributes as $attr){'
                . 'if($attr->name=="url"){'
                . '$property_img->url = $attr->value;'
                . '$output["property"][count($output["property"])-1]->property_imgs[] = $property_img ;}} ');
        //Inspection Time
        
       // $interpret->rule_forest["inspection"] = array("inspectionTimes"=>"function22");
      //  $interpret->function_rules['function22'] = create_function('&$node,  &$output','$inspection_date = new inspection_date();'
       //         . 'foreach($node->attributes as $attr){'
        //        . 'if($attr->name=="url"){'
        //        . '$property_img->url = $attr->value;'
        //        . '$output["property"][count($output["property"])-1]->inspection_dates[] = $inspection_date ;}} ');
        
        $interpret->rule_forest["agentID"] = "function22";
        $interpret->function_rules['function22'] = create_function('&$node,  &$output','$temp_property =&$output["property"][count($output["property"])-1]; '
                . '$temp_property->agent_id = $node->nodeValue;');
        
        $output = array();
        $interpret->interpret($dom,$output);
        if(array_key_exists("property",$output)){
           // echo "property_num:".count($output["property"])."<br/>";
        }
        
        foreach($output["property"] as $property){
           // echo $property->street_no.' '.$property->street." ".$property->town." ".$property->state." ".$property->postcode." ";
           // echo $property->property_pricings[0]->sale_method.' ';
            //echo $property->listing_agents[0]->first_name.' ';
           // echo $property->listing_agents[0]->last_name.' ';
           // echo $property->listing_agents[0]->telephone.' ';
            //echo $property->listing_agents[0]->email.' num_agents '.count($property->listing_agents)." ";
            //echo $property->property_details[0]->type." "; 
            //echo $property->property_details[0]->num_bedroom." ";
            //echo $property->property_details[0]->num_bathroom." ";
           // echo $property->property_details[0]->num_parking." ";
            //echo $property->property_details[0]->energy_rating." ";
            //echo $property->property_pricings[0]->sale_price." ";
            //echo $property->property_descriptions[0]->headline." ";
            //echo $property->property_descriptions[0]->description." ";
            //echo $property->mod_time." ";
            //echo $property->status." ";
            //echo $property->agent_id." ";
            //echo $property->property_imgs[0]->url." num_imgs:".count($property->property_imgs);
            //echo "<br/>";
            //print_r($property); 
        }
        
        return $output;
}

function update(&$input){
$dbhost ="localhost";// "localhost"; // this will ususally be 'localhost', but can sometimes differ
$dbname ="first_chinesedb";// "first_update_db"; // the name of the database that you are going to use for this project
$dbuser = "root";//"first_admin";// "first_userdata"; //the username that you created, or were given, to access your database
$dbpass ="1234";// "nimda";//"A1,#v-hGEvDK"; // the password that you created, or were given, to access your database

$_config = array($dbhost,$dbuser,$dbpass,$dbname);
$orm_adapter = new ORM_mapper('ORM.json');
$db_adapter = new MySQLAdapter($orm_adapter,$_config);
//print_r($db_adapter);
    if(count($input["property"])>0){
            $property = new property();
            $db_adapter-> db_load($property,array("property_id"=>769));
            $db_adapter->db_del($property,array("property_id"=>$property->property_id));
        foreach($input["property"] as $property){
            $result = -1;
			$result = $db_adapter->db_import($property,array('street_no'=>$property->street_no,'street'=>$property->street,'town'=>$property->town,'state'=>$property->state,'postcode'=>$property->postcode));
            //This method will automatically eliminate repetitive rows.
            
			//$result1 = -1;
			//Reload the db_adapter to operate on Chinese_db........................
			//$dbhost ="localhost";// "localhost"; // this will ususally be 'localhost', but can sometimes differ
			//$dbname ="first_chinesedb";// "first_update_db"; // the name of the database that you are going to use for this project
			//$dbuser = "root";//"first_admin";// "first_userdata"; //the username that you created, or were given, to access your database
			//$dbpass ="1234";// "nimda";//"A1,#v-hGEvDK"; // the password that you created, or were given, to access your database

			//$_config = array($dbhost,$dbuser,$dbpass,$dbname);
			//$orm_adapter = new ORM_mapper('ORM.json');
			//$db_adapter = new MySQLAdapter($orm_adapter,$_config);
			//$result1 = $db_adapter->
			//Reload the db_adapter to operate on Chinese_db........................
			
			
			
			if($result==0){
                //no row create 
			$recipients[] = "hzrtnt018@gmail.com";
 			$recipients[] = agent_mail::$dictionary[$property->agent_id];
                send_email($property->property_id,$recipients);
                $property_info = new property_info();
                $property_info->property = $property;
                $property_info->mod_time = $date = date('Y-m-d H:i:s');
                $property_info->translation_status = false;
                
                FileOperation::WriteJson($property_info,"propertyinfo/PropertyInfo-".$property->property_id.".json");
            }else if($result==1){
                // single row, replace
            }else if($result == 2){
                // multiple rows, no changes 
            }
        }
        
        //$db_adapter->query("truncate table master_properties");$db_adapter->query("truncate table property_description");$db_adapter->query("truncate table property_pricing");$db_adapter->query("truncate table property_contact");$db_adapter->query("truncate table property_detail");$db_adapter->query("truncate table property_img");
       // echo count($db_adapter->bulk_load("property","property_id"));
    }
    //$db_adapter->destruct();
    return true;
}
function ExtractXMLString($path){ 
$myfile = fopen($path, "r") or die("Unable to open file!");
		$xml_data = fread($myfile,filesize($path));
fclose($myfile);
return $xml_data;
}

//Auto Email forwarding functions
//Auto Email forwarding functions
function send_email($listingID,array $recipients){
	
	$url = "first2move.com";
	$port = 25;
	$user = "support@first2move.com";
	$pass = "kelsie17";
	$user_name = "first2move IT support";
	$subject = "Listing ".$listingID." has been uploaded, Chinese translation is required.";
	$content = "Listing ".$listingID." has been uploaded to first2move database, please translate the listing information to Chinese. Thank you.\r\nBest regards\r\nSupport@first2move";
	
	foreach($recipients as $recipient){
		send_mail($subject,$content,$recipient,$url,$port,$user,$pass,$user_name);
	}
}
function send_mail($subject,$content,$SendTo,$host_url,$port,$user,$pass,$user_name){
	$mail             = new PHPMailer();
	$body             = $content;
	$body             = eregi_replace("[\]",'',$body);
	$mail->IsSMTP(); // telling the class to use SMTP
	$mail->SMTPDebug  = 2;                     // enables SMTP debug information (for testing)
                                           // 1 = errors and messages
                                           // 2 = messages only
	$mail->SMTPAuth   = true;                  // enable SMTP authentication
	$mail->Host       = $host_url; // sets the SMTP server
	$mail->Port       = $port;                    // set the SMTP port for the GMAIL server
	$mail->Username   = $user; // SMTP account username
	$mail->Password   = $pass;        // SMTP account password
	$mail->SetFrom($user, $user_name);
	$mail->AddReplyTo($user, $user_name);
	$mail->Subject    = $subject;
	$mail->MsgHTML($body);
	$address = $SendTo;
	$mail->AddAddress($address, $user_name);
	if(!$mail->Send()) {
		echo "Mailer Error: " . $mail->ErrorInfo;
	} else {
		echo "Message sent!";
	}
}


class data_formatter{
    static $dictionary ;
    function __construct(){
        
    }
    static function load($json_file){
        data_formatter::$dictionary = array();
    }
    static function translate($data){
        return data_formatter::$dictionary[$data];
    }
}
class agent_mail{
    static $dictionary;
}
class reverse_rule_tree_parser{
    var $state_property;
    var $parent_property;
    var $children_property;
    var $rule_forest;
    var $function_rules;
    //var $tasks;
    
    function __construct(){
       // $tasks = array();
    }
    function initialize($node_name,$parent_node_property,$child_node_property,$rule_forest,$function_rules){
        $this-> state_property = $node_name;
        $this-> parent_node  = $parent_node_property;
        $this-> child_node_property = $child_node_property;
        $this-> rule_forest = $rule_forest;
        $this-> function_rules = $function_rules;
    }
    function interpret($node,array &$output){
        //Interpret Process
        //echo $node->nodeName.'<br/>';
        $ancestry = array();
        $this->trace_ancestry($node,$ancestry);
        if(count($ancestry!=0)){
            $i=0; 
            $category = null;
            $this->loopcheck($ancestry,$this->rule_forest,$i,$category);
            if($category!=null){
               // echo $category;
            $this->perform_function($category,$node,$output);
            }
        }
        
        //Children Loop
        if(count($node->{$this->children_property})!=null){
            foreach($node->{$this->children_property} as $child){
                $this->interpret($child,$output);
            }
        }
    }
    private function loopcheck(array &$ancestors,array &$rules, $i, &$result){
        if(count($ancestors)>0){
            if(array_key_exists($ancestors[$i]->{$this->state_property} , $rules)){
                $result = $rules[$ancestors[$i]->{$this->state_property}];
                $i++;
                //print_r($result);
                if(is_array($result)){
                    $this->loopcheck($ancestors,$result, $i,$result);
                }else{
                    
                }
            }
        }
    }
    private function trace_ancestry(&$node, array &$ancestry){
           $ancestry[] =  $node; // First Add itself;
        if($node-> {$this->parent_property} !==null){
           $parent =  $node-> {$this->parent_property} ;
           $this->trace_ancestry($parent,$ancestry);
        }
    }
    private function perform_function($parse_result,&$node,&$output){
       $this->function_rules[$parse_result]($node,$output);
    }
}
?>
