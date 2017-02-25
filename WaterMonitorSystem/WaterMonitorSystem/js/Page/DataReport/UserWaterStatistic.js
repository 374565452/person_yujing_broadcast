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

var waterRecordsCount = "";

var numflag = false;

var yearflag = false;

var monthflag = false;

var dayflag = false;

//-----------
var loginIdentifer = window.parent.guid;
var excelUrl = "";

var numexcelUrl = "";

var yearexcelUrl = "";

var monthexcelUrl = "";

var dayexcelUrl = "";

var managerRealName = "";

var operateIdentifer = "";

//---------------------
var operateNumIdentifer = "";

var operateYearIdentifer = "";

var operateMonthIdentifer = "";

var operateDayIdentifer = "";

var isAsc = false;



//次数
var numtext = "";

var numTime = "";

var yearTime = "";

var monthTime = "";

var dayTime = "";

$(document).ready(function () {
    var defaultTheme = $.cookie("psbsTheme");
    if (defaultTheme != null && defaultTheme != "default") {
        var link = $(document).find('link:first');
        link.attr('href', '../App_Themes/easyui/themes/' + defaultTheme + '/easyui.css');
    }
    InitTimeControl();
    GetSystemInfo();
    $.CheckBoxCombobox("cbb_DevCombobox");
    $("#userStatisticTabs").tabs({
        onSelect: function (title, index) {
            //按次
            numtext = $("#numText").val();
            numTime = $("#txt_NumTime").val();

            //向前向后
            $("input[type=radio]").each(function () {
                if ($(this)[0].checked) {
                    isAsc = $(this).val();
                }
            });
            if (title == "按次数统计" && !numflag) {
                $("#numStatistic").datagrid({
                    view: detailview,
                    detailFormatter: function (index, row) {
                        return '<div style="padding:2px"><table class="ddv"></table></div>';
                    },
                    onExpandRow: function (index, row) {
                        $.ajax(
                        {
                            url: "../WebServices/DataQueryService.asmx/GetUseWaterRecordsCountByTimes",
                            data: { "loginIdentifer": window.parent.guid, "waterUserIds": row.userID, "baseTime": numTime, "isAsc": isAsc, "useWaterTimes": numtext },
                            type: "GET",
                            dataType: "text",
                            cache: false,
                            success: function (responseText) {
                                var data = eval("(" + $.xml2json(responseText) + ")");
                                if (data.Result == true) {
                                    var numRecordsCount = data.Count;
                                    operateNumIdentifer = data.Guid;
                                    if (numRecordsCount == 0) {
                                        $.HideMask();
                                        var ddv = $("#numStatistic").datagrid('getRowDetail', index).find('div');
                                        ddv.empty().append("<span style='display:block;width:150px;margin:0 auto;color:red;'>该时间段内无用水记录！</span>")
                                    }
                                    else {
                                        QueryNumData(1, numRecordsCount, index);
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
                });
                numflag = true;
            } else if (title == "按年统计" && !yearflag) {
                $("#yearStatistic").datagrid({
                    view: detailview,
                    detailFormatter: function (index, row) {
                        return '<div style="padding:2px"><table class="ddv"></table></div>';
                    },
                    onExpandRow: function (index, row) {
                        $.ajax(
                        {
                            url: "../WebServices/DataQueryService.asmx/GetPeriodUseWaterRecordsByWaterUser",
                            data: { "loginIdentifer": loginIdentifer, "waterUserId": row.userID, "periodType": "月", "periodTime": row.startTime },
                            type: "GET",
                            dataType: "text",
                            cache: false,
                            success: function (responseText) {
                                var data = eval("(" + $.xml2json(responseText) + ")");
                                if (data.Result == true) {
                                    datas = [];
                                    var useWaterRecords = data.Records;
                                    if (useWaterRecords.length>0) {
                                        for (var i = 0; i < useWaterRecords.length; i++) {
                                            var dataRow = {};
                                            dataRow["deviceName"] = useWaterRecords[i].设备名称;
                                            dataRow["irrigationTime"] = "<span style='font-weight:bold'>" + useWaterRecords[i].灌溉时长 + "</span><br/><span>" + useWaterRecords[i].开泵时间 + "/" + useWaterRecords[i].关泵时间 + "</span>";
                                            dataRow["waterNum"] = "<span style='font-weight:bold'>" + useWaterRecords[i].本次用水量 + "</span><br/><span>" + useWaterRecords[i].开泵卡剩余水量 + "/" + useWaterRecords[i].关泵卡剩余水量 + "</span>";
                                            dataRow["electricNum"] = "<span style='font-weight:bold'>" + useWaterRecords[i].本次用电量 + "</span><br/><span>" + useWaterRecords[i].开泵卡剩余电量 + "/" + useWaterRecords[i].关泵卡剩余电量 + "</span>";
                                            datas.push(dataRow);
                                        }


                                        var ddv = $("#yearStatistic").datagrid('getRowDetail', index).find('table.ddv');
                                        ddv.datagrid({
                                            data: datas,
                                            fitColumns: true,
                                            singleSelect: true,
                                            rownumbers: true,
                                            view: bufferview,
                                            pageSize: 20,
                                            loadMsg: '数据加载中……',
                                            height: '300',
                                            columns: [[
                                                { field: 'deviceName', title: '设备名称', width: 100, align: 'center' },
                                                { field: 'irrigationTime', title: '灌溉时长<br />(开泵时间/关泵时间)', width: 100, align: 'center' },
                                                { field: 'waterNum', title: '本次用水量(m³)<br />(开泵时剩余/关泵时剩余)', width: 100, align: 'center' },
                                                { field: 'electricNum', title: '本次用电量(kw·h)<br />(开泵时剩余/关泵时剩余)', width: 100, align: 'center' }
                                            ]],
                                            onResize: function () {
                                                $('#yearStatistic').datagrid('fixDetailRowHeight', index);
                                            },
                                            onLoadSuccess: function () {
                                                setTimeout(function () {
                                                    $('#yearStatistic').datagrid('fixDetailRowHeight', index);
                                                }, 0);
                                            }
                                        });
                                        $('#yearStatistic').datagrid('fixDetailRowHeight', index);
                                    } else {
                                        var ddv = $("#yearStatistic").datagrid('getRowDetail', index).find('div');
                                        ddv.empty().append("<span style='display:block;width:150px;margin:0 auto;color:red;'>该时间段内无用水记录！</span>");
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
                });
                yearflag = true;
            } else if (title == "按月统计" && !monthflag) {
                $("#monthStatistic").datagrid({
                    view: detailview,
                    detailFormatter: function (index, row) {
                        return '<div style="padding:2px"><table class="ddv"></table></div>';
                    },
                    onExpandRow: function (index, row) {
                        $.ajax(
                        {
                            url: "../WebServices/DataQueryService.asmx/GetPeriodUseWaterRecordsByWaterUser",
                            data: { "loginIdentifer": loginIdentifer, "waterUserId": row.userID, "periodType": "日", "periodTime": row.startTime },
                            type: "GET",
                            dataType: "text",
                            cache: false,
                            success: function (responseText) {
                                var data = eval("(" + $.xml2json(responseText) + ")");
                                if (data.Result == true) {
                                    datas = [];
                                    var useWaterRecords = data.Records;
                                    if (useWaterRecords.length>0) {
                                        for (var i = 0; i < useWaterRecords.length; i++) {
                                            var dataRow = {};
                                            dataRow["deviceName"] = useWaterRecords[i].设备名称;
                                            dataRow["irrigationTime"] = "<span style='font-weight:bold'>" + useWaterRecords[i].灌溉时长 + "</span><br/><span>" + useWaterRecords[i].开泵时间 + "/" + useWaterRecords[i].关泵时间 + "</span>";
                                            dataRow["waterNum"] = "<span style='font-weight:bold'>" + useWaterRecords[i].本次用水量 + "</span><br/><span>" + useWaterRecords[i].开泵卡剩余水量 + "/" + useWaterRecords[i].关泵卡剩余水量 + "</span>";
                                            dataRow["electricNum"] = "<span style='font-weight:bold'>" + useWaterRecords[i].本次用电量 + "</span><br/><span>" + useWaterRecords[i].开泵卡剩余电量 + "/" + useWaterRecords[i].关泵卡剩余电量 + "</span>";
                                            datas.push(dataRow);
                                        }


                                        var ddv = $("#monthStatistic").datagrid('getRowDetail', index).find('table.ddv');
                                        ddv.datagrid({
                                            data: datas,
                                            fitColumns: true,
                                            singleSelect: true,
                                            rownumbers: true,
                                            view: bufferview,
                                            pageSize: 20,
                                            loadMsg: '数据加载中……',
                                            height: '300',
                                            columns: [[
                                                { field: 'deviceName', title: '设备名称', width: 100, align: 'center' },
                                                { field: 'irrigationTime', title: '灌溉时长<br />(开泵时间/关泵时间)', width: 100, align: 'center' },
                                                { field: 'waterNum', title: '本次用水量(m³)<br />(开泵时剩余/关泵时剩余)', width: 100, align: 'center' },
                                                { field: 'electricNum', title: '本次用电量(kw·h)<br />(开泵时剩余/关泵时剩余)', width: 100, align: 'center' }
                                            ]],
                                            onResize: function () {
                                                $('#monthStatistic').datagrid('fixDetailRowHeight', index);
                                            },
                                            onLoadSuccess: function () {
                                                setTimeout(function () {
                                                    $('#monthStatistic').datagrid('fixDetailRowHeight', index);
                                                }, 0);
                                            }
                                        });
                                        $('#monthStatistic').datagrid('fixDetailRowHeight', index);
                                    } else {
                                        var ddv = $("#monthStatistic").datagrid('getRowDetail', index).find('div');
                                        ddv.empty().append("<span style='display:block;width:150px;margin:0 auto;color:red;'>该时间段内无用水记录！</span>");
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
                });
                monthflag = true;
            } else if (title == "按日统计" && !dayflag) {
                $("#dayStatistic").datagrid({
                    view: detailview,
                    detailFormatter: function (index, row) {
                        return '<div style="padding:2px"><table class="ddv"></table></div>';
                    },
                    onExpandRow: function (index, row) {
                        $.ajax(
                        {
                            url: "../WebServices/DataQueryService.asmx/GetPeriodUseWaterRecordsByWaterUser",
                            data: { "loginIdentifer": loginIdentifer, "waterUserId": row.userID, "periodType": "时", "periodTime": row.startTime },
                            type: "GET",
                            dataType: "text",
                            cache: false,
                            success: function (responseText) {
                                var data = eval("(" + $.xml2json(responseText) + ")");
                                if (data.Result == true) {
                                    datas = [];
                                    var useWaterRecords = data.Records;
                                    if (useWaterRecords.length>0) {
                                        for (var i = 0; i < useWaterRecords.length; i++) {
                                            var dataRow = {};
                                            dataRow["deviceName"] = useWaterRecords[i].设备名称;
                                            dataRow["irrigationTime"] = "<span style='font-weight:bold'>" + useWaterRecords[i].灌溉时长 + "</span><br/><span>" + useWaterRecords[i].开泵时间 + "/" + useWaterRecords[i].关泵时间 + "</span>";
                                            dataRow["waterNum"] = "<span style='font-weight:bold'>" + useWaterRecords[i].本次用水量 + "</span><br/><span>" + useWaterRecords[i].开泵卡剩余水量 + "/" + useWaterRecords[i].关泵卡剩余水量 + "</span>";
                                            dataRow["electricNum"] = "<span style='font-weight:bold'>" + useWaterRecords[i].本次用电量 + "</span><br/><span>" + useWaterRecords[i].开泵卡剩余电量 + "/" + useWaterRecords[i].关泵卡剩余电量 + "</span>";
                                            datas.push(dataRow);
                                        }


                                        var ddv = $("#dayStatistic").datagrid('getRowDetail', index).find('table.ddv');
                                        ddv.datagrid({
                                            data: datas,
                                            fitColumns: true,
                                            singleSelect: true,
                                            rownumbers: true,
                                            view: bufferview,
                                            pageSize: 20,
                                            loadMsg: '数据加载中……',
                                            height: '300',
                                            columns: [[
                                                { field: 'deviceName', title: '设备名称', width: 100, align: 'center' },
                                                { field: 'irrigationTime', title: '灌溉时长<br />(开泵时间/关泵时间)', width: 100, align: 'center' },
                                                { field: 'waterNum', title: '本次用水量(m³)<br />(开泵时剩余/关泵时剩余)', width: 100, align: 'center' },
                                                { field: 'electricNum', title: '本次用电量(kw·h)<br />(开泵时剩余/关泵时剩余)', width: 100, align: 'center' }
                                            ]],
                                            onResize: function () {
                                                $('#dayStatistic').datagrid('fixDetailRowHeight', index);
                                            },
                                            onLoadSuccess: function () {
                                                setTimeout(function () {
                                                    $('#dayStatistic').datagrid('fixDetailRowHeight', index);
                                                }, 0);
                                            }
                                        });
                                        $('#dayStatistic').datagrid('fixDetailRowHeight', index);
                                        $.HideMask();
                                    } else {
                                        var ddv = $("#dayStatistic").datagrid('getRowDetail', index).find('div');
                                        ddv.empty().append("<span style='display:block;width:150px;margin:0 auto;color:red;'>该时间段内无用水记录！</span>");
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
                });
                dayflag = true;
            }
        }
    });

    $("#tbRechargeInfos").datagrid({
        view: detailview,
        detailFormatter: function (index, row) {
            return '<div style="padding:2px"><table class="ddv"></table></div>';
        },
        onExpandRow: function (index, row) {
            $.ajax(
    {
        url: "../WebServices/DataQueryService.asmx/GetUseWaterRecordsCountByWaterUser",
        data: { "loginIdentifer": window.parent.guid, "waterUserId": row.userID, "startTime": $("#txt_StartTime").val(), "endTime": $("#txt_EndTime").val() },
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
                    var ddv = $("#tbRechargeInfos").datagrid('getRowDetail', index).find('div');
                    ddv.empty().append("<span style='display:block;width:150px;margin:0 auto;color:red;'>该时间段内无用水记录！</span>")
                }
                else {
                    QueryCurrentPageData(1, waterRecordsCount, index);
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
    })
});

function QueryCurrentPageData(pageNumber, pageSize, index) {
    var startIndex = pageNumber;
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
                    dataRow["deviceName"] = useWaterRecords[i].设备名称;
                    dataRow["irrigationTime"] = "<span style='font-weight:bold'>" + useWaterRecords[i].灌溉时长 + "</span><br/><span>" + useWaterRecords[i].开泵时间 + "/" + useWaterRecords[i].关泵时间 + "</span>";
                    dataRow["waterNum"] = "<span style='font-weight:bold'>" + useWaterRecords[i].本次用水量 + "</span><br/><span>" + useWaterRecords[i].开泵卡剩余水量 + "/" + useWaterRecords[i].关泵卡剩余水量 + "</span>";
                    dataRow["electricNum"] = "<span style='font-weight:bold'>" + useWaterRecords[i].本次用电量 + "</span><br/><span>" + useWaterRecords[i].开泵卡剩余电量 + "/" + useWaterRecords[i].关泵卡剩余电量 + "</span>";
                    datas.push(dataRow);
                }


                var ddv = $("#tbRechargeInfos").datagrid('getRowDetail', index).find('table.ddv');
                ddv.datagrid({
                    data: datas,
                    fitColumns: true,
                    singleSelect: true,
                    rownumbers: true,
                    view: bufferview,
                    pageSize: 20,
                    loadMsg: '数据加载中……',
                    height: '300',
                    columns: [[
                        { field: 'deviceName', title: '设备名称', width: 100, align: 'center' },
                        { field: 'irrigationTime', title: '灌溉时长<br />(开泵时间/关泵时间)', width: 100, align: 'center' },
                        { field: 'waterNum', title: '本次用水量(m³)<br />(开泵时剩余/关泵时剩余)', width: 100, align: 'center' },
                        { field: 'electricNum', title: '本次用电量(kw·h)<br />(开泵时剩余/关泵时剩余)', width: 100, align: 'center' }
                    ]],
                    onResize: function () {
                        $('#tbRechargeInfos').datagrid('fixDetailRowHeight', index);
                    },
                    onLoadSuccess: function () {
                        setTimeout(function () {
                            $('#tbRechargeInfos').datagrid('fixDetailRowHeight', index);
                        }, 0);
                    }
                });
                $('#tbRechargeInfos').datagrid('fixDetailRowHeight', index);

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

function checkit(isChecked) {
    var url = "";
    var data = {};
    if (isChecked) {
        url = "../WebServices/WaterUserService.asmx/GetAllWaterUsersByVillageId";
        data = { "loginIdentifer": window.parent.guid, "villageId": villageid }
        LoadUserCombobox(url, data, true);
    } else {
        url = "../WebServices/WaterUserService.asmx/GetWaterUsersByVillageId";
        data = { "loginIdentifer": window.parent.guid, "villageId": villageid, "isExport": false };
        LoadUserCombobox(url, data, false);
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
                TreeBindSelect();
                LoadWaterUserTree("divAreaTree", mnId, false, false);
                managerRealName = data.SysStateInfo.监测点管理名称;
                $('#divContainer').layout('panel', 'west').panel({ title: managerRealName + "列表" });

                setTimeout(function () {
                    Btn_Query_Click();
                    Btn_QueryChart_Click();
                }, 1000);
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
            $("#cbb_DevCombobox").combobox({ data: [] });
            $("#cbb_NumDevCombobox").combobox({ data: [] });
            $("#cbb_YearDevCombobox").combobox({ data: [] });
            $("#cbb_MonthDevCombobox").combobox({ data: [] });
            $("#cbb_DayDevCombobox").combobox({ data: [] });
            if (node.id.indexOf("cz") >= 0) {
                villageid = node.attributes["mid"];
                LoadUserCombobox("../WebServices/WaterUserService.asmx/GetWaterUsersByVillageId", { "loginIdentifer": window.parent.guid, "villageId": villageid, "isExport": false }, false);
                LoadNumCombobox("../WebServices/WaterUserService.asmx/GetWaterUsersByVillageId", { "loginIdentifer": window.parent.guid, "villageId": villageid, "isExport": false }, false);
                LoadYearCombobox("../WebServices/WaterUserService.asmx/GetWaterUsersByVillageId", { "loginIdentifer": window.parent.guid, "villageId": villageid, "isExport": false }, false);
                LoadMonthCombobox("../WebServices/WaterUserService.asmx/GetWaterUsersByVillageId", { "loginIdentifer": window.parent.guid, "villageId": villageid, "isExport": false }, false);
                LoadDayCombobox("../WebServices/WaterUserService.asmx/GetWaterUsersByVillageId", { "loginIdentifer": window.parent.guid, "villageId": villageid, "isExport": false }, false);
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
    var datas = [];

    //按用水户进行指定时间段内的用水汇总统计

    var startTime = $("#txt_StartTime").val();
    var endTime = $("#txt_EndTime").val();

    //    var waterUserIds = $("#cbb_DevCombobox").combobox('getValues');
    //    waterUserIds = waterUserIds.join(",");

    //数据完整性验证
    if (!$.CheckTime(startTime, endTime)) {
        return;
    }

    var waterUserIds = $.QueryCheckBoxCombobox("cbb_DevCombobox");
    if (waterUserIds.indexOf("all") >= 0) {
        waterUserIds = waterUserIds.substr(0, waterUserIds.length - 3);
    }

    if (waterUserIds == "" || waterUserIds == null) {
        $.messager.alert("提示信息", "用水户不存在，请重新选择！");
        return;
    }

    //清空datagrid列表、合计行
    $("#tbRechargeInfos").datagrid('loadData', { total: 0, rows: [], footer: [] });
    $.ShowMask("数据加载中，请稍等……");
    //清空导出路径
    excelUrl = "";

    $.ajax({
        url: "../WebServices/DataReportService.asmx/GetUseWaterSummaryReportByWaterUsers",
        data: { "loginIdentifer": loginIdentifer, "waterUserIds": waterUserIds, "startTime": startTime, "endTime": endTime },
        type: "Get",
        cache: false,
        dataType: "text",
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                //户用水列表
                var useWaterInfo = data.户用水;
                for (var i = 0; i < useWaterInfo.length; i++) {
                    var dataRow = {};
                    dataRow["userID"] = useWaterInfo[i].用水户ID;
                    dataRow["userName"] = useWaterInfo[i].用水户;
                    dataRow["waterTime"] = useWaterInfo[i].灌溉时长;
                    dataRow["waterNum"] = useWaterInfo[i].用水量;
                    dataRow["electricNum"] = useWaterInfo[i].用电量;
                    datas.push(dataRow);
                }
                $("#tbRechargeInfos").datagrid({ data: datas });
                //户用水合计
                var useWaterTotal = data.合计;
                $("#tbRechargeInfos").datagrid({ loadFilter: pagerFilter }).datagrid('reloadFooter', [{ "userName": "合计", "waterTime": useWaterTotal.灌溉时长, "waterNum": useWaterTotal.用水量, "electricNum": useWaterTotal.用电量 }]);
                $.HideMask();
                //导出Excel路径
                excelUrl = data.ExcelURL;
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

//导出Excel的按钮事件
function Btn_Excel_Click() {
    if (excelUrl == null || excelUrl == "") {
        $.messager.alert("提示信息", "请先进行查询");
        return;
    }

    location.href = excelUrl;
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
                allnormalObj["id"] = allnormalid + "all";
                allnormalObj["text"] = "全部(正常)";
                if (!flag) {
                    allnormalObj["selected"] = true;
                }
                comboBoxDataLevel.unshift(allnormalObj);
                if (flag) {
                    var allObj = {};
                    allObj["id"] = allid + "all";
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

function LoadNumCombobox(url, data, flag) {
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
                allnormalObj["id"] = allnormalid + "all";
                allnormalObj["text"] = "全部(正常)";
                if (!flag) {
                    allnormalObj["selected"] = true;
                }
                comboBoxDataLevel.unshift(allnormalObj);
                if (flag) {
                    var allObj = {};
                    allObj["id"] = allid + "all";
                    allObj["text"] = "全部(含已注销)";
                    allObj["selected"] = true;
                    comboBoxDataLevel.unshift(allObj);
                }
                $("#cbb_NumDevCombobox").combobox({
                    data: comboBoxDataLevel
                });
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}
function LoadYearCombobox(url, data, flag) {
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
                    } else {
                        levelObj["id"] = levelJson[i].ID;
                        levelObj["text"] = levelJson[i].名称 + "(" + levelJson[i].状态 + ")";
                        comboBoxDataLevel.push(levelObj);
                    }
                }
                $("#cbb_YearDevCombobox").combobox({
                    data: comboBoxDataLevel
                });
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}
function LoadMonthCombobox(url, data, flag) {
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
                    } else {
                        levelObj["id"] = levelJson[i].ID;
                        levelObj["text"] = levelJson[i].名称 + "(" + levelJson[i].状态 + ")";
                        comboBoxDataLevel.push(levelObj);
                    }
                }
                $("#cbb_MonthDevCombobox").combobox({
                    data: comboBoxDataLevel
                });
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}
function LoadDayCombobox(url, data, flag) {
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
                    } else {
                        levelObj["id"] = levelJson[i].ID;
                        levelObj["text"] = levelJson[i].名称 + "(" + levelJson[i].状态 + ")";
                        comboBoxDataLevel.push(levelObj);
                    }
                }
                $("#cbb_DayDevCombobox").combobox({
                    data: comboBoxDataLevel
                });
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

