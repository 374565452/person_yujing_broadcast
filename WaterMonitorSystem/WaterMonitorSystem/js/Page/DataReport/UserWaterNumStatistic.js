// JScript 文件



$(document).ready(function () {
    InitNumTimeControl();
    $.CheckBoxCombobox("cbb_NumDevCombobox");
});

function QueryNumData(pageNumber, pageSize, index) {
    var startIndex = pageNumber;
    $.ajax({
        url: "../WebServices/DataQueryService.asmx/GetUseWaterRecordsByTimes",
        data: { "loginIdentifer": window.parent.guid, "operateIdentifer": operateNumIdentifer, "startIndex": startIndex, "count": pageSize },
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


                var ddv = $("#numStatistic").datagrid('getRowDetail', index).find('table.ddv');
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
                        $('#numStatistic').datagrid('fixDetailRowHeight', index);
                    },
                    onLoadSuccess: function () {
                        setTimeout(function () {
                            $('#numStatistic').datagrid('fixDetailRowHeight', index);
                        }, 0);
                    }
                });
                $('#numStatistic').datagrid('fixDetailRowHeight', index);

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


function numcheckit(isChecked) {
    var url = "";
    var data = {};
    if (isChecked) {
        url = "../WebServices/WaterUserService.asmx/GetAllWaterUsersByVillageId";
        data = { "loginIdentifer": window.parent.guid, "villageId": villageid }
        LoadNumCombobox(url, data, true);
    } else {
        url = "../WebServices/WaterUserService.asmx/GetWaterUsersByVillageId";
        data = { "loginIdentifer": window.parent.guid, "villageId": villageid, "isExport": false };
        LoadNumCombobox(url, data, false);
    }
}



//初始化时间框
function InitNumTimeControl() {
    var e = new Date();
    $("#txt_NumTime").val(e.Format("yyyy-MM-dd 00:00"));
}

//查询的按钮事件
function Btn_NumQuery_Click() {



    var datas = [];

    numtext = $("#numText").val();
    numTime = $("#txt_NumTime").val();
    //向前向后
    $("input[type=radio]").each(function () {
        if ($(this)[0].checked) {
            isAsc = $(this).val();
        }
    });
    //数据完整性验证
    
    if (numTime == "" || numTime == null) {
        $.messager.alert("提示信息", "请选择时间！");
        return;
    }
    if (numtext == "" || numtext==null) {
        $.messager.alert("提示信息", "请填写次数！");
        return;
    }
    var reNum = /^\d*$/;
    if (!reNum.test(numtext)) {
        $.messager.alert("提示信息", "次数必须为数字！");
        return;
    }

    var waterUserIds = $.QueryCheckBoxCombobox("cbb_NumDevCombobox");
    if (waterUserIds.indexOf("all") >= 0) {
        waterUserIds = waterUserIds.substr(0, waterUserIds.length - 3);
    }

    if (waterUserIds == "" || waterUserIds == null) {
        $.messager.alert("提示信息", "用水户不存在，请重新选择！");
        return;
    }

    //清空datagrid列表、合计行
    $("#numStatistic").datagrid('loadData', { total: 0, rows: [], footer: [] });
    $.ShowMask("数据加载中，请稍等……");
    //清空导出路径
    excelUrl = "";

    $.ajax({
        url: "../WebServices/DataReportService.asmx/GetUseWaterTimesSummaryReportByWaterUsers",
        data: { "loginIdentifer": loginIdentifer, "waterUserIds": waterUserIds, "baseTime": numTime, "useWaterTimes": numtext, "isAsc": isAsc },
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
                $("#numStatistic").datagrid({ data: datas });
                //户用水合计
                var useWaterTotal = data.合计;
                $("#numStatistic").datagrid({ loadFilter: pagerFilter }).datagrid('reloadFooter', [{ "userName": "合计", "waterTime": useWaterTotal.灌溉时长, "waterNum": useWaterTotal.用水量, "electricNum": useWaterTotal.用电量 }]);
                $.HideMask();
                //导出Excel路径
                numexcelUrl = data.ExcelURL;
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
function Btn_NumExcel_Click() {
    if (numexcelUrl == null || numexcelUrl == "") {
        $.messager.alert("提示信息", "请先进行查询");
        return;
    }

    location.href = numexcelUrl;
}



