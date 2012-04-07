<?php
	require_once('/opt/moronbot/loghashes.php');
        if ($argc < 2) print_r($logfilesarr);
        else if (array_key_exists($argv[1],$logfilesarr)) print ($logfilesarr[$argv[1]]['hash']);
        else print "Not found";
?>
