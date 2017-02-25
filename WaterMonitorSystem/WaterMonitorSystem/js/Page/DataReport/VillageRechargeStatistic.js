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
//-----------
//登录标示
var loginIdentifer = window.parent.guid;
//导出路径
var ExcUrl = "";

$(document).ready(function () {
    var defaultTheme = $.cookie("psbsTheme");
    if (defaultTheme != null && defaultTheme != "default") {
        var link = $(document).find('link:first');
        link.attr('href', '../App_Themes/easyui/themes/' + defaultTheme + '/easyui.css');
    }
    InitTimeControl();
    GetSystemInfo();
    $('#tbRechargeInfos').datagrid('loadData', { total: 0, rows: [],footer:[] });
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
                LoadStatisticTree("divAreaTree", mnId, false, false);
                TreeBindSelect();

                setTimeout(function () {
                    Btn_Query_Click();
                    Btn_QueryChart_Click();
                }, 1000);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

//点击左侧树形重新加载右侧Combobox
function TreeBindSelect() {
    $('#divAreaTree').tree({
        onSelect: function (node) {
            //$("#spanTownName").html(node.attributes.manage.ID + ' | ' + node.attributes.manage.级别名称 + ' | ' + node.attributes.manage.名称);
            if (node.attributes.manage.级别名称 == "乡镇") {
                $("#spanTownName").html(node.attributes.manage.上级名称 + " " + node.attributes.manage.名称);
            }
            else {
                $("#spanTownName").html("");
            }
            $("#cbb_DevCombobox").combobox({data: []});
            if (node.id.indexOf("z_") >= 0) 
            {
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
                            allcomboid = "";
                            for (i = 0; i < villagenode.length; i++) {
                                var levelObj = {};
                                levelObj["id"] = villagenode[i].ID;
                                levelObj["text"] = villagenode[i].Name;
                                comboBoxDataLevel.push(levelObj);
                                if (allcomboid != "") {
                                    allcomboid += ",";
                                }
                                allcomboid += villagenode[i].ID;
                            }
                            var allObj = {};
                            allObj["id"] = allcomboid+"all";
                            allObj["text"] = "全部";
                            allObj["selected"] = true;
                            comboBoxDataLevel.unshift(allObj);
                            $("#cbb_DevCombobox").combobox({
                                data: comboBoxDataLevel
                            });
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
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
    
    var villageIDs = $.QueryCombobox("cbb_DevCombobox");
    if (villageIDs.indexOf("all")>=0) 
    {
        villageIDs = villageIDs.substr(0, villageIDs.length - 3);
    }
    if (villageIDs == "" || villageIDs==null) 
    {
        $.messager.alert("提示信息", "村庄不存在，请重新选择！");
        return;
    }
    var startTime = $("#txt_StartTime").val();
    var endTime = $("#txt_EndTime").val();
    if (!$.CheckTime(startTime, endTime)) 
    {
        return;
    }
    
    ExcUrl = "";
    $('#tbRechargeInfos').datagrid('loadData', { total: 0, rows: [],footer:[] });
    $.ShowMask("数据加载中，请稍等……");
    $.ajax(
    {
        url: "../WebServices/DataReportService.asmx/GetSaleWaterSummaryReportByVillages",
        type: "GET",
        data: { 'loginIdentifer': loginIdentifer, 'villageIDs':villageIDs , 'startTime': startTime, 'endTime': endTime },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                ExcUrl = data.ExcelURL;
                SaleWaterSummary = data.村充值;
                SaleWaterSum = data.合计;
                tableData = [];
                for (var i in SaleWaterSummary) {
                    var tableRow = {};
                    tableRow["villageName"] = SaleWaterSummary[i].村庄;
                    tableRow["chargeAmount"] = SaleWaterSummary[i].充值金额;
                    tableRow["waterAmount"] = SaleWaterSummary[i].购水金额;
                    tableRow["waterNum"] = SaleWaterSummary[i].购水量;
                    tableRow["electricAmount"] = SaleWaterSummary[i].购电金额;
                    tableRow["electricNum"] = SaleWaterSummary[i].购电量;
                    tableData.push(tableRow);
                }
                $("#tbRechargeInfos").datagrid({ loadFilter: pagerFilter }).datagrid("loadData", tableData);

                $('#tbRechargeInfos').datagrid('reloadFooter', [
                            { "villageName": "合计", "chargeAmount": SaleWaterSum.充值金额, "waterAmount": SaleWaterSum.购水金额, "waterNum": SaleWaterSum.购水量, "electricAmount": SaleWaterSum.购电金额, "electricNum": SaleWaterSum.购电量 }
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
function Btn_Excel_Click() {
    if (ExcUrl != "") {
        location.href = ExcUrl;
    }
    else {
        $.messager.alert("提示信息", "请先查询再导出！");
    }
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

