<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="UTF-8">
	<title>National Mailing & Marketing</title>
	<!--link rel="stylesheet" href="css/style.css" type="text/css"-->
	<script src="js/jquery-1.11.1.min.js"></script>
    <script src="js/jquery.flexslider.js"></script>


    <style>

	/*background: url(../images/bg-navi-active.png) repeat-x;
	background-color: #007229;*/
        body {
            margin:0px;
            padding:0px;
            margin:0px;
        }

        .trans {
            transition:all 2s;
        }
/************************************FOOTER**************************************************************************/
/*************************************FOOTER********************************************************************************/
#footer {
	margin: 0px;
	padding: 0px 0;
	width: 100%;
    height:auto;
    background-color: Black;
}
.footertext {
	padding: 0px 0;
	margin: 0 auto;
	max-width: 960px;
    height: 35px;
}
.footertext p{
	color: White;
	line-height: 1em;
	width:auto;
	font-size: 0.8em;
	margin: 0 0 0px;
	text-transform: capitalize;
	display: inline;
    height: auto;
    vertical-align: middle;
}
.footertext p a {
	color: White;
	line-height: 1em;
	text-decoration: underline;
	padding: 0 0px;
}
.footertext p a:hover {
	color: #007229;
}
.footertext img 
{
    margin-left:0px;
    display:inline-block;
    max-height: 100%;
    max-width: 100%;
    vertical-align: middle;
}

/************************************FOOTER**************************************************************************/
/*************************************FOOTER********************************************************************************/

        </style>
</head>
<body onload="responsive()" onresize="responsive()">

    <h3>Hello This is just a Test</h3>
    <div class="navigation" style="position:fixed;z-index:-1;">
        <ul>
            <li> <a href="test_jerry.html">Home</a>
                <ul>
                    <li style="z-index:-1;">a1</li>
                </ul>
            </li>
            <li>b
                <ul>
                    <li style="z-index:-1;">b1</li>
                </ul>
            </li>
        </ul>
    </div>

    <!--iframe width="640" height="390" style="z-index:-2;" src="https://www.youtube.com/embed/C2n13ll0-cU?wmode=transparent" frameborder="0"  wmode="Opaque" allowfullscreen></!--iframe-->

    <div class="trans" style="position:relative;left:100px;top:100px;width:100px;height:100px;background-color:#ff6a00;display:none" onmouseover="grow(this)" onmouseout="shrink(this)"></div>
    <div class="trans" style="position:relative;left:100px;top:100px;width:100px;height:100px;background-color:#007229;display:none" onmouseover="grow(this)" onmouseout="shrink(this)"></div>
    <div style="width:50px;height:50px;background-color:#ff6a00" onclick =""></div>
    <div id="div1" class="trans" style="position:absolute;left:0px;top:0px;width:200px;height:100px;background-color:#d9d9d9; "></div>

    <div id="footer">
        <div id="footertext" class="footertext" >
            <div id ="footerdiv5" style="float:left;margin:10px">
                <div id="footerdiv1" style="float:left;margin:10px" >
	                <p>&copy; 2015 National Mailing & Marketing.</p>
                </div>
                <div>
                    <p> All rights reserved.</p>
                </div>
                <div id="footerdiv2" style="float:left;margin:10px" >
                    <p>
                        <a href="missionstatement.htm">Mission Statement</a>
                    </p>
                </div>
                <div id="footerdiv3" style="float:left;margin:10px" >
                    <p>
		                <a href="privacy.htm">Privacy Policy</a>
                    </p>
                </div>
            </div>
            <div id="footerdiv4" style="float:right;margin: 20px;border: white dashed ">
                <img style="" src="images/icon_youtube.png" alt=""/>
                <img src="images/icon_face.png" alt=""/>
                <img src="images/icon_twitter.png" alt=""/>
                <img src="images/icon_google_plus.png" alt=""/>
            </div>
        </div>
    </div>


    <script type="text/javascript">
        //When The Browser Is Initialized
        //onload(responsive_footer());


        function grow(obj) {
            obj.style.width =  200 + "px";
        }

        function shrink(obj) {
            obj.style.left = 100 + "px";
        }
        
        function responsive() { //This is the responsive function that adjust according to the window width
            responsive_footer();
        }
        function responsive_footer() {

            var footer = document.getElementById("footer");
            var footertext = document.getElementById("footertext");
            var footerdiv1 = document.getElementById("footerdiv1");
            var footerdiv2 = document.getElementById("footerdiv2");
            var footerdiv3 = document.getElementById("footerdiv3");
            var footerdiv4 = document.getElementById("footerdiv4");
            var footerdiv5 = document.getElementById("footerdiv5");


            footerdiv5.style.width = (getWidth(footertext) - getOuterWidth(footerdiv4) - getExtraWidth(footerdiv5)) + "px";
            footertext.style.width = window.innerWidth;
            footertext.style.height = getChildrenOuterHeight(footertext) + "px";

            if (!compareChildrenWidthToParentWidth(footertext)) {//If the function returns true, it means that all child Elements can be aligned in a line in the parent container, else it means we need to either adjust child elements to keep in a line or generate new line
               
                document.getElementById("div1").innerHTML = "inside\r\nfootertext-width:" + getWidth(footertext) + "\r\nfooterdiv4-width:" + getOuterWidth(footerdiv4) + "\r\nfooterdiv5-width:" + getOuterWidth(footerdiv5);
            } else {
                document.getElementById("div1").innerHTML = "outside\r\nfootertext-width:" + getWidth(footertext) + "\r\nfooterdiv4-width:" + getOuterWidth(footerdiv4) + "\r\nfooterdiv5-width:" + getOuterWidth(footerdiv5);

            }



        }



        function getWidth(object){
            return parseInt(getComputedStyle(object).width);
        }
        function compareChildrenWidthToParentWidth(object) {
            return parseInt(getComputedStyle(object).width) >= getChildrenOuterWidth(object);
        }
        function getChildrenOuterWidth(object) {

            //alert(getOuterWidth(object) + "==?" + object.getBoundingClientRect().width + "==?" + getComputedStyle(object).width);

                    var totalChildrenWidth = 0;
                    for (i = 0; i < object.childNodes.length; i++) {
                        var child = object.childNodes[i];
                        if (child.nodeName == "DIV") {
                            totalChildrenWidth += getOuterWidth(child);
                        }
                    }
                    return totalChildrenWidth;
                    //alert(totalChildrenWidth + "==?" + getComputedStyle(child).width);
        }
        function getOuterWidth(object) {
            return parseInt(getComputedStyle(object).width)+parseInt(getComputedStyle(object).marginLeft) + parseInt(getComputedStyle(object).marginRight) + parseInt(getComputedStyle(object).paddingLeft) + parseInt(getComputedStyle(object).paddingRight) + parseInt(getComputedStyle(object).borderLeftWidth) + parseInt(getComputedStyle(object).borderRightWidth);
        }
        function getExtraWidth(object) {
            return parseInt(getComputedStyle(object).marginLeft) + parseInt(getComputedStyle(object).marginRight) + parseInt(getComputedStyle(object).paddingLeft) + parseInt(getComputedStyle(object).paddingRight) + parseInt(getComputedStyle(object).borderLeftWidth) + parseInt(getComputedStyle(object).borderRightWidth);
        }

        function getChildrenOuterHeight(object) {
            //This function is to get the Highest Child, That is all, No need to compute the total height of children
            var maxHeight = 0;
            for (i = 0; i < object.childNodes.length; i++) {
                var child = object.childNodes[i];
                if (child.nodeName == "DIV") {
                    var childHeight = getOuterHeight(child);
                    if(maxHeight<childHeight){
                        maxHeight = childHeight;
                    }
                }
            }
            //alert(maxHeight);
            return maxHeight;
        }
        function getOuterHeight(object) {
            return parseInt(getComputedStyle(object).height)+parseInt(getComputedStyle(object).marginTop) + parseInt(getComputedStyle(object).marginBottom) + parseInt(getComputedStyle(object).paddingTop) + parseInt(getComputedStyle(object).paddingBottom) + parseInt(getComputedStyle(object).borderTopWidth) + parseInt(getComputedStyle(object).borderBottomWidth);
        }
        function getExtraHeight(object) {
            return parseInt(getComputedStyle(object).marginTop) + parseInt(getComputedStyle(object).marginBottom) + parseInt(getComputedStyle(object).paddingTop) + parseInt(getComputedStyle(object).paddingBottom) + parseInt(getComputedStyle(object).borderTopWidth) + parseInt(getComputedStyle(object).borderBottomWidth);
        }

        function test(object) {
            return object.style.borderWidth;
        }

    </script>

</body>
</html>
