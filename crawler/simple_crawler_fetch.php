<?php
ini_set('max_execution_time', 300);
echo "<meta charset='UTF-8'>";
$root_url = 'http://www.erindaleshoppingcentre.com.au/';
$html = file_get_contents('http://www.yellowpages.com.au:80'); 
//preg_match_all("/Canberra Ticketing/",$html,$links);
//echo count($links[0]);

//echo '<textarea style="width : 800px;height:600px;">';
//for($i=0;$i<count($links[0]);$i++){
//	echo $links[0][$i];
//	echo '\r\n';
//}
echo $html;
//echo '</textarea>';
?>
