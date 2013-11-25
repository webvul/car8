@echo off
echo ----------------- JavaScript 开始编译 ----------------- 

setlocal
set curDisk=%~d0
set curPath=%~dp0
set log=R:\log.txt

%curDisk%
cd %curPath%


set /p = "1. 开始清除之前合成的脚本...       "： <nul
del myjs.js /f 
del myjs_admin.js /f 
del ..\App_Themes\Admin\MyAdmin.css /f 
echo 完成！

set /p = "2. 开始合成的脚本...               "： <nul
..\..\MyTool\bin\Debug\MyTool -NewFile "MyJs.js" -Clear true -ListFiles "jQuery\jquery.js" "jQuery\json2.js" "jQuery\jquery.query.js" "jQuery\jquery.timer.js" "MyJv.js" "MyJv_Extend.js" "MyJv_Biz.js" "MyJv_Csm.js" "Canvas\MyCorner.js"  >> %log%
..\..\MyTool\bin\Debug\MyTool -NewFile  "MyJs_Admin.js" -Clear true -ListFiles  "MyJv_Admin.js"  "jQuery\fixTableHeader.js" "FlexiGrid\flexigrid.js" "jQuery\tableDnd.js" "Boxy\boxy.js" "TextHelper\TextHelper.js"  "poshytip\jquery.poshytip.js"	jquery.ui\js\jquery-ui-1.7.3.custom.js jquery.ui\ui.slider.js jquery.ui\jquery-ui-timepicker-addon.js jquery.ui/myjQuery.js "FileUploader\fileuploader.js" "ListFile\ListFile.js" "ListFile\TableFile.js" 	 >> %log%

..\..\MyTool\bin\Debug\MyTool -NewFile  "MyJs_Host.js" -Clear true -ListFiles "Boxy\boxy.js" "TextHelper\TextHelper.js" "poshytip\jquery.poshytip.js" >> %log%

rem ..\..\MyTool\bin\Debug\MyTool -NewFile  "MyJs_chk.js" -Clear true -ListFiles  "poshytip\jquery.poshytip.js"			>> %log%
rem ..\..\MyTool\bin\Debug\MyTool -NewFile  "MyJs_date.js" -Clear true -ListFiles  jquery.ui\js\jquery-ui-1.7.3.custom.js jquery.ui\jquery-ui-timepicker-addon.js jquery.ui/myjQuery.js  	>> %log%
rem ..\..\MyTool\bin\Debug\MyTool -NewFile  "MyJs_file.js" -Clear true -ListFiles  "FileUploader\fileuploader.js" "ListFile\ListFile.js" 	>> %log%
..\..\MyTool\bin\Debug\MyTool -NewFile "..\App_Themes\Admin\MyAdmin.css" -Clear true -ListFiles  "TextHelper\TextHelper.css" "FileUploader\fileuploader.css"  "ListFile\ListFile.css" "Boxy\boxy.css" "FlexiGrid/flexigrid.css" "poshytip\tip-yellow\tip-yellow.css" jquery.ui\css\cupertino\jquery-ui-1.7.3.custom.css  jquery.ui\jquery-ui-timepicker-addon.css  >> %log%

xcopy "Flexigrid\flexigrid\*" ..\App_Themes\Admin\flexigrid\* /Y /Q 	>> %log%
xcopy "Boxy\boxy\*" ..\App_Themes\Admin\boxy\* /Y /Q 	>> %log%

echo 完成！

set /p = "3. 转移Res数据到 My_Build\Res ...  ":<nul
rem xcopy "Pinyin.js" My_Build\Res\* /Y											>> %log%
rem xcopy "MyJs_chk.js" My_Build\Res\* /Y 										>> %log%
rem xcopy "MyJs_date.js" My_Build\Res\* /Y  									>> %log%
rem xcopy "MyJs_file.js" My_Build\Res\* /Y 										>> %log%
rem xcopy "Img\*" My_Build\Res\Img\* /Y /S /Q 									>> %log%

xcopy "MyJs.js" My_Build\Res\* /Y  											>> %log%
xcopy "MyJs_Admin.js" My_Build\Res\* /Y  									>> %log%
xcopy "MyJs_Host.js" My_Build\Res\* /Y  									>> %log%
xcopy "MyCss.css" My_Build\Res\* /Y 										>> %log%
xcopy "flowplayer\*" My_Build\Res\flowplayer\* /Y /S /Q 					>> %log%
xcopy "MyPucker\*" My_Build\Res\MyPucker\* /Y /S /Q 						>> %log%
xcopy "FileUploader\FileUploader.ashx" My_Build\Res\FileUploader\ /Y /Q	 	>> %log%
xcopy "tiny_mce\*" My_Build\Res\tiny_mce\* /Y /S /Q 						>> %log%
xcopy "Images\*" My_Build\Res\Images\* /Y /S /Q 							>> %log%

xcopy "..\App_Themes\Admin\MyAdmin.css" My_Build\App_Themes\Admin\* /Y /Q 	>> %log%
echo 完成！

 

set /p = "4. 开始混淆...                     ": <nul
cd My_Build\Res
"java.exe" -jar ..\..\..\..\MyTool\yuicompressor-2.4.2.jar --type js --charset utf8 -o _MyJs.js MyJs.js					>> %log%
"java.exe" -jar ..\..\..\..\MyTool\yuicompressor-2.4.2.jar --type js --charset utf8 -o _MyJs_Admin.js MyJs_Admin.js   	>> %log%
"java.exe" -jar ..\..\..\..\MyTool\yuicompressor-2.4.2.jar --type js --charset utf8 -o _MyJs_Host.js MyJs_Host.js   	>> %log%
rem "java.exe" -jar ..\..\..\..\MyTool\yuicompressor-2.4.2.jar --type js --charset utf8 -o _MyJs_chk.js MyJs_chk.js   	>> %log%
rem "java.exe" -jar ..\..\..\..\MyTool\yuicompressor-2.4.2.jar --type js --charset utf8 -o _MyJs_date.js MyJs_date.js   	>> %log%
rem "java.exe" -jar ..\..\..\..\MyTool\yuicompressor-2.4.2.jar --type js --charset utf8 -o _MyJs_file.js MyJs_file.js   	>> %log%
"java.exe" -jar ..\..\..\..\MyTool\yuicompressor-2.4.2.jar --type css --charset utf8 -v -o _MyCss.css MyCss.css   >> %log%
echo 完成！

set /p = "5. 整理文件...                     ": <nul
del  MyJs.js			>> %log%
del  MyJs_Admin.js		>> %log%
del  MyJs_Host.js		>> %log%
rem del  MyJs_chk.js	>> %log%
rem del  MyJs_date.js	>> %log%
rem del  MyJs_file.js	>> %log%
del  MyCss.css	>> %log%

rename  _MyJs.js   MyJs.js				 >> %log%
rename  _MyJs_Admin.js   MyJs_Admin.js	 >> %log%
rename  _MyJs_Host.js   MyJs_Host.js	 >> %log%
rem rename  _MyJs_chk.js   MyJs_chk.js	 >> %log%
rem rename  _MyJs_date.js   MyJs_date.js >> %log%
rem rename  _MyJs_file.js   MyJs_file.js >> %log%
rename  _MyCss.css   MyCss.css	 >> %log%


cd ..

"java.exe" -jar ..\..\..\MyTool\yuicompressor-2.4.2.jar --type css --charset utf8 -v -o App_Themes\Admin\_MyAdmin.css App_Themes\Admin\MyAdmin.css   >> %log%
del  App_Themes\Admin\MyAdmin.css		>> %log%
rename  App_Themes\Admin\_MyAdmin.css   MyAdmin.css		>> %log%
 
echo 完成！


echo ----------------- JavaScript 编译完成 ----------------- 
endlocal
pause;
