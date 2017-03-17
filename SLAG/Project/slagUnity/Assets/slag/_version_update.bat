@echo off
cd %~dp0
set MAJORVER=0.7
set MINORVER=

echo Get-Date -format "yyyyMMdd" > ~tmp.ps1
powershell -NoProfile -ExecutionPolicy Unrestricted  ~tmp.ps1 >~tmp.txt
for /f %%i in (~tmp.txt) do if "%MINORVER%"=="" set MINORVER=%%i
del ~tmp.ps1
del ~tmp.txt

set VER=%MAJORVER%.%MINORVER%

echo : 
echo : バージョンファイルを更新
echo :
echo : 次のバージョンに変更します。
echo : %VER%
pause
echo public class slagunity_version { public const string version="%VER%";  } >slagunity\Scripts\slagunity_version.cs
echo :
echo : 次のファイルを作成しました
echo : slagunity\Scripts\slagunity_version.cs 
echo :
echo : 終了
pause