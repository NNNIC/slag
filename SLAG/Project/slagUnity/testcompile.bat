@echo off

if not "%UnityPath5%"=="" goto :_ok
echo : 環境変数 "UnityPath5" を設定してください。
echo : "UnityPath5" --- Unity.exeへのパス　例） C:\Program Files\Unity\Editor\Unity.exe
pause
goto :eof


:_ok

:_menu
echo : 
echo : バッチ実行テスト
echo : 
echo : === MENU ===
echo : 1 ... binファイル出力
echo : 2 ... base64ファイル出力
echo : 3 ... binファイル出力設定後、Unity Editor起動
set /p p=": ＞"
goto :_%p%

:_1
set FILELIST=N:\Project\test\_list.txt
set DSTEXT=bin

"%UnityPath5%" -projectPath "%~dp0" -batchmode -quit -executeMethod slageditortool.COMPILE
goto :_menu

:_2
set FILELIST=N:\Project\test\_list.txt
set DSTEXT=base64

"%UnityPath5%" -projectPath "%~dp0" -batchmode -quit -executeMethod slageditortool.COMPILE
goto :_menu

:_3
set FILELIST=N:\Project\test\_list.txt
set DSTEXT=bin

start "" "%UnityPath5%" -projectPath "%~dp0" 

goto :_menu
