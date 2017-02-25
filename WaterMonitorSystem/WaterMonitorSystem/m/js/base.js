function ShowMsg(msg, isMask, isOk) {
    HideMsg();

    if (isMask) {
        if (!($("#divMask").length > 0)) {
            $(document.body).append("<div id=\"divMask\"></div>")
        }
    }

    if (!($("#divShowMsg").length > 0)) {
        $(document.body).append("<div id=\"divShowMsg\"></div>")
    }

    $("#divShowMsg").css("top", ($(window).height() - $("#divShowMsg").height()) / 2 - 50);
    $("#divShowMsg").css("left", ($(window).width() - $("#divShowMsg").width()) / 2);

    $("#divShowMsg").html(msg);
    if (isOk) {
        $("#divShowMsg").append("<div id=\"divMaskOK\"><input type=\"button\" class=\"btnMaskOK\" value=\"确定\"></div>");
        $(".btnMaskOK").click(function () {
            HideMsg();
        });
    }
    $("#divMask").show();
    $("#divShowMsg").show();
}

function HideMsg() {
    $("#divMask").remove();
    $("#divShowMsg").remove();
}

function getUrlParam(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
    var r = window.location.search.substr(1).match(reg);  //匹配目标参数
    if (r != null) return unescape(r[2]); return null; //返回参数值
}