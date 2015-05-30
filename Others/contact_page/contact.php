<!DOCTYPE html/>


<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<meta charset="utf-8">
<style type="text/css">
.maintab{border:#000000 solid 1px; width:180px; }
</style>
<script src="http://code.jquery.com/jquery-1.11.0.min.js"></script>
<style>
table{ border:solid 1px}
tr{ border:solid 1px}
td{ border:solid 1px}
#subject {width:490px; height:50px}
#content {width:490px;height:300px}
</style>
</head>

<body>
<table>
	<tr>
		<td style="width:100px">
			<p>Subject</p>
		</td>
		<td style="width:500px;height:50px">
			<textarea id="subject">input your subject</textarea>
		</td>
	</tr>
	<tr>
		<td style="width:100px">
			<p>Content</p>
		</td>
		<td style="width:500px;height:300px">
			<textarea id="content">input your Content</textarea>
		</td>
	</tr>
	<tr>
		<td style="width:100px">
			<p></p>
		</td>
		<td style="width:500px;">
			<button style="float:right;" onclick="submit()">Submit</button>
		</td>
	</tr>
</table>


<script type="text/javascript">

//submit();

function submit(){
		$.ajax({
                                      type: "POST",
                                      url: "stmp.php",
                                      data: {"action":"send_mail","subject":document.getElementById("subject").value,"content":document.getElementById("content").value},
                                      success: function (msg) { alert(msg); },
                                      failure: function (msg) {  },
                                      error: function (msg) {  }
                                 });
}
</script>
</body>
</html>