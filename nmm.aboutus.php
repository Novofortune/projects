<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="UTF-8">
	<title>National Mailing & Marketing</title>
	<link rel="stylesheet" href="css/style.css" type="text/css">
	<!--load header and footer-->
	<script src="js/jquery-1.11.1.min.js"></script>
	<script type = "text/javascript">
        //Showing the Type and Version of the browser...
	    navigator.sayswho = (function () {
	        var ua = navigator.userAgent, tem,
            M = ua.match(/(opera|chrome|safari|firefox|msie|trident(?=\/))\/?\s*(\d+)/i) || [];
	        if (/trident/i.test(M[1])) {
	            tem = /\brv[ :]+(\d+)/g.exec(ua) || [];
	            return 'IE ' + (tem[1] || '');
	        }
	        if (M[1] === 'Chrome') {
	            tem = ua.match(/\bOPR\/(\d+)/);
	            if (tem != null) return 'Opera ' + tem[1];
	        }
	        M = M[2] ? [M[1], M[2]] : [navigator.appName, navigator.appVersion, '-?'];
	        if ((tem = ua.match(/version\/(\d+)/i)) != null) M.splice(1, 1, tem[1]);
	        return M.join(' ');
	    })();


	    $(function() {
	        $("#header").load("header.htm");
	        $("#footer").load("footer.htm");
	    });
        //Initialize the aboutus div width
	    $(function () {
	        resize();
	    });
	</script>
    </head>
    <body onresize="resize()" style="">
	<div id="header"></div>
        <div id="body" style="margin:14px">
            
<!----------Sestion About Us------------------------------------------------------------------------------------------------>
            <div class="section">
                <h1>About Us</h1>
                <div id="aboutustext" class="sub-section" style="min-width:480px;font-family:Arial;font-size:18px;line-height:130%">
                    I'm a paragraph. Click here to add your own text and edit me. It is easy. just Click "Edit buttion " or double click me to add your own content and make changes to the font. Feel free to drag and drop me anywhere you like on your page. I'm a great place for you to tell a story and let your users to know a little more about you.
                </div>
                <div id="aboutusline" class="sub-section" style="width:90%; text-align:right; border-bottom:4px solid #007229;">
                    <!--a class="readmore" href="#">Read more</!--a-->
                </div>
            </div>
<!----------Sestion Directors----------------------------------------------------------------------------------------------->
            <div class="section" style="min-width:480px">
                <h1>Directors</h1>
                <div class="sub-section" style="width:240px; text-align:center;" onmouseover="show_text(this) " onmouseout="hide_text(this)">
                    <img style =" width:100%;" src="images/WebsiteRefresh/Bab2_square.jpg" alt="Director's Name"/>
                    <div class="transbox_hidden" onclick="window.location = 'contactus.htm'">
                        <p style="color:black;">Name and Position</p>
                  </div>
                </div><div class="sub-section" style="width:240px; text-align:center;vertical-align:central;" onmouseover="show_text(this) " onmouseout="hide_text(this)">
                    <img style ="position:relative;top:0px;left:0px; width:100%;" src="images/WebsiteRefresh/Steve Conroy_square.jpg" alt="Director's Name" />
                    <div class="transbox_hidden" onclick="window.location = 'contactus.htm'">
                        <p style="color:black;">Name and Position</p>
                  </div>
                </div>
            </div>
<!----------Sestion General Manager----------------------------------------------------------------------------------------------->
            <div class="section" style="min-width:480px">
                <h1>General Manager</h1>
                <div class="sub-section" style="width:240px; text-align:center; margin-right:10px;" onmouseover="show_text(this) " onmouseout="hide_text(this)">
                    <img style =" width:100%;" src="images/WebsiteRefresh/Dennis Ogden_square.jpg" alt="Director's Name"/>
                    <div class="transbox_hidden" onclick="window.location = 'contactus.htm'">
                        <p style="color:black;" >Dennis Ogden<br/>General Manager</p>
                  </div>
                </div>
            </div>
<!----------Sestion Management Team----------------------------------------------------------------------------------------------->
            <div class="section" style="min-width:480px;margin-bottom:20px">
                <h1>Management Team</h1>
                <div class="sub-section" style="width:192px; text-align:center; margin-bottom:0px;" onmouseover="show_text(this) " onmouseout="hide_text(this)">
                    <img style =" width:100%;" src="images/WebsiteRefresh/Mathew Jones-Angel_square.jpg" alt="Director's Name "/>
                    <div class="transbox_hidden" onclick="window.location = 'contactus.htm'">
                        <p style="color:black;" >Name and Position</p>
                  </div>
                </div><div class="sub-section" style="width:192px; text-align:center;margin-bottom:0px;" onmouseover="show_text(this) " onmouseout="hide_text(this)">
                    <img style =" width:100%;" src="images/WebsiteRefresh/Kathryn Stefaniak_square.jpg" alt="Director's Name"/>
                    <div class="transbox_hidden" onclick="window.location = 'contactus.htm'">
                        <p style="color:black;" >Name and Position</p>
                  </div>
                </div><div class="sub-section" style="width:192px; text-align:center;margin-bottom:0px;" onmouseover="show_text(this) " onmouseout="hide_text(this)">
                    <img style =" width:100%;" src="images/WebsiteRefresh/Gail Davis_square.jpg" alt="Director's Name"/>
                    <div class="transbox_hidden"  onclick="window.location = 'contactus.htm'">
                        <p style="color:black;">Name and Position</p>
                  </div>
                </div><div class="sub-section" style="width:192px; text-align:center;margin-bottom:0px;" onmouseover="show_text(this) " onmouseout="hide_text(this)">
                    <img style =" width:100%;" src="images/WebsiteRefresh/Amy Ellis_square.jpg" alt="Director's Name"/>
                    <div class="transbox_hidden" onclick="window.location = 'contactus.htm'">
                        <p style="color:black;">Name and Position</p>
                  </div>
                </div><div class="sub-section" style="width:192px; text-align:center;margin-bottom:0px;" onmouseover="show_text(this) " onmouseout="hide_text(this)">
                    <img style =" width:100%;" src="images/WebsiteRefresh/Ali Don_square.jpg" alt="Director's Name"/>
                    <div class="transbox_hidden" onclick="window.location = 'contactus.htm'">
                        <p style="color:black;" >Name and Position<br/>Name and Position</p>
                  </div>
                </div>
            </div>
<!----------Sestion Footer----------------------------------------------------------------------------------------------->
       </div>
    <div id="footer" style="margin-top:10px"></div>
     
    <script type="text/javascript">
        function show_text(object) {

            var rect = getRect(object);
            object.childNodes[3].style.left = rect.left  + "px";
            object.childNodes[3].style.top = rect.top + rect.height -50 + "px";//rect.height * 0.2 + "px";

            object.childNodes[3].style.height = "50px";//rect.height*0.2+"px";
            object.childNodes[3].style.width = rect.width + "px";
            object.childNodes[3].className = "transbox";
        }
        function hide_text(object) {
            var rect = getRect(object);
            object.childNodes[3].style.left = rect.left + "px";
            object.childNodes[3].style.top = rect.top + rect.height - 50 + "px";//rect.height * 0.2 + "px";

            object.childNodes[3].style.height = "50px";//rect.height*0.2+"px";
            object.childNodes[3].style.width = rect.width + "px";
            object.childNodes[3].className = "transbox_hidden";
        }
        function getRect(object) {
            
            var rect = object.getBoundingClientRect();


            if (navigator.sayswho.match(/Chrome/)) {
                return {
                    top: rect.top + document.body.scrollTop - $("#body").offset().top, // This part is tricky. 
                    left: rect.left + document.body.scrollLeft - $("#body").offset().left,
                    width: rect.width,
                    height: rect.height
                };
            }
            else if (navigator.sayswho.match(/IE/)) {
                //alert(document.scrollTop);
                return {
                    top: rect.top + document.documentElement.scrollTop - $("#body").offset().top, // This part is tricky. 
                    left: rect.left + document.documentElement.scrollLeft- $("#body").offset().left, 
                    width: rect.width,
                    height: rect.height
                };
            }
            
        }
        function resizeWidth(object,width) {
            object.style.width = width + "px";
        }
        function resize() {
            if ($(window).width() >= 960) {
                resizeWidth(document.getElementById("aboutustext"), 750);
                resizeWidth(document.getElementById("aboutusline"), 930);
            } else if ($(window).width() < 960) {
                resizeWidth(document.getElementById("aboutustext"), 400);
                resizeWidth(document.getElementById("aboutusline"), 480);
            }
        }

    </script>
</body>
</html>
