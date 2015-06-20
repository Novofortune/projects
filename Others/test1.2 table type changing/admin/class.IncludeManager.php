<?php
require("FileOperation.php");
class IncludeManager
{

	var $dict = array();
	// load Include setting from json
	public function load($json)
	{
		return FileOperation::ReadJson($json);
	}
	// save Include setting to json
	public function save($json)
	{
		
	}
	//create Include setting as a json file
	public function createIncludeScript()
	{
	
	}
}
?>