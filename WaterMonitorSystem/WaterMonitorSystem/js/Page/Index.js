// JScript 文件

$(document).ready(setDiv);
$(window).resize(setDiv);
function setDiv() {
    //可视窗口的高度
    var windowHeight = $(window).height();
    var windowWidth = $(window).width()
    //整个登录DIV的高度
    var loginHeight = $("#divLogin").height();
    var loginWidth = $("#divLogin").width()
    //如果窗口的高度大于DIV的高度，设置DIV的TOP为(窗口高-DIV高)/2像素，使DIV居中
    if (windowHeight > loginHeight) {
        $("#divLogin").css("top", ((windowHeight - loginHeight) / 2) + "px");
    }
        //否则DIV居顶显示
    else {
        $("#divLogin").css("top", "0px");
    }

    //如果窗口的宽度大于DIV的宽度，设置DIV的Left为(窗口宽-DIV宽)/2像素，使DIV居中
    if (windowWidth > loginWidth) {
        $("#divLogin").css("left", ((windowWidth - loginWidth) / 2) + "px");
    }
        //否则DIV居左显示
    else {
        $("#divLogin").css("left", "0px");
    }

    $("#PersonName").focus();
}

//正在登录
var isLogging = false;

//把回车键设置到登录按钮上
function EnterLogin(event) {
    event = (event) ? event : window.event
    if (event.keyCode) {
        if (event.keyCode == 13) {
            LoginInfo();
        }
    }
    else {
        if (event.which == 13) {
            LoginInfo();
        }
    }
}

//登录
function LoginInfo() {
    if (isLogging) {
        return;
    }
    isLogging = true;
    $("#ResultMessage").text("");
    var userName = $("#PersonName").val();
    var password = $("#Password").val();
    //var vcode = $("#ValidateCode").val();
    var vcode = "";
    if (userName.trim() == "") {
        $("#ResultMessage").text("请输入用户名");
        isLogging = false;
        $("#PersonName").focus();
        return;
    }
    /*
    if(vcode.trim()=="")
    {
        $("#ResultMessage").text("请输入验证码");
        isLogging = false;
        $("#ValidateCode").focus();
        return;
    }
    */
    $.ShowMask("正在登录，请稍等……");
    $.ajax(
    {
        url: "WebServices/UserAdminService.asmx/LoginEx",
        type: "POST",
        data: { "LoginName": userName, "LoginPwd": password, "ValidateCode": vcode },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result)//登录成功
            {
                $.cookie("psbsLogin", data.Guid, { expires: 7, path: '/' });
                location.href = "MainNew.html";
            }
            else {
                $.messager.alert("提示信息", data.Message);
                if (data.Message != "验证码不正确，请重新输入") {
                    RefreshValidateCode(document.getElementById("imgValidateCode"));
                }
                $.HideMask();
            }
            isLogging = false;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            isLogging = false;
            $.HideMask();
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function RefreshValidateCode(obj) {
    //obj.src = "WebServices/UserAdminService.asmx/ValidateCode?" + Math.floor(Math.random() * 10000);
}
