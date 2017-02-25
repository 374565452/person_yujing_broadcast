
//记录总条数
var totalRecord = "";
//操作标示
var operateIdentifer = "";
//登录标示
var guid = window.parent.guid;

var monitorRealName = "";

var currentdevid = GetQueryString("devid");

$(document).ready(function () {
    var defaultTheme = $.cookie("psbsTheme");
    if (defaultTheme != null && defaultTheme != "default") {
        var link = $(document).find('link:first');
        link.attr('href', '../App_Themes/easyui/themes/' + defaultTheme + '/easyui.css');
    }
    InitTimeControl();
    GetSystemInfo();

    $("#cbb_DevCombobox").combobox({
        onSelect: function (rec) {
            var treeObj = $("#divAreaTree");
            var rootNode = treeObj.tree('getRoot');
            treeObj.tree("uncheck", rootNode.target);
            var node = treeObj.tree('find', "dn_" + rec.ID);
            treeObj.tree('check', node.target);
        }
    });
});

function checkcurid(devid) {
    if (!currentdevid) {
        return;
    }
    if (!deviceNodeLoaded) {
        window.setTimeout("checkcurid(" + devid + ")", 500);
        return;
    }
    var treeObj = $("#divAreaTree");
    var rootNode = treeObj.tree('getRoot');
    treeObj.tree("uncheck", rootNode.target);
    var node = treeObj.tree('find', "dn_" + currentdevid);
    treeObj.tree('check', node.target);
    Btn_Query_Click();
}

//初始化时间框
function InitTimeControl() {
    var e = new Date();
    $("#txt_StartTime").val(e.Format("yyyy-MM-dd 00:00"));
    $("#txt_EndTime").val(e.Format("yyyy-MM-dd HH:mm"));
}

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
                LoadTree("divAreaTree", mnId, true, true);
                LoadDevCbbData();
                monitorRealName = data.SysStateInfo.监测点级别名称;
                $('#divContainer').layout('panel', 'west').panel({ title: monitorRealName + "列表" });
                $("#wellname").text(monitorRealName);
                checkcurid(currentdevid);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function LoadDevCbbData() {
    $.ajax(
    {
        url: "../WebServices/DeviceNodeService.asmx/GetDeviceNodeInfosByMnId",
        type: "GET",
        data: { "loginIdentifer": guid, "mnID": mnId, "isRecursive": true, "isExport": false },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            deviceJson = data.DeviceNodes;

            //指定Json数组
            $("#cbb_DevCombobox").combobox({ data: deviceJson, textField: '名称', valueField: 'ID' });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function Btn_Query_Click() {
    $.ShowMask("数据加载中，请稍等……");
    var startTime = $("#txt_StartTime").val();
    var endTime = $("#txt_EndTime").val();
    if (!$.CheckTime(startTime, endTime)) {
        return;
    }
    var devIDs = "";
    var treeObj;
    treeObj = $('#divAreaTree');
    var checkedNodes = treeObj.tree('getChecked');
    for (var i = 0; i < checkedNodes.length; i++) {
        var checkedNode = checkedNodes[i];
        if (checkedNode.attributes["nodeType"] == "manage") {
            continue;
        }
        if (devIDs != "") {
            devIDs += ",";
        }
        devIDs += checkedNode.attributes["did"];
    }
    $('#tbOperateInfos').datagrid({ pageNumber: 1 }).datagrid('loadData', { total: 0, rows: [] });
    totalRecord = 0;
    $.ajax(
    {
        url: "../WebServices/DataQueryService.asmx/GetHistoryRecordsCount",
        type: "GET",
        data: { 'loginIdentifer': guid, 'deviceIDs': devIDs, 'startTime': startTime, 'endTime': endTime, 'dataSource': "", 'recordtype': "", 'timeType': false },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                totalRecord = data.Count;
                operateIdentifer = data.Guid;
                if (totalRecord == 0) {
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
    var startNumber = (pageNumber - 1) * pageSize + 1;
    var endNumber = pageNumber * pageSize;
    var tableData = [];
    var columnflag = false;
    var gridcolumns = [];
    //获取分页中的历史记录
    $.ajax(
    {
        url: "../WebServices/DataQueryService.asmx/GetHistoryRecords",
        type: "GET",
        data: { 'loginIdentifer': guid, 'operateIdentifer': operateIdentifer, 'startIndex': startNumber, 'count': pageSize },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                var historyDataJson = data.Records;
                for (var i = 0; i < historyDataJson.length; i++) {
                    if (i == 0) {
                        columnflag = true;
                    }
                    else {
                        columnflag = false;
                    }
                    var tableRow = {};
                    for (var item in historyDataJson[i]) {
                        tableRow[historyDataJson[i][item].Field] = historyDataJson[i][item].Value;
                        if (columnflag) {
                            var gridtitle = "";
                            if (item.length > 4) {
                                gridtitle = item.substr(0, 4) + "<br/>" + item.substr(4, item.length);
                            } else {
                                gridtitle = item;
                            }
                            var colWidth = 70;
                            if(item == "记录时间" || item == "采集时间" || item == "开泵时间" || item == "关泵时间")
                            {
                                colWidth = 128;
                            }
                            else if(item == "设备编号")
                            {
                                colWidth = 100;
                            }
                            gridcolumns.push({ field: historyDataJson[i][item].Field, title: gridtitle, width: colWidth, align: 'center' });
                        }
                    }
                    tableData.push(tableRow);
                }
                var tableDataObj = {
                    total: totalRecord,
                    rows: tableData
                }
                $('#tbOperateInfos').datagrid({ columns: [gridcolumns] }).datagrid('loadData', tableDataObj);
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

function BindOnSelectPage() {
    var dg = $("#tbOperateInfos");
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
            $.ShowMask("数据加载中，请稍等……");
            QueryCurrentPageData(pageNum, pageSize)
        }
    });
}

function Btn_Excel_Click() {
    $.ajax(
    {
        url: "../WebServices/DataQueryService.asmx/ExportHistoryRecords",
        type: "GET",
        data: { 'loginIdentifer': guid, 'operateIdentifer': operateIdentifer, 'startIndex': "1", 'count': totalRecord },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                var ExcelUrl = data.ExcelUrl;
                location.href = ExcelUrl;
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