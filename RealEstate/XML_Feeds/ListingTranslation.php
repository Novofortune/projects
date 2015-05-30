<!Doctype html>
<html>
<head>
<title>Listing Translation</title>
<meta charset="utf-8"/>
<style>
body { margin:40px; }
h4.heading { text-align:right;font-weight:bold; }
h4.content { text-align:left;font-weight:normal;width:600px }
div.column { float:left; margin:30px; }
table, td, tr { border:1px solid black; line-height:20px;vertical-align:top; }

#left_colunm {  }
#right_column {  }
</style>
<script src="http://first2move.com.au/js/jquery.min.js"></script>
</head>
<body>
<h3>Listing Details</h3>
<hr/>
<?php 
session_start();

require 'MySQLAdapter.php';
require 'property.php';
require 'phpmailer/PHPMailerAutoload.php';

ini_set('max_execution_time', 300);

$dbhost ="localhost";// "localhost"; // this will ususally be 'localhost', but can sometimes differ
$dbname ="first_chinesedb";// "first_update_db"; // the name of the database that you are going to use for this project
$dbuser = "root";//"first_admin";// "first_userdata"; //the username that you created, or were given, to access your database
$dbpass ="1234";// "nimda";//"A1,#v-hGEvDK"; // the password that you created, or were given, to access your database

$_config = array($dbhost,$dbuser,$dbpass,$dbname);
$orm_adapter = new ORM_mapper('ORM.json');
$db_adapter = new MySQLAdapter($orm_adapter,$_config);

if(isset($_REQUEST['pid'])){
	$property = new property();
	//$db_adapter->db_load($property,array('property_id'=>base64_decode(base64_encode($_REQUEST['pid']))));
	//$property->street_no = 13;
	//$property->property_descriptions[0]->headline = $property->property_descriptions[0]->headline."****";
	//$db_adapter->db_save($property,array('property_id'=>19));
	//$db_adapter->db_load($property,array('property_id'=>19));
	
	
	//$objects = $db_adapter->bulk_load('property',"property_id"); // only load main table properties 
	//print_r($objects);
	//for($i=0;$i<count($objects);$i++){
		//$db_adapter->db_load($objects[$i],array("property_id"=>$objects[$i]->property_id));
		//if($i==19)
		//print_r($objects[$i]);
	//}
	//echo count($objects);
	//print_r($objects[]);
			//$property->property_id = 254634;
			//$property->street_no = 999;
			//$property->listing_agents[] = new listing_agent();
			//$property->listing_agents[0]->listing_agent_id = 10001;
			//$property->listing_agents[0]->property_id = 10001;
			//$property->listing_agents[0]->first_name = 'zhenrui';
			//$property->listing_agents[0]->last_name = 'han';
			//$property->listing_agents[0]->telephone = 10001;
			//$property->listing_agents[0]->email = 'hzrtnt018@126.com';
	//print_r( $property->listing_agents[0]);
	//$db_adapter->db_save($property,array('property_id'=>$property->property_id));
	//$db_adapter->db_create($property);
	$db_adapter->db_load($property,array('property_id'=>$_REQUEST["pid"]));
	//$db_adapter->db_del($property,array('property_id'=>$property->property_id));
	//$db_adapter->db_del($property,array('property_id'=>$property->property_id));
	
	//$db_adapter->destruct(); // Destroy the adapter and Recreate it for another DB 
	
	
	$_SESSION['request_property'] = serialize($property);
	//$property->

	
}else{
	
}

?>

<table>
<tr>
	<td>
		<h4  class = "heading">Listing ID</h4>
	</td>
	<td>
		<h4 id="pid" class = "content"><?php echo $property->property_id;?></h4>
	</td>
</tr>

<tr>
	<td>
		<h4 class = "heading">Requested Time</h4>
	</td>
	<td>
		<h4 class = "content">&nbsp;</h4>
	</td>
</tr>

<tr>
	<td>
		<h4 class = "heading">Address</h4>
	</td>
	<td>
		<h4 class = "content"><?php echo $property->street_no." ".$property->street." ".$property->town." ".$property->state;?></h4>
	</td>
</tr>

<tr>
	<td>		
		<h4 class = "heading">Language</h4>
	</td>
	<td>
		<h4 class = "content">Chinese(Simplied)</h4>
	</td>
</tr>

<tr>
	<td>
		<h4 class = "heading">Office</h4>
	</td>
	<td>
		<h4 class = "content">&nbsp;</h4>
	</td>
</tr>

<tr>
	<td>
		<h4 class = "heading">Original Title</h4>
	</td>
	<td>
		<h4 class = "content"><?php echo $property->property_descriptions[0]->headline; ?></h4>
	</td>
</tr>

<tr>
	<td>
		<h4 class = "heading">Original Description</h4>
	</td>
	<td>
		<h4 class = "content"><?php echo $property->property_descriptions[0]->description; ?></h4>
	</td>
</tr>

<tr>
	<td>
		<h4 class = "heading">Translated Title</h4>
	</td>
	<td>
		<textarea id="headline" style="width:99%;height:100%;font-size:15px;">Please Enter Translated Title Here...</textarea>
	</td>
</tr>

<tr>
	<td>
		<h4 class = "heading">Translated Description</h4>
	</td>
	<td>
		<textarea id="description" style="width:99%;height:100%;font-size:15px;">Please Enter Translated Description Here...</textarea>
	</td>
</tr>
</table>

<button id = "btn_submit" onclick="submit();" style="margin:30px;text-align:center;position:relative;left:50%"><h3>Submit</h3></button>

<script type="text/javascript">
	function submit(){
		
		var headline = document.getElementById('headline').value;
		var description = document.getElementById('description').value;
		//alert(headline);
		//alert(description);
		$.ajax({
			type: "POST",
			url: "actions.php",
			data: {"action":"translation_submit","headline":headline,"description":description},
			success: function (msg) { 
			alert(msg);
			//window.open(msg); 
			},
			failure: function (msg) {  },
			error: function (msg) {  }
		});
	}
	
	
</script>
</body>
</html>