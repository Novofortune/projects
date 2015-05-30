<?php
	require 'MySQLAdapter.php';
	require 'phpmailer/PHPMailerAutoload.php';
	
	$files = FileOperation::GetFiles("propertyinfo");//print_r( $files);
	foreach($files as $file){
		
		$filedata = FileOperation::ExtractFileString($file);
		$propertyinfo = json_decode($filedata);
		
		 $time1 = strtotime($propertyinfo->mod_time);
		 $time2 = strtotime(date('Y-m-d H:i:s'));
		 $time3 = ($time2-$time1)/60/60/24;
			
		if($propertyinfo->translation_status){
			
			FileOperation::RecycleFile($file,"propertyinfo/recycle");
		}else{
			if( $time3>1){
				//echo "should send_mail	";
				//function send_mail($subject,$content,$SendTo,$host_url,$port,$user,$pass,$preferred_sender_name);
				PHPMailer::send_mail(
				(int)floor($time3)."day has expired after the translation for listing ".$propertyinfo->property->property_id." was requested ",
				(int)floor($time3)."day has expired after the translation for listing ".$propertyinfo->property->property_id." was requested ",
				"hzrtnt018@gmail.com",
				"first2move.com",
				"25",
				"support@first2move.com",
				"kelsie17",
				"First2move IT Support"
				);
			}
		}
	}
?>