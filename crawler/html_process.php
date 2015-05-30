<?php 
	$filename = "source.txt";
	$fh = fopen($filename,'r');
	$data = fread($fh,filesize($filename));
	fclose($fh);

	ini_set('max_execution_time', 300);
	ini_set('pcre.backtrack_limit', -1);

	$start = strpos($data,"cell platinum-tile-cell middle-cell");
	$end = strpos($data,"cell bottom-feedback middle-cell");
	$temp = substr($data,$start,$end-$start); //得到连续标记之间的部分
	
	$subtemps = array();
	$poss = find_all($temp,"cell in-area-cell middle-cell");
	for($i=0;$i<count($poss);$i++){
		if($i<count($poss)-1){
			$subtemp = substr($temp,$poss[$i],$poss[$i+1]-$poss[$i]);
			$subtemps[] = $subtemp;
		}else{
			$subtemp = substr($temp,$poss[$i],strlen($temp)-$poss[$i]);
			$subtemps[] = $subtemp;
		}
	}
	
	$str = "";
	$table = array();
	for($i=0;$i<count($subtemps);$i++){
		$entry = array();
		preg_match("/title='View more about this business'>(.*?)<\/a>/",$subtemps[$i],$match); 
		if(count($match)>1){
		$entry[]=$match[1];
		}else{
			$entry[] = " ";
		}
		preg_match("/<p class='listing-address(.*?)>(.*?)<\/p>/",$subtemps[$i],$match);
		if(count($match)>2){
		$entry[]=$match[2];
		}else{
			$entry[] = " ";
		}
		preg_match("/<a href='(.*?)'(.*?)class='contact contact-main contact-url/",$subtemps[$i],$match);
		if(count($match)>2){
		$entry[]=$match[1]; 
		}else{
			$entry[] = " ";
		}
		
		preg_match("/tel:(.*?)'/",$subtemps[$i],$match);
		if(count($match)>1){
		$entry[]=$match[1];
		}else{
			$entry[] = " ";
		}
		$table[]=$entry;
		echo $entry[0].$entry[1].$entry[2].$entry[3].'<br/>';
		$str=$str.str_replace(",",";",$entry[0]).','.str_replace(",",";",$entry[1]).','.str_replace(",",";",$entry[2]).','.str_replace(",",";",$entry[3])."\r\n";
	}
	
	$filename = "g1.csv";
	$fh = fopen($filename,'w');
	fwrite($fh,$str);
	fclose($fh);
	
	
	
	
	
	
	
	function find_all($data,$needle){
	$poss = array();
	$pos = 0;
	while(true){
		//echo $begin.'<br/>';
	$pos = strpos($data,$needle,$pos);
	if($pos===false){
		break;
	}else{
		$poss[]= $pos;
		$pos = $pos+1;
	}
	}
	return $poss;
	}
?>