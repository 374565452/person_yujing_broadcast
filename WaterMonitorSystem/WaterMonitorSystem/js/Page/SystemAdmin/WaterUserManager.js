// JScript 文件


//--------------------
//登录标示
var loginIdentifer = window.parent.guid;
//导出路径
var ExcUrl = "";
//水价信息
var waterPriceInfos = {};
//电价信息
var electricPriceInfos = {};

//待编辑的用水户ID
var UserID = "";
//用于存储查询出来的用水户信息
var waterUserObj = {};
//用于存储单位定额对应的亩均
var UnitQuotaObjs = {};
//用于存储添加单位定额的对象数组
var UnitQuotaObjArray = [];
//用于存储界面上点击删除的用水定额的ID
var deleteQuotaId = [];

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
                GetWaterPriceInfo();
                GetElectricPriceInfo();
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
                LoadWaterUserInfo("../WebServices/WaterUserService.asmx/GetWaterUsersByVillageId", { "loginIdentifer": window.parent.guid, "villageId": villageid, "isExport": false });
            }
        }
    });
}

function LoadWaterUserInfo(url, data) {
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
                var waterUsers = data.WaterUsers;
                var comboBoxDataLevel = [];
                var tableData = [];
                allcomboid = "";
                for (var i = 0; i < waterUsers.length; i++) {
                    var waterUser = waterUsers[i];
                    //更新datagrid
                    var tableRow = {};
                    tableRow["userID"] = waterUser["ID"];
                    tableRow["userName"] = waterUser["名称"];
                    tableRow["IdentityNumber"] = waterUser["身份证号"];
                    tableRow["Telephone"] = waterUser["电话"];
                    tableRow["Address"] = waterUser["地址"];
                    if (waterPriceInfos[waterUser["水价ID"]]) {
                        tableRow["WaterPrice"] = waterPriceInfos[waterUser["水价ID"]]["名称"];
                    }
                    if (electricPriceInfos[waterUser["电价ID"]]) {
                        tableRow["ElectricPrice"] = electricPriceInfos[waterUser["电价ID"]]["名称"];
                    }
                    tableRow["WaterQuota"] = waterUser["用水定额"];
                    tableRow["ElectricQuota"] = waterUser["用电定额"];
                    tableRow["State"] = waterUser["状态"];

                    tableRow["Edit"] = "<img src='../Images/Detail.gif' onclick='EditUserWater(" + waterUser["ID"] + ")' />";
                    tableRow["Cancel"] = "<img src='../Images/Delete.gif' onclick='WtiteOffUserWater(" + waterUser["ID"] + ")' />";
                    tableData.push(tableRow);
                    //更新combobox
                    var comboBoxData = {};
                    comboBoxData["id"] = waterUser["ID"];
                    comboBoxData["text"] = waterUser["名称"];// + "(" + waterUser["户号"] + ")"
                    comboBoxDataLevel.push(comboBoxData);
                    if (allcomboid != "") {
                        allcomboid += ",";
                    }
                    allcomboid += waterUser["ID"];

                    //向用水户对象中添加数据
                    waterUserObj[waterUser["ID"]] = waterUser;
                }
                var allObj = {};
                allObj["id"] = allcomboid + "all";
                allObj["text"] = "全部";
                allObj["selected"] = true;
                comboBoxDataLevel.unshift(allObj);
                $('#tbWaterUserInfos').datagrid({ loadFilter: pagerFilter }).datagrid('loadData', tableData);
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
//水价信息
function GetWaterPriceInfo() {
    $.ajax(
    {
        url: "../WebServices/PriceManageService.asmx/GetWaterPriceInfos",
        type: "GET",
        data: { 'loginIdentifer': loginIdentifer },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                var waterPriceJson = data.PriceInfos;
                waterPriceInfos = {};
                var waterPriceComDatas = [];
                for (var i = 0; i < waterPriceJson.length; i++) {
                    var waterPriceComData = {};
                    waterPriceComData["id"] = waterPriceJson[i]["ID"];
                    waterPriceComData["text"] = waterPriceJson[i]["名称"];
                    waterPriceComDatas.push(waterPriceComData);

                    waterPriceInfos[waterPriceJson[i]["ID"]] = waterPriceJson[i];
                }
                $("#ccb_WaterPrice").combobox({ data: waterPriceComDatas });
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
//电价信息
function GetElectricPriceInfo() {
    $.ajax(
    {
        url: "../WebServices/PriceManageService.asmx/GetPowerPriceInfos",
        type: "GET",
        data: { 'loginIdentifer': loginIdentifer },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                var elecPriceJson = data.PriceInfos;
                electricPriceInfos = {};
                var elecPriceComDatas = [];
                for (var i = 0; i < elecPriceJson.length; i++) {
                    var elecPriceComData = {};
                    elecPriceComData["id"] = elecPriceJson[i]["ID"];
                    elecPriceComData["text"] = elecPriceJson[i]["名称"];
                    elecPriceComDatas.push(elecPriceComData);

                    electricPriceInfos[elecPriceJson[i]["ID"]] = elecPriceJson[i];
                }
                $("#ccb_ElectricPrice").combobox({ data: elecPriceComDatas });
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

function Btn_Query_Click() {
    $('#tbWaterUserInfos').datagrid('loadData', { total: 0, rows: [] });
    var waterUserIds = $.QueryCombobox("cbb_UserNameCombobox");

    if (waterUserIds.indexOf("all") >= 0) {
        waterUserIds = waterUserIds.substr(0, waterUserIds.length - 3);
    }
    if (waterUserIds == "" || waterUserIds == null) {
        $.messager.alert("提示信息", "请选择用户！");
        return;
    }
    $.ShowMask("数据加载中，请稍等……");
    $.ajax(
    {
        url: "../WebServices/WaterUserService.asmx/GetWaterUsersHasQuotasByIds",
        type: "GET",
        data: { 'loginIdentifer': loginIdentifer, 'waterUserIds': waterUserIds },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                var waterUserInfos = data.WaterUsers;
                tableData = [];
                for (var i = 0; i < waterUserInfos.length; i++) {
                    var tableRow = {};
                    var waterUserInfo = waterUserInfos[i];
                    tableRow["userID"] = waterUser["ID"];
                    tableRow["userName"] = waterUser["名称"];
                    tableRow["IdentityNumber"] = waterUser["身份证号"];
                    tableRow["Telephone"] = waterUser["电话"];
                    tableRow["Address"] = waterUser["地址"];
                    if (waterPriceInfos[waterUser["水价ID"]]) {
                        tableRow["WaterPrice"] = waterPriceInfos[waterUser["水价ID"]]["名称"];
                    }
                    if (electricPriceInfos[waterUser["电价ID"]]) {
                        tableRow["ElectricPrice"] = electricPriceInfos[waterUser["电价ID"]]["名称"];
                    }
                    tableRow["WaterQuota"] = waterUser["用水定额"];
                    tableRow["ElectricQuota"] = waterUser["用电定额"];
                    tableRow["State"] = waterUser["状态"];

                    tableRow["Edit"] = "<img src='../Images/Detail.gif' onclick='EditUserWater(" + waterUserInfo["ID"] + ")'/>";
                    tableRow["Cancel"] = "<img src='../Images/Delete.gif' onclick='WtiteOffUserWater(" + waterUserInfo["ID"] + ")' />";
                    tableData.push(tableRow);
                }
                $('#tbWaterUserInfos').datagrid({ loadFilter: pagerFilter }).datagrid('loadData', tableData);
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

//添加按钮事件
function Btn_Add_Click() {
    //初始化存储单位定额对象的数组
    UnitQuotaObjArray = [];
    $('#tbWaterUserQuotaList').datagrid('loadData', { total: 0, rows: [] });
    $("#tbWaterUserQuotaList").datagrid('reloadFooter', [{ "unitQuotaName": "合计", "acreage": 0, "totalQuota": 0 }]);

    //$("#txt_userNum").textbox("setText", "");
    //$("#txt_userNum").textbox({ disabled: false });
    $('#AddUserWaterData').dialog({ closed: false, title: "添加用水户信息" });
    $("#txt_userName").textbox("setText", "");
    $("#txt_Address").textbox("setText", "");
    $("#ccb_WaterPrice").combobox('setValue', "");
    $("#ccb_ElectricPrice").combobox('setValue', "");

    $("#txt_Telephone").textbox("setText", "");
    $("#txt_IdentityNumber").textbox("setText", "");
    $("#txt_WaterQuota").textbox("setText", "");
    $("#txt_ElectricQuota").textbox("setText", "");

    //初始化用水户定额列表的年份下拉框（默认是当前年份和下一年）
    //Initccb_YearDefined();

    //初始化用水定额添加或修改页面
    //InitUnitQuota();
    if ($("#ccb_Year").combobox('getValue') == "") {
        var year = (new Date).getFullYear();
        var yearObj = {};
        comboBoxDatas = [];
        yearObj["id"] = year;
        yearObj["text"] = year;
        yearObj["selected"] = true;
        comboBoxDatas.push(yearObj);
        $("#ccb_YearQuota").combobox({
            data: comboBoxDatas
        });
    }
    else {
        $("#ccb_YearQuota").combobox('setValue', $("#ccb_Year").combobox('getValue'));
    }
    if (waterUserObj[waterUserId] != null) {
        $("#txt_WaterUser").textbox("setText", waterUserObj[waterUserId].名称);
    }
    $("#txt_Acreage").textbox("setText", "");
    $("#txt_Quota").textbox('setValue', "");
}

////点击下一步按钮事件
//function Btn_Next()
//{
//    if($('#AddUserWaterData').panel('options').title == "添加用水户信息")
//    {
//        if($("#txt_userNum").textbox("getText")==""||$("#txt_userName").textbox('getText')==""||$("#ccb_WaterPrice").combobox('getValue')==""||$("#ccb_ElectricPrice").combobox('getValue')==""||$("#txt_userName").textbox('getText').trim()=="") 
//         {
//             $.messager.alert("提示信息", "请将信息填写完整!");
//            return;
//         } 
//         if(isNaN($("#txt_userNum").textbox("getText"))||$("#txt_userNum").textbox("getText")<0||$("#txt_userNum").textbox("getText").length != 4||$("#txt_userNum").textbox("getText").indexOf(".")>=0||$("#txt_userNum").textbox("getText").trim()=="") 
//         {
//             $.messager.alert("提示信息", "请输入四位数字");
//            $("#txt_userNum").textbox("setText","");
//            return;
//         } 
//        $('#OperateWaterUserQuota').dialog({ closed: false, title:"添加用水户定额"});
//        $('#AddUserWaterData').dialog({ closed: true});
//        $("#A1_WaterUserQuotaDetail").attr("disabled",false);
//        $("#txt_WaterUser").textbox("setText",$("#txt_userName").textbox("getText"));
//    }
//    if($('#AddUserWaterData').panel('options').title == "编辑用水户信息")
//    {
//        //EditQuota(UserID);
//        //EditWaterUserQuota(WaterUserQuotaId);
//    }
//}

//function Btn_Back()
//{
//    $('#AddUserWaterData').dialog({ closed: false });
//}

function Btn_OK_Save() {
    if ($('#AddUserWaterData').panel('options').title == "添加用水户信息") {

        var SelectNode = $("#WaterUserTree").tree('getSelected');
        if ($("#txt_Telephone").textbox('getText').trim() == "" || $("#txt_IdentityNumber").textbox('getText').trim() == ""
                || $("#txt_WaterQuota").textbox('getText').trim() == "" || $("#txt_ElectricQuota").textbox('getText').trim() == ""
                || $("#ccb_WaterPrice").combobox('getValue') == "" || $("#ccb_ElectricPrice").combobox('getValue') == ""
                || $("#txt_userName").textbox('getText').trim() == "") {
            $.messager.alert("提示信息", "请将信息填写完整!");
            return;
        }
        /*
        if (isNaN($("#txt_userNum").textbox("getText")) || $("#txt_userNum").textbox("getText") < 0 || $("#txt_userNum").textbox("getText").length != 4 || $("#txt_userNum").textbox("getText").indexOf(".") >= 0 || $("#txt_userNum").textbox("getText").trim() == "") {
            $.messager.alert("提示信息", "请输入四位数字");
            $("#txt_userNum").textbox("setText", "");
            return;
        }
        */

        if (isNaN($("#txt_IdentityNumber").textbox("getText").trim()) || $("#txt_IdentityNumber").textbox("getText").trim().length != 18 || $("#txt_IdentityNumber").textbox("getText").indexOf(".") >= 0) {
            $.messager.alert("提示信息", "身份证号只能为18位");
            $("#txt_IdentityNumber").textbox("setText", "");
            return;
        }

        if (isNaN($("#txt_Telephone").textbox("getText").trim()) || $("#txt_Telephone").textbox("getText").trim().length != 11 || $("#txt_Telephone").textbox("getText").indexOf(".") >= 0) {
            $.messager.alert("提示信息", "电话号码只能为11位数字");
            $("#txt_Telephone").textbox("setText", "");
            return;
        }
        
        var StrWaterUser = "{";
        //StrWaterUser += "'户号'" + ":'" + $("#txt_userNum").textbox("getText") + "',";
        StrWaterUser += "'名称'" + ":'" + $("#txt_userName").textbox('getText') + "',";
        StrWaterUser += "'水价ID'" + ":'" + $("#ccb_WaterPrice").combobox('getValue') + "',";
        StrWaterUser += "'电价ID'" + ":'" + $("#ccb_ElectricPrice").combobox('getValue') + "',";
        StrWaterUser += "'地址'" + ":'" + $("#txt_Address").textbox('getText') + "',";
        StrWaterUser += "'电话'" + ":'" + $("#txt_Telephone").textbox('getText') + "',";
        StrWaterUser += "'身份证号'" + ":'" + $("#txt_IdentityNumber").textbox('getText') + "',";
        StrWaterUser += "'用水定额'" + ":'" + $("#txt_WaterQuota").textbox('getText') + "',";
        StrWaterUser += "'用电定额'" + ":'" + $("#txt_ElectricQuota").textbox('getText') + "',";
        StrWaterUser += "'管理ID'" + ":'" + SelectNode.attributes["mid"] + "'";
        StrWaterUser += "}";
        alert(StrWaterUser);
        $.ajax(
       {
           url: "../WebServices/WaterUserService.asmx/AddWaterUser",
           type: "GET",
           data: { 'loginIdentifer': loginIdentifer, 'waterUserJson': StrWaterUser },
           dataType: "text",
           cache: false,
           success: function (responseText) {
               var data = eval("(" + $.xml2json(responseText) + ")");
               if (data.Result) {
                   $.messager.alert("提示信息", "添加用水户信息成功");
                   LoadWaterUserInfo("../WebServices/WaterUserService.asmx/GetWaterUsersHasQuotasByVillageId", { "loginIdentifer": window.parent.guid, "villageId": villageid, "isExport": false });
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
    if ($('#AddUserWaterData').panel('options').title == "编辑用水户信息") {
        if ($("#txt_Telephone").textbox('getText').trim() == "" || $("#txt_IdentityNumber").textbox('getText').trim() == ""
                || $("#txt_WaterQuota").textbox('getText').trim() == "" || $("#txt_ElectricQuota").textbox('getText').trim() == ""
                || $("#ccb_WaterPrice").combobox('getValue') == "" || $("#ccb_ElectricPrice").combobox('getValue') == ""
                || $("#txt_userName").textbox('getText').trim() == "") {
            $.messager.alert("提示信息", "请将信息填写完整!");
            return;
        }
        /*
        if (isNaN($("#txt_userNum").textbox("getText")) || $("#txt_userNum").textbox("getText") < 0 || $("#txt_userNum").textbox("getText").length != 4 || $("#txt_userNum").textbox("getText").indexOf(".") >= 0 || $("#txt_userNum").textbox("getText").trim() == "") {
            $.messager.alert("提示信息", "请输入四位数字");
            $("#txt_userNum").textbox("setText", "");
            return;
        }
        */

        if (isNaN($("#txt_IdentityNumber").textbox("getText").trim()) || $("#txt_IdentityNumber").textbox("getText").trim().length != 18 || $("#txt_IdentityNumber").textbox("getText").indexOf(".") >= 0) {
            $.messager.alert("提示信息", "身份证号只能为18位");
            $("#txt_IdentityNumber").textbox("setText", "");
            return;
        }

        if (isNaN($("#txt_Telephone").textbox("getText").trim()) || $("#txt_Telephone").textbox("getText").trim().length != 11 || $("#txt_Telephone").textbox("getText").indexOf(".") >= 0) {
            $.messager.alert("提示信息", "电话号码只能为11位数字");
            $("#txt_Telephone").textbox("setText", "");
            return;
        }

        var SelectNode = $("#WaterUserTree").tree('getSelected');
        var StrWaterUser = "{";
        StrWaterUser += "'ID'" + ":'" + UserID + "',";
        //StrWaterUser += "'户号'" + ":'" + $("#txt_userNum").textbox("getText") + "',";
        StrWaterUser += "'名称'" + ":'" + $("#txt_userName").textbox('getText') + "',";
        StrWaterUser += "'水价ID'" + ":'" + $("#ccb_WaterPrice").combobox('getValue') + "',";
        StrWaterUser += "'电价ID'" + ":'" + $("#ccb_ElectricPrice").combobox('getValue') + "',";
        StrWaterUser += "'地址'" + ":'" + $("#txt_Address").textbox('getText') + "',";
        StrWaterUser += "'电话'" + ":'" + $("#txt_Telephone").textbox('getText') + "',";
        StrWaterUser += "'身份证号'" + ":'" + $("#txt_IdentityNumber").textbox('getText') + "',";
        StrWaterUser += "'用水定额'" + ":'" + $("#txt_WaterQuota").textbox('getText') + "',";
        StrWaterUser += "'用电定额'" + ":'" + $("#txt_ElectricQuota").textbox('getText') + "',";
        StrWaterUser += "'管理ID'" + ":'" + SelectNode.attributes["mid"] + "'";
        StrWaterUser += "}";
        $.ajax(
        {
            url: "../WebServices/WaterUserService.asmx/ModifyWaterUser",
            type: "GET",
            data: { 'loginIdentifer': loginIdentifer, 'waterUserJson': StrWaterUser },
            dataType: "text",
            cache: false,
            success: function (responseText) {
                var data = eval("(" + $.xml2json(responseText) + ")");
                if (data.Result) {
                    $.messager.alert("提示信息", "编辑用水户信息成功");
                    LoadWaterUserInfo("../WebServices/WaterUserService.asmx/GetWaterUsersHasQuotasByVillageId", { "loginIdentifer": window.parent.guid, "villageId": villageid, "isExport": false });
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
    $('#AddUserWaterData').dialog({ closed: true });
}

function Btn_Cancel_Exit() {
    $('#AddUserWaterData').dialog({ closed: true });
}

//初始化用水定额列表的年份下拉框（默认是当前年份和下一年）
function Initccb_YearDefined() {
    var comboBoxDatas = [];
    var year = (new Date).getFullYear();
    var yearObj = {};
    yearObj["id"] = year;
    yearObj["text"] = year;
    yearObj["selected"] = true;
    comboBoxDatas.push(yearObj);
    var yearObjNext = {};
    yearObjNext["id"] = year + 1;
    yearObjNext["text"] = year + 1;
    comboBoxDatas.push(yearObjNext);
    $("#ccb_Year").combobox({
        data: comboBoxDatas
    });
}

//初始化用水定额列表的年份下拉框(当通过点击用水户的编辑弹出用水定额列表界面时，年份下拉框中的值是读取数据库此用水户所有用水定额中的年份)
function Initccb_YearData(_waterUserId) {
    $.ajax(
    {
        url: "../WebServices/WaterUserService.asmx/GetWaterUserQuotaYearByWaterUserId",
        type: "GET",
        data: { 'loginIdentifer': loginIdentifer, 'waterUserId': _waterUserId },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                var year = (new Date).getFullYear();
                var isExits = false;
                var _years = data.Years;
                var comboBoxDatas = [];
                for (var i = 0; i < _years.length; i++) {
                    var yearObj = {};
                    yearObj["id"] = _years[i]["年份"];
                    yearObj["text"] = _years[i]["年份"];
                    comboBoxDatas.push(yearObj);
                    if (year == _years[i]["年份"]) {
                        yearObj["selected"] = true;
                    }
                    if (year + 1 == _years[i]["年份"]) {
                        isExits = true;
                    }
                }
                if (_years.length == 0) {
                    var yearObj = {};
                    yearObj["id"] = year;
                    yearObj["text"] = year;
                    yearObj["selected"] = true;
                    comboBoxDatas.push(yearObj);
                }
                if (!isExits) {
                    var yearObj = {};
                    yearObj["id"] = year + 1;
                    yearObj["text"] = year + 1;
                    comboBoxDatas.push(yearObj);
                }
                $("#ccb_Year").combobox({
                    data: comboBoxDatas
                });
                $("#ccb_YearQuota").combobox({
                    data: comboBoxDatas
                });
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

function EditUserWater(WaterUserID) {
    $('#tbWaterUserQuotaList').datagrid('loadData', { total: 0, rows: [] });
    $("#tbWaterUserQuotaList").datagrid('reloadFooter', [{ "unitQuotaName": "合计", "acreage": 0, "totalQuota": 0 }]);
    UnitQuotaObjArray = [];
    deleteQuotaId = [];
    UserID = WaterUserID;
    //$("#txt_userNum").textbox({ disabled: true });
    $('#AddUserWaterData').dialog({ closed: false });
    $('#AddUserWaterData').dialog({ title: "编辑用水户信息" });
    //Initccb_YearData(WaterUserID);
    //InitUnitQuota();
    $.ajax(
    {
        url: "../WebServices/WaterUserService.asmx/GetWaterUserHasQuotasById",
        type: "GET",
        data: { 'loginIdentifer': loginIdentifer, 'waterUserId': WaterUserID },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                WaterUserInfo = data.WaterUser;
                //$("#txt_userNum").textbox("setText", WaterUserInfo["户号"]);
                $("#txt_userName").textbox("setText", WaterUserInfo["名称"]);
                $("#txt_Address").textbox("setText", WaterUserInfo["地址"]);
                if (waterPriceInfos[WaterUserInfo["水价ID"]]) {
                    $("#ccb_WaterPrice").combobox('setValue', WaterUserInfo["水价ID"]);
                }
                if (electricPriceInfos[WaterUserInfo["电价ID"]]) {
                    $("#ccb_ElectricPrice").combobox('setValue', WaterUserInfo["电价ID"]);
                }
                $("#txt_IdentityNumber").textbox("setText", WaterUserInfo["身份证号"]);
                $("#txt_Telephone").textbox("setText", WaterUserInfo["电话"]);
                $("#txt_WaterQuota").textbox("setText", WaterUserInfo["用水定额"]);
                $("#txt_ElectricQuota").textbox("setText", WaterUserInfo["用电定额"]);

                //绑定用水户定额列表(默认显示当前年的用水定额)
                /*
                var _year = $("#ccb_Year").combobox('getValue');
                var _quotaList = WaterUserInfo["用水定额列表"];
                if (_quotaList != null && _quotaList.length > 0) {
                    for (var i = 0; i < _quotaList.length; i++) {
                        if (_quotaList[i]["单位定额名称"] == "合计") {
                            continue;
                        }
                        var _unitQuotaObj = {};
                        _unitQuotaObj["ID"] = _quotaList[i]["ID"];
                        _unitQuotaObj["Year"] = _quotaList[i]["年份"];;
                        _unitQuotaObj["UnitQuotaID"] = _quotaList[i]["单位定额ID"];
                        _unitQuotaObj["UnitQuotaName"] = _quotaList[i]["单位定额名称"];
                        _unitQuotaObj["UnitQuota"] = _quotaList[i]["单位定额"];;
                        _unitQuotaObj["Acreage"] = _quotaList[i]["面积"];;
                        _unitQuotaObj["Quota"] = _quotaList[i]["定额"];;
                        UnitQuotaObjArray.push(_unitQuotaObj);
                    }
                    //QueryQuotaByYear();
                }
                */
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

function WtiteOffUserWater(userID) {
    $.messager.confirm("提示信息", "是否注销用水户", function (r) {
        if (r) {
            $.ajax(
        {
            url: "../WebServices/WaterUserService.asmx/WriteOffWaterUserById",
            type: "GET",
            data: { 'loginIdentifer': loginIdentifer, 'waterUserId': userID },
            dataType: "text",
            cache: false,
            success: function (responseText) {
                var data = eval("(" + $.xml2json(responseText) + ")");
                if (data.Result) {
                    $.messager.alert("提示信息", "注销用水户信息成功");
                    LoadWaterUserInfo("../WebServices/WaterUserService.asmx/GetWaterUsersHasQuotasByVillageId", { "loginIdentifer": window.parent.guid, "villageId": villageid, "isExport": false });
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
    });
}

function Btn_Excel_Click() {

}

//=========================用水定额的操作开始=========================//

//=================声明变量=================//
//用水户ID
var waterUserId;
//用水户定额ID
var waterUserQuotaId;
//用于标识是修改还是添加用水定额
var operate = "NEW";
//当前操作的用水户名称
var waterUserName = "";
//用于存储下拉框中的年
var comboBoxDatasYear = [];
//用于存储点击用水定额列表编辑时的用水定额ID（也就是行号）
var quotaRowId = "";

////点击用户信息列表的编辑定额弹出此用水户的用水定额列表
//function EditQuota(WaterUserID)
//{
//    waterUserId=WaterUserID;
//    $('#WaterUserQuotaList').dialog({ closed: false, title:"用水定额列表"});    
//    $.ajax(
//    {
//        url: "../WebServices/WaterUserService.asmx/GetWaterUserQuotaYearByWaterUserId",
//        type: "GET",
//        data: { 'loginIdentifer':loginIdentifer, 'waterUserId':waterUserId},
//        dataType: "text",
//        cache: false,
//        success: function (responseText) 
//        { 
//            var data = eval("(" + $.xml2json(responseText) + ")");
//            if (data.Result) 
//            { 
//                _years = data.Years;
//                comboBoxDatasYear=[]; 
//                for(var i=0;i<_years.length;i++)
//                {
//                    var yearObj = {};
//                    yearObj["id"] = _years[i]["年份"];
//                    yearObj["text"] =_years[i]["年份"];
//                    comboBoxDatas.push(yearObj);
//                    if(year==_years[i]["年份"])
//                    {
//                        yearObj["selected"] = true;
//                    }
//                    if(year+1==_years[i]["年份"])
//                    {
//                        isExits=true;
//                    }
//                }
//                if(_years.length==0)
//                {
//                    var yearObj = {};
//                    yearObj["id"] = year;
//                    yearObj["text"] =year;
//                    yearObj["selected"] = true;
//                    comboBoxDatasYear.push(yearObj);
//                }
//                if(!isExits)
//                {
//                    var yearObj = {};
//                    yearObj["id"] = year+1;
//                    yearObj["text"] =year+1;
//                    comboBoxDatas.push(yearObj);
//                }
//                $("#ccb_Year").combobox({
//                    data: comboBoxDatasYear
//                });
//                InitComboboxYear(WaterUserID);
//            }
//            else
//            {
//                $.messager.alert("提示信息", data.Message);
//            }      
//        },
//        error: function (XMLHttpRequest, textStatus, errorThrown) 
//        {
//            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
//        }
//     });
//    GetWaterUserQuotaByWaterUserId(WaterUserID,year);
//    //=============================初始化下拉框=============================//
//    //获取所有的单位定额
//    InitUnitQuota();
//}

//年份下拉框改变时，根据年查询的用水定额列表随之变化
function QueryQuotaByYear() {
    var _year = $("#ccb_Year").combobox('getValue');
    $("#ccb_YearQuota").combobox('setValue', _year);
    //用于存储面积和定额的合计
    var _totalAcreage = 0;
    var _totalQuota = 0;
    var tableData = [];
    for (var i = 0; i < UnitQuotaObjArray.length; i++) {
        if (UnitQuotaObjArray[i].Year == _year) {
            //更新datagrid
            var tableRow = {};
            tableRow["quotaID"] = UnitQuotaObjArray[i].ID;
            tableRow["unitQuotaName"] = UnitQuotaObjArray[i].UnitQuotaName;
            tableRow["unitQuota"] = UnitQuotaObjArray[i].UnitQuota;
            tableRow["acreage"] = UnitQuotaObjArray[i].Acreage;
            tableRow["totalQuota"] = UnitQuotaObjArray[i].Quota;
            tableRow["Edit"] = "<img src='../Images/Detail.gif' onclick='EditWaterUserQuota(" + UnitQuotaObjArray[i].ID + ")' />";
            tableRow["Cancel"] = "<img src='../Images/Delete.gif' onclick='DeleteWaterUserQuota(" + UnitQuotaObjArray[i].ID + ")' />";
            tableData.push(tableRow);
            //计算合计
            _totalAcreage += parseFloat(UnitQuotaObjArray[i].Acreage);
            _totalQuota += parseFloat(UnitQuotaObjArray[i].Quota);
        }
        $('#tbWaterUserQuotaList').datagrid('loadData', tableData);
        $("#tbWaterUserQuotaList").datagrid('reloadFooter', [{ "unitQuotaName": "合计", "acreage": _totalAcreage, "totalQuota": _totalQuota }]);
    }
}

//根据用水户ID和年份得到此人的用水定额
function GetWaterUserQuotaByWaterUserId(_waterUserId, _year) {
    //根据用水户ID查询用水定额
    $.ajax(
    {
        url: "../WebServices/WaterUserService.asmx/GetWaterUserQuotaByWaterUserId",
        type: "GET",
        data: { "loginIdentifer": window.parent.guid, "waterUserId": _waterUserId, "year": _year },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result)//登录成功
            {
                quotaJson = data.WaterUserQuota;
                tableData = [];
                //用于存储面积和定额的合计
                var totalAcreage = "";
                var totalQuota = "";

                for (i = 0; i < quotaJson.length; i++) {
                    if (quotaJson[i]["单位定额名称"] == "合计") {
                        totalAcreage = quotaJson[i]["面积"];
                        totalQuota = quotaJson[i]["定额"];
                        continue;
                    }
                    //更新datagrid
                    var tableRow = {};
                    tableRow["quotaID"] = quotaJson[i]["ID"];
                    tableRow["unitQuotaName"] = quotaJson[i]["单位定额名称"];
                    tableRow["unitQuota"] = quotaJson[i]["单位定额"];
                    tableRow["acreage"] = quotaJson[i]["面积"];
                    tableRow["totalQuota"] = quotaJson[i]["定额"];
                    tableRow["Edit"] = "<img src='../Images/Detail.gif' onclick='EditWaterUserQuota(" + quotaJson[i]["ID"] + ")' />";
                    tableRow["Cancel"] = "<img src='../Images/Delete.gif' onclick='DeleteWaterUserQuota(" + quotaJson[i]["ID"] + ")' />";
                    tableData.push(tableRow);
                }
                $('#tbWaterUserQuotaList').datagrid('loadData', tableData);
                $("#tbWaterUserQuotaList").datagrid('reloadFooter', [{ "unitQuotaName": "合计", "acreage": totalAcreage, "totalQuota": totalQuota }]);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

//点击用水定额列表的取消按钮
function Btn_CancelQuota_Exit() {
    $('#WaterUserQuotaList').dialog({ closed: true });
}

//初始化单位定额下拉框
function InitUnitQuota() {
    $.ajax(
     {
         url: "../WebServices/QuotaManageService.asmx/GetUnitQuotasByType",
         type: "GET",
         data: { "loginIdentifer": window.parent.guid, "unitQuotaType": "作物" },
         dataType: "text",
         cache: false,
         success: function (responseText) {
             var data = eval("(" + $.xml2json(responseText) + ")");
             if (data.Result) {
                 _unitQuotas = data.UnitQuotas;
                 var comboBoxDataLevel = [];
                 for (var i = 0; i < _unitQuotas.length; i++) {
                     var unitQuotaObj = {};
                     unitQuotaObj["id"] = _unitQuotas[i]["ID"];
                     unitQuotaObj["text"] = _unitQuotas[i]["名称"];
                     if (i == 0) {
                         unitQuotaObj["selected"] = true;
                     }
                     comboBoxDataLevel.push(unitQuotaObj);
                     UnitQuotaObjs[_unitQuotas[i]["ID"]] = _unitQuotas[i]["单位定额"];
                 }
                 $("#ccb_UnitQuotaName").combobox({
                     data: comboBoxDataLevel
                 });
                 UnitQuotaTypeChange();
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

//单位定额下拉框改变事件
function UnitQuotaTypeChange() {
    var _unitQuatoId = $("#ccb_UnitQuotaName").combobox('getValue');
    if (UnitQuotaObjs[_unitQuatoId] != null) {
        $("#txt_UnitQuota").textbox("setText", UnitQuotaObjs[_unitQuatoId]);
        CalculateAcreage();
    }
}
//当面积文本框中的文本改变时，自动计算定额文本框中的值
function CalculateAcreage() {
    var unitQuota = $("#txt_UnitQuota").textbox('getText');
    var acreage = $("#txt_Acreage").textbox('getText');
    var _quota = parseFloat(unitQuota) * parseFloat(acreage);
    var re = /^\d+(?=\.{0,1}\d+$|$)/;
    if (acreage != "" && (re.test(acreage))) {
        $("#txt_Quota").textbox("setText", _quota);
    }
}

function EditWaterUserQuota(_waterUserQuotaId) {
    //先判断是否将新增的用水户的信息填写完整
    if ($("#txt_userNum").textbox("getText") == "" || $("#txt_userName").textbox('getText') == "" || $("#ccb_WaterPrice").combobox('getValue') == "" || $("#ccb_ElectricPrice").combobox('getValue') == "" || $("#txt_userName").textbox('getText').trim() == "") {
        $.messager.alert("提示信息", "请将用水户信息填写完整!");
        return;
    }
    if (isNaN($("#txt_userNum").textbox("getText")) || $("#txt_userNum").textbox("getText") < 0 || $("#txt_userNum").textbox("getText").length != 4 || $("#txt_userNum").textbox("getText").indexOf(".") >= 0 || $("#txt_userNum").textbox("getText").trim() == "") {
        $.messager.alert("提示信息", "请输入四位数字");
        $("#txt_userNum").textbox("setText", "");
        return;
    }

    //获取年份，如果是过去的年份则提示不可编辑
    var _cbbYear = $("#ccb_Year").combobox('getValue');
    var _currentYear = (new Date).getFullYear();
    if (_currentYear > _cbbYear) {
        $.messager.alert("提示信息", "不能编辑过去年份的用水定额!");
        return;
    }

    operate = "MODIFY";
    quotaRowId = _waterUserQuotaId;
    $('#OperateWaterUserQuota').dialog({ title: '编辑用水户定额', closed: false });
    $("#A1_WaterUserQuotaDetail").attr("disabled", false);
    for (var i = 0; i < UnitQuotaObjArray.length; i++) {
        if (UnitQuotaObjArray[i].ID == _waterUserQuotaId) {
            $("#txt_WaterUser").textbox("setText", $("#txt_userName").textbox("getText"));
            $("#ccb_YearQuota").combobox('setValue', UnitQuotaObjArray[i].Year);
            $("#ccb_UnitQuotaName").combobox('setValue', UnitQuotaObjArray[i].UnitQuotaID);
            UnitQuotaTypeChange();
            $("#txt_Acreage").textbox("setText", UnitQuotaObjArray[i].Acreage);
            $("#txt_Quota").textbox("setText", UnitQuotaObjArray[i].Quota);
            break;
        }
    }
}
//删除用水户定额
function DeleteWaterUserQuota(_waterUserQuotaId) {
    _waterUserQuotaId += "";
    if (confirm("确定要删除此用水户定额信息？")) {
        //将用水定额ID不包含Add_的记录添加到数组中
        if (_waterUserQuotaId.indexOf("Add_") < 0) {

            deleteQuotaId.push(_waterUserQuotaId);
        }

        for (var i = 0; i < UnitQuotaObjArray.length; i++) {
            if (UnitQuotaObjArray[i].ID == _waterUserQuotaId) {
                UnitQuotaObjArray.splice(i, 1);
                break;
            }
        }
        //-------更新单位定额列表-------//
        var _year = $("#ccb_Year").combobox('getValue');
        //用于存储面积和定额的合计
        var _totalAcreage = 0;
        var _totalQuota = 0;
        var tableData = [];
        for (i = 0; i < UnitQuotaObjArray.length; i++) {
            if (UnitQuotaObjArray[i].Year == _year) {
                //更新datagrid
                var tableRow = {};
                tableRow["quotaID"] = UnitQuotaObjArray[i].ID;
                tableRow["unitQuotaName"] = UnitQuotaObjArray[i].UnitQuotaName;
                tableRow["unitQuota"] = UnitQuotaObjArray[i].UnitQuota;
                tableRow["acreage"] = UnitQuotaObjArray[i].Acreage;
                tableRow["totalQuota"] = UnitQuotaObjArray[i].Quota;
                tableRow["Edit"] = "<img src='../Images/Detail.gif' onclick='EditWaterUserQuota(" + UnitQuotaObjArray[i].ID + ")' />";
                tableRow["Cancel"] = "<img src='../Images/Delete.gif' onclick='DeleteWaterUserQuota(" + UnitQuotaObjArray[i].ID + ")' />";
                tableData.push(tableRow);
                //计算合计
                _totalAcreage += parseFloat(UnitQuotaObjArray[i].Acreage);
                _totalQuota += parseFloat(UnitQuotaObjArray[i].Quota);
            }
        }
        $('#tbWaterUserQuotaList').datagrid('loadData', tableData);
        $("#tbWaterUserQuotaList").datagrid('reloadFooter', [{ "unitQuotaName": "合计", "acreage": _totalAcreage, "totalQuota": _totalQuota }]);
    }
}

//添加按钮事件
function Btn_AddQuota_Click() {
    //先判断是否将新增的用水户的信息填写完整
    if ($("#txt_userNum").textbox("getText") == "" || $("#txt_userName").textbox('getText') == "" || $("#ccb_WaterPrice").combobox('getValue') == "" || $("#ccb_ElectricPrice").combobox('getValue') == "" || $("#txt_userName").textbox('getText').trim() == "") {
        $.messager.alert("提示信息", "请将用水户信息填写完整!");
        return;
    }
    if (isNaN($("#txt_userNum").textbox("getText")) || $("#txt_userNum").textbox("getText") < 0 || $("#txt_userNum").textbox("getText").length != 4 || $("#txt_userNum").textbox("getText").indexOf(".") >= 0 || $("#txt_userNum").textbox("getText").trim() == "") {
        $.messager.alert("提示信息", "请输入四位数字");
        $("#txt_userNum").textbox("setText", "");
        return;
    }
    operate = "NEW";
    //初始化添加用水定额页面
    $('#OperateWaterUserQuota').dialog({ closed: false, title: "添加用水户定额" });
    $("#A1_WaterUserQuotaDetail").attr("disabled", false);
    $("#txt_WaterUser").textbox("setText", $("#txt_userName").textbox("getText"));
    $("#txt_Acreage").textbox("setText", "");
    $("#txt_Quota").textbox('setValue', "");
}

//添加或修改用水定额(只是添加或修改DataGrid的行，但还未添加到数据库中)
function Btn_OK_OperateWaterUser() {
    var urlStr = "";
    var message = "";
    var _year = $("#ccb_Year").combobox('getValue');
    var yearQuota = $("#ccb_YearQuota").combobox('getValue');
    var unitQuotaId = $("#ccb_UnitQuotaName").combobox('getValue');
    var unitQuotaName = $("#ccb_UnitQuotaName").combobox('getText');
    var unitQuota = $("#txt_UnitQuota").textbox("getText");
    var acreage = $("#txt_Acreage").textbox("getText");
    var quota = $("#txt_Quota").textbox("getText");

    //校验判断
    var reNum = /^\d*$/;
    var re = /^\d+(?=\.{0,1}\d+$|$)/;
    if (yearQuota == null || yearQuota == "") {
        $.messager.alert("提示信息", "年份不能为空！");
        return;
    }
    if (!reNum.test(yearQuota)) {
        $.messager.alert("提示信息", "年份必须是数字!");
        return;
    }
    if (unitQuotaId == null || unitQuotaId == "") {
        $.messager.alert("提示信息", "单位定额不能为空！");
        return;
    }
    if (acreage == null || acreage == "") {
        $.messager.alert("提示信息", "面积不能为空！");
        return;
    }
    else if (!re.test(acreage)) {
        $.messager.alert("提示信息", "面积只能为整数或小数！");
        $("#txt_Acreage").textbox("setValue", "");
        return;
    }
    if (quota == null || quota == "") {
        $.messager.alert("提示信息", "定额不能为空！");
        return;
    }
    else if (!re.test(quota)) {
        $.messager.alert("提示信息", "定额只能为整数或小数！");
        $("#txt_Quota").textbox("setValue", "");
        return;
    }
    $("#A1_WaterUserQuotaDetail").attr("disabled", "disabled");

    //组织对象
    if (operate == "NEW") {
        var _unitQuotaObj = {};
        _unitQuotaObj["ID"] = "Add_" + UnitQuotaObjArray.length + 1;
        _unitQuotaObj["Year"] = yearQuota;
        _unitQuotaObj["UnitQuotaID"] = unitQuotaId;
        _unitQuotaObj["UnitQuotaName"] = unitQuotaName;
        _unitQuotaObj["UnitQuota"] = unitQuota;
        _unitQuotaObj["Acreage"] = acreage;
        _unitQuotaObj["Quota"] = quota;
        UnitQuotaObjArray.push(_unitQuotaObj);
    }
    else if (operate == "MODIFY" && quotaRowId != "") {
        for (i = 0; i < UnitQuotaObjArray.length; i++) {
            if (UnitQuotaObjArray[i].ID == quotaRowId) {
                UnitQuotaObjArray[i].Year = yearQuota;
                UnitQuotaObjArray[i].UnitQuotaID = unitQuotaId;
                UnitQuotaObjArray[i].UnitQuotaName = unitQuotaName;
                UnitQuotaObjArray[i].UnitQuota = unitQuota;
                UnitQuotaObjArray[i].Acreage = acreage;
                UnitQuotaObjArray[i].Quota = quota;
            }
        }
    }
    $('#OperateWaterUserQuota').dialog({ closed: true });
    //-------更新单位定额列表-------//
    //用于存储面积和定额的合计
    var _totalAcreage = 0;
    var _totalQuota = 0;
    var tableData = [];
    for (i = 0; i < UnitQuotaObjArray.length; i++) {
        if (UnitQuotaObjArray[i].Year == _year) {
            //更新datagrid
            var tableRow = {};
            tableRow["quotaID"] = UnitQuotaObjArray[i].ID;;
            tableRow["unitQuotaName"] = UnitQuotaObjArray[i].UnitQuotaName;
            tableRow["unitQuota"] = UnitQuotaObjArray[i].UnitQuota;
            tableRow["acreage"] = UnitQuotaObjArray[i].Acreage;
            tableRow["totalQuota"] = UnitQuotaObjArray[i].Quota;
            tableRow["Edit"] = "<img src='../Images/Detail.gif' onclick='EditWaterUserQuota(" + UnitQuotaObjArray[i].ID + ")' />";
            tableRow["Cancel"] = "<img src='../Images/Delete.gif' onclick='DeleteWaterUserQuota(" + UnitQuotaObjArray[i].ID + ")' />";
            tableData.push(tableRow);
            //计算合计
            _totalAcreage += parseFloat(UnitQuotaObjArray[i].Acreage);
            _totalQuota += parseFloat(UnitQuotaObjArray[i].Quota);
        }
    }
    $('#tbWaterUserQuotaList').datagrid('loadData', tableData);
    $("#tbWaterUserQuotaList").datagrid('reloadFooter', [{ "unitQuotaName": "合计", "acreage": _totalAcreage, "totalQuota": _totalQuota }]);
}

function Btn_OK_AddQuota_Click() {
    //判断添加或修改的用水户信息中是否有当前年的用水定额，如果没有则提示
    /*
    if (UnitQuotaObjArray == null || UnitQuotaObjArray.length == 0) {
        $.messager.alert("提示信息", "请添加当前年的用水定额！");
        return;
    }
    */
    var SelectNode = $("#WaterUserTree").tree('getSelected');
    if ($.trim($("#txt_Telephone").textbox('getText')) == "" || $.trim($("#txt_IdentityNumber").textbox('getText')) == ""
                || $.trim($("#txt_WaterQuota").textbox('getText')) == "" || $.trim($("#txt_ElectricQuota").textbox('getText')) == ""
                || $("#ccb_WaterPrice").combobox('getValue') == "" || $("#ccb_ElectricPrice").combobox('getValue') == ""
                || $.trim($("#txt_userName").textbox('getText')) == "") {
        $.messager.alert("提示信息", "请将信息填写完整!");
        return;
    }
    /*
    if (isNaN($("#txt_userNum").textbox("getText")) || $("#txt_userNum").textbox("getText") < 0 || $("#txt_userNum").textbox("getText").length != 4 || $("#txt_userNum").textbox("getText").indexOf(".") >= 0 || $("#txt_userNum").textbox("getText").trim() == "") {
        $.messager.alert("提示信息", "请输入四位数字");
        $("#txt_userNum").textbox("setText", "");
        return;
    }
    */

    if (isNaN($("#txt_IdentityNumber").textbox("getText")) || $.trim($("#txt_IdentityNumber").textbox("getText")).length != 18 || $("#txt_IdentityNumber").textbox("getText").indexOf(".") >= 0) {
        $.messager.alert("提示信息", "身份证号只能为18位");
        $("#txt_IdentityNumber").textbox("setText", "");
        return;
    }

    if (isNaN($("#txt_Telephone").textbox("getText")) || $.trim($("#txt_Telephone").textbox("getText")).length != 11 || $("#txt_Telephone").textbox("getText").indexOf(".") >= 0) {
        $.messager.alert("提示信息", "电话号码只能为11位数字");
        $("#txt_Telephone").textbox("setText", "");
        return;
    }

    var StrWaterUser = "{";
    StrWaterUser += "'ID'" + ":'" + UserID + "',";
    //StrWaterUser += "'户号'" + ":'" + $("#txt_userNum").textbox("getText") + "',";
    StrWaterUser += "'名称'" + ":'" + $("#txt_userName").textbox('getText') + "',";
    StrWaterUser += "'水价ID'" + ":'" + $("#ccb_WaterPrice").combobox('getValue') + "',";
    StrWaterUser += "'电价ID'" + ":'" + $("#ccb_ElectricPrice").combobox('getValue') + "',";
    StrWaterUser += "'地址'" + ":'" + $("#txt_Address").textbox('getText') + "',";
    StrWaterUser += "'电话'" + ":'" + $("#txt_Telephone").textbox('getText') + "',";
    StrWaterUser += "'身份证号'" + ":'" + $("#txt_IdentityNumber").textbox('getText') + "',";
    StrWaterUser += "'用水定额'" + ":'" + $("#txt_WaterQuota").textbox('getText') + "',";
    StrWaterUser += "'用电定额'" + ":'" + $("#txt_ElectricQuota").textbox('getText') + "',";
    StrWaterUser += "'管理ID'" + ":'" + SelectNode.attributes["mid"] + "'";
    /*
    StrWaterUser += "'定额':[";
    var quotaStr = "";
    var _year = (new Date).getFullYear();
    var isExists = false;
    for (var i = 0; i < UnitQuotaObjArray.length; i++) {
        if (UnitQuotaObjArray[i].Year == _year) {
            isExists = true;
        }

        if (i > 0) {
            quotaStr += ",";
        }
        quotaStr += "{";
        quotaStr += "'ID':'" + UnitQuotaObjArray[i].ID + "',";
        quotaStr += "'年份':'" + UnitQuotaObjArray[i].Year + "',";
        quotaStr += "'单位定额ID':'" + UnitQuotaObjArray[i].UnitQuotaID + "',";
        quotaStr += "'面积':'" + UnitQuotaObjArray[i].Acreage + "',";
        quotaStr += "'定额':'" + UnitQuotaObjArray[i].Quota + "'";
        quotaStr += "}";
    }
    StrWaterUser += quotaStr;*/
    StrWaterUser += "}";
    /*
    if (!isExists) {
        $.messager.alert("提示信息", "请添加当前年的用水定额！");
        return;
    }
    */
    var url = "";
    var data = {};
    var deleteQuotaIdStr = "";
    if (deleteQuotaId != null && deleteQuotaId.length > 0) {
        for (var i = 0; i < deleteQuotaId.length; i++) {
            deleteQuotaIdStr += deleteQuotaId[i];
            if (i < deleteQuotaId.length - 1) {
                deleteQuotaIdStr += ",";
            }
        }
    }
    var isAddWaterUser = "添加";
    if ($('#AddUserWaterData').panel('options').title == "添加用水户信息") {
        url = "../WebServices/WaterUserService.asmx/AddWaterUser";
        data = { 'loginIdentifer': loginIdentifer, 'waterUserJson': StrWaterUser };
    }
    if ($('#AddUserWaterData').panel('options').title == "编辑用水户信息") {
        isAddWaterUser = "编辑";
        url = "../WebServices/WaterUserService.asmx/ModifyWaterUser";
        data = { 'loginIdentifer': loginIdentifer, 'waterUserJson': StrWaterUser, 'deleteQuotaId': deleteQuotaIdStr };
    }
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
                $.messager.alert("提示信息", isAddWaterUser + "用水户信息成功");
                $('#OperateWaterUserQuota').dialog({ closed: true });
                $('#AddUserWaterData').dialog({ closed: true });
                LoadWaterUserInfo("../WebServices/WaterUserService.asmx/GetWaterUsersHasQuotasByVillageId", { "loginIdentifer": window.parent.guid, "villageId": villageid, "isExport": false });
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

//点击用水定额列表的
function Btn_Cancel_AddUserWaterData() {
    $('#AddUserWaterData').dialog({ closed: true });
}

//点击用水定额操作层的取消按钮
function Btn_Cancel_OperateWaterUser() {
    $("#txt_Acreage").textbox("setText", "");
    $("#txt_Quota").textbox('setText', "");
    $('#OperateWaterUserQuota').dialog({ closed: true });
}
//=========================用水定额的操作结束=========================//