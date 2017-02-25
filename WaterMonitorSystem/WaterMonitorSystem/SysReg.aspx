<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SysReg.aspx.cs" Inherits="WaterMonitorSystem.SysReg" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>网站注册</title>
    <script src="js/jquery-1.11.1.min.js"></script>
    <style>
        * {
            padding:0; margin:0; font-size:12px;
        }

        table {
        }

        table tr {
            height:30px; line-height:30px;
        }

        table tr .t {
            text-align:right; width:100px;
        }

        .txt {
            border:1px solid #00ffff; padding:3px 5px;
        }

        .btn {
            padding:3px 5px;
        }
    </style>
    <script type="text/javascript">
        function chk()
        {
            var val = $("#TextBox2").val();
            if (val == "")
            {
                alert("注册码不能为空！");
                return false;
            }

            return true;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server"> 
        <table>
            <tr>
                <td class="t">字符串：</td>
                <td><asp:TextBox ID="TextBox3" runat="server" Width="300px" CssClass="txt"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="t">序列号：</td>
                <td><asp:TextBox ID="TextBox1" runat="server" Width="300px" CssClass="txt"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="t">注册码：</td>
                <td><asp:TextBox ID="TextBox2" runat="server" Width="300px" CssClass="txt"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="t"></td>
                <td><asp:Button ID="Button2" runat="server" CssClass="btn" Text="注册" OnClick="Button1_Click" OnClientClick="return chk()" />
                    <br />
                    <asp:Label ID="Label1" runat="server" Text="" ForeColor="Red"></asp:Label></td>
            </tr>
        </table>      
    </form>
</body>
</html>
