@echo off

if not "%UnityPath5%"=="" goto :_ok
echo : ���ϐ� "UnityPath5" ��ݒ肵�Ă��������B
echo : "UnityPath5" --- Unity.exe�ւ̃p�X�@��j C:\Program Files\Unity\Editor\Unity.exe
pause
goto :eof


:_ok

:_menu
echo : 
echo : �o�b�`���s�e�X�g
echo : 
echo : === MENU ===
echo : 1 ... bin�t�@�C���o��
echo : 2 ... base64�t�@�C���o��
echo : 3 ... bin�t�@�C���o�͐ݒ��AUnity Editor�N��
set /p p=": ��"
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
