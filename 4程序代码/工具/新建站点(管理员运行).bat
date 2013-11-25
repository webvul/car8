@echo off
setlocal
set curDisk=%~d0
set curPath=%~dp0

%curDisk%
cd %curPath%

echo.
echo ---------------------------------------------------
echo 在本机默认站点下安装 BBS 应用程序,需要以下环境：
echo 1. 名为 bbs 的应用程序池
echo 2. D:\Hyj_Base\Bbs_Ori 文件夹
echo 3. D:\Hyj\ 文件夹
echo 创建的站点应用名称规范：http://主机头/hyj_小区Id_bbs

echo 正在加载 powershell 环境

echo.

powershell.exe  -ImportSystemModules -file newSite.ps1
endlocal
pause;
