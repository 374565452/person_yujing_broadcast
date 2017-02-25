var mnId = "";
//左侧树形选中节点的管理ID
var currentSelManageId;
var managerRealName = "";
var monitorRealName = "";

//登录标示
var loginIdentifer = window.parent.guid;

$(document).ready(function () {
    var defaultTheme = $.cookie("psbsTheme");
    if (defaultTheme != null && defaultTheme != "default") {
        var link = $(document).find('link:first');
        link.attr('href', '../App_Themes/easyui/themes/' + defaultTheme + '/easyui.css');
    }
    $.ShowMask("数据加载中，请稍等……");
    GetSystemInfo();
    $('#btn_QueryChart').linkbutton({ disabled: true });
});

//从服务器取得系统运行状态信息
function GetSystemInfo() {
    $.ajax(
    {
        url: "../WebServices/SystemService.asmx/GetSystemStateInfo",
        type: "GET",
        data: { "loginIdentifer": window.parent.guid },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result)//登录成功
            {
                mnId = data.SysStateInfo.当前登录操作员管理ID;
                manageNodeLoaded = false;
                deviceNodeLoaded = false;
                monitorRealName = data.SysStateInfo.监测点级别名称;
                //LoadTree("divAreaTree", mnId, false, false);
                TreeBindSelect();
                LoadWaterUserTree("divAreaTree", mnId, false, false);

                setTimeout(function () {
                    Btn_QueryChart_Click();
                },1000);
            }
            $.HideMask();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.HideMask();
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function TreeBindSelect() {
    $('#divAreaTree').tree({
        onSelect: function (node) {
            $("#spanTownName").html("-");
            $("#villagename").html("-");
            if (node.attributes["nodeType"] == "manage") {
                currentSelManageId = node.attributes["manage"].ID;
                if (node.attributes["manage"]["级别名称"] == "村庄") {
                    $("#spanTownName").html(node.attributes.manage.上级名称);
                    $("#villagename").html(node.attributes.manage.名称);

                    $("#btn_QueryChart").linkbutton({
                        disabled: false
                    });
                }
                else {
                    $("#btn_QueryChart").linkbutton({
                        disabled: true
                    });
                }
            }
        }
    });
}