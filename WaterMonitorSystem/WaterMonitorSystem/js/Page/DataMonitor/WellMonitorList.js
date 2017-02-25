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

//详细数据的设备ID
var realDevId = "";
//用户站参数
var tempId = "";
//测点级别名称
var monitorRealName = "";
var pageFirstLoad = true;

var CameraInfos = {};
var timeInt;
var cameraInfo;

var devIDs = "";

$(document).ready(function () {
    $.ShowMask("数据加载中，请稍等……");
    var defaultTheme = $.cookie("psbsTheme");
    if (defaultTheme != null && defaultTheme != "default") {
        var link = $(document).find('link:first');
        link.attr('href', '../App_Themes/easyui/themes/' + defaultTheme + '/easyui.css');
    }
    GetSystemInfo();
    $("#cbb_DevCombobox").combobox({
        onSelect: function (rec) {
            var node = $('#divAreaTree').tree('find', "dn_" + rec.id);
            $('#divAreaTree').tree('uncheck', $("#divAreaTree").tree('getRoot').target).tree('check', node.target);
        }
    });

});

var isPollingWellDatas = false;
var pollingWellDatas = null;

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
                $('#divContainer').layout('panel', 'west').panel({ title: monitorRealName + "列表" });
                $("#wellname").text(monitorRealName);
                LoadTreeAndData();
                pollingWellDatas = setInterval("LoadTreeAndData()", 20 * 1000);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function LoadTreeAndData()
{
    if(isPollingWellDatas)
    {
        return;
    }
    manageNodeLoaded = false;
    deviceNodeLoaded = false;
    treeLoaded = false;
    isPollingWellDatas = true;
    LoadTree("divAreaTree", mnId, true, true);
    LoadDeviceDatas();
}

function LoadDeviceDatas() {
    devIDs = "";
    if (treeLoaded) {
        comboBoxData = [];
        var checkedNodes = $("#divAreaTree").tree('getChecked');
        for (var i = 0; i < checkedNodes.length; i++) {
            var checkedNode = checkedNodes[i];
            if (checkedNode.attributes["nodeType"] == "manage") {
                continue;
            }
            if (devIDs != "") {
                devIDs += ",";
            }
            devIDs += checkedNode.attributes["did"];
            var devObj = {};
            devObj["id"] = checkedNode.attributes["did"];
            devObj["text"] = $(checkedNode.text).text();
            comboBoxData.push(devObj);
        }
        LoadTableData(devIDs);
        $("#cbb_DevCombobox").combobox({
            data: comboBoxData
        });
    }
    else {
        setTimeout(LoadDeviceDatas, 100);
    }
}

//查询的按钮事件
function Btn_Query_Click() {
    $.ShowMask("数据加载中，请稍等……");
    devIDs = "";
    var treeObj = $("#divAreaTree");
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
    LoadTableData(devIDs);
}

function LoadTableData(_devIds) {
    isPollingWellDatas = true;
    tableData = [];
    var czobj = {};
    var colflag = false;
    $.ajax({
        url: "../WebServices/DeviceMonitorService.asmx/GetDeviceRealTimeDatasForWell",
        data: { "loginIdentifer": window.parent.guid, "devIDs": _devIds },
        type: "Get",
        dataType: "text",
        cache: false,
        success: function (responseText) {
            //var row = 0;
            try {
                var data = eval("(" + $.xml2json(responseText) + ")");
                if (data.Result) {
                    var tableData = [];
                    var DevDatas = data.DevDatas;
                    for (var i = 0; i < DevDatas.length; i++) {
                        //row = 0;
                        var tableRow = {};
                        tableRow["village"] = DevDatas[i].村庄.Value;
                        //row++;
                        tableRow["irrigationWell"] = DevDatas[i].设备.Value;
                        //row++;
                        tableRow["irrigationWellNo"] = DevDatas[i].长编号.Value;
                        //row++;
                        tableRow["deviceType"] = DevDatas[i].设备类型.Value;
                        //row++;
                        tableRow["communication"] = (DevDatas[i].通讯状态.Value == "全部正常") ? "<Img style='height:18px;width:18px;' src='../images/正常.png'/>" : "<Img style='height:18px;width:18px;' src='../images/中断.png'/>";
                        //row++;
                        tableRow["updateTime"] = DevDatas[i].更新时间.Value;
                        //row++;
                        tableRow["yearElectric"] = DevDatas[i].年累计用电量.Value;
                        //row++;
                        tableRow["yearWater"] = DevDatas[i].年累计用水量.Value;
                        //row++;
                        tableRow["yearAmount"] = (DevDatas[i].井剩余水量报警.Value == "水量超采") ? ("<span style='color:red;bold:weight'>" + DevDatas[i].年剩余可开采量.Value + "</span>") : DevDatas[i].年剩余可开采量.Value;
                        //row++;
                        tableRow["pumpStatus"] = DevDatas[i].水泵工作状态.Value;
                        //row++;
                        tableRow["waterUser"] = DevDatas[i].用水户.Value == "" ? "00000000" : DevDatas[i].用水户.Value;
                        //row++;
                        var irrigationTime = DevDatas[i].灌溉时长.Value;
                        
                        //tableRow["irrigationTime"] = "<span style='display:block;cursor:pointer;width:100%；height:100%' title='开泵时间：" + DevDatas[i].开泵时间.Value + "\r\n关泵时间：" + DevDatas[i].关泵时间.Value + "'>" + FormatSeconds(DevDatas[i].灌溉时长.Value) + "</span>";
                        tableRow["irrigationTime"] = "<span style='display:block;cursor:pointer;width:100%；height:100%'>" + FormatSeconds(irrigationTime) + "</span>";
                        //row++;
                        //var styleWaterAlarm = (DevDatas[i].用户剩余水量报警.Value == "报警") ? " style='color:red;'" : "";
                        //row++;
                        //var stylePowerAlarm = (DevDatas[i].用户剩余电量报警.Value == "报警") ? " style='color:red;'" : "";
                        //row++;
                        var waterNum = DevDatas[i].本次用水量.Value;
                        
                        //tableRow["waterNum"] = "<span style='display:block;cursor:pointer;width:100%;height:100%;" + (DevDatas[i].用户剩余水量报警.Value == "报警" ? "color:red;" : "") + "' title='开泵剩余水量：" + DevDatas[i].开泵剩余水量.Value + "\r\n关泵剩余水量：" + DevDatas[i].关泵剩余水量.Value + "'>" + DevDatas[i].本次用水量.Value + "</span>";
                        tableRow["waterNum"] = "<span style='display:block;cursor:pointer;width:100%;height:100%;" + (DevDatas[i].用户剩余水量报警.Value == "报警" ? "color:red;" : "") + "'>" + waterNum + "</span>";
                        //row++;
                        var electricNum = DevDatas[i].本次用电量.Value;
                        
                        //tableRow["electricNum"] = "<span style='display:block;cursor:pointer;width:100%;height:100%;" + (DevDatas[i].用户剩余电量报警.Value == "报警" ? "color:red;" : "") + "' title='开泵剩余电量：" + DevDatas[i].开泵剩余电量.Value + "\r\n关泵剩余电量：" + DevDatas[i].关泵剩余电量.Value + "'>" + DevDatas[i].本次用电量.Value + "</span>";
                        tableRow["electricNum"] = "<span style='display:block;cursor:pointer;width:100%;height:100%;" + (DevDatas[i].用户剩余电量报警.Value == "报警" ? "color:red;" : "") + "'>" + electricNum + "</span>";
                        //row++;
                        tableRow["operation"] = "<a href='javascript:void(0)' onclick='ShowMore(" + DevDatas[i].操作.Value + ",event)'><Img id='imgedit' style='height:18px;width:18px;' border=0 src='../images/edit.png' /></a>";
                        //row++;
                        tableRow["pumpStatus6"] = DevDatas[i].流量仪表状态.Value == "故障" ? ("<span style='color:red;bold:weight'>" + DevDatas[i].流量仪表状态.Value + "</span>") : DevDatas[i].流量仪表状态.Value;
                        tableRow["pumpStatus8"] = DevDatas[i].终端箱门状态.Value == "开启" ? ("<span style='color:red;bold:weight'>" + DevDatas[i].终端箱门状态.Value + "</span>") : DevDatas[i].终端箱门状态.Value;
                        tableRow["pumpStatus10"] = DevDatas[i].IC卡功能有效.Value == "IC卡有效" ? ("<span style='color:red;bold:weight'>" + DevDatas[i].IC卡功能有效.Value + "</span>") : DevDatas[i].IC卡功能有效.Value;
                        tableRow["pumpStatus16"] = DevDatas[i].电表信号报警.Value == "报警" ? ("<span style='color:red;bold:weight'>" + DevDatas[i].电表信号报警.Value + "</span>") : DevDatas[i].电表信号报警.Value;
                        //row++;
                        tableRow["Water"] = DevDatas[i].累计用水量.Value;
                        /*内蒙定点数据修改*/
                        tableRow["yearWater"] = DevDatas[i].累计用水量.Value;
                                                
                        tableData.push(tableRow);
                    }
                    $('#tbOperateInfos').datagrid({ loadFilter: pagerFilter }).datagrid('loadData', tableData);
                    $.HideMask();
                } else {
                    $.HideMask();
                    if (data.Message == "未登录" || data.Message == "登录超时") {
                        clearInterval(pollingWellDatas);
                    }
                    clearInterval(pollingWellDatas);
                    $.messager.alert("提示信息1", data.Message);
                }
            }
            catch (e) {
                //alert(row);
                $.HideMask();
                clearInterval(pollingWellDatas);
                $.messager.alert("提示信息2", e.message);
            }
            isPollingWellDatas = false;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            isPollingWellDatas = false;
            $.HideMask();
            $.messager.alert("提示信息3", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
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

function ShowMore(diviceID, e) {
    realDevId = diviceID;
    var DeviceInfo = [];
    $.ajax({
        url: "../WebServices/DeviceMonitorService.asmx/GetDeviceInfoForWell",
        data: { "loginIdentifer": window.parent.guid, "devID": diviceID },
        type: "Get",
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            DeviceInfo = data.DeviceInfo;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
    var e = e || window.event;
    var pagex = e.clientX + document.body.scrollLeft + document.documentElement.scrollLeft;
    var leftx;
    if ($(window).width() - pagex < 120) {
        leftx = e.clientX + document.body.scrollLeft + document.documentElement.scrollLeft - 120;
    } else {
        leftx = e.clientX + document.body.scrollLeft + document.documentElement.scrollLeft;
    }
    $('#mm').menu('show', {
        left: leftx,
        top: e.clientY + document.body.scrollTop + document.documentElement.scrollTop,
        onClick: function (item) {
            switch(item.text)
            {
                case "详细数据":
                    ShowDetailData(DeviceInfo);
                    break;
                case "历史数据":
                    LinkHistoryData(diviceID);
                    break;
                case "报警数据":
                    LinkAlarmData(diviceID);
                    break;
                case "事件数据":
                    LinkEventData(diviceID);
                    break;
                default:
                    break;
            }
        }
    });
}

function ShowDetailData(DeviceInfo) {
    if (DeviceInfo.SupportPhoto) {
        if (CameraInfos[realDevId] == null) {
            var ci = new CameraInfo();
            var photoID = ""
            if (typeof (deviceNodes[realDevId].attributes.device.相机ID) == "undefined" || deviceNodes[realDevId].attributes.device.相机ID == "") {
                photoID = realDevId;
            }
            else {
                deviceNodes[realDevId].attributes.device.相机ID;
            }
            ci["相机ID"] = photoID;
            ci["正在拍照"] = false;
            ci["拍照时间"] = "";
            ci["最新照片时间"] = "";
            CameraInfos[realDevId] = ci;
        }
        $("#divPhotoData").dialog({
            title: deviceNodes[realDevId].attributes.device.管理名称 + "——" + deviceNodes[realDevId].attributes.device.名称
        });
        CreateDeviceDetailPhotoTable(DeviceInfo);
        $('#divPhotoData').dialog({
            closed: false,
            onBeforeClose: function () {
                cameraInfo = CameraInfos[realDevId];
                if (cameraInfo["正在拍照"]) {
                    $.messager.alert("提示信息", "当前正在拍照，请耐心等待……");
                    return false;
                } else {
                    window.clearInterval(timeInt);
                    return true;
                }

            }
        });
        timeInt = window.setInterval(function () { CreateDeviceDetailPhotoTable(DeviceInfo) }, 10000);

    } else {
        CreateDeviceDetailDataTable(DeviceInfo);
    }
}

function CreateDeviceDetailPhotoTable(DeviceInfo) {
    cameraInfo = CameraInfos[realDevId];
    if (cameraInfo["正在拍照"] && cameraInfo["拍照时间"] != "") {
        var dtNow = new Date();
        var dateDiff = GetDateDiff(cameraInfo["拍照时间"], dtNow.Format("yyyy-MM-dd HH:mm:ss"), "second");
        if (dateDiff > 2100) {
            document.getElementById("btn_Photo").disabled = "";
            cameraInfo["正在拍照"] = false;
        }
        else {
            document.getElementById("btn_Photo").disabled = "disabled";
        }
    }
    if (DeviceInfo != "") {
        //设备监测数据
        var resultJSON = DeviceInfo;
        //设备监测数据
        var devDataJSON = resultJSON.RealDatas;
        //报警图片信息
        var imageInfo = resultJSON.PhotoInfo;
        //更新报警图片信息
        $("#imgMonitor").attr("src", imageInfo.Url);
        $("#photoTime").text(imageInfo.Time);
        if (imageInfo.Url != "") {
            $("#imgMonitor").attr("alt", "拍照时间：" + imageInfo.Time);
        }
        else {
            $("#imgMonitor").attr("alt", "没有报警图片");
        }

        if (cameraInfo["正在拍照"] && cameraInfo["拍照时间"] != "") {
            var dtNow = new Date();
            var dateDiff = GetDateDiff(cameraInfo["拍照时间"], dtNow.Format("yyyy-MM-dd HH:mm:ss"), "second");
            if (dateDiff > 210) {
                document.getElementById("btn_Photo").disabled = "";
                cameraInfo["正在拍照"] = false;
            }
            else {
                if (imageInfo.Time != "" && imageInfo.Time != cameraInfo["最新照片时间"]) {
                    document.getElementById("btn_Photo").disabled = "";
                    cameraInfo["最新照片时间"] = imageInfo.Time;
                    cameraInfo["正在拍照"] = false;
                }
            }
        }
        else {
            if (imageInfo.Time != "") {
                cameraInfo["最新照片时间"] = imageInfo.Time;
            }
        }

        //更新监测数据
        var rowCount = parseInt(devDataJSON.length);
        var tbMonitorData = $("<table id='tbMonitorData' class='pstbl' width='100%'></table>")
        var index = 0;
        for (var rowIndex = 0; rowIndex < rowCount; rowIndex++) {
            var trNew = $("<tr></tr>");

            var tdName = $("<td width='120px' align='center' style='vertical-align: middle;height:25px;background-color: #A3C2EA;'>" + devDataJSON[index].Name + "</td>");
            var tdValue = $("<td  align='center' style='vertical-align: middle;'>" + devDataJSON[index].Value + "</td>");

            tdName.appendTo(trNew);
            tdValue.appendTo(trNew);
            trNew.appendTo(tbMonitorData);

            index++;
        }
        if ($("#tbMonitorData") != null) {
            $("#tbMonitorData").remove();
        }

        var trbtn = "";
        if (DeviceInfo.SupportControl) {
            for (var i = 0; i < DeviceInfo.ControlNames.length; i++) {
                trbtn += "<button onclick='ControlDevice(\"" + DeviceInfo.ControlNames[i] + "\")' class='psbutton'>" + DeviceInfo.ControlNames[i] + "</button>";
            }
        }
        if (DeviceInfo.SupportRefresh) {
            trbtn += "<button onclick='RefreshDevice(" + realDevId + ")' style='display:none' class='psbutton'>即时刷新</button>";
        }
        if (DeviceInfo.SupportParam) {
            trbtn += "<button onclick='OpenSetParam()' class='psbutton'>参数设置</button>";
        }
        if (DeviceInfo.SupportWaterView) {
            trbtn += "<button onclick='OpenSetWater()' class='psbutton'>水文查询</button>";
            trbtn += "<button onclick='OpenSetWaterParam()' class='psbutton'>水文参数</button>";
        }
        var btnhtml = "<div id='btndiv'>" + trbtn + "</div>";
        tbMonitorData.appendTo($("#divPhotoDataList"));
        if ($("#btndiv") != null) {
            $("#btndiv").remove();
        }
        $(btnhtml).appendTo($("#divPhotoDataList"));
    }
}

function CameraInfo() {
    var 相机ID = "";
    var 正在拍照 = false;
    var 拍照时间 = "";
    var 最新照片时间 = "";
}

function CreateDeviceDetailDataTable(DeviceInfo) {
    var detailName = "基础信息";
    //var _tempId = deviceNodes[realDevId].attributes.device.用户站参数;
    var _tempId = 1;
    if (DeviceInfo != null && DeviceInfo != "") {
        //设备监测数据
        var resultJSON = DeviceInfo.RealDatas;

        //基础信息
        var tbDeviceData = "<div class='Panel2' id='tbDeviceData' style='height: auto; font-size: 10pt; width:725px;'>";
        var field_device = "<fieldset><legend>" + "实时数据" + "</legend>";
        var tbName_device = "<table id='tbName' class='pstbl' width='100%'>";

        var trNote_device = "";
        var tbNote = "";
        trNote_device = trNote_device + "<tr style='height:20px'>";
        for (var i = 0; i < 3; i++) {
            tbNote += "<td  width='100px' class='alt' align='center'>" + resultJSON[i].Name + "</td><td  width='100px' align='left'>" + resultJSON[i].Value + "</td>";
        }
        trNote_device = tbDeviceData + field_device + tbName_device + trNote_device + tbNote + "</tr>";
        //实时数

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
        var trbtn = "";
        if (DeviceInfo.SupportControl) {
            for (var i = 0; i < DeviceInfo.ControlNames.length; i++) {
                trbtn += "<button onclick='ControlDevice(\"" + DeviceInfo.ControlNames[i] + "\")' class='psbutton'>" + DeviceInfo.ControlNames[i] + "</button>";
            }
        }
        if (DeviceInfo.SupportRefresh) {
            trbtn += "<button onclick='RefreshDevice(" + realDevId + ")' style='display:none' class='psbutton'>即时刷新</button>";
        }
        if (DeviceInfo.SupportParam) {
            trbtn += "<button onclick='OpenSetParam()' class='psbutton'>参数设置</button>";
        }
        if (DeviceInfo.SupportWaterView) {
            trbtn += "<button onclick='OpenSetWater()' class='psbutton'>水文查询</button>";
            trbtn += "<button onclick='OpenSetWaterParam()' class='psbutton'>水文参数</button>";
        }
        trbtn += "<button onclick='OpenSendFile()' class='psbutton'>文件语音</button>";
        var btnhtml = "<div style='width:725px;height:auto;'><table><tr><td  width='725px' align='center'>" + trbtn + "</td></tr></div>";
        $("#divDataList").html(trNote_device + trNote_monitor + btnhtml);
        $('#divDetailData').dialog({ closed: false, title: deviceNodes[realDevId].attributes.device.管理名称 + "(" + deviceNodes[realDevId].attributes.device.名称 + ")详细信息" });
        $('#divDetailData').dialog({ top: ($("#divContainer").height() - $('#divDetailData')[0].clientHeight) / 3 });
    }
}


function CloseDetailData() {
    document.getElementById("divDetailData").style.display = "none";
}

//隐藏即时刷新的层
function hiddenDiv() {
    document.getElementById("divRealTimePopup").style.display = "none";
}


function OpenSetParam() {
    $('#dlgParam').dialog({ closed: false ,title:(deviceNodes[realDevId].attributes.device.名称+"参数设置")});
    $("#txt_Time").val("");
    $("#txt_Exploit").val("");
}

function OpenSendFile() {
    $('#dlgSendFile').dialog({ closed: false, title: (deviceNodes[realDevId].attributes.device.名称 + "文件语音") });
}

function OpenSetWater() {
    $('#dlgParamWater').dialog({ closed: false, title: (deviceNodes[realDevId].attributes.device.名称 + "水文查询") });
}

function OpenSetWaterParam() {
    $('#dlgParamWater2').dialog({ closed: false, title: (deviceNodes[realDevId].attributes.device.名称 + "水文查询") });
}

function LinkHistoryData(diviceID) {
    //window.parent.addTab('历史查询', 'DataQuery/HistoryDataQuery.html?devid=' + diviceID, 'HistoryDataQuery.html', true, 'menuC19');
    //window.parent.showM(window.parent.menuCObj["历史查询"]);
    //window.parent.showM("");
}

function LinkEventData(diviceID) {
    //window.parent.addTab('事件数据', 'DataQuery/EventDataQuery.html?devid=' + diviceID, 'tu1813', true, 'menuC19');
    //window.parent.showM(window.parent.menuCObj["事件数据"]);
    window.parent.showM("事件查询", "?devid=" + diviceID);
}

function LinkAlarmData(diviceID) {
    //window.parent.addTab('报警查询', 'DataQuery/AlarmDataQuery.html?devid=' + diviceID, 'tu1703', true, 'menuC19');
    //window.parent.showM(window.parent.menuCObj["报警查询"]);
    window.parent.showM("报警查询", "?devid=" + diviceID);
}

function ReadParam(paramName,paramTextId) {
    $.ShowMask("正在读参，请稍等……");
    $.ajax(
    {
        url: "../WebServices/DeviceMonitorService.asmx/GetParam",
        type: "GET",
        data: { "loginIdentifer": window.parent.guid, "devID": realDevId, "paramName": paramName },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            var paramtableData = [];
            if (data.Result) {
                /*
                for (var i = 0; i < data.Params.length; i++) {
                    if(data.Params[i].Name == paramName)
                    {
                        $("#" + paramTextId).val(data.Params[i].Value);
                        break;
                    }
                }
                */
                $("#" + paramTextId).val(data.Params);
                $.HideMask();
                $.messager.alert("提示信息", "读取成功");
            } else {
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

function SetParam(paramName,paramTextId) {
    var paramTextObj = $("#"+paramTextId);
    var _strValue = paramTextObj.val();
    if (_strValue == "" || _strValue == null) {
        $.messager.alert("提示信息", "请输入参数值！");
        paramTextObj.focus();
        return;
    }

    $.ShowMask("正在设参，请稍等……");
    $.ajax(
    {
        url: "../WebServices/DeviceMonitorService.asmx/SetParam",
        type: "GET",
        data: { "loginIdentifer": window.parent.guid, "devID": realDevId, "paramName": paramName, "paramValue": _strValue },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                $.HideMask();
                $.messager.alert("提示信息", "设置成功！");
            } else {
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

function ControlDevice(ctrlName) {
    if (ctrlName == "设置地址")
    {
        ctrlName = "设置主站射频地址";
    }
    $.ShowMask("正在" + ctrlName + "，请稍等……");
    $.ajax({
        type: "GET",
        url: "../WebServices/DeviceMonitorService.asmx/ControlDevice",
        data: { 'loginIdentifer': window.parent.guid, 'ctrlName': ctrlName, 'devID': realDevId },
        contentType: "content",
        dataType: "text",
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            $.HideMask();
            if (data.Result) {
                if (ctrlName == "拍照") {
                    cameraInfo["正在拍照"] = true;
                    $.messager.alert("提示信息", "拍照成功，正在取照片，大约需要3分钟，请耐心等待！");
                } else if (ctrlName == "状态查询") {
                    $.messager.alert("提示信息", "状态查询成功，请等待实时数据刷新或手动刷新实时数据！");
                } else {
                    $.messager.alert("提示信息", ctrlName + "成功！");
                }
            }
            else {
                var result = data.Message;
                $.messager.alert("提示信息", result);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.HideMask();
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.reponseText);
        }
    });
}

function ControlDevice2(ctrlName) {
    var txt_Range = $("#txt_Range");
    var txt_LineLength = $("#txt_LineLength");
    var txt_GroundWaterLevel = $("#txt_GroundWaterLevel");
    var txt_Acq_Time = $("#txt_Acq_Time");

    if (ctrlName != "参数设置" && ctrlName != "参数查询" && ctrlName != "水位查询") {
        $.messager.alert("提示信息", "非法操作！" + ctrlName);
        return;
    }

    $.ShowMask("正在" + ctrlName + "，请稍等……");
    $.ajax({
        type: "GET",
        url: "../WebServices/DeviceMonitorService.asmx/ControlDevice2",
        data: { 'loginIdentifer': window.parent.guid, 'ctrlName': ctrlName, 'devID': realDevId, 'Range': txt_Range.val(), 'LineLength': txt_LineLength.val() },
        contentType: "content",
        dataType: "text",
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            $.HideMask();
            if (data.Result) {
                $.messager.alert("提示信息", ctrlName + "成功！");
                if (ctrlName == "参数设置") {
                }
                else if (ctrlName == "参数查询") {
                    txt_Range.val(data.Range);
                    txt_LineLength.val(data.LineLength);
                }
                else if (ctrlName == "水位查询") {
                    txt_GroundWaterLevel.val(data.GroundWaterLevel);
                    txt_LineLength.val(data.LineLength);
                    txt_Acq_Time.val(data.Acq_Time);
                }
            }
            else {
                var result = data.Message;
                $.messager.alert("提示信息", result);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.HideMask();
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.reponseText);
        }
    });
}

function RefreshDevice(deviceID) {
    //$('#divDetailData').dialog({ closed: true });
    $.ShowMask("数据刷新中，请稍等……");
    $.ajax({
        type: "GET",
        url: "../WebServices/DeviceMonitorService.asmx/RefreshDevice",
        data: { 'loginIdentifer': window.parent.guid, 'devID': realDevId },
        contentType: "content",
        dataType: "text",
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                $.HideMask();
                $.messager.alert("提示信息", "即时刷新成功！");
            } else {
                $.HideMask();
                var result = data.Message;
                $.messager.alert("提示信息", result);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.HideMask();
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.reponseText);
        }
    });
}


