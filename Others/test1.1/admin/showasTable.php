<?php
include 'interface.DatabaseAdapter.php';
include 'class.MySQLAdapter.php';


function Transform($type, $db, $class)
{
	switch($type)
	{
		case('table'):
			return ReadTable($db ,$class);
		case('line'):
			return '';
		default:
			return null;
	}
	
}

function ReadTable($database,$class)
{
	$content = '';
	if($class=='')
	{
		$content = $content.'<table>';
	}
	else
	{
		$content=$content.'<table class="'.$class.'">';
	}
	$content='';
	$_config = array("localhost","root","","forum");

	$config = new MySQLAdapter($_config);

	$config->connect();

	$config->select($database,null,'*',null,null,null);
	$tmp = 0;
	while($row = $config->fetch())
	{

		if($tmp==0)
		{ 
			$content = $content. '<tr>';
			foreach($row as $key => $value)
			{
				$content = $content. '<th>';
					$content = $content.$key;
				$content = $content. '</th>';
			}
			$content = $content. '</tr>';
		}
		$tmp++;
		$content = $content.'<tr>';
		foreach($row as $key => $value)
		{
			$content = $content. '<td>';
				$content = $content.$value;
			$content = $content. '</td>';
		}
		$content = $content.'</tr>';
	}
	$content = $content.'</table>';
	return $content;
}


?>