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
$act=new ActionManager();
$actions = $act->load("json/action.json");
$tmp=array();
foreach($actions as $key => $value)
{
	$tmp[$key]=$value;
}



if(isset($_POST['action']))
{
	switch ($_POST['action']) 
	{
		case '1':
		{
			echo Transform('table',$tmp['1'],$class);
			break;
		}
		case '2':
		{
			echo Transform('table',$tmp['2'],$class);
			break;
		}
		case '3':
		{
			echo Transform('table',$tmp['3'],$class);
			break;
		}
		case '4':
		{
			echo Transform('table',$tmp['4'],$class);
			break;
		}
	}
}
?>