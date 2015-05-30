<?php //This code is to control the calling of php back-end functions from front-end javascript function, More packaged functions required...
session_start(); // Enable Session...must be placed before all other codes

require 'MySQLAdapter.php';
require 'property.php';


if(CheckPostVar('action')) {
    $action = $_POST['action'];
    switch($action) {
        case 'goto_listing_by_id' : {
			if(CheckPostVar('pid')){
				echo goto_listing_by_id($_POST['pid']); // Return this string back to front-end
			}
		}
		case 'translation_submit' : {
			if(CheckSessionVar('request_property')&&CheckPostVar('headline')&&CheckPostVar('description')){
				echo translation_submit(unserialize($_SESSION['request_property']),$_POST['headline'],$_POST['description']); // Return this string back to front-end
			}else{
				
			}
		}
	}
}



function goto_listing_by_id($pid){
	
	//$url = 'http://www.first2move.com.au/property-detail.php?pid='.base64_encode($pid); 
	$url = 'ListingTranslation.php?pid='.base64_encode($pid);
	return $url;
}



function translation_submit($property,$headline,$description){
	
$dbhost ="localhost";// "localhost"; // this will ususally be 'localhost', but can sometimes differ
$dbname ="first_chinesedb";// "first_update_db"; // the name of the database that you are going to use for this project
$dbuser = "root";//"first_admin";// "first_userdata"; //the username that you created, or were given, to access your database
$dbpass ="1234";// "nimda";//"A1,#v-hGEvDK"; // the password that you created, or were given, to access your database

$_config = array($dbhost,$dbuser,$dbpass,$dbname);
$orm_adapter = new ORM_mapper('ORM.json');
$db_adapter = new MySQLAdapter($orm_adapter,$_config);
	
	if($property->property_descriptions!=null){
		if(count($property->property_descriptions)>0){
			$property->property_descriptions[0]->headline = $headline;
			$property->property_descriptions[0]->description = $description;
		}else{
			$property_description = new property_description();
			$property_description->headline = $headline;
			$property_description->description = $description;
			$property->property_descriptions[] = $property_description;
		}
	}else{
		$property_description = new property_description();
		$property_description->headline = $headline;
		$property_description->description = $description;
		$property->property_descriptions[] = $property_description;
	}
	
	$db_adapter->db_save($property,array('property_id'=>$property->property_id));
	//Test If the entry is correctly updated
	//$test = new property();
	//$db_adapter->db_load($test,array('property_id'=>$property->property_id));
	//$r = new ReflectionClass($test);
	//$result = true;
	//foreach($r->getProperties() as $prop_item){
		//if($test->{$prop_item->getName()}!=$property->{$prop_item->getName()}){
			//$result = false;
		//}
	//}
	//Test Ended
	//if($result){
		$filedata  = FileOperation::ExtractFileString("propertyinfo/PropertyInfo-".$property->property_id.".json"); // Check if the file exists and get its data
		$propertyinfo = json_decode($filedata);
		$propertyinfo->translation_status = true;
		FileOperation::WriteJson($propertyinfo,"propertyinfo/PropertyInfo-".$property->property_id.".json");
		//$db_adapter->db_save($property,array('property_id'=>$property->property_id));
	//}
	
	return $result;
}


function CheckSessionVar($variable_name){
	if(isset($_SESSION[$variable_name])&& !empty($_SESSION[$variable_name])){
				return true;
			}else{
				return false;
			}
}
function CheckPostVar($variable_name){
	if(isset($_POST[$variable_name])&& !empty($_POST[$variable_name])){
				return true;
			}else{
				return false;
			}
}
?>