//当前登录操作员管理ID
var mnId = "";
//登录用户标识
var loginIdentifer;

var monitorRealName = "";
var villageid;
var villagename;

//用于存储所有级别的节点
var levelJson;

var seltype = 0;

$(document).ready(function () {
    var defaultTheme = $.cookie("psbsTheme");
    if (defaultTheme != null && defaultTheme != "default") {
        var link = $(document).find('link:first');
        link.attr('href', '../App_Themes/easyui/themes/' + defaultTheme + '/easyui.css');
    }
    $.ShowMask("数据加载中，请稍等……");
    GetSystemInfo();
    $.HideMask();
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
                monitorRealName = data.SysStateInfo.监测点级别名称;

                $('#divAreaTree1').tree({
                    onSelect: function (node) {
                        $("#cbb_DevCombobox1").combobox({ data: [] });
                        if (node.id.indexOf("cz") >= 0) {
                            villageid = node.attributes["mid"];
                            villagename = node.attributes["manage"]["上级名称"] + " " + node.attributes["manage"]["名称"];
                            $.ShowMask("获取村用水户，请稍等……");
                            LoadUserCombobox("../WebServices/WaterUserService.asmx/GetWaterUsersByVillageId", { "loginIdentifer": window.parent.guid, "villageId": villageid, "isExport": false }, $("#cbb_DevCombobox1"));
                            $.HideMask();
                        }
                    }
                });

                $('#divAreaTree2').tree({
                    onSelect: function (node) {
                        $("#cbb_DevCombobox2").combobox({ data: [] });
                        if (node.id.indexOf("cz") >= 0) {
                            villageid = node.attributes["mid"];
                            villagename = node.attributes["manage"]["上级名称"] + " " + node.attributes["manage"]["名称"];
                            $.ShowMask("获取村用水户，请稍等……");
                            LoadUserCombobox("../WebServices/WaterUserService.asmx/GetWaterUsersByVillageId", { "loginIdentifer": window.parent.guid, "villageId": villageid, "isExport": false }, $("#cbb_DevCombobox2"));
                            $.HideMask();
                        }
                    }
                });

                $('#cbb_DevCombobox1').combobox({
                    onSelect: function (record) {
                        var id = record["id"];
                        $.ShowMask("获取用水户用水定额，请稍等……");
                        GetWaterUserById(id);
                        $.HideMask();
                    }
                });

                LoadWaterUserTree2("divAreaTree1", "divAreaTree2", mnId, false, false);
                
                $("#btnDeal").unbind().bind("click", function () {
                    btnDealConfirm();
                });
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function GetWaterUserById(waterUserId) {
    $.ajax(
    {
        url: "../WebServices/WaterUserService.asmx/GetWaterUserById",
        type: "GET",
        data: { "loginIdentifer": window.parent.guid, "waterUserId": waterUserId },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                $("#divAllowedNumber").html(data.WaterUser.用水定额);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function LoadUserCombobox(url, data, obj) {
    $.ajax(
    {
        url: url,
        type: "GET",
        data: data,
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                levelJson = data.WaterUsers;
                var comboBoxDataLevel = [];
                for (i = 0; i < levelJson.length; i++) {
                    var levelObj = {};
                    if (levelJson[i].状态 == "正常") {
                        levelObj["id"] = levelJson[i].ID;
                        levelObj["text"] = levelJson[i].名称;
                        comboBoxDataLevel.push(levelObj);
                    }
                }

                $(obj).combobox({
                    data: comboBoxDataLevel
                });
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function btnDealConfirm() {
    var n = $.trim($("#txtNumber").val());
    if (n == "" || !isInteger(n)) {
        $.messager.alert("提示信息", "交易水量必须为正整数！");
        $("#txtNumber").val("");
        $("#txtNumber").focus();
        return;
    }
    if (parseInt(n) <= 0) {
        $.messager.alert("提示信息", "交易水量必须大于0！");
        $("#txtNumber").val("");
        $("#txtNumber").focus();
        return;
    }

    var userid1 = $.trim($('#cbb_DevCombobox1').combobox('getValue'));
    if (!isInteger(userid1)) {
        $.messager.alert("提示信息", "请正确选择出卖方！");
        return;
    }
    if (parseInt(userid1) <= 0) {
        $.messager.alert("提示信息", "请正确选择出卖方2！");
        return;
    }

    var userid2 = $.trim($('#cbb_DevCombobox2').combobox('getValue'));
    if (!isInteger(userid2)) {
        $.messager.alert("提示信息", "请正确选择买受方！");
        return;
    }
    if (parseInt(userid2) <= 0) {
        $.messager.alert("提示信息", "请正确选择买受方2！");
        return;
    }

    if (userid1 == userid2) {
        $.messager.alert("提示信息", "出卖方和买受方不能为同一人！");
        return;
    }

    if (confirm("确定交易？交易水量：" + n + "吨")) {
        $.ajax(
        {
            url: "../WebServices/WaterRightService.asmx/Recharge",
            type: "GET",
            data: { "loginIdentifer": window.parent.guid, "userid1": userid1, "userid2": userid2, "n": n },
            dataType: "text",
            cache: false,
            success: function (responseText) {
                var data = eval("(" + $.xml2json(responseText) + ")");
                if (data.Result) {
                    $.messager.alert("提示信息", "交易成功");
                    $("#txtNumber").val("");
                    $("#cbb_DevCombobox1").combobox('setValue', "");
                    $("#cbb_DevCombobox2").combobox('setValue', "");
                }
                else {
                    $.messager.alert("提示信息", data.Message);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
            }
        });
    }
}