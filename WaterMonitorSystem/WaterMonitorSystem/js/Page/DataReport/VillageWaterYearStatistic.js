// JScript 文件

$(document).ready(function () {
    InitYearTimeControl();
});

//初始化时间框
function InitYearTimeControl() {
    var e = new Date();
    $("#txt_YearTime").val(e.Format("yyyy"));
}

//查询的按钮事件
function Btn_YearQuery_Click() {
    
    var datas=[];
    //按村进行指定时间段内的用水汇总统计
  
    yearTime = $("#txt_YearTime").val();
    //数据完整性验证
    if (yearTime == "" || yearTime == null) {
        $.messager.alert("提示信息", "请选择时间！");
        return;
    }

    var _villageStatistic = $("#cbb_YearDevCombobox").combobox("getValue");

    if (_villageStatistic == "" || _villageStatistic==null) {
        $.messager.alert("提示信息", "村庄不存在，请重新选择！");
        return;
    }

    //清空导出路径
    yearexcelUrl = "";
    //清空datagrid列表、合计行
    $("#yearStatistic").datagrid('loadData', { total: 0, rows: [], footer: [] });
    $.ShowMask("数据加载中，请稍等……");
    $.ajax({
        url: "../WebServices/DataReportService.asmx/GetUseWaterPeriodReportByVillage",
        data: { "loginIdentifer": loginIdentifer, "villageId": _villageStatistic, "reportType": "年报表", "reportTime": yearTime },
        type:"Get",
        dataType:"text",
        cache:false,
        success:function(responseText)
        {
            var data=eval("("+$.xml2json(responseText)+")");
            
            if(data.Result)
            {
                var useWaterInfo = data.ReportDatas;
                for (var i = 0;i<useWaterInfo.length;i++)
                {
                    var dataRow = {};
                    dataRow["villageID"] = useWaterInfo[i].村庄ID;
                    dataRow["startTime"] = useWaterInfo[i].起始时间;
                    dataRow["month"] = useWaterInfo[i].月份;
                    dataRow["waterTime"]=useWaterInfo[i].灌溉时长;
                    dataRow["waterNum"]=useWaterInfo[i].用水量;
                    dataRow["electricNum"]=useWaterInfo[i].用电量;
                    datas.push(dataRow);
                }
                $("#yearStatistic").datagrid({ data: datas });
                var useWaterTotal = data.TotalData;
                $("#yearStatistic").datagrid('reloadFooter', [{ "month": "合计", "waterTime": useWaterTotal.灌溉时长, "waterNum": useWaterTotal.用水量, "electricNum": useWaterTotal.用电量 }]);
                $.HideMask();
                yearexcelUrl = data.ExcelURL;
            }
            else
            {
                $.HideMask();
                $.messager.alert(data.Message);
            }
        },
        error:function(XMLHttpRequest,textStatus,errorThrown)
        {
            $.HideMask();
            $.messager.alert(errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

//导出Excel的按钮事件
function Btn_YearExcel_Click() {
    if (yearexcelUrl == null || yearexcelUrl == "") {
        $.messager.alert("请先进行查询");
        return;
    }

    location.href = yearexcelUrl;
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

