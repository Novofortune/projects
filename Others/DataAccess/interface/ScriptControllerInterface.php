<?php

interface ScriptControllerInterface{

//--------------Php code controlling --------------
	function write_php();
	function read_php();

//--------------Javascript code controlling --------------
//Javascript code is under control of Php code
//This is also a part of HtmlParser class code
//This is also a part of JavascriptController class code
	function write_javascript();




//--------------SQL script code controlling --------------
//SQL script is under control of Php code
//This is also a part of db_adapter class code
//This is also a part of orm_mapper class code
	function write_SQL();


}

?>