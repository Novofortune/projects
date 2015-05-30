<?php

interface DatabaseAdapterInterface
{
    function connect();    
    function disconnect();      
    function query($query);    
    function fetch();  
    function select($table, $where, $fields, $order, $limit, $offset);
    function insert($table, array $data);    
    function update($table, array $data, $where);    
    function delete($table, $where);    
    function getInsertId();    
    function countRows();    
    function getAffectedRows();

//--------------------------------------------Above Default Code Implemented------------------------------------------------
//--------------------------------------------Below Custom Code Implemented-------------------------------------------------

	function adv_select($table, array $where = null, array $fields = null, $order = '', $limit = null, $offset = null);
	function GetArray($table, array $where = null, array $fields = null, $order = '', $limit = null, $offset = null); //Not Very Useful
	function fetch_array(); // Usually used after adv_select() and select() to get the result as an array.
	function adv_update($table, array $data, array $where);
	function adv_delete($table,array $where);

	function associate_array_to_string($array,$entry_separater='&',$key_value_separater='=',$start='',$end='');//Accessory Function

	function entry_exist($table,array $where_arr);

	function conditional_query($main_query,$condition);//Not In Use
	function conditional_insert($table, array $data,$condition);//Not In Use
	function unique_insert($table, array $data,array $where_arr);//Not In Use

	function db_load(&$object,array $primary_keys);
	function db_save(&$object,array $primary_keys);
	function db_del(&$object,array $primary_keys);
	function db_exist(&$object,array $primary_keys);

	function bulk_load($class_name, $key_property_name,$order_mode = 'desc',array $output_property_names = null,$limit = null, $offset = null);

}
?>