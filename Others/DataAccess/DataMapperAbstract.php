(DataMapperAbstract.php)

<?php

abstract class DataMapperAbstract
{
    protected $_adapter;
    protected $_collection; 
         
    /**
     * Class constructor
     */ 
    public function __construct(DatabaseAdapterInterface $adapter, CollectionAbstract $collection)
    {
        $this->_adapter = $adapter;
        $this->_collection = $collection;
        $this->init();
    }
    
    /**
     * Initialize the data mapper here
     */
    public function init(){}
    
    /**
     * Get the instance of the database adapter
     */  
    public function getAdapter()
    {
        return $this->_adapter;
    }
    
    /**
     * Get the collection
     */ 
    public function getCollection()
    {
        return $this->_collection;
    }  
}
?>