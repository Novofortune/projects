<?php
require("FileOperation.php");
//tests
$act=new ActionManager();
print_r( $act->load("json/action.json"));

class ActionManager
{
	
	var $dict = array();
	//load Action setting from json
	public function load($json)
	{
		return FileOperation::ReadJson($json);
	}
	//save Action setting to json
	public function save($json)
	{
		
	}
	//create Action setting as a json file
	public function createActionScript()
	{
	
	}
}
?>