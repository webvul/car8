@echo off
setlocal
set curDisk=%~d0
set curPath=%~dp0

%curDisk%
cd %curPath%
@echo 分析继承IModel接口的类是否正确。

bin\debug\MyTool -CheckIModel  -dlls "..\MyWeb\bin\PmEnt.dll" "..\MyWeb\bin\PmBiz.dll" "..\MyWeb\bin\MyWeb.dll"

endlocal
pause;