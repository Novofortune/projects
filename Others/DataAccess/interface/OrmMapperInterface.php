<?php

interface OrmMapperInterface{

	function __construct($json_file);
	function save_to_file($json_file);
	function obj_to_table($class_name);
	function property_to_field($class_name,$property_name);

	function ExtractFileString($file);
	function WriteToFile($str,$file);

//-------------------------------Above Code Implemented ---------------------------------------------------------------------
//-------------------------------Below Code Not Implemented------------------------------------------------------------------

	function get_class_structure($class_name); // Get a particular class to table mapping
	function get_orm(); // Get the entire system class to table mapping according to the current jsonfile.

	function get_system_structure(); //Read System Assembly to get all classes and their properties...
	function get_db_structure(); //Read db structure to get all table and field...
	function compare_structure(); // Looking for inconsistency between system and db structures
	
	function generate_db_entities($class_name); // Generate new db entities for a particular class
	function link_db_main_table($class_name,$table_name); //Link class to main table and store the information in a json file
	function link_db_field($class_name,$property_name,$table_name,$field_name); //Link property to fild and store the information in a json file

	function generate_db_structure($json_file); //Generate a new db and link all object to it and store the information in a new json file. 
	function update_db_structure($json_file); // Update 
	

	function create_table($table_name);
	function remove_table($table_name);
	function rename_table($table_name,$new_name);
	function add_field($table_name,$field_name);
	function remove_field($table_name,$field_name);
	function rename_field($table_name,$field_name,$new_name);
	
	function merge_table($  table_name1,$table_name2); // Including data
	function split_table($table_name,$table_name1,array $fields); //Including data

}

?>