<%@ Page Title="" Language="C#" Inherits="MyCon.MyMvcPage<MyOqlSet>" %>

<html>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<head>
    <title>Thinking in VML</title>
</head>
<body>
    <table>
        <thead>
            <tr>
                <%
                    foreach (var col in Model.Columns)
                    {
                        Ronse += "<th>" + col.Name + "</th>";
                    }
                %></tr>
        </thead>
        <tbody>
            <%
                foreach (var row in Model.Rows)
                {
                    Ronse += "<tr>";

                    foreach (var item in row)
                    {
                        Ronse += "<td>" + item + "</td>";
                    }

                    Ronse += "</tr>";
                }
            %>
        </tbody>
    </table>
    <%
        int i = 0;
        object ii = null;
        var r = i.IfNoValue(() => "Null", o => o.ToString() + "!");

        Ronse += r;
    %>
</body>
</html>
