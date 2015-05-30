<?php
//include 'MySQLAdapterException.php';
class MySQLAdapter //implements DataBaseAdapterInterface
{
    protected $_config = array();
    protected $_link;
    protected $_result;
    protected static $_instance;
	public $ORM_mapper;
    
    /**
     * Get the Singleton instance of the class
     */
    public static function getInstance($orm_mapper, array $config = array())
    {
        if (self::$_instance === null) {
            self::$_instance = new self($orm_mapper, $config);
        }
        return self::$_instance;
    }
    
    /**
     * Class constructor
     */ 
    public function __construct(&$orm_mapper, array $config)
    {
        if (count($config) !== 4) {
			ehco 'Invalid number of connection parameters.';
            //throw new MySQLAdapterException('Invalid number of connection parameters.');   
        }
        $this->_config = $config;
		$this->ORM_mapper = $orm_mapper;
    }
    
    /**
     * Prevent cloning the instance of the class
     */ 
    protected function __clone(){}
    
    /**
     * Connect to MySQL
     */
    public function connect()
    {
        // connect only once
        if ($this->_link === null) {
            list($host, $user, $password, $database) = $this->_config;
            if ((!$this->_link = @mysqli_connect($host, $user, $password, $database))) {
				echo 'Error connecting to MySQL : ' . mysqli_connect_error();
			   //throw new MySQLAdapterException('Error connecting to MySQL : ' . mysqli_connect_error());
            }
            unset($host, $user, $password, $database);       
        }
    }

    /**
     * Execute the specified query
     */
    public function query($query)
    {
        if (!is_string($query) || empty($query)) {
			echo 'The specified query is not valid.';
            //throw new MySQLAdapterException('The specified query is not valid.');   
        }
        // lazy connect to MySQL
        $this->connect();
        if (!$this->_result = mysqli_query($this->_link, $query)) {
			echo 'Error executing the specified query ' . $query . mysqli_error($this->_link);
            //throw new MySQLAdapterException('Error executing the specified query ' . $query . mysqli_error($this->_link));
        }
    }
    
    /**
     * Perform a SELECT statement
     */ 
    public function select($table, $where = '', $fields = '*', $order = '', $limit = null, $offset = null)
    {
        $query = 'SELECT ' . $fields . ' FROM ' . $table
               . (($where) ? ' WHERE ' . $where : '')
			   . (($order) ? ' ORDER BY ' . $order : '')
               . (($limit) ? ' LIMIT ' . $limit : '')
               . (($offset && $limit) ? ' OFFSET ' . $offset : '');
        $this->query($query);
        return $this->countRows();
    }
    /**
     * Perform an INSERT statement
     */  
    public function insert($table, array $data)
    {
        $fields = implode(',', array_keys($data));
        $values = implode(',', array_map(array($this, 'quoteValue'), array_values($data)));
        $query = 'INSERT INTO ' . $table . '(' . $fields . ')' . ' VALUES (' . $values . ')';
        $this->query($query);
        return $this->getInsertId();
    }
	
    /**
     * Perform an UPDATE statement
     */
    public function update($table, array $data, $where = '')
    {
        $set = array();
        foreach ($data as $field => $value) {
            $set[] = $field . '=' . $this->quoteValue($value);
        }
        $set = implode(',', $set);
        $query = 'UPDATE ' . $table . ' SET ' . $set 
               . (($where) ? ' WHERE ' . $where : '');
        $this->query($query);
        return $this->getAffectedRows();  
    }
    
    /**
     * Perform a DELETE statement
     */
    public function delete($table, $where = '')
    {
        $query = 'DELETE FROM ' . $table
               . (($where) ? ' WHERE ' . $where : '');
        $this->query($query);
        return $this->getAffectedRows();
    }
    
    /**
     * Single quote the specified value
     */ 
    public function quoteValue($value)
    {
        if ($value === null) {
            $value = 'NULL';
        }
        else if (!is_numeric($value)) {
            $value = "’" . mysqli_real_escape_string($this->_link, $value) . "’";
        }
        return $value;
    }
    
    /**
     * Fetch a single row from the current result set (as an associative array)
     */
    public function fetch()
    {
        if ($this->_result !== null) {
            if ((!$row = mysqli_fetch_array($this->_result, MYSQLI_ASSOC))) {
                $this->freeResult();
                return false;
            }
            return $row; 
        }
    }

    /**
     * Get the insertion ID
     */ 
    public function getInsertId()
    {
        return $this->_link !== null ? 
               mysqli_insert_id($this->_link) :
               null;  
    }
    
    /**
     * Get the number of rows returned by the current result set
     */  
    public function countRows()
    { 
        return $this->_result !== null ? 
               mysqli_num_rows($this->_result) : 
               0;
    }
    
    /**
     * Get the number of affected rows
     */ 
    public function getAffectedRows()
    {
        return $this->_link !== null ? 
               mysqli_affected_rows($this->_link) : 
               0;
    }
    
    /**
     * Free up the current result set
     */ 
    public function freeResult()
    {
        if ($this->_result !== null) {
            mysqli_free_result($this->_result);   
        }
    }
    
    /**
     * Close explicitly the database connection
     */ 
    public function disconnect()
    {
        if ($this->_link !== null) {
            mysqli_close($this->_link);
            $this->_link = null;
        }
    }
    
    /**
     * Close automatically the database connection when the instance of the class is destroyed
     */ 
    public function __destruct()
    {
        $this->disconnect();
    }
	
	
	//自定义代码=================================================================================================
	
	public function adv_select($table, array $where = null, array $fields = null, $order = '', $limit = null, $offset = null){
		$where_str = '';
		$fields_str = '*';
		if($where!=null){
			$where_str = $this->associate_array_to_string($where);
		}
		if($fields!=null){
			$fields_str = implode(',',$fields);
		}
		$this->select($table,$where_str,$fields_str,$order,$limit,$offset);		
	}
	//GetArray function is a simpler version of select
	public function GetArray($table, array $where = null, array $fields = null, $order = '', $limit = null, $offset = null){
		$this->adv_select($table,  $where, $fields, $order, $limit, $offset);
		$rows = array();
		while($row = $this->fetch()){
			$rows[] = $row;
		}
		return $rows;
	}
	//fetch_array() is simpler version of fetch
	public function fetch_array(){
		$rows = array();
		while($row = $this->fetch()){
			$rows[] = $row;
		}
		return $rows;
	}
	public function adv_update($table, array $data, array $where){
		$where_str = '';
		if($where!=null){
			$where_str = $this->associate_array_to_string($where);
		}
			return $this->update($table,$data,$where_str);
	}
	public function adv_delete($table,array $where){
		$where_str = '';
		if($where!=null){
			$where_str = $this->associate_array_to_string($where);
		}
		return $this->delete($table,$where_str);
	}
	public function associate_array_to_string($array,$entry_separater='&',$key_value_separater='=',$start='',$end=''){
		if($array!=null&&count($array)>0){
			$keys = array_keys($array);
			$values = array_values($array);
			$condition = $start;
			$condition = $condition." ".$keys[0]."=".$values[0];
			for($i=1;$i<count($keys);$i++){
				$condition = $condition." and ".$keys[$i]."=".$values[$i];
			}
			return $condition = $condition.$end;
		}
		return '';
	}
	
    /**
		在执行Query语句前进行判定
	*/
	public function entry_exist($table,array $where_arr){
		$this-> adv_select($table,$where_arr);
		if($this-> countRows()==0){
			return false;
		}
		return $this-> countRows();
	}
	public function conditional_query($main_query,$condition){		
        if (!is_string($main_query) || empty($main_query)||!is_string($condition)) {
            echo 'The specified main_query is not valid.';
			//throw new MySQLAdapterException('The specified main_query is not valid.');   
        }
        // lazy connect to MySQL
        $this->connect();
        if (!$this->_result = mysqli_query($this->_link, $main_query . " " . $condition)) {
            echo 'Error executing the specified main_query ' . $main_query . " " . $condition . mysqli_error($this->_link);
			//throw new MySQLAdapterException('Error executing the specified main_query ' . $main_query . " " . $condition . mysqli_error($this->_link));
        }
	}
    //执行有条件INSERT语句
	public function conditional_insert($table, array $data,$condition){
        $fields = implode(',', array_keys($data));
        $values = implode(',', array_map(array($this, 'quoteValue'), array_values($data)));
        $query = 'INSERT INTO ' . $table . '(' . $fields . ')' . ' VALUES (' . $values . ')';
        $this->conditional_query($query,$condition);
        return $this->getInsertId();
	}
	public function unique_insert($table, array $data,array $where_arr){
		$this-> adv_select($table,$where_arr);
		if($this-> countRows()==0){
			$this->insert($table,$data);
			return $this->getInsertId();
		};
		return 'key exist';
	}
	
	//逻辑层数据库操作辅助函数:
	public function db_load(&$object,array $primary_keys){ //This method require 1. the primary key has to be only one 2. no collection property
		$reflection = new ReflectionClass($object);
		if($this->db_exist($object,$primary_keys)==1){ //if the entry already exist
			echo 'single entry found<br/>';
			$properties = $reflection->getProperties();
			$tables = array();
			foreach($properties as $property){	
				$table = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[0];
				$field = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[1];
				if($field!=null){ // Create a two layer array to store all table and fields to property information
					if(array_key_exists($table,$tables)){
						$fields = &$tables[$table];
						$fields[$field] = $property->getName();	
					}else{
						$fields[$field] = $property->getName();
						$tables[$table] = $fields;
					}
				}else{
					//echo $property->getName(); Show all unconvertable properties (which are not existing in database)
				}
			}
			//print_r($tables);
			$condition = array();
			foreach($primary_keys as $primary_key){
				$table1 = $this->ORM_mapper->property_to_field($reflection->getName(),$primary_key)[0];
				$field1 = $this->ORM_mapper->property_to_field($reflection->getName(),$primary_key)[1];
				$condition[$field1] = $object->{$primary_key};		//key is the field in the table and the value is the property value	
			}
			foreach($tables as $table=>$fields){
				
				$this->adv_select($table,$condition,array_keys($fields));
				$rows = $this->fetch_array();
				foreach($rows as $row){ //The number of rows should always be 1
					foreach($row as $key=>$value){
						echo $key."-".$value.'<br/>';
						$object->{$fields[$key]} = $value; // give the value to $object->property.
					}
				}
			}
		}else if($this->db_exist($object,$primary_keys)>1){
			echo 'have multiple rows<br/>';
		}else{ // if the entry does not exist
			echo 'No such existing entry<br/>';
		}
	}
	public function db_save(&$object,array $primary_keys){	
		$reflection = new ReflectionClass($object);
		if($this->db_exist($object,$primary_keys)==1){ //if the entry already exist$properties = $reflection->getProperties();
			echo 'single entry found<br/>';
			$properties = $reflection->getProperties();
			$tables = array();
			foreach($properties as $property){	
				$table = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[0];
				$field = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[1];
				if($field!=null){ // Create a two layer array to store all table and fields to property information
					if(array_key_exists($table,$tables)){
						$fields = &$tables[$table];
						$fields[$field] = $property->getValue($object);	
					}else{
						$fields[$field] = $property->getValue($object);
						$tables[$table] = $fields;
					}
				}else{
				}
			}
			$condition = array();
			foreach($primary_keys as $primary_key){
				$table1 = $this->ORM_mapper->property_to_field($reflection->getName(),$primary_key)[0];
				$field1 = $this->ORM_mapper->property_to_field($reflection->getName(),$primary_key)[1];
				$condition[$field1] = $object->{$primary_key};		//key is the field in the table and the value is the property value	
			}
			foreach($tables as $table=>$fields){
				if($this->db_exist($object,$primary_keys)==1){
					echo $table." has a single row<br/>";
					$this->adv_update($table,$fields,$condition);
				}else if($this->db_exist($object,$primary_keys)>1){
					echo $table." has multiple rows, cannot update<br/>";
				}else{
					echo $table." has no row, will insert<br/>";
					$this->insert($table,array_merge($fields,$condition));
				}
			}
		}else if($this->db_exist($object,$primary_keys)>1){
			echo 'have multiple rows<br/>';
		}else{ // if the entry does not exist; the create and update logic are the same; this requires the main table property at least 1, otherwise, the main table won't be updated
			echo 'No entry found, should create<br/>';
			$properties = $reflection->getProperties();
			$tables = array();
			foreach($properties as $property){	
				$table = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[0];
				$field = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[1];
				if($field!=null){ // Create a two layer array to store all table and fields to property information
					if(array_key_exists($table,$tables)){
						$fields = &$tables[$table];
						$fields[$field] = $property->getValue($object);	
					}else{
						$fields[$field] = $property->getValue($object);
						$tables[$table] = $fields;
					}
				}else{
					//echo $property->getName(); Show all unconvertable properties (which are not existing in database)
				}
			}
			$condition = array();
			foreach($primary_keys as $primary_key){
				$table1 = $this->ORM_mapper->property_to_field($reflection->getName(),$primary_key)[0];
				$field1 = $this->ORM_mapper->property_to_field($reflection->getName(),$primary_key)[1];
				$condition[$field1] = $object->{$primary_key};		//key is the field in the table and the value is the property value	
			}
			foreach($tables as $table=>$fields){
				if($this->db_exist($object,$primary_keys)==1){
					echo $table." has a single row<br/>";
					$this->adv_update($table,$fields,$condition);
				}else if($this->db_exist($object,$primary_keys)>1){
					echo $table." has multiple rows, cannot update<br/>";
				}else{
					echo $table." has no row, will insert<br/>";
					$this->insert($table,array_merge($fields,$condition));
				}
			}
		}
	}
	public function db_del(&$object,array $primary_keys){
		$reflection = new ReflectionClass($object);
		if($this->db_exist($object,$primary_keys)==1){ //if the entry already exist
			echo 'single entry found, should be deleted<br/>';
			$properties = $reflection->getProperties();
			$tables = array();
			foreach($properties as $property){	
				$table = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[0];
				$field = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[1];
				if($field!=null){ // Create a two layer array to store all table and fields to property information
					if(array_key_exists($table,$tables)){
						$fields = &$tables[$table];
						$fields[$field] = $property->getName();	
					}else{
						$fields[$field] = $property->getName();
						$tables[$table] = $fields;
					}
				}else{
				}
			}
			$condition = array();
			foreach($primary_keys as $primary_key){
				$table1 = $this->ORM_mapper->property_to_field($reflection->getName(),$primary_key)[0];
				$field1 = $this->ORM_mapper->property_to_field($reflection->getName(),$primary_key)[1];
				$condition[$field1] = $object->{$primary_key};		//key is the field in the table and the value is the property value	
			}
			foreach($tables as $table=>$fields){
				$this->adv_delete($table,$condition);
			}
		}else if($this->db_exist($object,$primary_keys)>1){
			echo 'have multiple rows, cannot be deleted<br/>';
		}else{ // if the entry does not exist
			echo 'No such existing entry<br/>';
		}
	}
	public function db_exist(&$object,array $primary_keys){		
		$reflection = new ReflectionClass($object);
		$temp = array();
		foreach($primary_keys as $primary_key){
			$table = $this->ORM_mapper->property_to_field($reflection->getName(),$primary_key)[0];
			$field = $this->ORM_mapper->property_to_field($reflection->getName(),$primary_key)[1];
			$temp[$field] = $object->{$primary_key};		//key is the field in the table and the value is the property value	
		}
		return $this->entry_exist($this->ORM_mapper->obj_to_table($reflection->getName()),$temp);
	}
	
	//This Bulk_load method only support for the main table property/field search, Which is safe 
	//Additionally, Unique property should be all stored into the main table..
	public function bulk_load($class_name, $key_property_name,$order_mode = 'desc',array $output_property_names = null,$limit = null, $offset = null){
		
		
		$objects = array();
		$instance = new $class_name(); //Dynamically create a new instance of the target class
		$reflection = new ReflectionClass($instance); //Create the reflection class for the instance 
		
		$main_table = $this->ORM_mapper->obj_to_table($class_name);
		$table = $this->ORM_mapper->property_to_field($reflection->getName(),$key_property_name)[0];
		if($main_table !=$table){ // Check if the key is in main table, If Not, Stop, If so, continue
			return array();
		}
		$field = $this->ORM_mapper->property_to_field($reflection->getName(),$key_property_name)[1];
		
		$properties;
		$fields = array(); //Transform all the property to database fields and temporarily stored in an array
		if($output_property_names!=null){
			$properties = $output_property_names; //Get certain properties as indicated in the parameters
			foreach($properties as $property){	
				$table = $this->ORM_mapper->property_to_field($reflection->getName(),$property)[0];
				if($main_table == $table){ // Check if the key is in main table, If Not, Stop, If so, continue
					$field = $this->ORM_mapper->property_to_field($reflection->getName(),$property)[1];
					if($field!=null){ // Create a two layer array to store all table and fields to property information
						$fields[$field] = $property;
					}else{
						//echo $property->getName(); Show all unconvertable properties (which are not existing in database)
					}
				}
			}
		}else{
			$properties = $reflection->getProperties(); //Get all the property information by reflection if there is no specification for output_properties		
			foreach($properties as $property){	
				$table = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[0];
				if($main_table == $table){ // Check if the key is in main table, If Not, Stop, If so, continue
					$field = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[1];
					if($field!=null){ // Create a two layer array to store all table and fields to property information
						$fields[$field] = $property->getName();
					}else{
						//echo $property->getName(); Show all unconvertable properties (which are not existing in database)
					}
				}
			}
		}
		
		$order; //Declare the order variable, which is the parameter for later select function
		if(strtolower($order_mode)=='asc'){
			$order = $field." ".$order_mode;
		}else if(strtolower($order_mode)=='desc'){
			$order = $field." ".$order_mode;
		}else{
			$order = $field." ".'desc'; // If the input is not valid, then ignore it and do a descending order as default
		}
		
		$this->adv_select($main_table,null,$fields,$order,$limit,$offset);
		$rows = $this->fetch_array();
		foreach($rows as $row){ //Multiple rows fetched
			$object = new $class_name(); // New Object created for each row
			foreach($row as $key=>$value){
				echo $key."-".$value.'	';
				$object->{$key_property_name} = $value; // give the value to $object->property.
			}
			echo "<br/>";
			$objects[] = $object;
		}
		return $objects;
	}
	
	//^^^自定义代码^^^=================================================================================================
}

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