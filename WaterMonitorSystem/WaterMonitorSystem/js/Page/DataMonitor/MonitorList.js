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

var monitorRealName = "";


var CameraInfos = {};

var timeInt;

var cameraInfo;

var tempIDObj = {};
var tempID = "";
var devIDs = "";
var columnNamesKey = "";
var getColumnNames = false;
//模板显示列名字典
var dicColumnNames = {};
var dicHeadTexts = {};

$(document).ready(function () {
    $.ShowMask("数据加载中，请稍等……");
    var defaultTheme = $.cookie("psbsTheme");
    if (defaultTheme != null && defaultTheme != "default") {
        var link = $(document).find('link:first');
        link.attr('href', '../App_Themes/easyui/themes/' + defaultTheme + '/easyui.css');
    }
    GetSystemInfo();
    LoadComboboxData();

    $("#cbb_DevCombobox").combobox({
        onSelect: function (rec) {
            var node = $('#divAreaTree').tree('find', "dn_" + rec.id);
            $('#divAreaTree').tree('uncheck', $("#divAreaTree").tree('getRoot').target).tree('check', node.target);
        }
    });

});
var isPollingDevDatas = false;
var pollingDevDatas = setInterval("CheckDeviceData()", 5000);
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
                monitorRealName = data.SysStateInfo.监测点级别名称;
                $('#divContainer').layout('panel', 'west').panel({ title: monitorRealName + "列表" });
                $("#wellname").text(monitorRealName);
                LoadTree("divAreaTree", mnId, true, true);
                CheckDeviceData();
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

//查询的按钮事件
function Btn_Query_Click() {
    $.ShowMask("数据加载中，请稍等……");
    tempIDObj = {};
    tempID = "";
    devIDs = "";
    columnNamesKey = "";
    getColumnNames = false;
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
        tempID = deviceNodes[checkedNode.attributes["did"]].attributes.device.用户站参数;

        if (tempIDObj[tempID] == null) {
            if (columnNamesKey != "") {
                columnNamesKey += "-";
            }
            columnNamesKey += tempID;
            tempIDObj[tempID] = tempID;
        }
    }
    if (dicColumnNames[columnNamesKey] == null) {
        getColumnNames = true;
    }
    LoadTableData(devIDs, getColumnNames);
}

function LoadTableData(_devIds, getColumnNames) {
    var gridcolumns = [];
    tableData = [];
    var czobj = {};
    var colflag = false;

    $.ajax({
        url: "../WebServices/DeviceMonitorService.asmx/GetDeviceRealTimeDatas",
        data: { "loginIdentifer": window.parent.guid, "devIDs": _devIds },
        type: "Get",
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                var headertext = "";
                var DevDatas = data.DevDatas;
                var Columns = data.Columns;
                for (var i = 0; i < DevDatas.length; i++) {
                    if (i == 0) {
                        colflag = true;
                    } else {
                        colflag = false;
                    }
                    var tableRow = {};
                    for (var item in DevDatas[i]) {
                        if (item == "通讯状态") {
                            if (DevDatas[i][item].Value == "全部正常") {
                                DevDatas[i][item].Value = "<Img style='height:18px;width:18px;' title='通讯状态:\r\n正常' src='../images/正常.png'/>";
                            } else {
                                DevDatas[i][item].Value = "<Img style='height:18px;width:18px;' src='../images/中断.png'/>";
                            }
                        } else if (item == "设备状态") {
                            if (DevDatas[i][item].Value == "全部正常") {
                                DevDatas[i][item].Value = "<Img style='height:18px;width:18px;' src='../images/正常.png'/>";
                            } else {
                                DevDatas[i][item].Value = "<Img style='height:18px;width:18px;' src='../images/中断.png'/>";
                            }
                        } else if (item == "运行状态") {
                            if (DevDatas[i][item].Value == "运行") {
                                DevDatas[i][item].Value = "<Img style='height:18px;width:18px;' src='../images/大圈绿.gif'/>";
                            } else if (DevDatas[i][item].Value == "停止") {
                                DevDatas[i][item].Value = "<Img style='height:18px;width:18px;' src='../images/大圈红.gif'/>";
                            } else {
                                DevDatas[i][item].Value = "<Img style='height:18px;width:18px;' src='../images/大圈灰.gif'/>";
                            }
                        } else if (item == "通讯") {
                            if (DevDatas[i]["通讯状态"] == "全部正常") {
                                var q = (DevDatas[i]["信号强度"].Value != null ? DevDatas[i]["信号强度"].Value : "--");
                                if (q == "--" || q == "未知") {
                                    DevDatas[i][item].Value = "<Img style='height:18px;width:18px;' src='../images/signal4.png'/>";
                                }
                                if (q < 15) {
                                    DevDatas[i][item].Value = "<Img style='height:18px;width:18px;' src='../images/signal1.png'/>";
                                }
                                if (q >= 15 && q <= 25) {
                                    DevDatas[i][item].Value = "<Img style='height:18px;width:18px;' src='../images/signal3.png'/>";
                                }
                                if (q > 25) {
                                    DevDatas[i][item].Value = "<Img style='height:18px;width:18px;' src='../images/signal4.png'/>";
                                }
                            }
                            else {
                                DevDatas[i][item].Value = "<Img style='height:18px;width:18px;' src='../images/signal0.png'/>";
                            }
                        }
                        tableRow[DevDatas[i][item].Field] = DevDatas[i][item].Value;
                        if (colflag) {
                            if (item == "操作") {
                                czobj = {
                                    field: DevDatas[i][item].Field, title: Columns[item].HeadText, width: 30, formatter: function (value, row, index) {
                                        return "<a href='javascript:void(0)' onclick='ShowMore(" + row[DevDatas[0][item].Field] + ",event)'><Img id='imgedit' style='height:18px;width:18px;' border=0 src='../images/edit.png' /></a>";
                                    }
                                };
                            } else {
                                var gridtitle = "";
                                if (Columns[item].HeadText.length > 6) {
                                    gridtitle = Columns[item].HeadText.substr(0, 6) + "<br/>" + Columns[item].HeadText.substr(6, Columns[item].HeadText.length);
                                } else {
                                    gridtitle = Columns[item].HeadText;
                                }
                                if (item == "更新时间") {
                                    gridcolumns.push({ field: DevDatas[i][item].Field, title: gridtitle, width: 120 });
                                } else {
                                    gridcolumns.push({ field: DevDatas[i][item].Field, title: gridtitle, width: 70 });
                                }

                            }
                        }
                    }
                    if (colflag) {
                        gridcolumns.push(czobj);
                    }
                    tableData.push(tableRow);
                }

                $('#tbOperateInfos').datagrid({ loadFilter: pagerFilter, columns: [gridcolumns], fitColumns: false, nowrap: true }).datagrid('loadData', tableData);
                $.HideMask();
            } else {
                $.HideMask();
                if (data.Message == "未登录" || data.Message == "登录超时") {
                    clearInterval(pollingDevDatas);
                }
                $.messager.alert("提示信息", data.Message);
            }
            isPollingDevDatas = false;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.HideMask();
            isPollingDevDatas = false;
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
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
        url: "../WebServices/DeviceMonitorService.asmx/GetDeviceInfo",
        data: { "loginIdentifer": window.parent.guid, "devID": diviceID },
        type: "Get",
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            DeviceInfo = data.DeviceInfo;
            var comboBoxDataLevel = [];
            var unitdata = DeviceInfo.ParamNames;
            for (var i = 0; i < data.DeviceInfo.ParamNames.length; i++) {
                var levelObj = {};
                levelObj["id"] = data.DeviceInfo.ParamNames[i].名称;
                levelObj["text"] = data.DeviceInfo.ParamNames[i].名称;
                comboBoxDataLevel.push(levelObj);
            }
            var allObj = {};
            allObj["id"] = "全部";
            allObj["text"] = "全部";
            allObj["selected"] = true;
            comboBoxDataLevel.unshift(allObj);
            $("#ccb_ParamName").combobox({
                data: comboBoxDataLevel,
                onSelect: function (rec) {
                    if (rec.id == "全部") {
                        $("#txt_unit").html("");
                    } else {
                        for (var i = 0; i < unitdata.length; i++) {
                            if (rec.id == unitdata[i].名称) {
                                $("#txt_unit").html(unitdata[i].单位);
                            }
                        }
                    }
                    $("#txt_Param").val("");
                }
            });
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
            if (item.text == "详细数据") {
                ShowDetailData(DeviceInfo);
            } else if (item.text == "历史数据") {
                LinkHistoryData(diviceID);
            } else {
                LinkAlarmData(diviceID);
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
        timeInt = window.setInterval(function () { CreateDeviceDetailPhotoTable( DeviceInfo) }, 10000);

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
            trbtn += "<button onclick='RefreshDevice(" + realDevId + ")' class='psbutton'>即时刷新</button>";
        }
        if (DeviceInfo.SupportParam) {
            trbtn += "<button onclick='OpenSetParam()' class='psbutton'>参数设置</button>";
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
    var _tempId = deviceNodes[realDevId].attributes.device.用户站参数;
    if (DeviceInfo != null && DeviceInfo != "") {
        //设备监测数据
        var resultJSON = DeviceInfo.RealDatas;

        //基础信息
        var tbDeviceData = "<div class='Panel2' id='tbDeviceData' style='height: auto; font-size: 10pt; width:545px;'>";
        var field_device = "<fieldset><legend>" + "基础信息" + "</legend>";
        var tbName_device = "<table id='tbName' class='pstbl' width='100%'>";

        var trNote_device = "";
        var tbNote = "";
        trNote_device = trNote_device + "<tr style='height:20px'>";
        for (var i = 0; i < 3; i++) {
            tbNote += "<td  width='90px' align='center' class='alt'>" + resultJSON[i].Name + "</td><td  width='90px' align='left'>" + resultJSON[i].Value + "</td>";
        }
        trNote_device = tbDeviceData + field_device + tbName_device + trNote_device + tbNote + "</tr></table></fieldset></div>";
        //实时数
        var tbMonitorData = "<div class='Panel3' id='tbMonitorData' style='height: auto; font-size: 10pt; width:545px;padding-top:20px'>";
        var field_monitor = "<fieldset><legend>" + "监测数据" + "</legend>";
        var tbName_monitor = "<table id='tbName'class='pstbl' width='100%'>";

        var trNote_monitor = "";
        trNote_monitor = trNote_monitor + "<tr style='height:20px'>";
        var tbmonitorNote = "";
        for (var i = 3; i < resultJSON.length; i++) {
            tbmonitorNote += "<td  width='140px' align='center' class='alt'>" + resultJSON[i].Name + "</td><td  width='100px' align='left'>" + resultJSON[i].Value + "</td>";
            if (i != 3 && (i - 2) % 3 == 0) {
                tbmonitorNote += "</tr></tr style='height:20px'>";
            }
        }
        trNote_monitor = tbMonitorData + field_monitor + tbName_monitor + trNote_monitor + tbmonitorNote + "</tr></table></fieldset></div>";
        var trbtn = "";
        if (DeviceInfo.SupportControl) {
            for (var i = 0; i < DeviceInfo.ControlNames.length; i++) {
                trbtn += "<button onclick='ControlDevice(\"" + DeviceInfo.ControlNames[i] + "\")' class='psbutton'>" + DeviceInfo.ControlNames[i] + "</button>";
            }
        }
        if (DeviceInfo.SupportRefresh) {
            trbtn += "<button onclick='RefreshDevice(" + realDevId + ")' class='psbutton'>即时刷新</button>";
        }
        if (DeviceInfo.SupportParam) {
            trbtn += "<button onclick='OpenSetParam()' class='psbutton'>参数设置</button>";
        }
        var btnhtml = "<div style='width:545px;height:auto;'><table><tr><td  width='545px' align='center'>" + trbtn + "</td></tr></div>";
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
    $('#dlgParam').dialog({ closed: false });
    LoadCurrentDevCombobox();
    $("#txt_Param").val("");
    $("#txt_unit").html("");
    $('#paramgrid').datagrid('loadData', { rows: [] });
}

function CheckDeviceData() {
    devIDs = "";
    if (deviceNodeLoaded && treeLoaded) {
        if(isPollingDevDatas)
        {
            return;
        }
        isPollingDevDatas = true;
        var checkedNodes = checkedDeviceIds;
        for (var i = 0; i < checkedNodes.length; i++) {
            var checkedNode = checkedNodes[i];
            if (devIDs != "") {
                devIDs += ",";
            }
            devIDs += checkedNode;
            tempID = deviceNodes[checkedNodes[i]].attributes.device.用户站参数;
            tempId = deviceNodes[checkedNodes[i]].attributes.device.用户站参数;
            if (tempIDObj[tempID] == null) {
                if (columnNamesKey != "") {
                    columnNamesKey += "-";
                }
                columnNamesKey += tempID;
                tempIDObj[tempID] = tempID;
            }
        }
        if (dicColumnNames[columnNamesKey] == null) {
            getColumnNames = true;
        }
        LoadTree("divAreaTree", mnId, true, true);
        LoadTableData(devIDs, getColumnNames);
    }
    else {
        setTimeout(CheckDeviceData, 100);
    }
}

function LoadCurrentDevCombobox() {
    $.ajax(
        {
            url: "../WebServices/DeviceNodeService.asmx/GetDeviceNodeInfo",
            type: "GET",
            data: { "loginIdentifer": window.parent.guid, 'devID': realDevId },
            dataType: "text",
            cache: false,
            success: function (responseText) {
                var data = eval("(" + $.xml2json(responseText) + ")");
                if (data.Result)//登录成功
                {
                    deviceJson = data.DeviceNode;
                    var comboBoxDataLevel = [];
                    var levelObj = {};
                    levelObj["id"] = deviceJson.ID;
                    levelObj["text"] = deviceJson.名称;
                    levelObj["selected"] = true;
                    comboBoxDataLevel.push(levelObj);
                    $("#ccb_DeviceName").combobox({
                        data: comboBoxDataLevel
                    });
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
            }
        });
}

function LinkHistoryData(diviceID) {
    window.parent.addTab('历史查询', 'DataQuery/HistoryDataQuery.html?devid=' + diviceID, 'HistoryDataQuery.html', true, 'menuC19');
    window.parent.showM(window.parent.menuCObj["历史查询"]);
}

function LinkAlarmData(diviceID) {
    window.parent.addTab('报警查询', 'DataQuery/AlarmDataQuery.html?devid=' + diviceID, 'tu1703', true, 'menuC19');
    window.parent.showM(window.parent.menuCObj["报警查询"]);
}

function LoadComboboxData() {
    if (!deviceNodeLoaded) {
        window.setTimeout("LoadComboboxData()", 500);
        return;
    }
    comboBoxData = [];
    for (i = 0; i < deviceJson.length; i++) {
        var devObj = {};
        devObj["id"] = deviceJson[i].ID;
        devObj["text"] = deviceJson[i].名称;
        comboBoxData.push(devObj);
    }
    $("#cbb_DevCombobox").combobox({
        data: comboBoxData
    });
}


function ReadParam() {
    var _strDeviceID = $("#ccb_DeviceName").combobox('getValue');
    $.ajax(
    {
        url: "../WebServices/DeviceMonitorService.asmx/GetParams",
        type: "GET",
        data: { "loginIdentifer": window.parent.guid, "devID": _strDeviceID },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            var paramtableData = [];
            if (data.Result) {
                for (var i = 0; i < data.Params.length; i++) {
                    var tableRow = {};
                    tableRow["paramName"] = data.Params[i].Name;
                    tableRow["param"] = data.Params[i].Value;
                    tableRow["unit"] = data.Params[i].Unit;
                    paramtableData.push(tableRow);
                }
                $("#paramgrid").datagrid('loadData', paramtableData);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function SetParam() {
    var _strDeviceID = $("#ccb_DeviceName").combobox('getValue');
    var _strParamName = $("#ccb_ParamName").combobox('getValue');
    var _strValue = $("#txt_Param").val();
    if (_strParamName == "全部") {
        $.messager.alert("提示信息", "设参操作参数名称不能为全部！");
        return;
    }
    $.ajax(
    {
        url: "../WebServices/DeviceMonitorService.asmx/SetParam",
        type: "GET",
        data: { "loginIdentifer": window.parent.guid, "devID": _strDeviceID, "paramName": _strParamName, "paramValue": _strValue },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                $.messager.alert("提示信息", "设置成功！");
                ReadParam();
            } else {
                $.messager.alert("提示信息", data.Message);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function ControlDevice(ctrlName) {
    $.ajax({
        type: "GET",
        url: "../WebServices/DeviceMonitorService.asmx/ControlDevice",
        data: { 'loginIdentifer': window.parent.guid, 'ctrlName': ctrlName, 'devID': realDevId },
        contentType: "content",
        dataType: "text",
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                if (ctrlName == "拍照") {
                    cameraInfo["正在拍照"] = true;
                    $.messager.alert("提示信息", "拍照成功，正在取照片，大约需要3分钟，请耐心等待！");
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
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.reponseText);
        }
    });
}

function RefreshDevice(deviceID) {
    $.ajax({
        type: "GET",
        url: "../WebServices/DeviceMonitorService.asmx/RefreshDevice",
        data: { 'loginIdentifer': window.parent.guid, 'devID': realDevId },
        contentType: "content",
        dataType: "text",
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                $.messager.alert("提示信息", "即时刷新成功！");
            } else {
                var result = data.Message;
                $.messager.alert("提示信息", result);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.reponseText);
        }
    });
}
