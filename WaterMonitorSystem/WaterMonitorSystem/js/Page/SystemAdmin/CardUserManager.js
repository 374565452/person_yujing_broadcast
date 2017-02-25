// JScript 文件


//--------------------
//登录标示
var loginIdentifer = window.parent.guid;
//用于存储查询出来的用水户卡信息
var cardUserObj = {};

$(document).ready(function () {
    $.ShowMask("数据加载中，请稍等……");
    GetSystemInfo();
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
                TreeBindSelect();
                $.HideMask();
                $("#btn_Add").linkbutton({
                    disabled: true
                });
                LoadWaterUserTree("WaterUserTree", mnId, false, false);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

//点击左侧树形重新加载右侧Combobox
function TreeBindSelect() {
    $('#WaterUserTree').tree({
        onSelect: function (node) {
            $("#cbb_UserNameCombobox").combobox({ data: [] });
            if (node.attributes["nodeType"] == "manage") {
                if (node.attributes["manage"]["级别名称"] == "村庄") {
                    $("#btn_Add").linkbutton({
                        disabled: false
                    });
                }
                else {
                    $("#btn_Add").linkbutton({
                        disabled: true
                    });
                }
            }
            if (node.id.indexOf("cz") >= 0) {
                villageid = node.attributes["mid"];
                LoadCardUserInfo("../WebServices/CardUserService.asmx/GetCardUsersByVillageId", { "loginIdentifer": window.parent.guid, "villageId": villageid, "isExport": false });
            }
        }
    });
}

function LoadCardUserInfo(url, data) {
    $.ajax(
    {
        url: url,
        type: "GET",
        data: data,
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result)//登录成功
            {
                var cardUsers = data.CardUsers;
                var comboBoxDataLevel = [];
                var tableData = [];
                allcomboid = "";
                for (var i = 0; i < cardUsers.length; i++) {
                    var cardUser = cardUsers[i];
                    //更新datagrid
                    var tableRow = {};
                    tableRow["ID"] = cardUser["ID"];
                    tableRow["SerialNumber"] = cardUser["SerialNumber"];
                    tableRow["UserNo"] = cardUser["UserNo"];
                    tableRow["UserName"] = cardUser["UserName"];
                    tableRow["IdentityNumber"] = cardUser["IdentityNumber"];
                    tableRow["Telephone"] = cardUser["Telephone"];
                    tableRow["ResidualWater"] = cardUser["ResidualWater"];
                    tableRow["ResidualElectric"] = cardUser["ResidualElectric"];
                    tableRow["TotalWater"] = cardUser["TotalWater"];
                    tableRow["TotalElectric"] = cardUser["TotalElectric"];
                    tableRow["IsCountermand"] = cardUser["IsCountermand"] == "是" ? "<span style='color:red;bold:weight'>" + cardUser["IsCountermand"] + "</span>" : cardUser["IsCountermand"];
                    tableRow["OpenTime"] = cardUser["OpenTime"];
                    tableRow["LastChargeTime"] = cardUser["LastChargeTime"];

                    tableRow["Edit"] = "<img src='../Images/Detail.gif' onclick='ShowCardUser(" + cardUser["ID"] + ")' />";
                    tableRow["Cancel"] = "<img src='../Images/Delete.gif' onclick=\"CancelCardUser('" + cardUser["SerialNumber"] + "')\" />";
                    tableRow["Countermand"] = getCountermandStr(cardUser["IsCountermand"], cardUser["SerialNumber"]);
                    tableData.push(tableRow);
                    //更新combobox
                    var comboBoxData = {};
                    comboBoxData["id"] = cardUser["WaterUserId"];
                    comboBoxData["text"] = cardUser["UserName"];// + "(" + waterUser["户号"] + ")"
                    var flag = false;
                    for (var k = 0; k < comboBoxDataLevel.length; k++) {
                        if (comboBoxDataLevel[k].id == comboBoxData.id) {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag) {
                        comboBoxDataLevel.push(comboBoxData);
                        if (allcomboid != "") {
                            allcomboid += ",";
                        }
                        allcomboid += cardUser["WaterUserId"];
                    }

                    //向用水户卡对象中添加数据
                    //cardUserObj[cardUser["ID"]] = cardUser;
                }
                var allObj = {};
                allObj["id"] = allcomboid + "all";
                allObj["text"] = "全部";
                allObj["selected"] = true;
                comboBoxDataLevel.unshift(allObj);
                $('#tbCardUserInfos').datagrid({ loadFilter: pagerFilter }).datagrid('loadData', tableData);
                $.HideMask();
                //$('#tbWaterUserInfos').datagrid('loadData', tableData)
                $("#cbb_UserNameCombobox").combobox({
                    data: comboBoxDataLevel
                });
            }
            else {
                $.HideMask();
                $.messager.alert("提示信息", data.Message);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.HideMask();
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function ShowCardUser(CardUserID) {
    $('#ShowCardUserData').dialog({ closed: false });
    $('#ShowCardUserData').dialog({ title: "查看用水户卡信息" });
    $.ajax(
    {
        url: "../WebServices/CardUserService.asmx/GetCardUserById",
        type: "GET",
        data: { 'loginIdentifer': loginIdentifer, 'cardUserID': CardUserID },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                var CardUserInfo = data.CardUser;
                $("#txt_SerialNumber").html(CardUserInfo["SerialNumber"]);
                $("#txt_UserNo").html(CardUserInfo["UserNo"]);
                $("#txt_UserName").html(CardUserInfo["UserName"]);
                $("#txt_IdentityNumber").html(CardUserInfo["IdentityNumber"]);
                $("#txt_Telephone").html(CardUserInfo["Telephone"]);
                $("#txt_IsCountermand").html(CardUserInfo["IsCountermand"]);
                $("#txt_ResidualWater").html(CardUserInfo["ResidualWater"]);
                $("#txt_ResidualElectric").html(CardUserInfo["ResidualElectric"]);
                $("#txt_TotalWater").html(CardUserInfo["TotalWater"]);
                $("#txt_TotalElectric").html(CardUserInfo["TotalElectric"]);
                $("#txt_OpenTime").html(CardUserInfo["OpenTime"]);
                $("#txt_LastChargeTime").html(CardUserInfo["LastChargeTime"]);
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

function pagerFilter(data) {
    if (typeof data.length == 'number' && typeof data.splice == 'function') {	// is array
        data = {
            total: data.length,
            rows: data
        }
    }
    var dg = $(this);
    var opts = dg.datagrid('options');
    var pager = dg.datagrid('getPager');
    pager.pagination({
        onSelectPage: function (pageNum, pageSize) {
            opts.pageNumber = pageNum;
            opts.pageSize = pageSize;
            pager.pagination('refresh', {
                pageNumber: pageNum,
                pageSize: pageSize
            });
            dg.datagrid('loadData', data);
        }
    });
    if (!data.originalRows) {
        data.originalRows = (data.rows);
    }
    var start = (opts.pageNumber - 1) * parseInt(opts.pageSize);
    var end = start + parseInt(opts.pageSize);
    data.rows = (data.originalRows.slice(start, end));
    return data;
}

function Btn_Query_Click() {
    $('#tbCardUserInfos').datagrid('loadData', { total: 0, rows: [] });
    var waterUserIds = $.QueryCombobox("cbb_UserNameCombobox");

    if (waterUserIds.indexOf("all") >= 0) {
        waterUserIds = waterUserIds.substr(0, waterUserIds.length - 3);
    }
    else if (waterUserIds == "" || waterUserIds == null) {
        $.messager.alert("提示信息", "请选择用户！");
        return;
    }
    $.ShowMask("数据加载中，请稍等……");
    $.ajax(
    {
        url: "../WebServices/CardUserService.asmx/GetCardUsersByWaterUserIds",
        type: "GET",
        data: { 'loginIdentifer': loginIdentifer, 'waterUserIds': waterUserIds },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                var cardUsers = data.CardUsers;
                tableData = [];
                for (var i = 0; i < cardUsers.length; i++) {
                    var cardUser = cardUsers[i];
                    //更新datagrid
                    var tableRow = {};
                    tableRow["ID"] = cardUser["ID"];
                    tableRow["SerialNumber"] = cardUser["SerialNumber"];
                    tableRow["UserNo"] = cardUser["UserNo"];
                    tableRow["UserName"] = cardUser["UserName"];
                    tableRow["IdentityNumber"] = cardUser["IdentityNumber"];
                    tableRow["Telephone"] = cardUser["Telephone"];
                    tableRow["ResidualWater"] = cardUser["ResidualWater"];
                    tableRow["ResidualElectric"] = cardUser["ResidualElectric"];
                    tableRow["TotalWater"] = cardUser["TotalWater"];
                    tableRow["TotalElectric"] = cardUser["TotalElectric"];
                    tableRow["IsCountermand"] = cardUser["IsCountermand"] == "是" ? "<span style='color:red;bold:weight'>" + cardUser["IsCountermand"] + "</span>" : cardUser["IsCountermand"];
                    tableRow["OpenTime"] = cardUser["OpenTime"];
                    tableRow["LastChargeTime"] = cardUser["LastChargeTime"];

                    tableRow["Edit"] = "<img src='../Images/Detail.gif' onclick='ShowCardUser(" + cardUser["ID"] + ")' />";
                    tableRow["Cancel"] = "<img src='../Images/Delete.gif' onclick=\"CancelCardUser('" + cardUser["SerialNumber"] + "')\" />";
                    tableRow["Countermand"] = getCountermandStr(cardUser["IsCountermand"], cardUser["SerialNumber"]);
                    tableData.push(tableRow);
                }
                $('#tbCardUserInfos').datagrid({ loadFilter: pagerFilter }).datagrid('loadData', tableData);
                $.HideMask();
            }
            else {
                $.HideMask();
                $.messager.alert("提示信息", data.Message);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.HideMask();
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

//点击用水定额列表的
function Btn_Cancel_AddUserWaterData() {
    $('#ShowCardUserData').dialog({ closed: true });
}

function CancelCardUser(v) {
    if (!confirm("确定要注销卡序列号" + v + "？")) {
        return;
    }
    $.ShowMask("正在注销，请稍等……");
    $.ajax(
    {
        url: "../Ajaxjson.ashx?Method=CancelCardUser",
        type: "post",
        data: { 'loginIdentifer': loginIdentifer, 'SerialNumber': v },
        dataType: "json",
        cache: false,
        success: function (data) {
            $.HideMask();
            if (data.Result) {
                $.messager.alert("提示信息", "注销成功");
                Btn_Query_Click();
            }
            else {
                $.messager.alert("提示信息", data.Message);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.HideMask();
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function CountermandCardUser(v) {
    if (!confirm("确定要挂失卡序列号" + v + "？")) {
        return;
    }
    //alert("挂失：" + v);
    $.ShowMask("正在挂失，请稍等……");
    $.ajax(
    {
        url: "../Ajaxjson.ashx?Method=CountermandCardUser",
        type: "post",
        data: { 'loginIdentifer': loginIdentifer, 'SerialNumber': v },
        dataType: "json",
        cache: false,
        success: function (data) {
            $.HideMask();
            if (data.Result) {
                $.messager.alert("提示信息", "注销成功");
                Btn_Query_Click();
            }
            else {
                $.messager.alert("提示信息", data.Message);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.HideMask();
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function CountermandCancelCardUser(v) {
    if (!confirm("确定要取消挂失卡序列号" + v + "？")) {
        return;
    }
    //alert("取消挂失：" + v);
    $.ShowMask("正在挂失，请稍等……");
    $.ajax(
    {
        url: "../Ajaxjson.ashx?Method=CountermandCancelCardUser",
        type: "post",
        data: { 'loginIdentifer': loginIdentifer, 'SerialNumber': v },
        dataType: "json",
        cache: false,
        success: function (data) {
            $.HideMask();
            if (data.Result) {
                $.messager.alert("提示信息", "注销成功");
                Btn_Query_Click();
            }
            else {
                $.messager.alert("提示信息", data.Message);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.HideMask();
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function getCountermandStr(a, b) {
    if (a == "是") {
        return "<a style=\"color:black;\" href='javascript:void(0)' onclick=\"CountermandCancelCardUser('" + b + "')\">取消挂失</a>";
    } else {
        return "<a style=\"color:black;\" href='javascript:void(0)' onclick=\"CountermandCardUser('" + b + "')\">挂失</a>";
    }
}
