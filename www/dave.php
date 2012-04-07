<?php
        require_once('/opt/moronbot/loghashes.php');
	$loghashesarr = array();
	foreach ($logfilesarr as $key=>$val) {
		$entry = array();
		$entry['path'] = $val['path'];
		$entry['name'] = $key;
		$loghashesarr[$val['hash']] = $entry;
	}
	$foundit = (array_key_exists('l',$_GET) && array_key_exists($_GET['l'],$loghashesarr));
?>
<html><head><title><?php
	if ($foundit) {
		$chan = strtok($loghashesarr[$_GET['l']]['name'],'-');
		$date = strtok("\n");
		$date = substr($date,0,4)."/".substr($date,4,2)."/".substr($date,6);
		echo "Log for ".$chan." - ".$date;
	}
	else echo "Logs";
?></title></head>
<body<?php if (!$foundit) echo ' style="background: url(\'logs.jpg\'); background-size:100%; background-repeat:no-repeat;"'?>>
<?php if ($foundit) echo '<pre>'.htmlspecialchars(file_get_contents($loghashesarr[$_GET['l']]['path'])).'</pre>' ?>
</body></html>
