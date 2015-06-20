function postMSG(action, area)	
{
	$.ajax({
			type: "POST",
			url: "admin/actions.php",
			data: {"action":action},
			success:function(msg) 
			{
                $(area).html(msg);
			},
			error:function(){
				$(area).html("error");
			}
		});
}