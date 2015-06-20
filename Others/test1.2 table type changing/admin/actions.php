<?php
include ("showasTable.php");
include ("class.ActionManager.php");
//main function
$class='';
if(isset($_POST['class']))
{
	$class = $_POST['class'];
}

//json object to array
$act = new ActionManager();
$actions = $act->load("json/action.json");
$tmp = LoopReadJsonObject($actions);

//print_r($tmp);

function LoopReadJsonObject($object)
{
	$array = array();
	foreach($object as $key => $value)
	{
		if(is_object($value))
		{
			$array[$key] = LoopReadJsonObject($value);
		}
		else
		{
			$array[$key] = $value;
		}
	}
	return $array;
}


if(isset($_POST['action']))
{
	$temp = $tmp['display'];

	$post_action = $_POST['action'];
	$class_name = $temp[$post_action]['class'];
	$instance = new $class_name();
	foreach($temp[$post_action]['parameter'] as $key => $value)
	{
		$instance -> $key = $value;
	}
	$function_name = $temp[$post_action]['function'];
	echo $instance -> $function_name();
}
?>