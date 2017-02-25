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


//--------------------
//登录标示
var loginIdentifer = window.parent.guid;
//导出路径
var ExcUrl = "";
var numExcelUrl = "";

var managerRealName = "";

var operateIdentifer = "";

var waterRecordsCount = "";

var numflag = false;

//---------------------
var operateNumIdentifer = "";

var isAsc = false;
//次数
var numtext = "";

var numTime = "";


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
                            url: "../WebServices/DataQueryService.asmx/GetSaleWaterRecordsCountByTimes",
                            data: { "loginIdentifer": window.parent.guid, "waterUserIds": row.userID, "baseTime": numTime, "isAsc": isAsc, "saleWaterTimes": numtext },
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
                                        ddv.empty().append("<span style='display:block;width:150px;margin:0 auto;color:red;'>该时间段内售用水记录！</span>")
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
        url: "../WebServices/DataQueryService.asmx/GetSaleWaterRecordsCountByWaterUser",
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
                    ddv.empty().append("<span style='display:block;width:150px;margin:0 auto;color:red;'>该时间段内无售水记录！</span>")
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
        url: "../WebServices/DataQueryService.asmx/GetSaleWaterRecordsByWaterUser",
        data: { "loginIdentifer": window.parent.guid, "operateIdentifer": operateIdentifer, "startIndex": startIndex, "count": pageSize },
        type: "GET",
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result == true) {
                var useWaterRecords = data.Records;
                tableData = [];
                for (var i in useWaterRecords) {
                    var tableRow = {};
                    tableRow["userName"] = useWaterRecords[i].用水户名称;
                    tableRow["chargeTime"] = useWaterRecords[i].售水时间;
                    tableRow["chargeAmount"] = useWaterRecords[i].实收金额;
                    tableRow["waterAmount"] = useWaterRecords[i].售水金额;
                    tableRow["waterNum"] = useWaterRecords[i].售出水量;
                    tableRow["electricAmount"] = useWaterRecords[i].售电金额;
                    tableRow["electricNum"] = useWaterRecords[i].售出电量;
                    tableRow["handler"] = useWaterRecords[i].购水人;
                    tableRow["operator"] = useWaterRecords[i].操作员名称;
                    tableData.push(tableRow);
                }


                var ddv = $("#tbRechargeInfos").datagrid('getRowDetail', index).find('table.ddv');
                ddv.datagrid({
                    data: tableData,
                    fitColumns: true,
                    singleSelect: true,
                    rownumbers: true,
                    view: bufferview,
                    pageSize: 20,
                    loadMsg: '数据加载中……',
                    height: '300',
                    columns: [[
                        { field: 'chargeTime', title: '充值时间', width: 40, align: 'center' },
                        { field: 'chargeAmount', title: '充值金额', width: 40, align: 'center' },
                        { field: 'waterAmount', title: '购水金额', width: 40, align: 'center' },
                        { field: 'waterNum', title: '购水量', width: 40, align: 'center' },
                        { field: 'electricAmount', title: '购电金额', width: 40, align: 'center' },
                        { field: 'electricNum', title: '购电量', width: 40, align: 'center' },
                        { field: 'handler', title: '代办人', width: 40, align: 'center' },
                        { field: 'operator', title: '操作员', width: 40, align: 'center' }
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
            $("#cbb_DevCombobox").combobox({data:[]});
            if (node.id.indexOf("cz") >= 0) {
                villageid = node.attributes["mid"];
                LoadUserCombobox("../WebServices/WaterUserService.asmx/GetWaterUsersByVillageId", { "loginIdentifer": window.parent.guid, "villageId": villageid, "isExport": false }, false);
                LoadNumCombobox("../WebServices/WaterUserService.asmx/GetWaterUsersByVillageId", { "loginIdentifer": window.parent.guid, "villageId": villageid, "isExport": false }, false);
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
    
    var waterUserIds =$.QueryCheckBoxCombobox("cbb_DevCombobox");
    if (waterUserIds.indexOf("all")>=0) 
    {
        waterUserIds = waterUserIds.substr(0, waterUserIds.length - 3);
    }
    if (waterUserIds == "" || waterUserIds==null) 
    {
        $.messager.alert("提示信息", "用水户不存在，请重新选择！");
        return;
    }
    var startTime = $("#txt_StartTime").val();
    var endTime = $("#txt_EndTime").val();
    if (!$.CheckTime(startTime, endTime)) 
    {
        return;
    }
    
    ExcUrl = "";
    $('#tbRechargeInfos').datagrid('loadData', { total: 0, rows: [],footer:[] });
    $.ShowMask("数据加载中，请稍等……");
    $.ajax(
    {
        url: "../WebServices/DataReportService.asmx/GetSaleWaterSummaryReportByWaterUsers",
        type: "GET",
        data: { 'loginIdentifer': loginIdentifer, 'waterUserIds': waterUserIds, 'startTime': startTime, 'endTime': endTime},
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                ExcUrl = data.ExcelURL;
                SaleWaterSummary = data.户充值;
                SaleWaterSum = data.合计;
                tableData = [];
                for (var i in SaleWaterSummary) {
                    var tableRow = {};
                    tableRow["userID"] = SaleWaterSummary[i].用水户ID;
                    tableRow["userName"] = SaleWaterSummary[i].用水户;
                    tableRow["chargeAmount"] = SaleWaterSummary[i].充值金额;    
                    tableRow["waterAmount"] = SaleWaterSummary[i].购水金额;
                    tableRow["waterNum"] = SaleWaterSummary[i].购水量;
                    tableRow["electricAmount"] = SaleWaterSummary[i].购电金额;
                    tableRow["electricNum"] = SaleWaterSummary[i].购电量;
                    tableData.push(tableRow);
                }
                $("#tbRechargeInfos").datagrid({ loadFilter: pagerFilter }).datagrid("loadData", tableData)

                $('#tbRechargeInfos').datagrid('reloadFooter', [
                            { "userName": "合计", "chargeAmount": SaleWaterSum.充值金额, "waterAmount": SaleWaterSum.购水金额, "waterNum": SaleWaterSum.购水量, "electricAmount": SaleWaterSum.购电金额, "electricNum": SaleWaterSum.购电量 }
                ]);
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

//导出Excel的按钮事件
function Btn_Excel_Click() {
    if (ExcUrl != "") {
        location.href = ExcUrl;
    }
    else {
        $.messager.alert("提示信息", "请先查询再导出！");
    }
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

