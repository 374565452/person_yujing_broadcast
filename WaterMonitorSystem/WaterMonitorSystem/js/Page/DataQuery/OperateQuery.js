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

$(document).ready(function () {
    var defaultTheme = $.cookie("psbsTheme");
    if (defaultTheme != null && defaultTheme != "default") {
        var link = $(document).find('link:first');
        link.attr('href', '../App_Themes/easyui/themes/' + defaultTheme + '/easyui.css');
    }
    InitTimeControl();
    GetSystemInfo();
    $('#tbOperateInfos').datagrid('loadData', { total: 0, rows: [] });
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
                LoadUserTree("divAreaTree", mnId, true, true);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

//初始化时间框
function InitTimeControl() {
    var e = new Date();
    $("#txt_StartTime").val(e.Format("yyyy-MM-dd 00:00"));
    $("#txt_EndTime").val(e.Format("yyyy-MM-dd HH:mm"));
}


//查询的按钮事件
function Btn_Query_Click() {
    var usrIDs = "";
    var treeObj = $("#divAreaTree");
    var checkedNodes = treeObj.tree("getChecked");
    for (var i = 0; i < checkedNodes.length; i++) {
        var checkedNode = checkedNodes[i];
        if (checkedNode.attributes["nodeType"] == "manage") {
            continue;
        }
        if (usrIDs != "") {
            usrIDs = usrIDs + ",";
        }
        usrIDs = usrIDs + checkedNode.attributes["uid"];
    }
    var userIds = [];
    //获取起始时间
    var _startTime = $("#txt_StartTime").val();
    //获取结束时间
    var _endTime = $("#txt_EndTime").val();

    if (!$.CheckTime(_startTime, _endTime)) {
        return;
    }

    if (usrIDs == null || usrIDs == "") {
        $.messager.alert("提示信息", "请选择操作员");
        return;
    }
    $.ShowMask("数据加载中，请稍等……");
    LoadTableData(usrIDs, _startTime, _endTime);
}

function LoadTableData(_operatorIds, _startTime, _endTime) {
    _excelURL = "";
    $.ajax(
    {
        url: "../WebServices/DataQueryService.asmx/GetOperateRecordsByUser",
        type: "POST",
        data: { "loginIdentifer": window.parent.guid, operatorIds: _operatorIds, startTime: _startTime, endTime: _endTime, isExprot: false },
        cache: false,
        dataType: "text",
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result)//成功
            {
                tableData = [];
                var recoreJson = data.Records;
                for (var i = 0; i < recoreJson.length; i++) {
                    var tableRow = {};
                    tableRow["operatorName"] = recoreJson[i].操作员[1];
                    tableRow["operateTime"] = recoreJson[i].操作时间[1];
                    tableRow["operateName"] = recoreJson[i].操作名称[1];
                    tableRow["operateDesc"] = "<span style='display:block;cursor:pointer;width:100%；height:100%' title='" + recoreJson[i].操作描述[1] + "'>" + recoreJson[i].操作描述[1] + "</span>";
                    tableRow["RawData"] = recoreJson[i].发送数据[1];
                    tableRow["State"] = recoreJson[i].发送状态[1];
                    tableData.push(tableRow);
                }
                $('#tbOperateInfos').datagrid({ loadFilter: pagerFilter }).datagrid('loadData', tableData);
                $.HideMask();
            }
            else {
                $.HideMask();
                $.messager.alert("提示信息", data.Message);
                $('#tbRechargeInfos').datagrid('loadData', { total: 0, rows: [] });

            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.HideMask();
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function ExportExcel(_operatorIds, _startTime, _endTime) {
    $.ajax(
    {
        url: "../WebServices/DataQueryService.asmx/ExportOperateRecordsByUser",
        type: "POST",
        data: { "loginIdentifer": window.parent.guid, operatorIds: _operatorIds, startTime: _startTime, endTime: _endTime },
        cache: false,
        dataType: "text",
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result)//成功
            {
                _excelURL = data.ExcelURL;
                if (_excelURL != null && _excelURL != "") {
                    window.location.href = _excelURL;
                }
            } else {
                $.messager.alert("提示信息", data.Message);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

//导出Excel的按钮事件
function Btn_Excel_Click() {
    var usrIDs = "";
    var treeObj = $("#divAreaTree");
    var checkedNodes = treeObj.tree("getChecked");
    for (var i = 0; i < checkedNodes.length; i++) {
        var checkedNode = checkedNodes[i];
        if (checkedNode.attributes["nodeType"] == "manage") {
            continue;
        }
        if (usrIDs != "") {
            usrIDs = usrIDs + ",";
        }
        usrIDs = usrIDs + checkedNode.attributes["uid"];
    }
    if (usrIDs == null || usrIDs == "") {
        $.messager.alert("提示信息", "请选择操作员");
        return;
    }
    //获取起始时间
    var _startTime = $("#txt_StartTime").val();
    //获取结束时间
    var _endTime = $("#txt_EndTime").val();

    ExportExcel(usrIDs, _startTime, _endTime);
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
