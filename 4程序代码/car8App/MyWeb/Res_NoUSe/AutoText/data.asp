<%

if len(request.querystring("key")) > 0 then
	datas = ""
	for i=0 to 10
		datas = datas & request.querystring("key") & i & ";"
	next 
	response.write(datas)
end if

%>