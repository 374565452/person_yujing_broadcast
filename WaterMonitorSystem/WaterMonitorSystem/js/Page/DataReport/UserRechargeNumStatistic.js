// JScript 文件



$(document).ready(function () {
    InitNumTimeControl();
    $.CheckBoxCombobox("cbb_NumDevCombobox");
});

function QueryNumData(pageNumber, pageSize, index) {
    var startNumber = pageNumber;

    //获取分页中的历史记录
    $.ajax(
    {
        url: "../WebServices/DataQueryService.asmx/GetSaleWaterRecordsByTimes",
        type: "GET",
        data: { 'loginIdentifer': loginIdentifer, 'operateIdentifer': operateNumIdentifer, 'startIndex': startNumber, 'count': pageSize },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                var SaleWaterRecords = data.Records;
                datas = [];
                for (var i in SaleWaterRecords) {
                    var tableRow = {};
                    tableRow["userName"] = SaleWaterRecords[i].用水户名称;
                    tableRow["chargeTime"] = SaleWaterRecords[i].售水时间;
                    tableRow["chargeAmount"] = SaleWaterRecords[i].实收金额;
                    tableRow["waterAmount"] = SaleWaterRecords[i].售水金额;
                    tableRow["waterNum"] = SaleWaterRecords[i].售出水量;
                    tableRow["electricAmount"] = SaleWaterRecords[i].售电金额;
                    tableRow["electricNum"] = SaleWaterRecords[i].售出电量;
                    tableRow["handler"] = SaleWaterRecords[i].购水人;
                    tableRow["operator"] = SaleWaterRecords[i].操作员名称;
                    datas.push(tableRow);
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
                        { field: 'chargeTime', title: '充值时间', width: 100, align: 'center' },
                        { field: 'chargeAmount', title: '充值金额', width: 100, align: 'center' },
                        { field: 'waterAmount', title: '购水金额', width: 100, align: 'center' },
                        { field: 'waterNum', title: '购水量', width: 100, align: 'center' },
                        { field: 'electricAmount', title: '购电金额', width: 100, align: 'center' },
                        { field: 'electricNum', title: '购电量', width: 100, align: 'center' },
                        { field: 'handler', title: '代办人', width: 100, align: 'center' },
                        { field: 'operator', title: '操作员', width: 100, align: 'center' }
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
    numExcelUrl = "";

    $.ajax(
    {
        url: "../WebServices/DataReportService.asmx/GetSaleWaterTimesSummaryReportByWaterUsers",
        type: "GET",
        data: { 'loginIdentifer': loginIdentifer, 'waterUserIds': waterUserIds, 'baseTime': numTime, 'isAsc': isAsc, 'saleWaterTimes': numtext },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                numExcelUrl = data.ExcelURL;
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
                $("#numStatistic").datagrid({ loadFilter: pagerFilter }).datagrid("loadData", tableData)

                $('#numStatistic').datagrid('reloadFooter', [
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
function Btn_NumExcel_Click() {
    if (numExcelUrl == null || numExcelUrl == "") {
        $.messager.alert("提示信息", "请先进行查询");
        return;
    }

    location.href = numExcelUrl;
}



