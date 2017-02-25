

//登录标识
var loginIdentifer = window.parent.guid;
//报警记录数量
var alarmRecordsCount = "";

var realdevid = "";

var monitorRealName = "";

var currentdevid = GetQueryString("devid");

$(document).ready(function () {
    var defaultTheme = $.cookie("psbsTheme");
    if (defaultTheme != null && defaultTheme != "default") {
        var link = $(document).find('link:first');
        link.attr('href', '../App_Themes/easyui/themes/' + defaultTheme + '/easyui.css');
    }
    InitTimeControl();
    GetSystemInfo();

    $("#cbb_DevCombobox").combobox({
        onSelect: function (rec) {

            var treeObj = $("#areaTree");
            var rootNode = treeObj.tree('getRoot');
            treeObj.tree("uncheck", rootNode.target);
            var node = treeObj.tree('find', "dn_" + rec.ID);
            treeObj.tree('check', node.target);

        }
    });

});

function checkcurid(devid) {
    if (!currentdevid) {
        return;
    }
    if (!deviceNodeLoaded) {
        window.setTimeout("checkcurid(" + devid + ")", 500);
        return;
    }
    var treeObj = $("#areaTree");
    var rootNode = treeObj.tree('getRoot');
    treeObj.tree("uncheck", rootNode.target);
    var node = treeObj.tree('find', "dn_" + currentdevid);
    treeObj.tree('check', node.target);
    Btn_Query_Click();
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
                manageNodeLoaded = false;
                deviceNodeLoaded = false;
                LoadTree("areaTree", mnId, true, true);
                LoadCbbDevice(mnId);
                //报警类型
                loadAlarmType();
                monitorRealName = data.SysStateInfo.监测点级别名称;
                $('#divContainer').layout('panel', 'west').panel({ title: monitorRealName + "列表" });
                $("#wellname").text(monitorRealName);
                checkcurid(currentdevid);

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

function Btn_Query_Click() {
    var devIDs = "";
    var treeObj = $('#areaTree');

    var checkedNodes = treeObj.tree('getChecked');
    for (var i = 0; i < checkedNodes.length; i++) {
        var checkedNode = checkedNodes[i];
        if (checkedNode.attributes["nodeType"] == "manage") {
            continue;
        }
        if (devIDs != "") {
            devIDs += ",";
        }
        devIDs += checkedNode.attributes["did"];
    }
    //起止时间
    var startTime = $("#txt_StartTime").val();
    var endTime = $("#txt_EndTime").val();
    if (!$.CheckTime(startTime, endTime)) {
        return;
    }
    $('#tbOperateInfos').datagrid({ pageNumber: 1 }).datagrid('loadData', { total: 0, rows: [] });
    $.ShowMask("数据加载中，请稍等……");
    alarmRecordsCount = 0;
    $.ajax(
    {
        url: "../WebServices/DataQueryService.asmx/GetAlarmRecordsCount",
        type: "GET",
        data: { "loginIdentifer": loginIdentifer, "deviceIDs": devIDs, "startTime": startTime, "endTime": endTime, "alarmType": $("#AlarmType").combobox('getText') },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                alarmRecordsCount = data.Count;
                operateIdentifer = data.Guid;
                if (alarmRecordsCount == 0) {
                    $.HideMask();
                    $.messager.alert("提示信息", "查询结果为空");
                }
                else {
                    QueryCurrentPageData(1, 20);
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

function QueryCurrentPageData(pageNumber, pageSize) {
    var startIndex = (pageNumber - 1) * pageSize + 1;
    //获取分页中的报警记录
    $.ajax(
    {
        url: "../WebServices/DataQueryService.asmx/GetLiteAlarmRecords",
        type: "GET",
        data: { "loginIdentifer": loginIdentifer, "operateIdentifer": operateIdentifer, "startIndex": startIndex, "count": pageSize },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                //加载显示列
                var Columns = data.Columns;
                var displayColumns = [];
                for (var a in Columns) {
                    if (a == "记录时间" || a == "采集时间" || a == "报警描述" || a == "报警相关量") {
                        displayColumns.push({ field: Columns[a].Field, title: Columns[a].HeadText, width: 160, align: "center" });
                    } else if (a == "报警时间") {
                        displayColumns.push({ field: Columns[a].Field, title: Columns[a].HeadText, width: 110, align: "center" });
                    }
                    else {
                        displayColumns.push({ field: Columns[a].Field, title: Columns[a].HeadText, width: 80, align: "center" });
                    }
                }
                $("#tbOperateInfos").datagrid({ columns: [displayColumns] });

                //加载数据
                var alarmData = data.Records;
                var datas = [];
                for (var i = 0; i < alarmData.length; i++) {
                    //行数据
                    var rowData = alarmData[i];
                    var dataRow = {};
                    for (var a in rowData) {
                        dataRow[rowData[a].Field] = rowData[a].Value;
                        if (a == "报警相关量") {
                            var titlearr = rowData[a].Value.split(',');
                            var titlestr = "";
                            for (var j = 0; j < titlearr.length; j++) {
                                titlestr += titlearr[j] + "\r\n";
                            }
                            dataRow[rowData[a].Field] = "<span style='cursor:pointer;' title='" + titlestr + "'>" + rowData[a].Value + "</span>";
                        } else if (a == "报警描述") {
                            dataRow[rowData[a].Field] = "<span style='cursor:pointer;' title='" + rowData[a].Value + "'>" + rowData[a].Value + "</span>";
                        }
                    }
                    if (a == "详细") {
                        dataRow[rowData[a].Field] = "<a href='javascript:void(0)' onclick='CreateDeviceDetailDataTable(" + rowData[a].Value + "," + rowData["报警时间"].Value.substr(0, 4) + ")'><Img id='imgdetail' style='height:18px;width:18px;' border=0 src='../images/edit.png' /></a>";
                    }
                    datas.push(dataRow);
                }
                var tableDataObj = {
                    total: alarmRecordsCount,
                    rows: datas
                }
                $('#tbOperateInfos').datagrid('loadData', tableDataObj);
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
    var dg = $("#tbOperateInfos");
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
    $.ShowMask("数据加载中，请稍等……");
    //获取分页中的报警记录
    $.ajax(
    {
        url: "../WebServices/DataQueryService.asmx/ExportAlarmRecords",
        type: "GET",
        data: { "loginIdentifer": loginIdentifer, "operateIdentifer": operateIdentifer, "startIndex": "1", "count": alarmRecordsCount },
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
            $.HideMask();
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}


//加载测点combobox
function LoadCbbDevice(mid) {
    $.ajax(
    {
        url: "../WebServices/DeviceNodeService.asmx/GetDeviceNodeInfosByMnId",
        type: "GET",
        data: { "loginIdentifer": loginIdentifer, "mnID": mid, "isRecursive": true, "isExport": false },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            deviceJson = data.DeviceNodes;

            //指定Json数组
            $("#cbb_DevCombobox").combobox({ data: deviceJson, textField: '名称', valueField: 'ID' });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

//加载报警类型
function loadAlarmType() {
    $.ajax(
     {
         url: "../WebServices/DataQueryService.asmx/GetAlarmTypes",
         type: "GET",
         data: { "loginIdentifer": loginIdentifer },
         dataType: "text",
         cache: false,
         success: function (responseText) {
             var data = eval("(" + $.xml2json(responseText) + ")");
             if (data.Result) {
                 var alarmTypes = data.AlarmTypes;
                 var typesData = [];
                 for (var i = 0; i < alarmTypes.length; i++) {
                     var datarow = {};
                     datarow["id"] = i;
                     datarow["text"] = alarmTypes[i];
                     typesData.push(datarow);
                 }
                 $("#AlarmType").combobox('loadData', typesData);
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

function CreateDeviceDetailDataTable(Id, year) {

    createhtmldata(Id, year);

}

function createhtmldata(Id, year) {
    $.ajax(
     {
         url: "../WebServices/DataQueryService.asmx/GetLiteAlarmRecordsById",
         type: "GET",
         data: { "loginIdentifer": loginIdentifer, "id": Id, "year": year },
         dataType: "text",
         cache: false,
         success: function (responseText) {

             var data = eval("(" + $.xml2json(responseText) + ")");
             if (data.Result) {
                 var resultJSON = data.Records;
                 for (var i = 0; i < resultJSON.length; i++) {
                     if (resultJSON[i].Name == "设备ID") {
                         realdevid = resultJSON[i].Value;
                         resultJSON.splice(i, 1);
                     }
                 }
                 var titleJSON = data.TitleRecords;

                 //基础信息
                 var tbDeviceData = "<div class='Panel2' id='tbDeviceData' style='height: auto; font-size: 10pt; width:545px;'>";
                 var field_device = "<fieldset><legend>" + "报警数据" + "</legend>";
                 var tbName_device = "<table id='tbName' class='pstbl' width='100%'>";

                 var trNote_device = "";
                 var tbNote = "";
                 trNote_device = trNote_device + "<tr style='height:20px'>";
                 for (var i = 0; i < 3; i++) {
                     tbNote += "<td  width='90px' class='alt' align='center'>" + resultJSON[i].Name + "</td><td  width='90px' align='left'>" + resultJSON[i].Value + "</td>";
                 }
                 trNote_device = tbDeviceData + field_device + tbName_device + trNote_device + tbNote + "</tr>";


                 var trNote_monitor = "";
                 trNote_monitor = trNote_monitor + "<tr style='height:20px'>";
                 var tbmonitorNote = "";
                 for (var i = 3; i < resultJSON.length; i++) {
                     tbmonitorNote += "<td  width='140px' class='alt' align='center'>" + resultJSON[i].Name + "</td><td  width='100px' align='left'>" + resultJSON[i].Value + "</td>";
                     if (i != 3 && (i - 2) % 3 == 0) {
                         tbmonitorNote += "</tr></tr style='height:20px'>";
                     }
                 }
                 trNote_monitor = trNote_monitor + tbmonitorNote + "</tr></table></fieldset></div>";
                 $("#divDataList").html(trNote_device + trNote_monitor);
                 $('#divDetailData').dialog({ closed: false });
                 $('#divDetailData').dialog({ closed: false, title: titleJSON[0].Value + "--" + titleJSON[1].Value + "--" + titleJSON[2].Value + "--详细信息" });
                 $('#divDetailData').dialog({ top: ($("#divContainer").height() - $('#divDetailData')[0].clientHeight) / 4 });
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