<?php 
/*
	static functions implemented:
	ExtractFileString(); // get string from file;
	WriteToFile(); // write string to an existing file or create a new file
	ReadJson(); // Read object from a json file;
	WriteJson(); // write object to json file;
	GetFiles(); // get all files (can specify extension filter) of a specified directory
	GetFileName(); // get the filename from a file full path;
	GetFileExtension(); // get the extension of a file
	RecycleFile(); // move the file from current directory to a specified recycle_bin_path... can be renamed to MoveFile() in the future...

	functions Not Implemented:
	GetFileTree();
	
	class Not Implemented:
	FileNode;
	
	Any futher functions or classes can be added to this code for more use cases...
*/
class FileOperation{
	
	static function GetFileTree($path){
		//Not Implemented....
		// return $root_node and $nodes ($root_node can be the first element of the $nodes array)
	}
	static function RecycleFile( $file_path,$recycle_bin_path){
		
				if(file_exists($recycle_bin_path.DIRECTORY_SEPARATOR)){
					$newfilefullpath = $recycle_bin_path. DIRECTORY_SEPARATOR .FileOperation::GetFileName($file_path);
					$filedata = FileOperation::ExtractFileString($file_path);
					FileOperation::WriteToFile($filedata,$newfilefullpath);
					unlink (  $file_path );
				}else{
					mkdir($recycle_bin_path.DIRECTORY_SEPARATOR,0777,true);
					$newfilefullpath = $recycle_bin_path. DIRECTORY_SEPARATOR .FileOperation::GetFileName($file_path);
					$filedata = FileOperation::ExtractFileString($file_path);
					FileOperation::WriteToFile($filedata,$newfilefullpath);
					unlink (  $file_path );
				}
	}
    static function GetFiles($path,$extension = "*"){
		$dh = opendir($path);
		$result = array();
		while(($file = readdir($dh))!==false){
			$sub_path = $path . DIRECTORY_SEPARATOR . $file;
		
			if($file == '.' || $file == '..') { //Determine if it is empty
			continue;
			} else if(is_dir($sub_path)) {    //Determine if it is a folder
				
			} else { // Determine if it is a file
				if($extension=="*"){
					$result[] =  $sub_path; // Default Get all files
				}else{
					if($extension==FileOperation::GetExtension($sub_path)){
						$result[] =  $sub_path;
					}			
				}
			}
		}
		return $result;
	}
	static function GetFileName($fullpath){
		$sep_pos = strrchr($fullpath,DIRECTORY_SEPARATOR);
		return $FileName = substr($sep_pos,1,strlen($sep_pos));
	}
	static function GetExtension($file){
		
		$dot_pos = strrchr($file,".");
		return $ext = substr($dot_pos,1,strlen($dot_pos));
	
	}
	
    static function ReadJson($json_file){
		$json_data = FileOperation::ExtractFileString($json_file);
		$object = json_decode($json_data);
		return  $object;
    }
    
    static function WriteJson($object,$json_file){
        $json_string = json_encode($object);
		FileOperation::WriteToFile($json_string,$json_file);
    }
    
    static function  ExtractFileString($path){
		$fh = fopen($path, "r") or die("Unable to open file!");
		$data = fread($fh,filesize($path));
		fclose($fh);
		return $data;
	}
    static function WriteToFile($str,$path){
		$fh = fopen($path, "w") or die("Unable to write to file");
		fwrite($fh,$str);
		fclose($fh);
    }
}

class FileNode{
	//Not Implemented...
}
?>