<?php

$header = array( 
            //"POST ".$page." HTTP/1.0", 
            //"Content-type: text/xml;charset=\"utf-8\"", 
            //"Accept: text/xml", 
            //"Cache-Control: no-cache", 
            //"Pragma: no-cache", 
            //"SOAPAction: \"run\"", 
            //"Content-length: ".strlen($xml_data), 
            //"Authorization: Basic " . base64_encode($credentials) 
			
			
			"Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8",
			"Accept-Encoding:utf-8",
			"Accept-Language:en-US,en;q=0.8,und;q=0.6,zh-CN;q=0.4,zh;q=0.2",
			"Cache-Control:max-age=0",
			"Connection:keep-alive",
			"Cookie:premiumProductListingId=14815866; yellow-guid=fef40271-fbd5-4bac-a19b-0230eb73d9f0; tilesOrStrip=Strip; AMCV_8412403D53AC3D7E0A490D4C%40AdobeOrg=1766948455%7CMCMID%7C75135328805954459731450894205292375763%7CMCAAMLH-1429152850%7C8%7CMCAAMB-1429152850%7CNRX38WO0n5BH8Th-nqAG_A%7CMCAID%7C2A3AB3F8852C3242-600000C0200115E9; TS7e1ed3=0486f3a0ef785cb7f199fcb1871cf3d787ded202707ab0385525fd8e; clue=Entertainment; locationClue=Australian%20Capital%20Territory; _qst_s=4; _qsst_s=1428553205635; _ga=GA1.3.189326175.1428480558",
			"Host:www.yellowpages.com.au",
			"Referer:http://www.yellowpages.com.au/search/listings",
			"User-Agent:Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/40.0.2214.91 Safari/537.36"
			
        ); 
echo "<meta charset='UTF-8'>";
echo http_request('http://www.yellowpages.com.au/search/listings?clue=Entertainment&locationClue=Australian+Capital+Territory&lat=&lon=&selectedViewMode=list',$header);
echo "<textarea style='width:800px;height:600px;'>".http_request('http://www.yellowpages.com.au/search/listings?clue=Entertainment&locationClue=Australian+Capital+Territory&lat=&lon=&selectedViewMode=list',$header)."</textarea>";

function http_request($url,$header,$timeout=300){  
        if (!function_exists('curl_init')) {  
            throw new Exception('server not install curl');  
        }  
        $ch = curl_init();  
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);  
        curl_setopt($ch, CURLOPT_HEADER, true);  
        curl_setopt($ch, CURLOPT_URL, $url);  
        curl_setopt($ch, CURLOPT_TIMEOUT, $timeout);  
		//curl_setopt($ch, CURLOPT_PORT, $port);
		//curl_setopt($ch, CURLOPT_REFERER, "http://www.yellowpages.com.au/search/listings")
        if (!empty($header)) {  
            curl_setopt($ch, CURLOPT_HTTPHEADER, $header);  
        }  
        $data = curl_exec($ch);  
        list($header, $data) = explode("\r\n\r\n", $data);  
        $http_code = curl_getinfo($ch, CURLINFO_HTTP_CODE);  
        if ($http_code == 301 || $http_code == 302) {  
            $matches = array();  
            preg_match('/Location:(.*?)\n/', $header, $matches);  
            $url = trim(array_pop($matches));  
            curl_setopt($ch, CURLOPT_URL, $url);  
            curl_setopt($ch, CURLOPT_HEADER, false);  
            $data = curl_exec($ch);  
        }  

        if ($data == false) {  
            curl_close($ch);  
        }  
        @curl_close($ch);  
        return $data;  
}  

?>


curl_setopt($ch, CURLOPT_COOKIEJAR, 'cookie.txt '); //±£¥Êcookie
curl_setopt($ch, CURLOPT_COOKIEFILE, 'cookie.txt '); //∂¡»°cookie