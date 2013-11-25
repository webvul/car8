@echo off
rem 以下是发布脚本. 把发布代码移到 R:\ 盘. 

set /p CONVERT= 确认R盘存在，并且路径正确，下一步将发布到 R:\MyWeb 目录！ 
xcopy /Y /E ..\MyWeb  R:\MyWeb\

rem 如果R盘不存在，则发布到 D盘，否则发布到 C盘
R:
cd R:\MyWeb 

set /p CONVERT= 确认R盘存在，并且路径正确，以后将执行删除文件的动作！ 


del Web.config
del /S /F /Q  Upload\*

del /S /F /Q *.txt
del /S /F /Q *.cs
del /S /F /Q Doc\*
del /S /F /Q obj\*
del /S /F /Q /A:ARH *.scc 
del /S /F /Q bin\*.pdb

move Res\My_Build  My_Build
del /S /F /Q Res\*
xcopy /Y /S My_Build\*  ..\MyWeb\*

pause;