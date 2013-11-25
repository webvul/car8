@echo off
echo  编译MyCmnMyOql(Release) 
setlocal
set curDisk=%~d0
set curPath=%~dp0
set log=R:\log.txt

set /p = "1. 开始重新编译 MyCmn (Release) ...      "： <nul
c:
cd c:\Windows\Microsoft.NET\Framework\v4.0*
Msbuild %curPath%\Cne_MyOql4\Cne_MyCmn4.sln /t:Rebuild /p:Configuration=Release /consoleloggerparameters:ErrorsOnly >>log.txt
echo 完成!

set /p = "2. 开始重新编译 MyOql (Release) ...      "： <nul
cd c:\Windows\Microsoft.NET\Framework\v4.0*
Msbuild %curPath%\Cne_MyOql4\Cne_MyOql4.sln /t:Rebuild /p:Configuration=Release /consoleloggerparameters:ErrorsOnly >>log.txt
echo 完成!

%curDisk%
cd %curPath%


xcopy /Y /E /Q Cne_MyOql4\libs4\MyCmn.*  Hyj_Shop\lib\*
xcopy /Y /E /Q Cne_MyOql4\libs4\MyOql.*  Hyj_Shop\lib\*

endlocal
echo  编译MyCmnMyOql(Release) 完成!
pause;
