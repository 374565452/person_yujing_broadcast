

$(document).ready(function () {
    InitYearTimeControl();

});


function yearcheckit(isChecked) {
    var url = "";
    var data = {};
    if (isChecked) {
        url = "../WebServices/WaterUserService.asmx/GetAllWaterUsersByVillageId";
        data = { "loginIdentifer": window.parent.guid, "villageId": villageid }
        LoadYearCombobox(url, data, true);
    } else {
        url = "../WebServices/WaterUserService.asmx/GetWaterUsersByVillageId";
        data = { "loginIdentifer": window.parent.guid, "villageId": villageid, "isExport": false };
        LoadYearCombobox(url, data, false);
    }
}



//初始化时间框
function InitYearTimeControl() {
    var e = new Date();
    $("#txt_YearTime").val(e.Format("yyyy"));
}

//查询的按钮事件
function Btn_YearQuery_Click() {
    var datas = [];

    //按用水户进行指定时间段内的用水汇总统计

    yearTime = $("#txt_YearTime").val();

    //数据完整性验证
    if (yearTime == "" || yearTime == null) {
        $.messager.alert("提示信息", "请选择时间！");
        return;
    }

    var waterUserIds = $("#cbb_YearDevCombobox").combobox("getValue");

    if (waterUserIds == "" || waterUserIds == null) {
        $.messager.alert("提示信息", "用水户不存在，请重新选择！");
        return;
    }

    //清空datagrid列表、合计行
    $("#yearStatistic").datagrid('loadData', { total: 0, rows: [], footer: [] });
    $.ShowMask("数据加载中，请稍等……");
    //清空导出路径
    yearexcelUrl = "";

    $.ajax({
        url: "../WebServices/DataReportService.asmx/GetUseWaterPeriodReportByWaterUser",
        data: { "loginIdentifer": loginIdentifer, "waterUserId": waterUserIds, "reportTime": yearTime,"reportType":"年报表" },
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
                    dataRow["month"] = useWaterInfo[i].月份;
                    dataRow["waterTime"] = useWaterInfo[i].灌溉时长;
                    dataRow["waterNum"] = useWaterInfo[i].用水量;
                    dataRow["electricNum"] = useWaterInfo[i].用电量;
                    dataRow["startTime"] = useWaterInfo[i].起始时间;
                    datas.push(dataRow);
                }
                $("#yearStatistic").datagrid({ data: datas });
                //户用水合计
                var useWaterTotal = data.TotalData;
                $("#yearStatistic").datagrid('reloadFooter', [{ "month": "合计", "waterTime": useWaterTotal.灌溉时长, "waterNum": useWaterTotal.用水量, "electricNum": useWaterTotal.用电量 }]);
                $.HideMask();
                //导出Excel路径
                yearexcelUrl = data.ExcelURL;
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
function Btn_YearExcel_Click() {
    if (yearexcelUrl == null || yearexcelUrl == "") {
        $.messager.alert("提示信息", "请先进行查询");
        return;
    }

    location.href = yearexcelUrl;
}



