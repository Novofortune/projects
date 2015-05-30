<?php

interface HtmlParseInterface{

	function __construct($html_file);
	function find_tags($start_mark,$end_mark);
	function pair_tags();
	function find_parents();
	function construct_html_nodes();
	function assign_depth($node,$depth);

	function find_all($needle);
	function get_property($property_string);
	function ExtractFileString($file);
	function WriteToFile($str,$file);


//--------------------------------------------Above Code Implemented-------------------------------------
//--------------------------------------------Below Code Not Implemented---------------------------------

	function write($start,$end,$string);
	function create_html_node($parent,$type,$if_single,$property,$innerhtml);
	function edit_html_node($node,$type,$property,$innerhtml);
	function delete_html_node($node);
} 

interface tagInterface{
	function get_type();
	function get_property_string();
}

interface pairInterface{
	function __construct();

}
interface htmlnode{

	function __construct();
	function getKin($node_li,$min_depth,$max_depth);
	function getChildren($node_li,$child_list,$min_depth,$max_depth);
	function getParents($node_li,$min_depth,$max_depth);


}
?>