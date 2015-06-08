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

print_r($tmp);

function LoopReadJsonObject($object){
	$array = array();
	foreach($object as $key=>$value){
		if(is_object($value)){
			$array[$key] = LoopReadJsonObject($value);
		}else{
			$array[$key] = $value;
		}
	}
	return $array;
}


//class a {
//$p;
//function function1(){}
//}

//$class_name = "a";
//$property_name = "p";
//$function_name = "function1";

//$instance = new $class_name();
//$instance->{$property_name};
//$instance->$function_name();

//$instance = new a();
//$instance->p;
//$instance->function1();



//$functionName = 'Transform';

if(isset($_POST['action']))
{
	switch ($_POST['action']) 
	{
		case '1':
		{
			$class_name = $tmp['1']['class'];
			$instance = new $class_name();
			foreach($tmp['1']['parameter'] as $key => $value)
			{
				$instance -> $key = $value;
			}
			$function_name = $tmp['1']['function'];
			echo $instance -> $function_name();
			
			break;
		}
		case '2':
		{
			$class_name = $tmp['2']['class'];
			$instance = new $class_name();
			foreach($tmp['2']['parameter'] as $key => $value)
			{
				$instance -> $key = $value;
			}
			$function_name = $tmp['2']['function'];
			echo $instance -> $function_name();
			break;
		}
		case '3':
		{
			$class_name = $tmp['3']['class'];
			$instance = new $class_name();
			foreach($tmp['3']['parameter'] as $key => $value)
			{
				$instance -> $key = $value;
			}
			$function_name = $tmp['3']['function'];
			echo $instance -> $function_name();
			break;
		}
		case '4':
		{
			$class_name = $tmp['4']['class'];
			$instance = new $class_name();
			foreach($tmp['4']['parameter'] as $key => $value)
			{
				$instance -> $key = $value;
			}
			$function_name = $tmp['4']['function'];
			echo $instance -> $function_name();
			break;
		}
	}
}
?>