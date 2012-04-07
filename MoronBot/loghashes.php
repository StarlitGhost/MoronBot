<?php
	$logfilesarr = array();
	foreach(glob('/opt/moronbot/logs/irc.desertbus.org/*.txt') as $textfile) {
		$salt = "mooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo";
		$file = array();
		$file['path'] =	$textfile;
		$file['hash'] = str_replace('/','_',str_replace('+','-',substr(base64_encode(hash("sha256",$textfile.$salt,true)),0,8)));
		$logfilesarr[pathinfo($textfile,PATHINFO_FILENAME)] = $file;
	}
?>
