import subprocess

proc = subprocess.Popen(['/usr/bin/php','/opt/moronbot/loggrep.php',"\""+"#desertbus".replace("\"","\\\"")+"\"",'#desertbus', 'mention'], stdout=subprocess.PIPE)
output = proc.stdout.read()
print output
