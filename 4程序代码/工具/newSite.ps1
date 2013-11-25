$commId = read-host  "请输入新小区的ID "
$server = read-host "请输入数据库IP "

$filePath = -join("D:\Hyj\",$commId,"_Bbs")
$siteName = -join("Hyj_",$commId,"_Bbs")


if ( [System.Io.Directory]::Exists($filePath) )
{
	echo  "$filePath文件夹已存在,请检查！" 
	return ;
}

if ( [System.Io.Directory]::Exists("D:\Hyj\Bbs_Ori") )
{
	echo "D:\Hyj\Bbs_Ori 文件夹已存在，请检查！"
	return ;
}

#(New-Object System.IO.DirectoryInfo("D:\Hyj_Base\Bbs_Ori")).Delete($true)
Copy-Item D:\Hyj_Base\Bbs_Ori  -Recurse $filePath

cd iis:
$siteExists  = ( Get-ChildItem '.\Sites\Default Web Site' | Select-Object Name | Where-Object { $_.Name -eq  $siteName } ) -eq $null

if ( $siteExists -eq $false )
{
    echo  "Default Web Site 已存在 应用： $siteName ! 请重新指定其它名字再试！"
    return ;
}



New-Item "IIS:\Sites\Default Web Site\$siteName" -physicalPath "$filePath" -type Application
Set-ItemProperty "IIS:\Sites\Default Web Site\$siteName" -name applicationPool -value bbs


echo "创建站点应用: $siteName 完成！";
echo "";

$content = [System.IO.File]::ReadAllText((-join($filePath,"\bbsmax.config"))).Replace('$Server$',$server).Replace('$CommId$',$commId)
[System.IO.File]::WriteAllText((-join($filePath,"\bbsmax.config")) , $content ) ;