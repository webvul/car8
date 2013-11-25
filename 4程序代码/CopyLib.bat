@echo "开始发布"
@echo off

 
xcopy /Y /E /Q Cne_MyOql4\libs4\* dev8App\lib\*
 
@set/p nul=Copy Complete!
 
pause;