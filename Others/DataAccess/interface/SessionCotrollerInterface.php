<?php
interface SessionControllerInterface{ 
	function __construct($json_file); // The config file is read-only to this class, the information is the mapping from class and function to certain php file in the file system, besides are the authority control based on user id...
	function call_function($class_name,$function_name,$json_format_parameters,$user_id);
	//function get_property($class_name,$property_name);
	//function set_property($class_name,$property_name,$property_value);

	function get_function_map();
	function get_variable_map();
}

?>