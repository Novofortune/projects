<?php 
require 'phpmailer/PHPMailerAutoload.php';
ini_set('max_execution_time', 300);
//echo dirname(__FILE__);
//$dir = '/home2/first/public_html/Boxdice_XML_Feeds';
//$dir='/home2/first/public_html/AgentBox_XML_Feeds';


$path='.';
$current_dir = opendir($path);    
while(($file = readdir($current_dir)) !== false) {    //Get one item in current_dir 
	$sub_dir = $path . DIRECTORY_SEPARATOR . $file;    //Get the path of the item
	if($file == '.' || $file == '..') { //Determine if it is empty
		continue;
	} else if(is_dir($sub_dir)) {    //Determine if it is a folder
		update($path.'/'.$file);
	} else { // Determine if it is a file
    }
}
//Update functions
function update($dir){

$files = get_files($dir);
$link = db_connect();
//$all_data = array();

if($files!=null){ 
	foreach($files as $value){
		$dot_pos = strrchr($value,".");
		$ext = substr($dot_pos,1,strlen($dot_pos));
	
		if($ext=='xml'){ // for each xml file, extract their information into separate arrays and put them all in the all_data array
	
			echo "newfile!!!!";
			$myfile = fopen($dir."/".$value, "r") or die("Unable to open file!");
			$xml_string = ExtractXMLString($dir."/".$value);
			fclose($myfile);
			$all_data = array(); //each file has one 'all_data'
			ExtractArray($xml_string,$all_data); 
			foreach($all_data as $tempArr){
				//echo $tempArr[]
				//tempArr is one entry of property / one xml file
				//$all_data[$value] = $tempArr; // put all xml file info into a single array
				//mysql_query("SET NAMES 'UTF8';");
				$prop_id = db_update_address($tempArr);
				db_update_agent($tempArr,$prop_id);
				db_update_description($tempArr,$prop_id);
				db_update_property_details($tempArr,$prop_id);
				db_update_price($tempArr,$prop_id);
				db_update_inspection($tempArr,$prop_id);
				db_update_img($tempArr,$prop_id);
			}
			//When operations completed, move the file to temp folder
			if(file_exists($dir."/temp/")){
				$myfile = fopen($dir."/temp/".$value, "w") or die("Unable to open file!");
				fwrite($myfile,$xml_string);
				fclose($myfile);
				//unlink (  $dir."/".$value );
			}else{
				mkdir($dir."/temp/",0777,true);
				$myfile = fopen($dir."/temp/".$value, "w") or die("Unable to open file!");
				fwrite($myfile,$xml_string);
				fclose($myfile);
				//unlink (  $dir."/".$value );
			}
		}
	}
}
db_close($link);
//return $all_data;
}
function db_update_address($tempArr){
$entry = array();
		foreach($tempArr as $key=>$value){
			if(strpos(strtolower($key),"address")!==false){
				foreach($value as $key1=>$value1){
					$value1 = str_replace("'","\'",$value1);
				if(strpos(strtolower($key1),"streetnumber")!==false){
					$entry["street_no"]=$value1;
					}	
				if(strpos(strtolower($key1),"street")!==false){
					$entry["street_name"]=$value1;
					}	
				if(strpos(strtolower($key1),"suburb")!==false){
					$entry["town"]=$value1;
					}	
				if(strpos(strtolower($key1),"state")!==false){
					$entry["state"]=$value1;
					}	
				if(strpos(strtolower($key1),"postcode")!==false){
					$entry["postcode"]=$value1;
					}	
				}
			}
			if(strpos(strtolower($key),"root")!==false){
				foreach($value as $key1=>$value1){
					$value1 = str_replace("'","\'",$value1);
				if(strpos(strtolower($key1),"agentid")!==false){
					$entry["agent"]=$value1;
					}	
				}
			}					
		}
		$res = mysql_query("SELECT * FROM master_properties WHERE street_no = '".$entry["street_no"]."' AND street_name = '".$entry["street_name"]."' AND town='".$entry["town"]."' AND state = '".$entry["state"]."' AND postcode='".$entry["postcode"]."'"); 
		if(mysql_num_rows($res)){
			$resArr=mysql_fetch_array($res);
			
			//update lattitude and longitude
			$response_a = get_lat_lng($entry["street_no"],$entry["street_name"],$entry["town"],$entry["state"]);
			if($response_a!=null){
			$lat = $response_a->results[0]->geometry->location->lat;
			$lng = $response_a->results[0]->geometry->location->lng;	
						
			//echo $lat;echo $lng;
			//echo $resArr["id"];
				if($lat!=''&&$lng!='')
			mysql_query("UPDATE master_properties SET lat=".$lat.",lng=".$lng.",complete=255 WHERE id=".$resArr["id"]) or die(mysql_error());
			}
			return $resArr["id"];
		}else{
			mysql_query("INSERT INTO master_properties ( street_no,street_name,town,state,postcode)	VALUES ('".$entry["street_no"]."','".$entry["street_name"]."','".$entry["town"]."','".$entry["state"]."','".$entry["postcode"]."')") or die(mysql_error());
			
			$res = mysql_query("SELECT * FROM master_properties WHERE street_no = '".$entry["street_no"]."' AND street_name = '".$entry["street_name"]."' AND town='".$entry["town"]."' AND state = '".$entry["state"]."' AND postcode='".$entry["postcode"]."'"); 
			if(mysql_num_rows($res)){
			$resArr=mysql_fetch_array($res);
			
			//update lattitude and longitude
			$response_a = get_lat_lng($entry["street_no"],$entry["street_name"],$entry["town"],$entry["state"]);
			if($response_a!=null){
			$lat = $response_a->results[0]->geometry->location->lat;
			$lng = $response_a->results[0]->geometry->location->lng;			
			mysql_query("UPDATE master_properties SET lat=".$lat." , lng=".$lng.",complete=255 WHERE id=".$resArr["id"]) or die(mysql_error());
			}
			//Sending Emails
			$recipients[] = "hzrtnt018@gmail.com";
			
 			$AgentID_Mail[4001] = 'rutherfordjohnson@first2move.com';

			echo '<h2>'.$AgentID_Mail[$entry["agent"]].'</h2>';
			$recipients[] = $AgentID_Mail[$entry["agent"]];
			send_email($resArr["id"],$recipients);
			return $resArr["id"];
			}else{}
		}
}
function db_update_img($tempArr,$prop_id){
$entries = array();		
					
		foreach($tempArr as $key=>$value){
			if(strpos(strtolower($key),"img")!==false){
				foreach($value as $key1=>$value1){
				if(strpos(strtolower($key1),"url")!==false){
					$entry = array();
					$entry["img_url"]=$value1;
					$entry["prop_id"] = $prop_id;
					$entries[]=$entry;
					}	
				}
			}					
		}
		foreach($entries as $entry){
		$res = mysql_query("SELECT * FROM property_img WHERE prop_img = '".$entry["img_url"]."'"); 
		if(mysql_num_rows($res)){}else{
			mysql_query("INSERT INTO property_img ( prop_id,prop_img )	VALUES ('".$entry["prop_id"]."','".$entry["img_url"]."')") or die(mysql_error());
		}
		}
}
function db_update_inspection($tempArr,$prop_id){
$entry = array();
		foreach($tempArr as $key=>$value){
					$entry["prop_id"]=$prop_id;
			
			if(strpos(strtolower($key),"inspection")!==false){
				foreach($value as $key1=>$value1){
				if(strpos(strtolower($key1),"inspection")!==false){
					$temp = strtok($value1," ");
					$entry["inspection_date"]=$temp;
					}	
				}
			}					
		}
		$res = mysql_query("SELECT * FROM property_inspection WHERE prop_id = '".$entry["prop_id"]."'"); 
		if(mysql_num_rows($res)){}else{
		foreach($entry as $key=>$value){ /// this is for preprocessing of the data
			}
			mysql_query("INSERT INTO property_inspection ( prop_id,inspection_date1 )	VALUES ('".$entry["prop_id"]."','".$entry["inspection_date"]."')") or die(mysql_error());
		}
}
function db_update_price($tempArr,$prop_id){
$entry = array();
$entry["prop_id"]=$prop_id;
		foreach($tempArr as $key=>$value){
			
			if(strpos(strtolower($key),"residential")!==false){
				foreach($value as $key1=>$value1){
				if(strpos(strtolower($key1),"uniqueid")!==false){
					//$entry["prop_id"]=$value1;
					}
				if(strtolower($key1)=="price"){
					$entry["sprice1"]=$value1;
					}
				}
			}
			if(strpos(strtolower($key),"priceview")!==false){
				foreach($value as $key1=>$value1){
				if(strpos(strtolower($key1),"energyrating")!==false){
					//$entry["price"]=$value1;
					}	
				}
			}
			
			if(strpos(strtolower($key),"authority")!==false){
				foreach($value as $key1=>$value1){
				if(strpos(strtolower($key1),"value")!==false){
					//foreach($pty as $t=>$n)
					//$entry["how_sell"]=$pty[strtolower($value1)];
					}	
				}
			}				
		}
		$res = mysql_query("SELECT * FROM property_pricing WHERE prop_id = '".$entry["prop_id"]."'"); 
		if(mysql_num_rows($res)){}else{
		foreach($entry as $key=>$value){ /// this is for preprocessing of the data
			//$entry[$key]=mysql_real_escape_string($value);
			//if($value==""){ $entry[$key]="not available"; }
		}
			mysql_query("INSERT INTO property_pricing ( prop_id,how_sell,sprice,sprice1,pr_from,pr_to,viewed_price )	VALUES ('".$entry["prop_id"]."',  'FS','SP','".$entry["sprice1"]."','','','')") or die(mysql_error());
		}
}
function db_update_property_details($tempArr,$prop_id){
	//$pty=array('1'=>'House','2'=>'Unit/Flat/Apartment','3'=>'Studio','4'=>'Townhouse','7'=>'Villa','15'=>'Vacant Land');
	$pty=array('house'=>'1','unit'=>'2','flat'=>'2','apartment'=>'2','studio'=>'3','townhouse'=>'4','villa'=>'7','vacant land'=>'15');
	//$pty=array('1'=>'房屋','2'=>'公寓','3'=>'Studio','4'=>'别墅','7'=>'别墅','15'=>'农场','16'=>'土地');
	$entry = array();
	$entry["prop_id"]=$prop_id;
		foreach($tempArr as $key=>$value){
			
			if(strpos(strtolower($key),"residential")!==false){
				foreach($value as $key1=>$value1){
				if(strpos(strtolower($key1),"uniqueid")!==false){
					//$entry["prop_id"]=$value1;
					}
				}
			}
			if(strpos(strtolower($key),"buildingdetails")!==false){
				foreach($value as $key1=>$value1){
				if(strpos(strtolower($key1),"energyrating")!==false){
					$entry["energyrating"]=$value1;
					}	
				}
			}
			
			if(strpos(strtolower($key),"category")!==false){
				foreach($value as $key1=>$value1){
				if(strpos(strtolower($key1),"name")!==false){
					foreach($pty as $t=>$n)
					if(strpos(strtolower($value1),$t)!==false)
					$entry["type"]=$pty[strtolower($value1)];
					//echo $value1;
					}	
				}
			}
			
			if(strpos(strtolower($key),"features")!==false){
				foreach($value as $key1=>$value1){
				if(strpos(strtolower($key1),"bedroom")!==false){
					$entry["bedroom"]=$value1;
					}						
				if(strpos(strtolower($key1),"bathroom")!==false){
					$entry["bathroom"]=$value1;
					}	
				if(strpos(strtolower($key1),"carport")!==false){
					$entry["parking"]=$value1;
					}	
				}
			}
				
		}
					//print_r($entry);
		$res = mysql_query("SELECT * FROM property_detail WHERE prop_id = '".$entry["prop_id"]."'"); 
		if(mysql_num_rows($res)){}else{
		foreach($entry as $key=>$value){ /// this is for preprocessing of the data
			//$entry[$key]=mysql_real_escape_string($value);
			//if($value==""){ $entry[$key]="not available"; }
		}
			mysql_query("INSERT INTO property_detail ( prop_id,prop_type,no_bedroom,no_bathroom,parking,energy_eff )	VALUES ('".$entry["prop_id"]."',  '".$entry["type"]."','".$entry["bedroom"]."','".$entry["bathroom"]."','".$entry["parking"]."','".$entry["energyrating"]."')") or die(mysql_error());
		}
}
function db_update_description($tempArr,$prop_id){
	$entry = array();
	$entry["prop_id"]=$prop_id;
		foreach($tempArr as $key=>$value){
			if(strpos(strtolower($key),"residential")!==false){
				foreach($value as $key1=>$value1){
					$value1 = str_replace("'","\'",$value1);
				if(strpos(strtolower($key1),"uniqueid")!==false){
					//$entry["prop_id"]=$value1;
				}	
				if(strpos(strtolower($key1),"headline")!==false){
						$entry["headline"]=$value1;
				}
				if(strpos(strtolower($key1),"description")!==false){
					$entry["description"]=$value1;
				}
				}
				}
			if(strpos(strtolower($key),"root")!==false){ // for agentpoint format...
				foreach($value as $key1=>$value1){	
				$value1 = str_replace("'","\'",$value1);
				if(strpos(strtolower($key1),"headline")!==false){
						$entry["headline"]=$value1;
				}
				if(strpos(strtolower($key1),"description")!==false){
					$entry["description"]=$value1;
				}
				}
				}
		}
		
		$res = mysql_query("SELECT * FROM property_description WHERE prop_id = '".$entry["prop_id"]."'"); 
		if(mysql_num_rows($res)){}else{
		mysql_query("SET NAMES 'UTF8';");
		$temp=$entry["description"];
		//$temp=addslashes($temp) ;
		$temp=mysql_real_escape_string($temp);
		//$temp=str_replace("&","and",$temp);
			mysql_query("INSERT INTO property_description ( prop_id,headline,description )	VALUES ('".$entry["prop_id"]."',  '".$entry["headline"]."','".$temp."')") or die(mysql_error());
		}
}
function db_update_agent($tempArr,$prop_id){
	$entry = array();
	$entry["prop_id"] = $prop_id;
		foreach($tempArr as $key=>$value){
			if(strpos(strtolower($key),"residential")!==false){
				foreach($value as $key1=>$value1){
				if(strpos(strtolower($key1),"uniqueid")!==false){
					}
				}
			}
			if(strpos(strtolower($key),"listingagent")!==false){
				foreach($value as $key1=>$value1){
					if(strpos(strtolower($key1),"name")!==false){
						$last_name = strrchr($value1," ");
						$last_name = substr($last_name,1,strlen($last_name ));
						$first_name = substr($value1,0,strlen($value1)-strlen($last_name));
						if(!array_key_exists("fname1",$entry)){
						$entry["fname1"]=$first_name;
						}else{
						$entry["fname2"]=$first_name;
						}
						if(!array_key_exists("lname1",$entry)){
						$entry["lname1"]=$last_name;
						}else{
						$entry["lname2"]=$last_name;
						}
					}
					if(strpos(strtolower($key1),"telephone")!==false){
						if(!array_key_exists("telephone1",$entry)){
						$entry["telephone1"]=$value1;
						}else{
						$entry["telephone2"]=$value1;
						}
					}
					if(strpos(strtolower($key1),"email")!==false){
						if(!array_key_exists("email1",$entry)){
						$entry["email1"]=$value1;
						}else{
						$entry["email2"]=$value1;
						}
					}
				}
			}
		}
		$res = mysql_query("SELECT * FROM property_contact WHERE prop_id = '".$entry["prop_id"]."'"); 
		if(mysql_num_rows($res)){}else{
			mysql_query("INSERT INTO property_contact ( prop_id,fname1,lname1,email1,phone1,fname2,lname2,email2,phone2 )	VALUES ('".$entry["prop_id"]."', '".$entry["fname1"]."', '".$entry["lname1"]."','".$entry["email1"]."','".$entry["telephone1"]."','".$entry["fname2"]."','".$entry["lname2"]."','".$entry["email2"]."','".$entry["telephone2"]."')");
		}
}
//Google API
function get_lat_lng($street_no,$street_name,$town,$state){//Returning an array, [0] is lat, [1] is lng
$street_no=merge_spaces($street_no);
$street_name=merge_spaces($street_name);
$town=merge_spaces($town);
$state=merge_spaces($state);
$street_no=replace_space($street_no);
$street_name=replace_space($street_name);
$town=replace_space($town);
$state=replace_space($state);

$address = $street_no."+".$street_name."+".$town."+".$state;
$url = "http://maps.google.com/maps/api/geocode/json?address=".$address."&sensor=false&region=Australia";
$ch = curl_init();
curl_setopt($ch, CURLOPT_URL, $url);
curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);
curl_setopt($ch, CURLOPT_PROXYPORT, 3128);
curl_setopt($ch, CURLOPT_SSL_VERIFYHOST, 0);
curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, 0);
$response = curl_exec($ch);
curl_close($ch);
$response_a = json_decode($response);
//echo $lat = $response_a->results[0]->geometry->location->lat;
//echo $address;
//echo $long = $response_a->results[0]->geometry->location->lng;
return $response_a;
}
//Database Connection
function db_connect(){

$dbhost ="localhost";// "localhost"; // this will ususally be 'localhost', but can sometimes differ
$dbname ="first_chinesedb";// "first_update_db"; // the name of the database that you are going to use for this project
$dbuser = "first_chinesedb";//"first_admin";// "first_userdata"; //the username that you created, or were given, to access your database
$dbpass ="Ww,Kf;w.MXeo";// "nimda";//"A1,#v-hGEvDK"; // the password that you created, or were given, to access your database


$link = mysql_connect($dbhost, $dbuser, $dbpass) or die("MySQL Error: " . mysql_error());
mysql_select_db($dbname) or die("MySQL Error: " . mysql_error());
return $link;
}
function db_query($table,$pk,$pk_value,$k,$v){
$res = mysql_query("SELECT * FROM test WHERE name = 'Peter'"); 
if(mysql_num_rows($res)){
	}
else{
	mysql_query("INSERT INTO test ( name, description,date)	VALUES ('Peter', 'Griffin', '1999-6-25')");
	}
}
function db_close($link){
mysql_close($link);
}
//File&String Operation
function get_files($dir){
 $handler = opendir($dir);  
 $fileNTimes=array();  
 $files = array();
    while (($filename = readdir($handler)) !== false) {//务必使用!==，防止目录下出现类似文件名“0”等情况  
        if ($filename != "." && $filename != "..") {  
               $fileNTimes[filemtime($dir.'/'.$filename)]=$filename; 
            }  
       }  
    closedir($handler); 
    ksort($fileNTimes);
    foreach($fileNTimes as $mtime=>$filename){
    	$files[]=$filename;
    }
return $files;
}
function ExtractXMLString($path){
$myfile = fopen($path, "r") or die("Unable to open file!");
		$xml_data = fread($myfile,filesize($path));
fclose($myfile);
return $xml_data;
}
function ExtractArray($xml_data,&$all_data){ //Returning an array
$dom=new DOMDocument();
$dom->loadXML($xml_data);
//$array = array();
$arrays = array();
getArray($dom->documentElement,$arrays);
//print_r($arrays);

foreach($arrays as $arr){
	//print_r($arr);
	$flatArrs = array();
	$root =  array();
	$flatArrs["root"]=$root;
	getFlatArray($arr,$flatArrs,"root","root");
	$all_data[] = $flatArrs;
}
echo count($all_data);
//for testing
foreach($all_data as $flat_arrs){
	foreach($flat_arrs as $ak=>$av){
		echo "<h1>".$ak."</h1><br>";
	foreach($av as $key=>$value){
		echo $key.":".$value."<br>";
		}
	}
}
	//return $flatArrs;
}
function replace_space($string){
	return str_replace(" ","+",$string);
}
function merge_spaces($string){
    return preg_replace("/\s(?=\s)/","\\1",$string);
}
//This function change XML tree structure to a flat structure
function getFlatArray($arr,&$targetArrs,$arrName,$parent){
	$newArr = array();
	$newArr["parent"]=$parent;
	foreach($arr as $key=>$value){
	if(is_array($value)=="array"){ //Determine if the element is array
		getFlatArray($value,$targetArrs,$key,$arrName);
		}
	else{
		$newArr[$key] = $value;
		}
	}
	$targetArrs[$arrName]=$newArr;
}

function getRootInfo($node,&$array){
//print_r($node);
  if($node->hasAttributes()){ 
	foreach ($node->attributes as $attr){
      $array[$attr->nodeName]=$attr->nodeValue;
    }
  }
}

function getArray($node,&$arrays){ //using Dom Document for XML parsing
	$temp_arr=array();
  if($node->nodeType=="1"){ 
  
  if(strtolower($node->nodeName)=="rental"||strtolower($node->nodeName)=="residential"){ //Use rental to recognize a single property listing & separate 
	  $arrays[] =&$temp_arr;
	  getRootInfo($node->parentNode,$temp_arr);
  }
  if($node->hasAttributes()){ 
    foreach ($node->attributes as $attr){
      $temp_arr[$attr->nodeName]=$attr->nodeValue;
    }
  }
  //----------------Logic when node has children----------------------------
  if($node->hasChildNodes()){
    if($node->childNodes->length==1){
    if($node->firstChild->nodeType==1){ 
       		if($node->firstChild->hasAttributes()){        		
       				$id = "";      			
    				foreach ($node->attributes as $attr){
      					if($attr->nodeName=="name"||$attr->nodeName=="id"){
      					$id=$attr->nodeValue;
      					}
   				}
   				if($id!=""){
   				$temp=getArray($node->firstChild,$arrays);
       				$temp["tag"] = $node->firstChild->nodeName;
					if(!array_key_exists($node->firstChild->nodeName.$id)){ //Determine if the key already exists
						$temp_arr[$node->firstChild->nodeName.$id]= $temp;
					}
       			}else{
					if(!array_key_exists($node->firstChild->nodeName)){ //Determine if the key already exists
						$temp_arr[$node->firstChild->nodeName]=getArray($node->firstChild,$arrays);
					}
          		}
          	}else{ // <CDATA> CODES should be here...
          		if($node->firstChild->nodeName=='#cdata-section'){
					if(!array_key_exists($node->firstChild->nodeName)){ //Determine if the key already exists
          			$temp_arr[$node->firstChild->nodeName]=getArray($node->firstChild,$arrays); 
					}
          		}else{
					if(!array_key_exists($node->firstChild->nodeName)){ //Determine if the key already exists
          			$temp_arr[$node->firstChild->nodeName]=getArray($node->firstChild,$arrays); 
					}
          		}
          	}    	
      }else{            
    	return $node->firstChild->nodeValue;
      }
    } else { 
      foreach ($node->childNodes as $childNode){
       if($childNode->nodeType!=XML_TEXT_NODE){
      		if($childNode->hasAttributes()){ 
       				$id = "";      			
    				foreach ($childNode->attributes as $attr){
      					if($attr->nodeName=="name"||$attr->nodeName=="id"){
      					$id=$attr->nodeValue;
      					}
   				}
   				if($id!=""){
   				$temp=getArray($childNode,$arrays);
       				$temp["tag"] = $childNode->nodeName;
       				$temp_arr[$childNode->nodeName.$id]= $temp;
       				}else{
          			$temp_arr[$childNode->nodeName]=getArray($childNode,$arrays);
          			}
          		}else{// <CDATA> CODES should be here...
          		if($childNode->nodeName=='#cdata-section'){
          			return $childNode->nodeValue; 
          		}else{
          			$temp_arr[$childNode->nodeName]=getArray($childNode,$arrays);
          		}
          	}
        }
    }
  }
  }
  //------------------------Logic when node has children----------------------------------------------------
  } else {
    return $node->nodeValue;
  }
  return $temp_arr;
}
//Auto Email forwarding functions
function send_email($listingID,array $recipients){
	
	//$SendTo = "hzrtnt018@gmail.com";
	//$SendTo1 = "agentpoint@hzrtnt018.com";
	
	$url = "hzrtnt018.com";
	$port = 25;
	$user = "jerry@hzrtnt018.com";
	$pass = "12345";
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
?>