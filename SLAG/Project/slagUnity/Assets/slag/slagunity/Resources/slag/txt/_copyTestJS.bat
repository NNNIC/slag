cd %~dp0
del /q *.txt 
copy ..\..\..\..\..\..\..\test\test*.js   *.js.txt
copy ..\..\..\..\..\..\..\test\test*.inc  *.inc.txt

dir /b test??.js.txt  > _list.txt
dir /b test??.inc.txt >>_list.txt

pause
goto :eof









cd %~dp0
del /q *.txt 
copy ..\..\..\..\..\..\..\test\test0?.js  *.js.txt
copy ..\..\..\..\..\..\..\test\test1?.js  *.js.txt
copy ..\..\..\..\..\..\..\test\test5?.js  *.js.txt
copy ..\..\..\..\..\..\..\test\test91.js  *.js.txt
dir /b test*.txt > _list.txt
pause