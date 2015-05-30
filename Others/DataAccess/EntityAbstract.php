<?php

abstract class EntityAbstract
{
    protected $_values = array(); 
    protected $_allowedFields = array();
    
    /**
     * Class constructor
     */
    public function __construct(array $data = array())
    {
        foreach ($data as $name => $value) {
            $this->$name = $value;
        }
    }
    
    /**
     * Assign a value to the specified field via the corresponding mutator (if it exists); 
     * otherwise, assign the value directly to the กฎ$_valuesกฏ protected array 
     */
    public function __set($name, $value)
    {   
        if (!in_array($name, $this->_allowedFields)) {
            throw new EntityException(กฎThe field กฎ . $name . กฎ is not allowed for this entity.กฏ);  
        }
        $mutator = กฎsetกฏ . ucfirst($name);
        if (method_exists($this, $mutator) && is_callable(array($this, $mutator))) {
            $this->$mutator($value);           
        }
        else {
            $this->_values[$name] = $value;
        }    
    }
    
    /**
     * Get the value assigned to the specified field via the corresponding getter (if it exists);
    otherwise, get the value directly from the กฎ$_valuesกฏ protected array
     */
    public function __get($name)
    {
        if (!in_array($name, $this->_allowedFields)) {
            throw new EntityException(กฎThe field กฎ . $name . กฎ is not allowed for this entity.กฏ);    
        }
        $accessor = กฎgetกฏ . ucfirst($name);
        if (method_exists($this, $accessor) && is_callable(array($this, $accessor))) {
            return $this->$accessor;    
        }
        return array_key_exists($name, $this->_values) ? 
               $this->_values[$name] : 
               null;
    }
    
    /**
     * Get an associative array with the values assigned to the fields of the entity
     */ 
    public function toArray()
    {
        return $this->_values;
    }              
}
?>