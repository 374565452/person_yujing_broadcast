// JScript 文件
var dicDeviceAppGroups = {};
//表格数据
var tableData = [];
var comboBoxData = [];
//当前登录操作员管理ID
var mnId = "";
//用于存储所有级别的节点
//var levelJson;
//左侧树形选中节点的ID
var currentSelId;
//是否显示直接子管理
var isContainsChildManage = true;
//用于存储Excel文件的路径
var _excelURL = "";


//------------------
var totalRecord;
//登录标示
var loginIdentifer = window.parent.guid;
//操作标示
var operateIdentifer;

var managerRealName = "";

$(document).ready(function () {
    var defaultTheme = $.cookie("psbsTheme");
    if (defaultTheme != null && defaultTheme != "default") {
        var link = $(document).find('link:first');
        link.attr('href', '../App_Themes/easyui/themes/' + defaultTheme + '/easyui.css');
    }
    InitTimeControl();
    GetSystemInfo();
    $.CheckBoxCombobox("cbb_DevCombobox");
});

function checkit(isChecked) {
    var url = "";
    var data = {};
    if (isChecked) {
        url = "../WebServices/WaterUserService.asmx/GetAllWaterUsersByVillageId";
        data = { "loginIdentifer": window.parent.guid, "villageId": villageid }
        LoadUserCombobox(url, data,true);
    } else {
        url = "../WebServices/WaterUserService.asmx/GetWaterUsersByVillageId";
        data = { "loginIdentifer": window.parent.guid, "villageId": villageid, "isExport": false };
        LoadUserCombobox(url, data,false);
    }
}


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
                managerRealName = data.SysStateInfo.监测点管理名称;
                $('#divContainer').layout('panel', 'west').panel({ title: managerRealName + "列表" });
                LoadWaterUserTree("divAreaTree", mnId, false, false);
                TreeBindSelect();
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

//点击左侧树形重新加载右侧Combobox
function TreeBindSelect() {
    $('#divAreaTree').tree({
        onSelect: function (node) 
        {
            $("#cbb_DevCombobox").combobox({data: []});
            if (node.id.indexOf("cz") >= 0) {
                villageid = node.attributes["mid"];
                LoadUserCombobox("../WebServices/WaterUserService.asmx/GetWaterUsersByVillageId", { "loginIdentifer": window.parent.guid, "villageId": villageid, "isExport": false },false);
            }
        }
    });
}

//初始化时间框
function InitTimeControl() {
    var e = new Date();
    $("#txt_StartTime").val(e.Format("yyyy-MM-01 00:00"));
    $("#txt_EndTime").val(e.Format("yyyy-MM-dd HH:mm"));
}

//查询的按钮事件
function Btn_Query_Click() {
    
    var waterUserId = $.QueryCheckBoxCombobox("cbb_DevCombobox");
    if (waterUserId.indexOf("all")>=0) 
    {
        waterUserId = waterUserId.substr(0, waterUserId.length - 3);
    }
    if (waterUserId == "" || waterUserId==null) 
    {
        $.messager.alert("提示信息", "请选择用水户！");
        return;
    }
    var startTime = $("#txt_StartTime").val();
    var endTime = $("#txt_EndTime").val();
    if (!$.CheckTime(startTime, endTime)) 
    {
        return;
    }
    $('#tbRechargeInfos').datagrid({ pageNumber: 1 }).datagrid('loadData', { total: 0, rows: [] });
    $.ShowMask("数据加载中，请稍等……");
    SaleWaterRecordsCounts = 0;
    operateIdentifer = "";
    $.ajax( 
    {
        url: "../WebServices/DataQueryService.asmx/GetSaleWaterRecordsCountByWaterUser",
        type: "GET",
        data: { 'loginIdentifer': loginIdentifer, 'waterUserId': waterUserId, 'startTime': startTime, 'endTime': endTime },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                SaleWaterRecordsCounts = data.Count;
                totalRecord = 0;
                totalRecord += parseInt(SaleWaterRecordsCounts);
                operateIdentifer = data.Guid;
                if (totalRecord == 0)
                {
                    $.HideMask();
                    $.messager.alert("提示信息", "查询结果为空");
                }
                else
                {
                    QueryCurrentPageData(loginIdentifer, operateIdentifer, 1, 20);
                }
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

function QueryCurrentPageData(loginIdentifer, operateIdentifer, pageNumber, pageSize) {
    currentPageSize = pageSize;
    currentPageNumber = pageNumber;
    var startNumber = (pageNumber - 1) * pageSize + 1;

    //获取分页中的历史记录
    $.ajax(
    {
        url: "../WebServices/DataQueryService.asmx/GetSaleWaterRecordsByWaterUser",
        type: "GET",
        data: { 'loginIdentifer': loginIdentifer, 'operateIdentifer': operateIdentifer, 'startIndex': startNumber, 'count': pageSize },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                SaleWaterRecords = data.Records;
                tableData = [];
                for (var i in SaleWaterRecords) {
                    var tableRow = {};
                    tableRow["userName"] = SaleWaterRecords[i].用水户名称;
                    tableRow["userNo"] = SaleWaterRecords[i].用水户卡号;
                    tableRow["chargeTime"] = SaleWaterRecords[i].售水时间;
                    tableRow["chargeAmount"] = SaleWaterRecords[i].实收金额;
                    tableRow["waterAmount"] = SaleWaterRecords[i].售水金额;
                    tableRow["waterNum"] = SaleWaterRecords[i].售出水量;
                    tableRow["electricAmount"] = SaleWaterRecords[i].售电金额;
                    tableRow["electricNum"] = SaleWaterRecords[i].售出电量;
                    tableRow["handler"] = SaleWaterRecords[i].购水人;
                    tableRow["operator"] = SaleWaterRecords[i].操作员名称;
                    tableData.push(tableRow);
                }
                var tableDataObj = {
		            total: SaleWaterRecordsCounts,
		            rows: tableData
	            }
                $('#tbRechargeInfos').datagrid('loadData', tableDataObj);
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

function BindOnSelectPage(){
    var dg = $("#tbRechargeInfos");
    var opts = dg.datagrid('options');
    var pager = dg.datagrid('getPager');
    pager.pagination({
	    onSelectPage:function(pageNum, pageSize){
		    opts.pageNumber = pageNum;
		    opts.pageSize = pageSize;
		    pager.pagination('refresh',{
			    pageNumber:pageNum,
			    pageSize:pageSize
		    });
		    $.ShowMask("数据加载中，请稍等……");
		    QueryCurrentPageData(loginIdentifer, operateIdentifer, pageNum, pageSize)
	    }
    });
}

//导出Excel的按钮事件
function Btn_Excel_Click() {
    if(operateIdentifer == "" || operateIdentifer == null)
    {
        $.messager.alert("提示信息", "请先查询！");
        return;
    }
    if(totalRecord==0)
    {
        $.messager.alert("提示信息", "查询结果为空，不需要导出！");
        return;
    }
    $.ajax(
    {
        url: "../WebServices/DataQueryService.asmx/ExportSaleWaterRecordsByWaterUsers",
        type: "GET",
        data: { 'loginIdentifer': loginIdentifer, 'operateIdentifer': operateIdentifer, 'startIndex': "1", 'count': totalRecord },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                location.href = data.ExcelURL;
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

function LoadUserCombobox(url, data, flag) {
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
                var levelJson = data.WaterUsers;
                var comboBoxDataLevel = [];
                allid = "";
                allnormalid = "";
                for (i = 0; i < levelJson.length; i++) {
                    var levelObj = {};
                    if (flag) {
                        levelObj["id"] = levelJson[i].ID;
                        levelObj["text"] = levelJson[i].名称 + "(" + levelJson[i].状态 + ")";
                        comboBoxDataLevel.push(levelObj);
                        if (allid != "") {
                            allid += ",";
                        }
                        allid += levelJson[i].ID;
                        if (levelJson[i].状态 == "正常") {
                            if (allnormalid != "") {
                                allnormalid += ",";
                            }
                            allnormalid += levelJson[i].ID;
                        }
                    } else {
                        levelObj["id"] = levelJson[i].ID;
                        levelObj["text"] = levelJson[i].名称 + "(" + levelJson[i].状态 + ")";
                        comboBoxDataLevel.push(levelObj);
                        if (allnormalid != "") {
                            allnormalid += ",";
                        }
                        allnormalid += levelJson[i].ID;
                    }
                }
                var allnormalObj = {};
                allnormalObj["id"] = allnormalid+"all";
                allnormalObj["text"] = "全部(正常)";
                if (!flag) {
                    allnormalObj["selected"] = true;
                }
                comboBoxDataLevel.unshift(allnormalObj);
                if (flag) {
                    var allObj = {};
                    allObj["id"] = allid+"all";
                    allObj["text"] = "全部(含已注销)";
                    allObj["selected"] = true;
                    comboBoxDataLevel.unshift(allObj);
                }


                $("#cbb_DevCombobox").combobox({
                    data: comboBoxDataLevel
                });
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

