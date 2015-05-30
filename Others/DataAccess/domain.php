<?php
class user{
	public $user_id;
	public $user_name;
	public $user_level;
	
	private $db_adapter;
	private $ORM_mapper;
	private $reflection;
	
	public function __construct(){

	}
	public function initialize(&$db_adapter){
		$this->db_adapter = &$db_adapter;
	}
	public function __destruct(){
		
	}
	
	
	//Database functions
	//More objectives:
	//1. change all functions to static functions
	//2. create another bulk load functions
	public function db_load(array $primary_keys){
		$this->db_adapter->db_load($this,$primary_keys);
	}
	public function db_save(array $primary_keys){
		$this->db_adapter->db_save($this,$primary_keys);
	}
	public function db_del(array $primary_keys){
		$this->db_adapter->db_del($this,$primary_keys);
	}
	public function db_exist(array $primary_keys){
		$this->db_adapter->db_exist($this,$primary_keys);
	}
	
	
	//Presenet Layer functions
	public function show(){
		
	}
}

?>