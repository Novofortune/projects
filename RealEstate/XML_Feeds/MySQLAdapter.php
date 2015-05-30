<?php 
/*This code has the following functions: //parameters omitted
*	Basic SQL operations: 
	connect();
	query(); // primitive functions to perform any SQL query
	insert();update();delete();select(); //CRUD using string parameters
	quoteValue(); // Preprocessing string for SQL operation
	fetch(); // returning row one by one aftering executing select query
	getInsertId(); // Get the auto increment id after executing insert query
	countRows(); // get the number of rows after executing select query
	getAffectedRows();
	
	Advanced SQL operations:
	adv_delete();adv_select();adv_update();entry_exist() // using array as parameters rather than string 
	fetch_array(); // get an array of rows rather than getting row one by one
	
	Data Access Object Related Operations:
	db_exist($object,$primary_property_name_value_pair); //check existence of the entity that reflects the object
	db_load($object,$primary_property_name_value_pair); // load data from database to object
	db_save($object,$primary_property_name_value_pair); // save the object data back to database, applying changes, require loading the object from db first
	db_del($object,$primary_property_name_value_pair); // delete all related information of the object in database
	db_create($object); // create new entries related to an object without redundancy checking
	db_import($object, $component_keys) // create new entries related to an object with redundancy checking using a group of keys
	get_primary_keys($class_name,$component_keys); // use a range of keys to get the primary_key value(s), returning an array
	bulk_load($class_name,$key_property_name,$order_mode = 'desc',array $output_property_names = null,$limit = null, $offset = null); // This method can return object array of specified class with partial information from database main table in a specified order and number
*	
*	Note:	*
	All other functions are part of the functions above
	Any further functions can be added to this class to deal with more use cases
*/
require 'ORMMapper.php';
require 'FileOperation.php';

class MySQLAdapter 
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
			echo 'Invalid number of connection parameters.';
            //throw new MySQLAdapterException('Invalid number of connection parameters.');   
        }
        $this->_config = $config;
		$this->ORM_mapper = $orm_mapper;
                $this->connect();
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
		//echo $query;
        
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
			$value="''";
           // $value = 'NULL'; //Give the value NUll or "NONE"
        }
        else if (!is_numeric($value)) {
            //echo $this->_link;
            $value = "'" . mysqli_real_escape_string($this->_link, $value) . "'";
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
			$condition = $condition." ".$keys[0]."=".$this->quoteValue($values[0]);
			for($i=1;$i<count($keys);$i++){
				$condition = $condition." and ".$keys[$i]."=".$this->quoteValue($values[$i]);
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
        //Note: Primary Key should always be a single numerical id
	public function db_load(&$object,array $primary_keys){ //This method require 1. the primary key has to be only one 2. no collection property
		//$main_tables; //This is to record every single main_table object created to prevent recursive creation
		$reflection = new ReflectionClass($object);
		if($this->db_exist($object,$primary_keys)==1){ //if the entry already exist
			//echo 'single entry found<br/>';
			$properties = $reflection->getProperties();
			$tables = array();
			foreach($properties as $property){
			//If the property is Value...
			//If the property is Value...
			//If the property is Value...		
			//If the property is Value...
				if(count($this->ORM_mapper->property_to_field($reflection->getName(),$property->getName()))==2){
					$table = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[0];
					$field = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[1];
					if($field!=null){ // Create a two layer array to store all table and fields to property information
						if(array_key_exists($table,$tables)){
							$fields = &$tables[$table];
							$fields[$field] = $property->getName();	
							//echo $table." ++ ".$field.'<br/>';
						}else{
							$fields1 = array();
							$fields1[$field] = $property->getName();
							$tables[$table] = $fields1;
							//echo $table." ** ".$field.'<br/>';
						}
					}else{
					//echo $property->getName(); Show all unconvertable properties (which are not existing in database)
					}
				}
			//If the property is an array...
			//If the property is an array...
			//If the property is an array...
			//If the property is an array...
				
				if(count($this->ORM_mapper->property_to_field($reflection->getName(),$property->getName()))==3){
					$class_name = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[0];//Mapping to Class Name
					$new_primary_key = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[1];//Mapping to New Class Primary Key
					//echo $property->getName()."<br/>";
					$object->{$property->getName()} = $this->multiple_load($class_name,$primary_keys,array($new_primary_key));
				}
				
			}
			
			$condition = array();
			foreach($primary_keys as $primary_key=>$value){
				$table2 = $this->ORM_mapper->property_to_field($reflection->getName(),$primary_key)[0];
				$field2 = $this->ORM_mapper->property_to_field($reflection->getName(),$primary_key)[1];
				$condition[$field2] = $value;		//key is the field in the table and the value is the property value	
			}
			
			foreach($tables as $table=>$fields3){
				if($table==$this->ORM_mapper->obj_to_table($reflection->getName())[0]) // Check if is the main table
				$this->adv_select($table,$condition,array_keys($fields3));
				$rows = $this->fetch_array();
				foreach($rows as $row){ //The number of rows should always be 1
				
					foreach($row as $key=>$value){
						//echo $fields3[$key]."<br/>";
						$object->{$fields3[$key]} = stripslashes($value); // give the value to $object->property.
					}
				}
			}
		}else if($this->db_exist($object,$primary_keys)>1){
			//echo 'have multiple rows<br/>';
		}else{ // if the entry does not exist
			//echo 'No such existing entry<br/>';
		}
	}
	public function db_save($object,array $primary_keys){//Requiring Object having primary key information.. This happens when the obj is fetched freshly from db
		$reflection = new ReflectionClass($object);
		if($this->db_exist($object,$primary_keys)==1){ //if the entry already exist$properties = $reflection->getProperties();
			//echo 'single entry found<br/>';
			$properties = $reflection->getProperties();
			$tables = array();
			foreach($properties as $property){	
				if(!array_key_exists($property->getName(),$primary_keys)){
			//echo $property->getName()."<br/>";
				//??If the property is an value...
				//??If the property is an value...
				//??If the property is an value...
				//??If the property is an value...
				if(count($this->ORM_mapper->property_to_field($reflection->getName(),$property->getName()))==2){
					
					$table = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[0];
					$field = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[1];
					if($field!=null){ // Create a two layer array to store all table and fields to property information
						if(array_key_exists($table,$tables)){
							$fields = &$tables[$table];
							$fields[$field] = ($property->getValue($object));	
						}else{
							$fields1[$field] = ($property->getValue($object));
							$tables[$table] = $fields1;
						}
						
					}
				}
				//??If the property is an array...
				//??If the property is an array...
				//??If the property is an array...
				//??If the property is an array...
				if(count($this->ORM_mapper->property_to_field($reflection->getName(),$property->getName()))==3){
					$class_name = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[0];//Mapping to Class Name
					$new_primary_key = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[1];//Mapping to New Class Primary Key
					//echo $property->getName()."<br/>";
					foreach($object->{$property->getName()} as $child){
						//echo $new_primary_key."--".$child->{$new_primary_key};
						$this->db_save($child,array($new_primary_key=>$child->{$new_primary_key}));
					}
					//$object->{$property->getName()} = $this->multiple_save($class_name,$primary_keys,array($new_primary_key));
				}
			}
			}
			$condition = array();
			foreach($primary_keys as $primary_key=>$value){
				$table1 = $this->ORM_mapper->property_to_field($reflection->getName(),$primary_key)[0];
				$field1 = $this->ORM_mapper->property_to_field($reflection->getName(),$primary_key)[1];
				$condition[$field1] = $value;		//key is the field in the table and the value is the property value	
			}
			foreach($tables as $table=>$fields2){
				//echo $table."--".$this->ORM_mapper->obj_to_table($reflection->getName());
				if($table==$this->ORM_mapper->obj_to_table($reflection->getName())[0]){ //Only If the table is the main table
				//echo "good";
				if($this->db_exist($object,$primary_keys)==1){
					//echo $table." has a single row<br/>";
					//print_r($fields2);
					$this->adv_update($table,$fields2,$condition);
				}else if($this->db_exist($object,$primary_keys)>1){
					//echo $table." has multiple rows, cannot update<br/>";
				}else{
					//echo $table." has no row, will insert<br/>";
					//$this->insert($table,array_merge($fields2,$condition));
				}
				}
			}
		}else if($this->db_exist($object,$primary_keys)>1){
			//echo 'have multiple rows<br/>';
		}else{ // if the entry does not exist; the create and update logic are the same; this requires the main table property at least 1, otherwise, the main table won't be updated
			//echo 'No entry found, should create<br/>';
			$properties = $reflection->getProperties();
			$tables = array();
			foreach($properties as $property){	
			if(!array_key_exists($property->getName(),$primary_keys)){
			
				//??If the property is an value...
				//??If the property is an value...
				//??If the property is an value...
				//??If the property is an value...
				if(count($this->ORM_mapper->property_to_field($reflection->getName(),$property->getName()))==2){
					$table = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[0];
					$field = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[1];
					if($field!=null){ // Create a two layer array to store all table and fields to property information
						if(array_key_exists($table,$tables)){
							$fields = &$tables[$table];
							$fields[$field] = ($property->getValue($object));	
						}else{
							$fields1[$field] = ($property->getValue($object));
							$tables[$table] = $fields1;
						}
					}else{
						//echo $property->getName(); Show all unconvertable properties (which are not existing in database)
					}
				}
				//??If the property is an array...
				//??If the property is an array...
				//??If the property is an array...
				//??If the property is an array...
				if(count($this->ORM_mapper->property_to_field($reflection->getName(),$property->getName()))==3){
					$class_name = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[0];//Mapping to Class Name
					$new_primary_key = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[1];//Mapping to New Class Primary Key
					//echo $property->getName()."<br/>";
					foreach($object->{$property->getName()} as $child){
						
						$this->db_save($child,array($new_primary_key=>$child->{$new_primary_key}));
					}
					//$object->{$property->getName()} = $this->multiple_save($class_name,$primary_keys,array($new_primary_key));
				}
			}
			$condition = array();
			foreach($primary_keys as $primary_key=>$value){
				$table1 = $this->ORM_mapper->property_to_field($reflection->getName(),$primary_key)[0];
				$field1 = $this->ORM_mapper->property_to_field($reflection->getName(),$primary_key)[1];
				$condition[$field1] = $value;		//key is the field in the table and the value is the property value	
			}
			foreach($tables as $table=>$fields2){
				if($table==$this->ORM_mapper->obj_to_table($reflection->getName())[0]){ //Only If the table is the main table
				if($this->db_exist($object,$primary_keys)==1){
					//echo $table." has a single row<br/>";
					//$this->adv_update($table,$fields2,$condition);
				}else if($this->db_exist($object,$primary_keys)>1){
					//echo $table." has multiple rows, cannot update<br/>";
				}else{
					//echo $table." has no row, will insert<br/>";
					$this->insert($table,array_merge($fields2,$condition));
					}
				}
			}
			}
		}
	}
	public function db_del(&$object,array $primary_keys){ //Besides deleting the main table entry, we also need to erase any related tables
		$reflection = new ReflectionClass($object);
		if($this->db_exist($object,$primary_keys)==1){ //if the entry already exist
			//echo 'single entry found, should be deleted<br/>';
			$properties = $reflection->getProperties();
			$tables = array();
			foreach($properties as $property){	
				if(count($this->ORM_mapper->property_to_field($reflection->getName(),$property->getName()))==2){
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
				//If the property is an array
				if(count($this->ORM_mapper->property_to_field($reflection->getName(),$property->getName()))==3){
					$class_name = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[0];//Mapping to Class Name
					$new_primary_key = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[1];//Mapping to New Class Primary Key
					//echo $property->getName()."<br/>";
					foreach($object->{$property->getName()} as $child){
						//echo $new_primary_key."--".$child->{$new_primary_key};
						$this->db_del($child,array($new_primary_key=>$child->{$new_primary_key}));
						
					}
					//$object->{$property->getName()} = $this->multiple_save($class_name,$primary_keys,array($new_primary_key));
				}
			}
			$condition = array();
			foreach($primary_keys as $primary_key=>$value){
				$table1 = $this->ORM_mapper->property_to_field($reflection->getName(),$primary_key)[0];
				$field1 = $this->ORM_mapper->property_to_field($reflection->getName(),$primary_key)[1];
				$condition[$field1] = $value;		//key is the field in the table and the value is the property value	
			}
			foreach($tables as $table=>$fields){
				if($table ==$this->ORM_mapper->obj_to_table($reflection->getName())[0]) // only delete main table
				$this->adv_delete($table,$condition);
			}
		}else if($this->db_exist($object,$primary_keys)>1){
			//echo 'have multiple rows, cannot be deleted<br/>';
		}else{ // if the entry does not exist
			//echo 'No such existing entry<br/>';
		}
	}
	public function db_exist(&$object,array $primary_keys){	//Return not only bool, but also return number of entries
		$reflection = new ReflectionClass($object);
		$temp = array();
		foreach($primary_keys as $primary_key=>$value){
			//echo "Checking Object Existing Where ".$primary_key."==".$value."<br/>";
			$table = $this->ORM_mapper->property_to_field($reflection->getName(),$primary_key)[0];
			$field = $this->ORM_mapper->property_to_field($reflection->getName(),$primary_key)[1];
			//echo "Checking Entry Existing Where ".$table."---".$field."==".$value."<br/>";
			$temp[$field] = $value;		//key is the field in the table and the value is the property value	
		}
		return $this->entry_exist($this->ORM_mapper->obj_to_table($reflection->getName())[0],$temp);
	}
	public function db_create(&$object,array $parent_foreign_key = array()){ // Ignore ID properties to create only values and RETURN NEW IDs to the object itself, Please remember to collect the id information
		
		$reflection = new ReflectionClass($object);
		$maintable_primary = $this->ORM_mapper->obj_to_table($reflection->getName());
		$maintable = $maintable_primary[0];
		$primary_property = $maintable_primary[1];
		$primary_key_field = $this->ORM_mapper->property_to_field($reflection->getName(),$primary_property)[1]; //This part is tricky
		//First Get Main table and primary key, then ignore them during the operation
		
			$properties = $reflection->getProperties();
			$tables = array();
			foreach($properties as $property){	
				if($property->getName()!=$primary_property){ // Check if the property is the primary key, if so ignore it.. and continue with other properties
					if(!array_key_exists($property->getName(),$parent_foreign_key)){//Check if the property is the foreign key, if so ignore it 
					//??If the property is an value...
					$table_field_arr = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName());
						if(count($table_field_arr)==2){
							$table = $table_field_arr[0];
							$field = $table_field_arr[1];
							if($field!=null){ // Create a two layer array to store all table and fields to property information
								if(array_key_exists($table,$tables)){
									$fields = &$tables[$table];
									$fields[$field] = ($property->getValue($object));	
								}else{
									$fields1[$field] = ($property->getValue($object));
									$tables[$table] = $fields1;
								}
							}
						}
					}else if(array_key_exists($property->getName(),$parent_foreign_key)){
						$table_field_arr = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName());
						if(count($table_field_arr)==2){
							$table = $table_field_arr[0];
							$field = $table_field_arr[1];
							if($field!=null){ // Create a two layer array to store all table and fields to property information
								if(array_key_exists($table,$tables)){
									$fields = &$tables[$table];
									$fields[$field] = ($parent_foreign_key[$property->getName()]);	
								}else{
									$fields1[$field] = ($parent_foreign_key[$property->getName()]);
									$tables[$table] = $fields1;
								}
							}
						}
					}
				}
			}
			
			
		foreach($tables as $table=>$fields2){
				//if($this->db_exist($object,array($primary_property=>$object->{$primary_property}))==1){
				//}else if($this->db_exist($object,array($primary_property=>$object->{$primary_property}))>1){
				//}else{ //If there is no existing entry, then create a new entry 
				//print_r($fields2);
					$index = $this->insert($table,$fields2);
					//echo $index;
					$object->{$primary_property} = $index;// Give the primary key index value to the According Property
					if($parent_foreign_key!=null){
						foreach($parent_foreign_key as $key=>$value){
							$object->{$key} = $value;
						}
					}
					//return $index; //Return the primary key 
				//}
		}
		foreach($properties as $property){	
			
			//??If the property is an array...
				if(count($table_field_arr)==3){
					$class_name = $table_field_arr[0];//Mapping to Class Name
					$new_primary_key = $table_field_arr[1];//Mapping to New Class Primary Key
					//echo $property->getName()."<br/>";
					if(is_array($object->{$property->getName()})){ // Check if the array property is initialized
						foreach($object->{$property->getName()} as $child){
							//echo $new_primary_key."--".$child->{$new_primary_key};
							//echo $primary_property."=>".$object->{$primary_property};
							$this->db_create($child,array($primary_property=>$object->{$primary_property}));
							
						}
					}
					//$object->{$property->getName()} = $this->multiple_save($class_name,$primary_keys,array($new_primary_key));
				}
		}
	}
        public function db_create_with_fixed_id(&$object,$id,array $parent_foreign_key = array()){
            
		$reflection = new ReflectionClass($object);
		$maintable_primary = $this->ORM_mapper->obj_to_table($reflection->getName());
		$maintable = $maintable_primary[0];
		$primary_property = $maintable_primary[1];
		$primary_key_field = $this->ORM_mapper->property_to_field($reflection->getName(),$primary_property)[1]; //This part is tricky
		
                $object->{$primary_property} = $id; //give the fixed value 
                //First Get Main table and primary key, then ignore them during the operation
		
			$properties = $reflection->getProperties();
			$tables = array();
			foreach($properties as $property){	
					if(!array_key_exists($property->getName(),$parent_foreign_key)){//Check if the property is the foreign key, if so ignore it 
					//??If the property is an value...
					$table_field_arr = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName());
						if(count($table_field_arr)==2){
							$table = $table_field_arr[0];
							$field = $table_field_arr[1];
                                                        //if($field=="status") echo $property->getValue($object)." <br/>";
							if($field!=null){ // Create a two layer array to store all table and fields to property information
								if(array_key_exists($table,$tables)){
									$fields = &$tables[$table];
									$fields[$field] = ($property->getValue($object));	
								}else{
									$fields1[$field] = ($property->getValue($object));
									$tables[$table] = $fields1;
								}
							}
						}
					}
                                        else if(array_key_exists($property->getName(),$parent_foreign_key)){
						$table_field_arr = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName());
						if(count($table_field_arr)==2){
							$table = $table_field_arr[0];
							$field = $table_field_arr[1];
							if($field!=null){ // Create a two layer array to store all table and fields to property information
								if(array_key_exists($table,$tables)){
									$fields = &$tables[$table];
									$fields[$field] = ($parent_foreign_key[$property->getName()]);	
								}else{
									$fields1[$field] = ($parent_foreign_key[$property->getName()]);
									$tables[$table] = $fields1;
								}
							}
						}
					}
				
			}
			
			
		foreach($tables as $table=>$fields2){
				//if($this->db_exist($object,array($primary_property=>$object->{$primary_property}))==1){
				//}else if($this->db_exist($object,array($primary_property=>$object->{$primary_property}))>1){
				//}else{ //If there is no existing entry, then create a new entry 
				//print_r($fields2);
					$index = $this->insert($table,$fields2);
					//echo $index;
					$object->{$primary_property} = $index;// Give the primary key index value to the According Property
					if($parent_foreign_key!=null){
						foreach($parent_foreign_key as $key=>$value){
							$object->{$key} = $value;
						}
					}
					//return $index; //Return the primary key 
				//}
		}
		foreach($properties as $property){	
			
			//??If the property is an array...
				if(count($table_field_arr)==3){
					$class_name = $table_field_arr[0];//Mapping to Class Name
					$new_primary_key = $table_field_arr[1];//Mapping to New Class Primary Key
					//echo $property->getName()."<br/>";
					if(is_array($object->{$property->getName()})){ // Check if the array property is initialized
						foreach($object->{$property->getName()} as $child){
							//echo $new_primary_key."--".$child->{$new_primary_key};
							//echo $primary_property."=>".$object->{$primary_property};
							$this->db_create($child,array($primary_property=>$object->{$primary_property}));
							
						}
					}
					//$object->{$property->getName()} = $this->multiple_save($class_name,$primary_keys,array($new_primary_key));
				}
		}
        }
                
        private function component_key_check(  &$object,array $component_keys, array &$primary_keys){ // Only Load Primary Keys
            
                $reflection = new ReflectionClass($object);
                $maintable = $this->ORM_mapper->obj_to_table($reflection->getName())[0];
                $primary_key_property = $this->ORM_mapper->obj_to_table($reflection->getName())[1];
                $temp = array();
                
		foreach($component_keys as $component_key=>$value){
			//echo "Checking Object Existing Where ".$component_key."==".$value."<br/>";
			$field = $this->ORM_mapper->property_to_field($reflection->getName(),$component_key)[1];
			//echo "Checking Entry Existing Where ".$maintable."---".$field."==".$value."<br/>";
			$temp[$field] = $value;		//key is the field in the table and the value is the property value	
		}
                //print_r($temp);
                $this-> adv_select($maintable,$temp);
                $count=$this-> countRows();
		if($count==0){
			return false;
		}
                else if($count==1){
                    $rows = $this->fetch_array();
                    //print_r($rows);
                    foreach($rows as $row){
                        foreach($row as $key=>$value){
                           // echo $this->ORM_mapper->property_to_field($reflection->getName(),$primary_key_property)[1];
                            if($key==$this->ORM_mapper->property_to_field($reflection->getName(),$primary_key_property)[1]){
                                
                             $object->{$primary_key_property} = $value;
                             $primary_keys[$primary_key_property] = $value;
                            // echo $primary_key_property." ".$value;
                            }
                        }
                    }
                    //echo count($primary_keys);
                    return $count;
                }else{
                    return $count;
                }
        }//not finished...
        public function get_primary_keys($class_name,array $component_keys){
                $object = new $class_name();
                $reflection = new ReflectionClass($object);
                $maintable = $this->ORM_mapper->obj_to_table($reflection->getName())[0];
                $primary_key_property = $this->ORM_mapper->obj_to_table($reflection->getName())[1];
                $temp = array();
                $primary_keys = array();
		foreach($component_keys as $component_key=>$value){
			//echo "Checking Object Existing Where ".$component_key."==".$value."<br/>";
			$field = $this->ORM_mapper->property_to_field($reflection->getName(),$component_key)[1];
			//echo "Checking Entry Existing Where ".$maintable."---".$field."==".$value."<br/>";
			$temp[$field] = $value;		//key is the field in the table and the value is the property value	
		}
                //print_r($temp);
                $this-> adv_select($maintable,$temp);
                $count=$this-> countRows();
		if($count==0){
			return array();
		}
                else if($count>0){
                    //This is another use of the method
                    $rows = $this->fetch_array();
                    //print_r($rows);
                    foreach($rows as $row){
                        foreach($row as $key=>$value){
                           // echo $this->ORM_mapper->property_to_field($reflection->getName(),$primary_key_property)[1];
                            if($key==$this->ORM_mapper->property_to_field($reflection->getName(),$primary_key_property)[1]){
                                
                             $object->{$primary_key_property} = $value;
                             $primary_keys[] = $value;
                             //echo $primary_key_property." ".$value;
                            }
                        }
                    }
                    //echo count($primary_keys);
                    return $primary_keys;
                }else{
                    
                    return array();
                }
        }
        public function db_import(&$object,array $component_keys){ // For children objects, delete them all and recreate new 
            
            $reflection = new ReflectionClass($object);
            $primary_property = $this->ORM_mapper->obj_to_table($reflection->getName())[1];
            $ids =  $this->get_primary_keys ($reflection->getName(),$component_keys);
                    
            if(count($ids)==1){
                	//echo 'single entry found, shall kill and rebirth';
                        $temp_object = clone $object;
                        $this->db_load($temp_object,array($primary_property=>$ids[0]));
                        $this->db_del($temp_object,array($primary_property=>$temp_object->{$primary_property})); // Delete Existing Row
                        $this->db_create_with_fixed_id($object,$temp_object->{$primary_property}); // Entry Reborn
                        //echo $ids[0]."<br/>";
                        return 1;
		}
                else if(count($ids)>1){
                        //echo "Cannot know you:";
                      
                        //foreach($ids as $id){  }
                        //echo "<br/>";
			//throw new Exception("mutiple rows!");//echo 'have multiple rows<br/>'; 
                        return 2;
		}
                else{
                   /// echo "No Existing Entry, New Entry Created<br/>";
                    $this->db_create($object);
                    return 0;
                }
            
        } 
        public function db_update_backup(&$object,array $component_keys){ // For children objects, delete them all and recreate new 
            $primary_keys = array();
            $reflection = new ReflectionClass($object);
            $count = $this->component_key_check($object,$component_keys,$primary_keys);// Get the primary Key Value first
            //echo count($primary_keys);
            if($count==1){
                	//echo 'single entry found<br/>';
			$properties = $reflection->getProperties();
			$tables = array();
			foreach($properties as $property){	
				if(!array_key_exists($property->getName(),$primary_keys)){
			//echo $property->getName()."<br/>";
				//??If the property is an value...
				//??If the property is an value...
				//??If the property is an value...
				//??If the property is an value...
				if(count($this->ORM_mapper->property_to_field($reflection->getName(),$property->getName()))==2){
					
					$table = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[0];
					$field = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[1];
					if($field!=null){ // Create a two layer array to store all table and fields to property information
						if(array_key_exists($table,$tables)){
							$fields = &$tables[$table];
							$fields[$field] = ($property->getValue($object));	
						}else{
							$fields1[$field] = ($property->getValue($object));
							$tables[$table] = $fields1;
						}
						
					}
				}
				//??If the property is an array...
				//??If the property is an array...
				//??If the property is an array...
				//??If the property is an array...
				if(count($this->ORM_mapper->property_to_field($reflection->getName(),$property->getName()))==3){
					$class_name = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[0];//Mapping to Class Name
					$new_primary_key = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[1];//Mapping to New Class Primary Key
					//echo $property->getName()."<br/>";
                                                
                                                $ids = array();
                                            if(count($object ->{$property->getName()})>0){
                                                $child = RefletionClass($object ->{$property->getName()}[0]);
                                                
                                                $this->component_key_check($child,$primary_keys,$ids);
                                                //$this->db_del($child,array($new_primary_key=>$child->{$new_primary_key}));
                                            }
					foreach($object->{$property->getName()} as $child){
						//echo $new_primary_key."--".$child->{$new_primary_key};
                                                
						$this->db_create($child,array($new_primary_key=>$child->{$new_primary_key}),$primary_keys);
					}
					//$object->{$property->getName()} = $this->multiple_save($class_name,$primary_keys,array($new_primary_key));
				}
			}
			}
			$condition = array();
			foreach($primary_keys as $primary_key=>$value){
				$table1 = $this->ORM_mapper->property_to_field($reflection->getName(),$primary_key)[0];
				$field1 = $this->ORM_mapper->property_to_field($reflection->getName(),$primary_key)[1];
				$condition[$field1] = $value;		//key is the field in the table and the value is the property value	
			}
			foreach($tables as $table=>$fields2){
				//echo $table."--".$this->ORM_mapper->obj_to_table($reflection->getName());
				if($table==$this->ORM_mapper->obj_to_table($reflection->getName())[0]){ //Only If the table is the main table
                                    //echo $table."--".count($condition);
                                    $this->adv_update($table,$fields2,$condition);
				}
                        }
		}
                else if($count>1){
                    foreach($primary_keys as $v){
                        echo "Cannot know you:".$v."<br/>";
                    }
			//throw new Exception("mutiple rows!");//echo 'have multiple rows<br/>';
		}
                else{
                    $this->db_create($object);
                }
            
        }
        public function db_update1(&$object,array $primary_keys){
            $reflection = new ReflectionClass($object);
            $count = $this->db_exist($object,$primary_keys);
		if($count==1){ //if the entry already exist$properties = $reflection->getProperties();
			//echo 'single entry found<br/>';
			$properties = $reflection->getProperties();
			$tables = array();
			foreach($properties as $property){	
				if(!array_key_exists($property->getName(),$primary_keys)){
			//echo $property->getName()."<br/>";
				//??If the property is an value...
				//??If the property is an value...
				//??If the property is an value...
				//??If the property is an value...
				if(count($this->ORM_mapper->property_to_field($reflection->getName(),$property->getName()))==2){
					
					$table = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[0];
					$field = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[1];
					if($field!=null){ // Create a two layer array to store all table and fields to property information
						if(array_key_exists($table,$tables)){
							$fields = &$tables[$table];
							$fields[$field] = ($property->getValue($object));	
						}else{
							$fields1[$field] = ($property->getValue($object));
							$tables[$table] = $fields1;
						}
						
					}
				}
				//??If the property is an array...
				//??If the property is an array...
				//??If the property is an array...
				//??If the property is an array...
				if(count($this->ORM_mapper->property_to_field($reflection->getName(),$property->getName()))==3){
					$class_name = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[0];//Mapping to Class Name
					$new_primary_key = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[1];//Mapping to New Class Primary Key
					//echo $property->getName()."<br/>";
					foreach($object->{$property->getName()} as $child){
						//echo $new_primary_key."--".$child->{$new_primary_key};
                                                $this->db_del($child,array($new_primary_key=>$child->{$new_primary_key}));
						$this->db_create($child,array($new_primary_key=>$child->{$new_primary_key}));
					}
					//$object->{$property->getName()} = $this->multiple_save($class_name,$primary_keys,array($new_primary_key));
				}
			}
			}
			$condition = array();
			foreach($primary_keys as $primary_key=>$value){
				$table1 = $this->ORM_mapper->property_to_field($reflection->getName(),$primary_key)[0];
				$field1 = $this->ORM_mapper->property_to_field($reflection->getName(),$primary_key)[1];
				$condition[$field1] = $value;		//key is the field in the table and the value is the property value	
			}
			foreach($tables as $table=>$fields2){
				//echo $table."--".$this->ORM_mapper->obj_to_table($reflection->getName());
				if($table==$this->ORM_mapper->obj_to_table($reflection->getName())[0]){ //Only If the table is the main table
				
					$this->adv_update($table,$fields2,$condition);
				
				}
			}
		}
                else if($count>1){
			//echo 'have multiple rows<br/>';
		}else{ // if the entry does not exist; the create and update logic are the same; this requires the main table property at least 1, otherwise, the main table won't be updated
			//echo 'No entry found, should create<br/>';
			$this->db_create($object);
		}
        }
        public function db_update1_backup(&$object,array $primary_keys){
            $reflection = new ReflectionClass($object);
            $count = $this->db_exist($object,$primary_keys);
		if($count==1){ //if the entry already exist$properties = $reflection->getProperties();
			//echo 'single entry found<br/>';
			$properties = $reflection->getProperties();
			$tables = array();
			foreach($properties as $property){	
				if(!array_key_exists($property->getName(),$primary_keys)){
			//echo $property->getName()."<br/>";
				//??If the property is an value...
				//??If the property is an value...
				//??If the property is an value...
				//??If the property is an value...
				if(count($this->ORM_mapper->property_to_field($reflection->getName(),$property->getName()))==2){
					
					$table = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[0];
					$field = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[1];
					if($field!=null){ // Create a two layer array to store all table and fields to property information
						if(array_key_exists($table,$tables)){
							$fields = &$tables[$table];
							$fields[$field] = ($property->getValue($object));	
						}else{
							$fields1[$field] = ($property->getValue($object));
							$tables[$table] = $fields1;
						}
						
					}
				}
				//??If the property is an array...
				//??If the property is an array...
				//??If the property is an array...
				//??If the property is an array...
				if(count($this->ORM_mapper->property_to_field($reflection->getName(),$property->getName()))==3){
					$class_name = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[0];//Mapping to Class Name
					$new_primary_key = $this->ORM_mapper->property_to_field($reflection->getName(),$property->getName())[1];//Mapping to New Class Primary Key
					//echo $property->getName()."<br/>";
					foreach($object->{$property->getName()} as $child){
						//echo $new_primary_key."--".$child->{$new_primary_key};
                                                $this->db_del($child,array($new_primary_key=>$child->{$new_primary_key}));
						$this->db_create($child,array($new_primary_key=>$child->{$new_primary_key}));
					}
					//$object->{$property->getName()} = $this->multiple_save($class_name,$primary_keys,array($new_primary_key));
				}
			}
			}
			$condition = array();
			foreach($primary_keys as $primary_key=>$value){
				$table1 = $this->ORM_mapper->property_to_field($reflection->getName(),$primary_key)[0];
				$field1 = $this->ORM_mapper->property_to_field($reflection->getName(),$primary_key)[1];
				$condition[$field1] = $value;		//key is the field in the table and the value is the property value	
			}
			foreach($tables as $table=>$fields2){
				//echo $table."--".$this->ORM_mapper->obj_to_table($reflection->getName());
				if($table==$this->ORM_mapper->obj_to_table($reflection->getName())[0]){ //Only If the table is the main table
				
					$this->adv_update($table,$fields2,$condition);
				
				}
			}
		}
                else if($count>1){
			//echo 'have multiple rows<br/>';
		}else{ // if the entry does not exist; the create and update logic are the same; this requires the main table property at least 1, otherwise, the main table won't be updated
			//echo 'No entry found, should create<br/>';
			$this->db_create($object);
		}
        }
	//This Bulk_load method only support for the main table property/field search, Which is safe 
	//Additionally, Unique property should be all stored into the main table..
	public function bulk_load($class_name, $key_property_name,$order_mode = 'desc',array $output_property_names = null,$limit = null, $offset = null){
		
		
		$objects = array();
		$instance = new $class_name(); //Dynamically create a new instance of the target class
		$reflection = new ReflectionClass($instance); //Create the reflection class for the instance 
		
		$main_table = $this->ORM_mapper->obj_to_table($class_name)[0];
		$table = $this->ORM_mapper->property_to_field($reflection->getName(),$key_property_name)[0];
		if($main_table !=$table){ // Check if the key is in main table, If Not, Stop, If so, continue
			return array();
		}
		$field0 = $this->ORM_mapper->property_to_field($reflection->getName(),$key_property_name)[1];
		
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
			$order = $field0." ".$order_mode;
		}else if(strtolower($order_mode)=='desc'){
			$order = $field0." ".$order_mode;
		}else{
			$order = $field0." ".'desc'; // If the input is not valid, then ignore it and do a descending order as default
		}
		//print_r($fields);
		$this->adv_select($main_table,null,array_keys($fields),$order,$limit,$offset);
		$rows = $this->fetch_array();
		foreach($rows as $row){ //Multiple rows fetched
			$object = new $class_name(); // New Object created for each row
			foreach($row as $key=>$value){
				//echo $key."-".$value.'	';
				$object->{$fields[$key]} = $value; // give the value to $object->property.
			}
			//echo "<br/>";
			$objects[] = $object;
		}
		return $objects;
	}
	public function multiple_load($class_name,array $foreign_keys,array $new_primary_keys){ //a supportive function for db_load, can use to search multiple entries by foreign key
		$object = new $class_name();//Create an empty class Just to call db_exist function
		$objects = array();
		
		
		if($this->db_exist($object,$foreign_keys)>0){ //if the entry already exist
				
			$main_table = $this->ORM_mapper->obj_to_table($class_name)[0];
			$temp = array();
			
			foreach($foreign_keys as $foreign_key=>$value){
				//echo "Checking Object Existing Where ".$foreign_key."==".$value."<br/>";
				$table = $this->ORM_mapper->property_to_field($class_name,$foreign_key)[0];
				$field = $this->ORM_mapper->property_to_field($class_name,$foreign_key)[1];
				//echo "Checking Entry Existing Where ".$table."---".$field."==".$value."<br/>";
				$temp[$field] = $value;		//key is the field in the table and the value is the property value	
			}
			$temp1 = array();
						
			foreach($new_primary_keys as $new_primary_key){
				//echo "Checking Object Existing Where ".$foreign_key."==".$value."<br/>";
				$table = $this->ORM_mapper->property_to_field($class_name,$new_primary_key)[0];
				$field = $this->ORM_mapper->property_to_field($class_name,$new_primary_key)[1];
				//echo "Checking Entry Existing Where ".$table."---".$field."==".$value."<br/>";
				$temp1[$field] = $new_primary_key;		//key is the field in the table and the value is the property value	
			}
			
			$this->adv_select($main_table,$temp,array_keys($temp1));
			$rows = $this->fetch_array();
			foreach($rows as $row){ //Multiple rows fetched
				$object = new $class_name(); // New Object created for each row
				$temp3 = array();
				foreach($row as $key=>$value){
					//echo $key."-".$value.'	';
					//echo $temp1[$key];
					$object->{$temp1[$key]} = stripslashes($value); // give the value to $object->primary key.
					$temp3[$temp1[$key]]=stripslashes($value);
				}
				//echo "<br/>";
				
				$this->db_load($object,$temp3);
				
				$objects[] = $object;
			}
			
		}
		else {
			//echo 'No such existing entry<br/>';
		}
		return $objects;
	}
	
	//^^^自定义代码^^^=================================================================================================
}

?>