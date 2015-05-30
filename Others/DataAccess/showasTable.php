<?php
require 'DatabaseAdapterInterface.php';
require 'MySQLAdapter.php';
require 'domain.php';

$_config = array("localhost","root","1234","users");
$orm_adapter = new ORM_mapper('ORM.json');
$db_adapter = new MySQLAdapter($orm_adapter,$_config);
//$rows = $db_adapter->getArray('users',array('user_id'=>'1'));
//print_r($rows);
$user = new user();
$user->initialize($db_adapter);
$user->user_id = '5';
$user->user_name = '6';
$user->user_level = '6';

echo "Before Operation: user_name ".$user->user_name.";";
echo "user_level ".$user->user_level.";<br/>";

//$db_adapter->getArray();
if($user->db_exist(array('user_id'))){
	echo 'good<br/>';
}else{
	echo 'bad<br/>';
}
//$user->db_save(array('user_id'));
$user->db_load(array('user_id'));
//$user->db_del(array('user_id'));
echo "After Operation: user_name ".$user->user_name.";";
echo "user_level ".$user->user_level.";<br/>";

echo "<hr/>";
echo "<h3>Bulk Load</h3>";
$users = $db_adapter->bulk_load('user','user_id','asc',array('user_id','user_name'));
echo count($users);

?>