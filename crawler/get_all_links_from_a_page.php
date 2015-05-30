<?php
ini_set('max_execution_time', 300);
//$html = file_get_contents('http://www.first2move.com.au');
$html = file_get_contents('http://www.erindaleshoppingcentre.com.au/finder.php');
$links = extract_links1($html,'http://www.erindaleshoppingcentre.com.au');
$htmls = array();
$html_combine = "";
for($i=0;$i<count($links);$i++){
	try{
	$htmls[] = file_get_contents($links[$i]);
	}catch(Exception $e){ echo $e;}
	echo $links[$i].'<br/>';
}
for($i=0;$i<count($htmls);$i++){
	$html_combine = $html_combine.$htmls[$i];
}
	//echo '<textarea style="width:800px;height:300px;float:left;clear:both;">'.$html_combine.'</textarea>';

function extract_links($html){
//$html = file_get_contents('http://www.first2move.com.au'); 
$dom = new DOMDocument(); 
@$dom->loadHTML($html); 
// grab all the links on the page 
$xpath = new DOMXPath($dom); 
$hrefs = $xpath->evaluate("/html/body//a"); 

$urls = array();
for ($i = 0; $i < $hrefs->length; $i++) { 
$href = $hrefs->item($i); 
$url = $href->getAttribute('href'); 
preg_match("/http(.*)/", $url, $a);//用正则表达式来判定是否为http/https链接
if(count($a)>0){
$urls[] = $a[0];//将链接加入到输出数组$urls中
}
}

return $urls;
}


function extract_links1($html,$root_url){
//$html = file_get_contents('http://www.first2move.com.au'); 
$dom = new DOMDocument(); 
@$dom->loadHTML($html); 
// grab all the links on the page 
$xpath = new DOMXPath($dom); 
$hrefs = $xpath->evaluate("/html/body//a"); 

$urls = array();
for ($i = 0; $i < $hrefs->length; $i++) { 
$href = $hrefs->item($i); 
$url = $href->getAttribute('href'); 
preg_match("/http(.*)/", $url, $a);//用正则表达式来判定是否为http/https链接
if(count($a)>0){
$urls[] = $url;//将链接加入到输出数组$urls中
}else{
	$urls[] = $root_url.'/'.$url;
}
}

return $urls;
}
?>