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

var managerRealName = "";
//-----------------
var loginIdentifer = window.parent.guid;
var excelUrl = "";
var yearexcelUrl = "";
var monthexcelUrl = "";
var dayexcelUrl = "";

//-------------
var yearTime = "";
var monthTime = "";
var dayTime = "";

var yearflag = false;
var monthflag = false;
var dayflag = false;



$(document).ready(function () {
    var defaultTheme = $.cookie("psbsTheme");
    if (defaultTheme != null && defaultTheme != "default") {
        var link = $(document).find('link:first');
        link.attr('href', '../App_Themes/easyui/themes/' + defaultTheme + '/easyui.css');
    }
    InitTimeControl();
    GetSystemInfo();
    $("#villageStatisticTabs").tabs({
        onSelect: function (title, index) {
            if (title == "按年统计" && !yearflag) {
                $("#yearStatistic").datagrid({
                    view: detailview,
                    detailFormatter: function (index, row) {
                        return '<div style="padding:2px"><table class="ddv"></table></div>';
                    },
                    onExpandRow: function (index, row) {
                        $.ajax(
                        {
                            url: "../WebServices/DataReportService.asmx/GetWaterUsersUseWaterSummaryReportByVillage",
                            data: { "loginIdentifer": window.parent.guid, "villageId": row.villageID, "reportType": "月", "reportStartTime": row.startTime, "reportEndTime": "" },
                            type: "GET",
                            dataType: "text",
                            cache: false,
                            success: function (responseText) {
                                var data = eval("(" + $.xml2json(responseText) + ")");
                                if (data.Result == true) {
                                    datas = [];
                                    var useWaterRecords = data.ReportDatas;
                                    if (useWaterRecords.length > 0) {
                                        for (var i = 0; i < useWaterRecords.length; i++) {
                                            var dataRow = {};
                                            dataRow["userID"] = useWaterRecords[i].用水户ID;
                                            dataRow["userName"] = useWaterRecords[i].用水户;
                                            dataRow["waterTime"] = useWaterRecords[i].灌溉时长;
                                            dataRow["waterNum"] = useWaterRecords[i].用水量;
                                            dataRow["electricNum"] = useWaterRecords[i].用电量;
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
                                                { field: 'userName', title: '用水户', width: 100, align: 'center' },
                                                { field: 'waterTime', title: '用水时间', width: 100, align: 'center' },
                                                { field: 'waterNum', title: '用水量', width: 100, align: 'center' },
                                                { field: 'electricNum', title: '用电量', width: 100, align: 'center' }
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
                            url: "../WebServices/DataReportService.asmx/GetWaterUsersUseWaterSummaryReportByVillage",
                            data: { "loginIdentifer": window.parent.guid, "villageId": row.villageID, "reportType": "日", "reportStartTime": row.startTime, "reportEndTime": "" },
                            type: "GET",
                            dataType: "text",
                            cache: false,
                            success: function (responseText) {
                                var data = eval("(" + $.xml2json(responseText) + ")");
                                if (data.Result == true) {
                                    datas = [];
                                    var useWaterRecords = data.ReportDatas;
                                    if (useWaterRecords.length > 0) {
                                        for (var i = 0; i < useWaterRecords.length; i++) {
                                            var dataRow = {};
                                            dataRow["userID"] = useWaterRecords[i].用水户ID;
                                            dataRow["userName"] = useWaterRecords[i].用水户;
                                            dataRow["waterTime"] = useWaterRecords[i].灌溉时长;
                                            dataRow["waterNum"] = useWaterRecords[i].用水量;
                                            dataRow["electricNum"] = useWaterRecords[i].用电量;
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
                                                { field: 'userName', title: '用水户', width: 100, align: 'center' },
                                                { field: 'waterTime', title: '用水时间', width: 100, align: 'center' },
                                                { field: 'waterNum', title: '用水量', width: 100, align: 'center' },
                                                { field: 'electricNum', title: '用电量', width: 100, align: 'center' }
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
                            url: "../WebServices/DataReportService.asmx/GetWaterUsersUseWaterSummaryReportByVillage",
                            data: { "loginIdentifer": window.parent.guid, "villageId": row.villageID, "reportType": "时", "reportStartTime": row.startTime, "reportEndTime": "" },
                            type: "GET",
                            dataType: "text",
                            cache: false,
                            success: function (responseText) {
                                var data = eval("(" + $.xml2json(responseText) + ")");
                                if (data.Result == true) {
                                    datas = [];
                                    var useWaterRecords = data.ReportDatas;
                                    if (useWaterRecords.length > 0) {
                                        for (var i = 0; i < useWaterRecords.length; i++) {
                                            var dataRow = {};
                                            dataRow["userID"] = useWaterRecords[i].用水户ID;
                                            dataRow["userName"] = useWaterRecords[i].用水户;
                                            dataRow["waterTime"] = useWaterRecords[i].灌溉时长;
                                            dataRow["waterNum"] = useWaterRecords[i].用水量;
                                            dataRow["electricNum"] = useWaterRecords[i].用电量;
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
                                                { field: 'userName', title: '用水户', width: 100, align: 'center' },
                                                { field: 'waterTime', title: '用水时间', width: 100, align: 'center' },
                                                { field: 'waterNum', title: '用水量', width: 100, align: 'center' },
                                                { field: 'electricNum', title: '用电量', width: 100, align: 'center' }
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
        url: "../WebServices/DataReportService.asmx/GetWaterUsersUseWaterSummaryReportByVillage",
        data: { "loginIdentifer": window.parent.guid, "villageId": row.villageID, "reportType": "任意", "reportStartTime": $("#txt_StartTime").val(), "reportEndTime": $("#txt_EndTime").val() },
        type: "GET",
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result == true) {
                datas = [];
                var useWaterRecords = data.ReportDatas;
                if (useWaterRecords.length > 0) {
                    for (var i = 0; i < useWaterRecords.length; i++) {
                        var dataRow = {};
                        dataRow["userID"] = useWaterRecords[i].用水户ID;
                        dataRow["userName"] = useWaterRecords[i].用水户;
                        dataRow["waterTime"] = useWaterRecords[i].灌溉时长;
                        dataRow["waterNum"] = useWaterRecords[i].用水量;
                        dataRow["electricNum"] = useWaterRecords[i].用电量;
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
                            { field: 'userName', title: '用水户', width: 100, align: 'center' },
                            { field: 'waterTime', title: '用水时间', width: 100, align: 'center' },
                            { field: 'waterNum', title: '用水量', width: 100, align: 'center' },
                            { field: 'electricNum', title: '用电量', width: 100, align: 'center' }
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
                } else {
                    var ddv = $("#tbRechargeInfos").datagrid('getRowDetail', index).find('div');
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
    })
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
                managerRealName = data.SysStateInfo.监测点管理名称;
                //$("#villagename").text(managerRealName);
                //$("#yvillagename").text(managerRealName);
                //$("#mvillagename").text(managerRealName);
                //$("#dvillagename").text(managerRealName);
                TreeBindSelect();
                LoadStatisticTree("divAreaTree", mnId, false, false);

                setTimeout(function () {
                    Btn_Query_Click();
                }, 1000);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert(errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

//点击左侧树形重新加载右侧Combobox
function TreeBindSelect() {
    $('#divAreaTree').tree({
        onSelect: function (node) {
            //清空combox数据
            $("#cbb_DevCombobox").combobox({ data: [] });
            $("#cbb_YearDevCombobox").combobox({ data: [] });
            $("#cbb_MonthDevCombobox").combobox({ data: [] });
            $("#cbb_DayDevCombobox").combobox({ data: [] });

            if (node.id.indexOf("z_") >= 0) {
                $.ajax(
                {
                    url: "../WebServices/ManageNodeService.asmx/GetSimpleManageNodeInfosByMnID",
                    type: "GET",
                    data: { loginIdentifer: window.parent.guid, mnID: node.attributes.mid },
                    cache: false,
                    dataType: "text",
                    success: function (responseText) {
                        var data = eval("(" + $.xml2json(responseText) + ")");
                        if (data.Result)//成功
                        {
                            var villagenode = data.ManageNodes;
                            var comboBoxDataLevel = [];
                            var singlecombodata = [];
                            allcomboid = "";
                            for (i = 0; i < villagenode.length; i++) {
                                var levelObj = {};
                                levelObj["id"] = villagenode[i].ID;
                                levelObj["text"] = villagenode[i].Name;
                                comboBoxDataLevel.push(levelObj);
                                singlecombodata.push(levelObj);
                                if (allcomboid != "") {
                                    allcomboid += ",";
                                }
                                allcomboid += villagenode[i].ID;
                            }
                            var allObj = {};
                            allObj["id"] = allcomboid + "all";
                            allObj["text"] = "全部";
                            allObj["selected"] = true;
                            comboBoxDataLevel.unshift(allObj);
                            $("#cbb_DevCombobox").combobox({
                                data: comboBoxDataLevel
                            });
                            $("#cbb_YearDevCombobox").combobox({
                                data: singlecombodata
                            });
                            $("#cbb_MonthDevCombobox").combobox({
                                data: singlecombodata
                            });
                            $("#cbb_DayDevCombobox").combobox({
                                data: singlecombodata
                            });
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        $.messager.alert(errorThrown + "<br/>" + XMLHttpRequest.responseText);
                    }
                });
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
    //按村进行指定时间段内的用水汇总统计

    var startTime = $("#txt_StartTime").val();
    var endTime = $("#txt_EndTime").val();

    //数据完整性验证
    if (!$.CheckTime(startTime, endTime)) {
        return;
    }

    var _villageStatistic = $.QueryCombobox("cbb_DevCombobox");
    if (_villageStatistic.indexOf("all") >= 0) {
        _villageStatistic = _villageStatistic.substr(0, _villageStatistic.length - 3);
    }
    if (_villageStatistic == "" || _villageStatistic == null) {
        $.messager.alert("村庄不存在，请重新选择！");
        return;
    }

    //清空导出路径
    excelUrl = "";
    //清空datagrid列表、合计行
    $("#tbRechargeInfos").datagrid('loadData', { total: 0, rows: [], footer: [] });
    $.ShowMask("数据加载中，请稍等……");
    $.ajax({
        url: "../WebServices/DataReportService.asmx/GetUseWaterSummaryReportByVillages",
        data: { "loginIdentifer": loginIdentifer, "villageIds": _villageStatistic, "startTime": startTime, "endTime": endTime },
        type: "Get",
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");

            if (data.Result) {
                var useWaterInfo = data.村用水;
                for (var i = 0; i < useWaterInfo.length; i++) {
                    var dataRow = {};
                    dataRow["villageID"] = useWaterInfo[i].村庄ID;
                    dataRow["villageName"] = useWaterInfo[i].村庄;
                    dataRow["waterTime"] = useWaterInfo[i].灌溉时长;
                    dataRow["waterNum"] = useWaterInfo[i].用水量;
                    dataRow["electricNum"] = useWaterInfo[i].用电量;
                    datas.push(dataRow);
                }
                $("#tbRechargeInfos").datagrid({ data: datas });
                var useWaterTotal = data.合计;
                $("#tbRechargeInfos").datagrid({ loadFilter: pagerFilter }).datagrid('reloadFooter', [{ "villageName": "合计", "waterTime": useWaterTotal.灌溉时长, "waterNum": useWaterTotal.用水量, "electricNum": useWaterTotal.用电量 }]);
                $.HideMask();
                excelUrl = data.ExcelURL;
            }
            else {
                $.HideMask();
                $.messager.alert(data.Message);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.HideMask();
            $.messager.alert(errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

//导出Excel的按钮事件
function Btn_Excel_Click() {
    if (excelUrl == null || excelUrl == "") {
        $.messager.alert("请先进行查询");
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

