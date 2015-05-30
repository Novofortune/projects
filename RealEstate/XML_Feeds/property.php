<?php
class property_info{
    var $property;
    var $mod_time;
    var $translation_status;
}
class property{
        public $mod_time; // Newly Added
        public $status; //Newly Added
        public $agent_id; //Newly Added
    
    
	public $property_id;
	public $street_no;
	public $street;
	public $town;
	public $state;
	public $postcode;
	
	
	public $property_descriptions = array();
	public $property_pricings = array();
	public $property_details = array();
	public $property_imgs = array();
	public $inspection_dates = array();
	public $listing_agents = array();
	
	function __construct(){
		
	}
}
class property_description{
	public $property_description_id;
	public $property_id;
	
	public $headline;
	public $description;
	
}
class property_pricing{
	public $property_pricing_id;
	public $property_id;
	
	public $sale_method;
	public $sale_price;
}
class property_detail{
	public $property_detail_id;
	public $property_id;
	
	public $type;
	public $num_bedroom;
	public $num_bathroom;
	public $num_parking;
	public $energy_rating; 
}
class inspection_date{
	public $inspection_date_id;
	public $property_id;
	
	public $value;
}
class property_img{
	public $property_img_id;
	public $property_id;
	
	public $url;
}

class listing_agent{
	public $listing_agent_id;
	public $property_id;
	
	public $first_name;
	public $last_name;
	public $telephone;
	public $email;
}
?>