// JScript 文件
var dicDeviceAppGroups = {};
//表格数据
var tableData = [];
var comboBoxData = [];
//当前登录操作员管理ID
var mnId = "";
//用于存储所有级别的节点
var levelJson;
//左侧树形选中节点的ID
var currentSelId;
//是否显示直接子管理
var isContainsChildManage = true;
//用于存储Excel文件的路径
var _excelURL = "";


//-------------
//登录用户标识
var loginIdentifer;
//用水户Id
var waterUserIds = "";
//用水记录数量
var waterRecordsCount;
//操作标识
var operateIdentifer;
//列表中显示的用水记录
var datas = [];

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
                waterUserLoaded = false;
                TreeBindSelect();
                LoadWaterUserTree("divAreaTree", mnId, false, false);
                managerRealName = data.SysStateInfo.监测点管理名称;
                $('#divContainer').layout('panel', 'west').panel({ title: managerRealName + "列表" });
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
        onSelect: function (node) {
            //清空combox数据
            $("#cbb_DevCombobox").combobox({data:[]});
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
    //起止时间
    var startTime = $("#txt_StartTime").val();
    var endTime = $("#txt_EndTime").val();
    
    if (!$.CheckTime(startTime, endTime)) {
        return;
    }

    var waterUserIds = $.QueryCheckBoxCombobox("cbb_DevCombobox");
    if (waterUserIds.indexOf("all")>=0) {
        waterUserIds = waterUserIds.substr(0, waterUserIds.length - 3);
    }
    if (waterUserIds == "" || waterUserIds==null) {
        $.messager.alert("提示信息", "请选择用水户！");
        return;
    }
    
    $('#tbWaterInfos').datagrid('loadData', { total: 0, rows: [] });
    $.ShowMask("数据加载中，请稍等……");
    waterRecordsCount = 0;
    //清空操作标识
    operateIdentifer="";
    
    //按用水户查询指定时间段内的用水记录数量
    $.ajax(
    {
        url: "../WebServices/DataQueryService.asmx/GetUseWaterRecordsCountByWaterUser",
        data: { "loginIdentifer": window.parent.guid, "waterUserId": waterUserIds, "startTime": startTime, "endTime": endTime },
        type: "GET",
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result == true) {
                waterRecordsCount = data.Count;
                operateIdentifer = data.Guid;
                if (waterRecordsCount == 0) {
                    $.HideMask();
                    $.messager.alert("提示信息", "查询结果为空");
                }
                else {
                    QueryCurrentPageData(1, 20);
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

function QueryCurrentPageData(pageNumber, pageSize) {
    var startIndex = (pageNumber - 1) * pageSize + 1;
    $.ajax({
        url: "../WebServices/DataQueryService.asmx/GetUseWaterRecordsByWaterUser",
        data: { "loginIdentifer": window.parent.guid, "operateIdentifer": operateIdentifer, "startIndex": startIndex, "count": pageSize },
        type: "GET",
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result == true) {
                datas = [];
                var useWaterRecords = data.Records;
                for (var i = 0; i < useWaterRecords.length; i++) {
                    var dataRow = {};
                    dataRow["userName"] = useWaterRecords[i].用水户名称;
                    dataRow["userNo"] = useWaterRecords[i].用户卡号;
                    dataRow["deviceName"] = useWaterRecords[i].设备名称;
                    dataRow["irrigationTime"] = "<span style='font-weight:bold'>" + useWaterRecords[i].灌溉时长 + "</span><br/><span>" + useWaterRecords[i].开泵时间 + "/" + useWaterRecords[i].关泵时间 + "</span>";
                    dataRow["waterNum"] = "<span style='font-weight:bold'>" + useWaterRecords[i].本次用水量 + "</span><br/><span>" + useWaterRecords[i].开泵卡剩余水量 + "/" + useWaterRecords[i].关泵卡剩余水量 + "</span>";
                    dataRow["electricNum"] = "<span style='font-weight:bold'>" + useWaterRecords[i].本次用电量 + "</span><br/><span>" + useWaterRecords[i].开泵卡剩余电量 + "/" + useWaterRecords[i].关泵卡剩余电量 + "</span>";
                    datas.push(dataRow);
                }
                var tableDataObj = {
		            total: waterRecordsCount,
		            rows: datas
	            }
                $('#tbWaterInfos').datagrid('loadData', tableDataObj);
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
    var dg = $("#tbWaterInfos");
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
		    QueryCurrentPageData(pageNum,pageSize)
	    }
    });
}

//导出Excel的按钮事件
function Btn_Excel_Click() {
    if (operateIdentifer == null||operateIdentifer =="") {
        $.messager.alert("提示信息", "请先进行查询");
        return;
    }
    //查询结果为空
    if(waterRecordsCount==0)
    {
        $.messager.alert("提示信息", "查询结果为空，不需要导出");
       return;
    }
    
    $.ajax({
        url: "../WebServices/DataQueryService.asmx/ExportUseWaterRecordsByWaterUsers",
        data: { "loginIdentifer": window.parent.guid, "operateIdentifer": operateIdentifer, "startIndex": "1", "count": waterRecordsCount },
        type: "Get",
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result == true) {
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
                levelJson = data.WaterUsers;
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
                        if (levelJson[i].状态=="正常") {
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