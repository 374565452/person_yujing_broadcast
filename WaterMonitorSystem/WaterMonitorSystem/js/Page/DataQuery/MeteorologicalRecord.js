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
var monitorRealName = "";

$(document).ready(function () {
    var defaultTheme = $.cookie("psbsTheme");
    if (defaultTheme != null && defaultTheme != "default") {
        var link = $(document).find('link:first');
        link.attr('href', '../App_Themes/easyui/themes/' + defaultTheme + '/easyui.css');
    }
    InitTimeControl();
    GetSystemInfo();
    Btn_Query_Click();
});


//初始化时间框
function InitTimeControl() {
    var e = new Date();
    $("#txt_StartTime").val(e.Format("yyyy-MM-dd 00:00"));
    $("#txt_EndTime").val(e.Format("yyyy-MM-dd HH:mm"));
    $("#txt_StationID").val("");
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
                monitorRealName = data.SysStateInfo.监测点级别名称;
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

    var _stationID = $("#txt_StationID").val();

    $('#tbWaterInfos').datagrid('loadData', { total: 0, rows: [] });
    $.ShowMask("数据加载中，请稍等……");
    waterRecordsCount = 0;
    //清空操作标识
    operateIdentifer = "";

    $.ajax(
    {
        url: "../WebServices/DataQueryNewService.asmx/GetMeteorologicalRecordsCount",
        data: { "loginIdentifer": window.parent.guid, "stationID": _stationID, "startTime": _startTime, "endTime": _endTime },
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

                    QueryCurrentChartData(1, waterRecordsCount);
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

function QueryCurrentChartData(pageNumber, pageSize) {
    var startIndex = (pageNumber - 1) * pageSize + 1;
    $.ajax({
        url: "../WebServices/DataQueryNewService.asmx/GetMeteorologicalRecordsChart",
        data: { "loginIdentifer": window.parent.guid, "operateIdentifer": operateIdentifer, "startIndex": startIndex, "count": pageSize },
        type: "GET",
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result == true) {
                var xAxis_categories = [];
                var series_name = [];
                var series1 = [];
                var series2 = [];
                var series3 = [];
                var series4 = [];
                var series5 = [];
                for (var i = 0; i < data.Records.length; i++) {
                    if (xAxis_categories.indexOf(data.Records[i].Acq_Time) == -1)
                        xAxis_categories.push(data.Records[i].Acq_Time);

                    if (series_name.indexOf(data.Records[i].StationID) == -1)
                        series_name.push(data.Records[i].StationID);
                }
                for (var i = 0; i < series_name.length; i++) {
                    var myMap1 = {};
                    myMap1['name'] = series_name[i];
                    myMap1['data'] = [];
                    var myMap2 = {};
                    myMap2['name'] = series_name[i];
                    myMap2['data'] = [];
                    var myMap3 = {};
                    myMap3['name'] = series_name[i];
                    myMap3['data'] = [];
                    var myMap4 = {};
                    myMap4['name'] = series_name[i];
                    myMap4['data'] = [];
                    var myMap5 = {};
                    myMap5['name'] = series_name[i];
                    myMap5['data'] = [];

                    var flag = false;
                    for (var j = 0; j < xAxis_categories.length; j++) {
                        flag = false;

                        for (var k = 0; k < data.Records.length; k++) {
                            if (data.Records[k].Acq_Time == xAxis_categories[j] && data.Records[k].StationID == series_name[i]) {
                                myMap1['data'].push(parseFloat(data.Records[k].Temperature));
                                myMap2['data'].push(parseFloat(data.Records[k].Rainfall));
                                myMap3['data'].push(parseFloat(data.Records[k].AirPressure));
                                myMap4['data'].push(parseFloat(data.Records[k].AA_AirRH));
                                myMap5['data'].push(parseFloat(data.Records[k].WindPower));
                                flag = true;
                                break;
                            }
                        }

                        if (!flag) {
                            myMap1['data'].push(null);
                            myMap2['data'].push(null);
                            myMap3['data'].push(null);
                            myMap4['data'].push(null);
                            myMap5['data'].push(null);
                        }
                    }

                    series1.push(myMap1);
                    series2.push(myMap2);
                    series3.push(myMap3);
                    series4.push(myMap4);
                    series5.push(myMap5);
                }

                $('#divChart1').highcharts({
                    chart: { type: 'line' },
                    title: { text: '气象温度折线图' },
                    xAxis: { categories: xAxis_categories },
                    yAxis: { title: { text: '温度（℃）' }, labels: { formatter: function () { return this.value + '℃' } } },
                    tooltip: { crosshairs: true, shared: true },
                    plotOptions: { spline: { marker: { radius: 4, lineColor: '#666666', lineWidth: 1 } } },
                    series: series1
                });

                $('#divChart2').highcharts({
                    chart: { type: 'line' },
                    title: { text: '气象降雨折线图' },
                    xAxis: { categories: xAxis_categories },
                    yAxis: { title: { text: '降雨（mm）' }, labels: { formatter: function () { return this.value + 'mm' } } },
                    tooltip: { crosshairs: true, shared: true },
                    plotOptions: { spline: { marker: { radius: 4, lineColor: '#666666', lineWidth: 1 } } },
                    series: series2
                });

                $('#divChart3').highcharts({
                    chart: { type: 'line' },
                    title: { text: '气象气压折线图' },
                    xAxis: { categories: xAxis_categories },
                    yAxis: { title: { text: '气压（hpa）' }, labels: { formatter: function () { return this.value + 'hpa' } } },
                    tooltip: { crosshairs: true, shared: true },
                    plotOptions: { spline: { marker: { radius: 4, lineColor: '#666666', lineWidth: 1 } } },
                    series: series3
                });

                $('#divChart4').highcharts({
                    chart: { type: 'line' },
                    title: { text: '气象大气湿度折线图' },
                    xAxis: { categories: xAxis_categories },
                    yAxis: { title: { text: '大气湿度（%）' }, labels: { formatter: function () { return this.value + '%' } } },
                    tooltip: { crosshairs: true, shared: true },
                    plotOptions: { spline: { marker: { radius: 4, lineColor: '#666666', lineWidth: 1 } } },
                    series: series4
                });

                $('#divChart5').highcharts({
                    chart: { type: 'line' },
                    title: { text: '气象风速折线图' },
                    xAxis: { categories: xAxis_categories },
                    yAxis: { title: { text: '风速（m/s）' }, labels: { formatter: function () { return this.value + 'm/s' } } },
                    tooltip: { crosshairs: true, shared: true },
                    plotOptions: { spline: { marker: { radius: 4, lineColor: '#666666', lineWidth: 1 } } },
                    series: series5
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

function QueryCurrentPageData(pageNumber, pageSize) {
    var startIndex = (pageNumber - 1) * pageSize + 1;
    $.ajax({
        url: "../WebServices/DataQueryNewService.asmx/GetMeteorologicalRecords",
        data: { "loginIdentifer": window.parent.guid, "operateIdentifer": operateIdentifer, "startIndex": startIndex, "count": pageSize },
        type: "GET",
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result == true) {
                var tableDataObj = {
                    total: waterRecordsCount,
                    rows: data.Records
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

function BindOnSelectPage() {
    var dg = $("#tbWaterInfos");
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
        url: "../WebServices/DataQueryNewService.asmx/ExportMeteorologicalRecords",
        type: "GET",
        data: { "loginIdentifer": window.parent.guid, "operateIdentifer": operateIdentifer, "startIndex": "1", "count": waterRecordsCount },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            $.HideMask();
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
