<?php
include 'interface.DatabaseAdapter.php';
include 'class.MySQLAdapter.php';

class show
{
	var $type = '';
	var $db = '';
	var $cla = '';
	public function Transform()
	{
		//echo $this -> type."</br>";
		switch($this -> type)
		{
			case('table'):
				return ReadTable($this -> db ,$this -> cla);
			case('line'):
				return '';
			default:
				return null;
		}
	}
}


function ReadTable($database,$class)
{
	$content = '';
	$content.=($class==""?'<table>':'<table class="'.$class["table"].'">');
	$_config = array("localhost","root","","forum");

	$config = new MySQLAdapter($_config);

	$config->connect();

	$config->select($database,null,'*',null,null,null);
	$tmp = 0;
	while($row = $config->fetch())
	{

		if($tmp==0)
		{ 
			$content .= ($class==""?'<tr>':'<tr class="'.$class["tr"].'">');
			foreach($row as $key => $value)
			{
				$content.=($class==""?'<td>':'<td class="'.$class["th"].'">');
					$content = $content.$key;
				$content = $content. '</td>';
			}
			$content = $content. '</tr>';
		}
		$tmp++;
		$content .= ($class==""?'<tr>':'<tr class="'.$class["tr"].'">');
		foreach($row as $key => $value)
		{
			$content .= ($class==""?'<td>':'<td class="'.$class["td"].'">');
				$content = $content.$value;
			$content = $content. '</td>';
		}
		$content = $content.'</tr>';
	}
	$content = $content.'</table>';
	return $content;
}


?>