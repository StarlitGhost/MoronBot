<?php
if ($argc < 3) die("Not enough args");
$nick = str_replace("*",".*",$argv[1]);
$chan = str_replace("\#","#",str_replace("#","\#",$argv[2]));
if ($argc < 4 || $argv[3] == "seen") {
	$found = shell_exec('grep -i ^[^\<]*\<' . $nick . '\> /opt/moronbot/logs/irc.desertbus.org/' . $chan . '*  | tail -n 1');
}
if ($argv[3] == "sawed") {
	$found = shell_exec('grep -i ^[^\<]*\<' . $nick . '\> /opt/moronbot/logs/irc.desertbus.org/' . $chan . '*  | grep -v .*'.gmdate('Ymd').'.* | tail -n 1');
}
else if ($argv[3] == "mention") {
	$found = shell_exec('grep -i \<.*\>[^\|]*' . $nick . '.* /opt/moronbot/logs/irc.desertbus.org/' . $chan . '* | grep -v \|lastsaid | grep -v \|lastmessage | grep -v ^[^\<]*\<MoronBot\> | tail -n 1');
}
else if ($argv[3] == "mentionnottoday") {
	$found = shell_exec('grep -i \<.*\>[^\|]*' . $nick . '.* /opt/moronbot/logs/irc.desertbus.org/' . $chan . '* | grep -v \|lastsaid | grep -v \|lastmessage | grep -v .*'.gmdate('Ymd').'.* | grep -v ^[^\<]*\<MoronBot\> | tail -n 1');
}
if (strlen(trim($found)) < 1) die($argv[1]." not found in the logs.");
$date = array_pop(explode("-",pathinfo(strtok($found,":"),PATHINFO_FILENAME)));
$date = substr($date,0,4)."/".substr($date,4,2)."/".substr($date,6);
$lastsaid = str_ireplace("Fugi","POOT",trim(strtok("\n")));
die($date." ".$lastsaid);
?>
