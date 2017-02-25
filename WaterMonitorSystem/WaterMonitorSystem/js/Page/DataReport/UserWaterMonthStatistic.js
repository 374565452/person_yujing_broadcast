
$(document).ready(function () {
    InitMonthTimeControl();
});


function monthcheckit(isChecked) {
    var url = "";
    var data = {};
    if (isChecked) {
        url = "../WebServices/WaterUserService.asmx/GetAllWaterUsersByVillageId";
        data = { "loginIdentifer": window.parent.guid, "villageId": villageid }
        LoadMonthCombobox(url, data, true);
    } else {
        url = "../WebServices/WaterUserService.asmx/GetWaterUsersByVillageId";
        data = { "loginIdentifer": window.parent.guid, "villageId": villageid, "isExport": false };
        LoadMonthCombobox(url, data, false);
    }
}



//初始化时间框
function InitMonthTimeControl() {
    var e = new Date();
    $("#txt_MonthTime").val(e.Format("yyyy-MM"));
}

//查询的按钮事件
function Btn_MonthQuery_Click() {
    var datas = [];

    //按用水户进行指定时间段内的用水汇总统计

    monthTime = $("#txt_MonthTime").val();

    //数据完整性验证
    if (monthTime == "" || monthTime == null) {
        $.messager.alert("提示信息", "请选择时间！");
        return;
    }

    var waterUserIds = $("#cbb_MonthDevCombobox").combobox("getValue");

    if (waterUserIds == "" || waterUserIds == null) {
        $.messager.alert("提示信息", "用水户不存在，请重新选择！");
        return;
    }

    //清空datagrid列表、合计行
    $("#monthStatistic").datagrid('loadData', { total: 0, rows: [], footer: [] });
    $.ShowMask("数据加载中，请稍等……");
    //清空导出路径
    monthexcelUrl = "";

    $.ajax({
        url: "../WebServices/DataReportService.asmx/GetUseWaterPeriodReportByWaterUser",
        data: { "loginIdentifer": loginIdentifer, "waterUserId": waterUserIds, "reportTime": monthTime, "reportType": "月报表" },
        type: "Get",
        cache: false,
        dataType: "text",
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                //户用水列表
                var useWaterInfo = data.ReportDatas;
                for (var i = 0; i < useWaterInfo.length; i++) {
                    var dataRow = {};
                    dataRow["userID"] = useWaterInfo[i].用水户ID;
                    dataRow["date"] = useWaterInfo[i].日期;
                    dataRow["waterTime"] = useWaterInfo[i].灌溉时长;
                    dataRow["waterNum"] = useWaterInfo[i].用水量;
                    dataRow["electricNum"] = useWaterInfo[i].用电量;
                    dataRow["startTime"] = useWaterInfo[i].起始时间;
                    datas.push(dataRow);
                }
                $("#monthStatistic").datagrid({ data: datas });
                //户用水合计
                var useWaterTotal = data.TotalData;
                $("#monthStatistic").datagrid('reloadFooter', [{ "date": "合计", "waterTime": useWaterTotal.灌溉时长, "waterNum": useWaterTotal.用水量, "electricNum": useWaterTotal.用电量 }]);
                $.HideMask();
                //导出Excel路径
                monthexcelUrl = data.ExcelURL;
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
function Btn_MonthExcel_Click() {
    if (monthexcelUrl == null || monthexcelUrl == "") {
        $.messager.alert("提示信息", "请先进行查询");
        return;
    }

    location.href = monthexcelUrl;
}



