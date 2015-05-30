<!DOCTYPE html/>


<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<meta charset="utf-8">
<style type="text/css">
.maintab{border:#000000 solid 1px; width:180px; }
</style>
<script src="http://code.jquery.com/jquery-1.11.0.min.js"></script>
</head>

<body>
<?php 
	include('b.php');
?>
<button onclick="clickme()">click me</button>



<script type="text/javascript" src="http://code.jquery.com/jquery-latest.js"></script>
<script type="text/javascript">
//jQuery(function($){  //以后需要学习学习这种用法。。。
//  $('button').click(function(){
 //   $name = $(this).attr('name');
 //   $('#out').empty().load('test1.php',{ name : $name });
 // });
//});
</script>






<script type="text/javascript">

clickme();

function clickme(){
		$.ajax({
                                      type: "POST",
                                      url: "b.php",
                                      data: {"action":"test"},
                                      success: function (msg) { alert(msg); },
                                      failure: function (msg) {  },
                                      error: function (msg) {  }
                                 });
}
</script>
</body>
</html>