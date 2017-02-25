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
//操作标识
var operateIdentifer;
var dicColumnNames = {};

var monitorRealName = "";
var currentDevId = GetQueryString("devid");

$(document).ready(function () {
    var defaultTheme = $.cookie("psbsTheme");
    if (defaultTheme != null && defaultTheme != "default") {
        var link = $(document).find('link:first');
        link.attr('href', '../App_Themes/easyui/themes/' + defaultTheme + '/easyui.css');
    }
    InitTimeControl();
    GetSystemInfo();
    $("#cbb_DevNameCombobox").combobox({
        onSelect: function (rec) {

            var treeObj = $("#divAreaTree");
            var rootNode = treeObj.tree('getRoot');
            treeObj.tree("uncheck", rootNode.target);
            var node = treeObj.tree('find', "dn_" + rec.ID);
            treeObj.tree('check', node.target);

        }
    });
});


//初始化时间框
function InitTimeControl() {
    var e = new Date();
    $("#txt_StartTime").val(e.Format("yyyy-MM-dd 00:00"));
    $("#txt_EndTime").val(e.Format("yyyy-MM-dd HH:mm"));
}

function checkCurId(devid) {
    if (!currentDevId) {
        return;
    }
    if (!deviceNodeLoaded) {
        window.setTimeout("checkCurId(" + devid + ")", 500);
        return;
    }
    var treeObj = $("#divAreaTree");
    var rootNode = treeObj.tree('getRoot');
    treeObj.tree("uncheck", rootNode.target);
    var node = treeObj.tree('find', "dn_" + currentDevId);
    treeObj.tree('check', node.target);
    Btn_Query_Click();
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
                LoadEventTree("divAreaTree", mnId, true, true);
                LoadCbbDevice(mnId);
                monitorRealName = data.SysStateInfo.监测点级别名称;
                $('#divContainer').layout('panel', 'west').panel({ title: monitorRealName + "列表" });
                $("#wellname").text(monitorRealName);
                checkCurId(currentDevId);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

//查询的按钮事件
function Btn_Query_Click() {
    //获取起始时间
    var _startTime = $("#txt_StartTime").val();
    //获取结束时间
    var _endTime = $("#txt_EndTime").val();

    if (!$.CheckTime(_startTime, _endTime)) {
        return;
    }
    var _eventType = $.QueryCombobox("cbb_DevCombobox");
    if (_eventType == "" || _eventType == null) {
        $.messager.alert("提示信息", "请选择事件类型！");
        return;
    }
    if (checkedDeviceIds.length == 0) {
        $.messager.alert("提示信息", "请选择设备！");
        return;
    }
    var devIDs = checkedDeviceIds.join(",");
    $.ShowMask("数据加载中，请稍等……");
    LoadTableData(devIDs, _startTime, _endTime, _eventType);
}

function LoadTableData(_devIds, _startTime, _endTime, _eventType) {

    $('#tbOperateInfos').datagrid({ pageNumber: 1 }).datagrid('loadData', { total: 0, rows: [] });
    waterRecordsCount = 0;

    $.ajax(
    {
        url: "../WebServices/DataQueryService.asmx/GetEventRecordsCount",
        data: { "loginIdentifer": window.parent.guid, "deviceIDs": _devIds, "startTime": _startTime, "endTime": _endTime, "eventType": _eventType },
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
    var gridcolumns = [];
    tableData = [];
    var colflag = false;
    $.ajax({
        url: "../WebServices/DataQueryService.asmx/GetEventRecords",
        data: { "loginIdentifer": window.parent.guid, "operateIdentifer": operateIdentifer, "startIndex": startIndex, "count": pageSize },
        type: "GET",
        dataType: "text",
        cache: false,
        success: function (responseText) {
            try {
                var data = eval("(" + $.xml2json(responseText) + ")");
                if (data.Result) {
                    var headertext = "";
                    var DevDatas = data.Records;
                    var Columns = data.Columns;
                    for (var i = 0; i < DevDatas.length; i++) {
                        if (i == 0) {
                            colflag = true;
                        } else {
                            colflag = false;
                        }
                        var tableRow = {};
                        for (var item in DevDatas[i]) {
                            if (item == "通讯状态") {
                                if (DevDatas[i][item].Value == "全部正常") {
                                    DevDatas[i][item].Value = "<Img style='height:18px;width:18px;' title='通讯状态:\r\n正常' src='../images/正常.png'/>";
                                } else {
                                    DevDatas[i][item].Value = "<Img style='height:18px;width:18px;' src='../images/中断.png'/>";
                                }
                            } else if (item == "设备状态") {
                                if (DevDatas[i][item].Value == "全部正常") {
                                    DevDatas[i][item].Value = "<Img style='height:18px;width:18px;' src='../images/正常.png'/>";
                                } else {
                                    DevDatas[i][item].Value = "<Img style='height:18px;width:18px;' src='../images/中断.png'/>";
                                }
                            }
                            tableRow[DevDatas[i][item].Field] = DevDatas[i][item].Value;
                            if (colflag) {
                                var colWidth = 100;
                                if (item == "事件类型" || item == "事件时间" || item == "设备编号") {
                                    colWidth = 120;
                                }
                                else if (item == "事件描述" || item == "事件数据") {
                                    colWidth = 500;
                                }
                                var gridtitle = "";
                                if (item.length > 4) {
                                    gridtitle = Columns[item].HeadText.substr(0, 4) + "<br/>" + Columns[item].HeadText.substr(4, item.length);
                                } else {
                                    gridtitle = Columns[item].HeadText;
                                }
                                gridcolumns.push({ field: DevDatas[i][item].Field, title: gridtitle, width: colWidth });

                            }
                        }
                        tableData.push(tableRow);
                    }

                    var tableDataObj = {
                        total: waterRecordsCount,
                        rows: tableData
                    }
                    $('#tbOperateInfos').datagrid({ columns: [gridcolumns] }).datagrid('loadData', tableDataObj);
                    $.HideMask();
                }
                else {
                    $.HideMask();
                    $.messager.alert("提示信息", data.Message);
                }
            }
            catch (e) {
                $.HideMask();
                $.messager.alert("提示信息", e.message);
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
    if (operateIdentifer == null || operateIdentifer == "") {
        $.messager.alert("提示信息", "请先进行查询");
        return;
    }
    //查询结果为空
    if (waterRecordsCount == 0) {
        $.messager.alert("提示信息", "查询结果为空，不需要导出");
        return;
    }
    //获取分页中的报警记录
    $.ajax(
    {
        url: "../WebServices/DataQueryService.asmx/ExportEventRecords",
        type: "GET",
        data: { "loginIdentifer": window.parent.guid, "operateIdentifer": operateIdentifer, "startIndex": "1", "count": waterRecordsCount },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            $.HideMask();
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                location.href = data.ExcelUrl;
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

//加载测点combobox
function LoadCbbDevice(mid) {
    $.ajax(
    {
        url: "../WebServices/DeviceNodeService.asmx/GetDeviceNodeInfosByMnId",
        type: "GET",
        data: { "loginIdentifer": window.parent.guid, "mnID": mid, "isRecursive": true, "isExport": false },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            deviceJson = data.DeviceNodes;

            //指定Json数组
            $("#cbb_DevNameCombobox").combobox({ data: deviceJson, textField: '名称', valueField: 'ID' });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}