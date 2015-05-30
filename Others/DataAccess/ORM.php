<?php

class ORM_mapper{
	public $map = array();
	
	public function __construct($json_file){
		$json_data = $this->ExtractFileString($json_file);
		$temp = json_decode($json_data);
		$this->map = $temp;
	}
	public function save_to_file($json_file){
		$json_string = json_encode($this->map);
		$this->WriteToFile($json_string,$json_file);
	}
	public function obj_to_table($class_name){
		if(array_key_exists($class_name,$this->map)){
			$temp = (array)$this->map;
			return $temp[$class_name];
		}
		return null;
	}
	public function property_to_field($class_name,$property_name){
		$key=$class_name.'-'.$property_name;
		if(array_key_exists($key,$this->map)){
			$temp = (array)$this->map;
			$res = explode('-',$temp[$key]);
			/*
			** $res[0] is table name; $res[1] is field name
			*/
			return $res;
		}
		return null;
	}
	public function ExtractFileString($path){
		$fh = fopen($path, "r") or die("Unable to open file!");
		$data = fread($fh,filesize($path));
		fclose($fh);
		return $data;
	}
	public function WriteToFile($str,$path){
		$fh = fopen($path, "w") or die("Unable to write to file");
		fwrite($fh,$str);
		fclose($fh);
	}	
}

?>