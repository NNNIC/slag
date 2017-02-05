cd %~dp0
if exist n: subst n: /d
subst n: "%cd%"
::pause
start n:
