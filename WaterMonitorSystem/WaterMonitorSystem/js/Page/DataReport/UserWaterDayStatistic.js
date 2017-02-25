
$(document).ready(function () {
    InitDayTimeControl();
});

function daycheckit(isChecked) {
    var url = "";
    var data = {};
    if (isChecked) {
        url = "../WebServices/WaterUserService.asmx/GetAllWaterUsersByVillageId";
        data = { "loginIdentifer": window.parent.guid, "villageId": villageid }
        LoadDayCombobox(url, data, true);
    } else {
        url = "../WebServices/WaterUserService.asmx/GetWaterUsersByVillageId";
        data = { "loginIdentifer": window.parent.guid, "villageId": villageid, "isExport": false };
        LoadDayCombobox(url, data, false);
    }
}



//初始化时间框
function InitDayTimeControl() {
    var e = new Date();
    $("#txt_DayTime").val(e.Format("yyyy-MM-dd"));
}

//查询的按钮事件
function Btn_DayQuery_Click() {
    var datas = [];

    //按用水户进行指定时间段内的用水汇总统计

    dayTime = $("#txt_DayTime").val();

    //数据完整性验证
    if (dayTime == "" || dayTime == null) {
        $.messager.alert("提示信息", "请选择时间！");
        return;
    }

    var waterUserIds = $("#cbb_DayDevCombobox").combobox("getValue");

    if (waterUserIds == "" || waterUserIds == null) {
        $.messager.alert("提示信息", "用水户不存在，请重新选择！");
        return;
    }

    //清空datagrid列表、合计行
    $("#dayStatistic").datagrid('loadData', { total: 0, rows: [], footer: [] });
    $.ShowMask("数据加载中，请稍等……");
    //清空导出路径
    dayexcelUrl = "";

    $.ajax({
        url: "../WebServices/DataReportService.asmx/GetUseWaterPeriodReportByWaterUser",
        data: { "loginIdentifer": loginIdentifer, "waterUserId": waterUserIds, "reportTime": dayTime, "reportType": "日报表" },
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
                    dataRow["hour"] = useWaterInfo[i].小时;
                    dataRow["waterTime"] = useWaterInfo[i].灌溉时长;
                    dataRow["waterNum"] = useWaterInfo[i].用水量;
                    dataRow["electricNum"] = useWaterInfo[i].用电量;
                    dataRow["startTime"] = useWaterInfo[i].起始时间;
                    datas.push(dataRow);
                }
                $("#dayStatistic").datagrid({ data: datas });
                //户用水合计
                var useWaterTotal = data.TotalData;
                $("#dayStatistic").datagrid('reloadFooter', [{ "hour": "合计", "waterTime": useWaterTotal.灌溉时长, "waterNum": useWaterTotal.用水量, "electricNum": useWaterTotal.用电量 }]);
                $.HideMask();
                //导出Excel路径
                dayexcelUrl = data.ExcelURL;
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
function Btn_DayExcel_Click() {
    if (dayexcelUrl == null || dayexcelUrl == "") {
        $.messager.alert("提示信息", "请先进行查询");
        return;
    }

    location.href = dayexcelUrl;
}



