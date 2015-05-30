<?php
require 'phpmailer/PHPMailerAutoload.php';

$SendTo = "hzrtnt018@gmail.com";
$url = "smtp.126.com";
$port = 25;
$user = "hzrtnt018@126.com";
$pass = "tnthzr018312";
$user_name = "Zhenrui Han";
//$subject = "PHPMailer Test Subject via smtp, basic with authentication";
if(isset($_POST['action']) && !empty($_POST['action'])) {
    $action = $_POST['action'];
    switch($action) {
        case 'send_mail' : {
			if(isset($_POST['subject'])&& !empty($_POST['subject'])){
				if(isset($_POST['content'])&& !empty($_POST['content'])){
					send_mail(
					$_POST['subject'],
					$_POST['content'],
					$SendTo,
					$url,
					$port,
					$user,
					$pass,
					$user_name
					);
				}
			}
		break;
		}
    }
}

function send_mail($subject,$content,$SendTo,$host_url,$port,$user,$pass,$user_name){
	$mail             = new PHPMailer();
	$body             = $content;
	$body             = eregi_replace("1[\]",'',$body);
	$mail->IsSMTP(); // telling the class to use SMTP
	$mail->SMTPDebug  = 2;                     	// enables SMTP debug information (for testing)
												// 1 = errors and messages
												// 2 = messages only
	$mail->SMTPAuth   = true;                  	// enable SMTP authentication
	$mail->Host       = $host_url; // sets the SMTP server
	$mail->Port       = $port;                  // set the SMTP port for the GMAIL server
	$mail->Username   = $user; // SMTP account username
	$mail->Password   = $pass;        // SMTP account password
	$mail->SetFrom($user, $user_name);
	$mail->AddReplyTo($user, $user_name);
	$mail->Subject    = $subject;
	//$mail->AltBody    = "To view the message, please use an HTML compatible email viewer!"; // optional, comment out and test
	$mail->MsgHTML($body);
	$address = $SendTo;
	$mail->AddAddress($address, "Zhenrui Han");
	//$mail->AddAttachment("images/phpmailer.gif");      // attachment
	//$mail->AddAttachment("images/phpmailer_mini.gif"); // attachment
	if(!$mail->Send()) {
		echo "Mailer Error: " . $mail->ErrorInfo;
	} else {
		echo "Message sent!";
	}
}
?>