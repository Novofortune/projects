<?php
include 'class.MySQLAdapterException.php';
class MySQLAdapter implements DataBaseAdapterInterface
{
    protected $_config = array();
    protected $_link;
    protected $_result;
    protected static $_instance;
    
    /**
     * Get the Singleton instance of the class
     */
    public static function getInstance(array $config = array())
    {
        if (self::$_instance === null) {
            self::$_instance = new self($config);
        }
        return self::$_instance;
    }
    
    /**
     * Class constructor
     */ 
    public function __construct(array $config)
    {
        if (count($config) !== 4) {
            throw new MySQLAdapterException('Invalid number of connection parameters.');   
        }
        $this->_config = $config;
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
                throw new MySQLAdapterException('Error connecting to MySQL : ' . mysqli_connect_error());
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
            throw new MySQLAdapterException('The specified query is not valid.');   
        }
        // lazy connect to MySQL
        $this->connect();
        if (!$this->_result = mysqli_query($this->_link, $query)) {
            throw new MySQLAdapterException('Error executing the specified query ' . $query . mysqli_error($this->_link));
        }
    }
    
    /**
     * Perform a SELECT statement
     */ 
    public function select($table, $where = '', $fields = '*', $order = '', $limit = null, $offset = null)
    {
        $query = 'SELECT ' . $fields . ' FROM ' . $table
               . (($where) ? ' WHERE ' . $where : '')
               . (($limit) ? ' LIMIT ' . $limit : '')
               . (($offset && $limit) ? ' OFFSET ' . $offset : '')
               . (($order) ? ' ORDER BY ' . $order : '');
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
}
?>