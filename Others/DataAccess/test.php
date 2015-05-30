<?php

class MyClass{
	public $name;
	function __construct(){
		$this->name = "MyName";
	}
}
$class_name = 'MyClass';
$object = new $class_name();
echo get_class($object);
echo $object->name;
?>

