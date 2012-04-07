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
?><html>
  <head>
    <title><?php
	if ($foundit) {
		$chan = strtok($loghashesarr[$_GET['l']]['name'],'-');
		$date = strtok("\n");
		$date = substr($date,0,4)."/".substr($date,4,2)."/".substr($date,6);
		echo "Log for ".$chan." - ".$date;
	}
	else echo "Logs";
?></title>
    <style type="text/css">
      tr:nth-child(odd)
      {
        background: #EEE;
      }
      tr:nth-child(even)
      {
        background: #DDD;
      }
      td
      {
        max-width: 900px;
      }
      .log
      {
        border-collapse: collapse;
      }
      .user
      {
        font-weight: bold;
      }
      .message .user
      {
        text-align: right;
        border-right: 1px solid #000;
        padding-right: 0.5em;
      }
      .spacer
      {
        text-align: right;
        border-right: 1px solid #000;
        padding-right: 0.5em;
      }
      .text
      {
        word-wrap: break-word;
        padding-left: 0.5em;
      }
      .other .text
      {
        color: #666;
      }
    </style>
  </head>
  <body<?php
  if (!$foundit)
  {
    echo ' style="background: url(\'logs.jpg\'); background-size:100%; background-repeat:no-repeat;"';
  }
  ?>>
<?php
    if ($foundit) 
    {
        $log = htmlspecialchars(file_get_contents($loghashesarr[$_GET['l']]['path']));
        
        $patterns = array();
        $patterns[0] = '/^(\[\d{2}:\d{2}\])\s(&lt;[^&]+&gt;)\s(.+?)$/mi';
        $patterns[1] = '/^(\[\d{2}:\d{2}\])\s\*([^\s]+)\s(.+?)\*$/mi';
        $patterns[2] = '/^(\[\d{2}:\d{2}\])\s(.+?)$/mi';
        $replacements = array();
        $replacements[0] = '<tr class="message"><td class="time">\1</td><td class="user">\2</td><td class="text">\3</td></tr>';
        $replacements[1] = '<tr class="action"><td class="time">\1</td><td class="spacer">*</td><td class="text"><span class="user">\2</span> \3</td></tr>';
        $replacements[2] = '<tr class="other"><td class="time">\1</td><td class="spacer">&nbsp;</td><td class="text">\2</td></tr>';
        $log = preg_replace($patterns, $replacements, $log);
        
        echo '<pre><table class="log">'.$log.'</table></pre>';
    }
?>
  </body>
</html>
