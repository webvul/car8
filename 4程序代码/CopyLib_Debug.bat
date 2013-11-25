@echo off
echo  编译MyCmnMyOql(Debug) 
setlocal
set curDisk=%~d0
set curPath=%~dp0
set log=R:\log.txt

set /p = "1. 开始重新编译 MyCmn (Debug) ...      "： <nul
c:
cd c:\Windows\Microsoft.NET\Framework\v4.0*
Msbuild %curPath%\Cne_MyOql4\Cne_MyCmn4.sln /t:Rebuild /p:Configuration=Debug /consoleloggerparameters:ErrorsOnly >>log.txt
echo 完成!

set /p = "2. 开始重新编译 MyOql (Debug) ...      "： <nul
cd c:\Windows\Microsoft.NET\Framework\v4.0*
Msbuild %curPath%\Cne_MyOql4\Cne_MyOql4.sln /t:Rebuild /p:Configuration=Debug /consoleloggerparameters:ErrorsOnly >>log.txt
echo 完成!

%curDisk%
cd %curPath%

CopyLib.bat

endlocal
echo  编译MyCmnMyOql(Debug) 完成!
pause;
