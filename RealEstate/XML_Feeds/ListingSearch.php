<!doctype html>
<html>
<head>
<title>First2move Listing Search</title>
<script src="http://ajax.googleapis.com/ajax/libs/jquery/1.11.2/jquery.min.js"></script>
</head>
<body>
<h3>Listing Search</h3>
<p>Please enter listing ID</p>
<textarea id = "search_text" ></textarea>
<button id = "search" onclick = "search()">Search</button>
<script type = "text/javascript">
function search(){
	var pid = document.getElementById('search_text').value;
	$.ajax({
        type: "POST",
        url: "actions.php",
        data: {"action":"goto_listing_by_id","pid":pid},
        success: function (msg) { 
        alert(msg);
        window.open(msg); 
        },
        failure: function (msg) {  },
        error: function (msg) {  }
    });
}
</script>
<?php  
//Listing Search

?>
</body>
</html>