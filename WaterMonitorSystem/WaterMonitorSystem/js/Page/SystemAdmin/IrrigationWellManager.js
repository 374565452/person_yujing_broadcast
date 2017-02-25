// JScript 文件
var dicDeviceAppGroups = {};
//表格数据
var tableData = [];
var comboBoxData = [];
var mnId = "";
var operateType = "NEW";
var levelid = [];
//编辑数据
var editJson = {};
//左侧树形选中节点的管理ID
var currentSelManageId;

var currentSelDeviceId;

var monitorRealName = "";
$.extend($.fn.validatebox.defaults.rules, {
    numLength: {
        validator: function (value, param) {
            return value.length == param[0];
        },
        message: '设备编码必须为3位数字！'
    }
});
$(document).ready(function () {
    var defaultTheme = $.cookie("psbsTheme");
    if (defaultTheme != null && defaultTheme != "default") {
        var link = $(document).find('link:first');
        link.attr('href', '../App_Themes/easyui/themes/' + defaultTheme + '/easyui.css');
    }
    $.ShowMask("数据加载中，请稍等……");
    GetSystemInfo();
    $('#btn_Add').linkbutton({ disabled: true });
    $("#txt_DevNum").blur(function () {
        var numtext = $("#txt_DevNum").val();
        if (numtext.length < 3) {
            var leftnum = "";
            for (var i = 0; i < (3 - numtext.length) ; i++) {
                leftnum += "0";
            }
            $("#txt_DevNum").val(leftnum + numtext);
        }
    });
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
                monitorRealName = data.SysStateInfo.监测点级别名称;
                $("#wellname").text(monitorRealName);
                LoadTree("divAreaTree", mnId, true, false);
                LoadTableData();
                LoadComboboxData();
                LoadLevelComboboxData();
                LoadQuotaComboboxData();
                LoadDeviceTypeCodeComboboxData();

                var comboBoxStationType = [];
                for (var i = 0; i < 3; i++) {
                    if (i == 0) {
                        var levelObj = {};
                        levelObj["id"] = "00";
                        levelObj["text"] = "00 - 单站";
                        comboBoxStationType.push(levelObj);
                    } else if (i == 1) {
                        var levelObj = {};
                        levelObj["id"] = "01";
                        levelObj["text"] = "01 - 主站";
                        comboBoxStationType.push(levelObj);
                    }
                    else {
                        var levelObj = {};
                        levelObj["id"] = "02";
                        levelObj["text"] = "02 - 从站";
                        comboBoxStationType.push(levelObj);
                    }
                }
                $("#cbb_StationType").combobox({
                    data: comboBoxStationType
                });

                var comboBoxDeviceType = [];
                for (var i = 0; i < 3; i++) {
                    if (i == 0) {
                        var levelObj = {};
                        levelObj["id"] = "1";
                        levelObj["text"] = "水泵";
                        comboBoxDeviceType.push(levelObj);
                    } else if (i == 1) {
                        var levelObj = {};
                        levelObj["id"] = "2";
                        levelObj["text"] = "水位仪";
                        comboBoxDeviceType.push(levelObj);
                    }
                    else {
                        var levelObj = {};
                        levelObj["id"] = "3";
                        levelObj["text"] = "水泵、水位仪";
                        comboBoxDeviceType.push(levelObj);
                    }
                }
                $("#cbb_DeviceType").combobox({
                    data: comboBoxDeviceType
                });
                /*
                var comboBoxStationCode = [];
                for (var i = 0; i < 33; i++) {
                    var levelObj = {};
                    var v = i;
                    if (v < 10) v = "0" + i;
                    levelObj["id"] = v;
                    levelObj["text"] = v;
                    comboBoxStationCode.push(levelObj);
                }
                $("#cbb_StationCode").combobox({
                    data: comboBoxStationCode
                });
                */
                /*
                var comboBoxFrequency = [];
                for (var i = 0; i < 32; i++) {
                    var levelObj = {};
                    var v = i.toString(16);
                    if (v.length < 2) v = "0" + v;
                    levelObj["id"] = v;
                    levelObj["text"] = v;
                    comboBoxFrequency.push(levelObj);
                }
                $("#cbb_Frequency").combobox({
                    data: comboBoxFrequency
                });
                */
                TreeBindSelect();
                InitTimeControl();
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function TreeBindSelect() {
    $('#divAreaTree').tree({
        onSelect: function (node) {
            var devIds = [];
            if (node.attributes["nodeType"] == "manage") {
                currentSelManageId = node.attributes["manage"].ID;
                devIds = GetDeviceIDsByNode(node);
                if (node.attributes["manage"]["级别名称"] == "村庄") {
                    $("#btn_Add").linkbutton({
                        disabled: false
                    });
                }
                else {
                    $("#btn_Add").linkbutton({
                        disabled: true
                    });
                }
            } else {
                currentSelManageId = "";
                currentSelDeviceId = node.attributes["device"].ID;
                devIds.push(node.attributes["device"].ID);
                $("#btn_Add").linkbutton({
                    disabled: true
                });
            }
            ReLoadTableData(devIds,true);
        }
    });
}

//初始化时间框
function InitTimeControl() {
    var e = new Date();
    $("#txt_InstallTime").val(e.Format("yyyy-MM-dd 00:00"));
}

function btn_Add_Click() {
    operateType = "NEW";
    ClearManageDetailInfo();
    if (currentSelManageId == null || currentSelManageId == "") {
        $.messager.alert("提示信息", "请选择要添加下级管理的节点！");
        return;
    }
    $('#cbb_DevManager').combobox({ disabled: true }).combobox('select', currentSelManageId);
    $('#dlgDevice').dialog({ title: "添加设备信息", closed: false });
}

function EditMonitor(deviceid) {
    operateType = "MODIFY";
    $('#dlgDevice').dialog({ title: "修改设备信息", closed: false });
    $('#cbb_DevManager').combobox({ disabled: true });
    $("#txt_ID").val(deviceid);
    $("#txt_DevName").val(editJson[deviceid].名称);
    $("#cbb_DevManager").combobox('select', editJson[deviceid].管理ID);
    $("#txt_DevNum").val(editJson[deviceid].DeviceNo);
    $("#txt_PhoneNum").val(editJson[deviceid].SimNo);
    $("#txt_InstallTime").val(typeof (editJson[deviceid].SetupDate) == "undefined" ? "" : editJson[deviceid].SetupDate);
    $("#txt_InstallPosition").val(typeof (editJson[deviceid].SetupAddress) == "undefined" ? "" : editJson[deviceid].SetupAddress);
    //$("#txt_PumpType").val(typeof (editJson[deviceid].辅助信息.水泵型号) == "undefined" ? "" : editJson[deviceid].辅助信息.水泵型号);
    //$("#txt_MeterType").val(typeof (editJson[deviceid].辅助信息.水表型号) == "undefined" ? "" : editJson[deviceid].辅助信息.水表型号);
    //$("#txt_ElectricType").val(typeof (editJson[deviceid].辅助信息.电表型号) == "undefined" ? "" : editJson[deviceid].辅助信息.电表型号);
    $("#txt_DevPosition").val((typeof (editJson[deviceid].LON) == "undefined" ? "" : editJson[deviceid].LON) + "|" + (typeof (editJson[deviceid].LAT) == "undefined" ? "" : editJson[deviceid].LAT));
    $("#txt_YearExploitation").val(editJson[deviceid].YearExploitation);
    $("#txt_AlertAvailableWater").val(editJson[deviceid].AlertAvailableWater);
    $("#txt_MeterPulse").val(editJson[deviceid].MeterPulse);
    $("#txt_AlertWaterLevel").val(editJson[deviceid].AlertWaterLevel);
    $("#txt_AlertAvailableElectric").val(editJson[deviceid].AlertAvailableElectric);
    $("#txt_Area").val(editJson[deviceid].Area);
    $("#cbb_DeviceTypeCode").combobox('select', editJson[deviceid].DeviceTypeCodeId);
    $("#cbb_Crop").combobox('select', editJson[deviceid].CropId);

    $("#cbb_StationType").combobox('setValue', editJson[deviceid].StationType);
    //$("#cbb_StationCode").combobox('setValue', editJson[deviceid].StationCode);
    $("#txt_StationCode").val(editJson[deviceid].StationCode);
    //$("#cbb_Frequency").combobox('setValue', editJson[deviceid].Frequency);
    $("#txt_MainDevNum").val(editJson[deviceid].MainDevNum);
    $("#cbb_DeviceType").combobox('setValue', editJson[deviceid].DeviceType);
    $("#txt_RemoteStation").val(editJson[deviceid].RemoteStation);
}

function ClearManageDetailInfo() {
    $("#cbb_DevManager").combobox('setValue', "");
    $("#cbb_DeviceTypeCode").combobox('setValue', "");
    $("#cbb_Crop").combobox('setValue', "");
    $("#txt_DevName").val("");
    $("#txt_DevNum").val("");
    $("#txt_PhoneNum").val("");
    $("#txt_InstallPosition").val("");
    $("#txt_DevPosition").val("");
    $("#txt_PumpType").val("");
    $("#txt_MeterType").val("");
    $("#txt_ElectricType").val("");
    $("#txt_YearExploitation").val("");
    $("#txt_AlertAvailableWater").val("");
    $("#txt_AlertAvailableElectric").val("");
    $("#txt_MeterPulse").val("");
    $("#txt_AlertWaterLevel").val("");
    $("#txt_Area").val("");

    $("#cbb_StationType").combobox('setValue', "00");
    //$("#cbb_StationCode").combobox('setValue', "00");
    $("#txt_StationCode").val("");
    //$("#cbb_Frequency").combobox('setValue', "00");
    $("#txt_MainDevNum").val("");
    $("#cbb_DeviceType").combobox('setValue', "水泵");
    $("#txt_RemoteStation").val("");

    InitTimeControl();
}

function LoadTableData(treeid) {
    if (!deviceNodeLoaded) {
        window.setTimeout("LoadTableData(" + treeid + ")", 500);
        return;
    }
    if (treeid) {
        var node = $('#divAreaTree').tree('find', "mn_" + treeid);
        $('#divAreaTree').tree('select', node.target);
        return;
    }

    tableData = [];
    var devJson;
    for (var key in deviceNodes) {
        devJson = deviceNodes[key].attributes["device"];
        var tableRow = {};
        editJson[devJson.ID] = devJson;
        tableRow["devId"] = devJson.ID;
        tableRow["mnName"] = devJson.管理名称;
        tableRow["devName"] = devJson.名称;
        tableRow["devType"] = devJson.DeviceType;
        tableRow["DeviceNo"] = devJson.DeviceNo;
        tableRow["SimNo"] = devJson.SimNo;
        tableRow["InstallTime"] = devJson.SetupDate;
        tableRow["installSite"] = devJson.SetupAddress;
        tableRow["YearExploitation"] = devJson.YearExploitation;
        tableRow["Crop"] = devJson.Crop;
        tableRow["Area"] = devJson.Area;
        tableRow["showSlave"] = "<img src='../Images/Detail.gif' onclick='javascript:ShowSlave(this)' />";
        tableRow["editDev"] = "<img src='../Images/Detail.gif' onclick='javascript:EditMonitor(" + devJson.ID + ")' />";
        tableRow["removeDev"] = "<img src='../Images/Delete.gif' onclick='javascript:DeleteMonitor(" + devJson.ID + ")' />";

        tableRow["StationTypeStr"] = devJson.StationTypeStr;
        tableRow["MainDevNum"] = devJson.MainDevNum;
        var SlaveList = '<table width="100%" class="gridtable"><tr><th>从站名称</th><th>从站类型</th><th>从站编号</th><th>从站地址码</th></tr>';
        for (var j = 0; j < devJson.SlaveList.length; j++) {
            SlaveList += '<tr><td>' + devJson.SlaveList[j].名称 + '</td><td>' + devJson.SlaveList[j].DeviceType + '</td><td>' + devJson.SlaveList[j].编号 + '</td><td>' + devJson.SlaveList[j].StationCode + '</td></tr>';
        }
        SlaveList += "</table>";
        tableRow["SlaveList"] = SlaveList;
        tableData.push(tableRow);
    }
    $('#tbDevInfos').datagrid({ loadFilter: pagerFilter }).datagrid('loadData', tableData);
    $.HideMask();

}

function ReLoadTableData(devIds,flag) {
    tableData = [];
    var devJson;
    comboBoxData = [];
    for (var i = 0; i < devIds.length; i++) {
        var tableRow = {};
        devJson = deviceNodes[devIds[i]].attributes["device"];
        editJson[devJson.ID] = devJson;
        tableRow["devId"] = devJson.ID;
        tableRow["mnName"] = devJson.管理名称;
        tableRow["devName"] = devJson.名称;
        tableRow["devType"] = devJson.DeviceType;
        tableRow["DeviceNo"] = devJson.DeviceNo;
        tableRow["SimNo"] = devJson.SimNo;
        tableRow["InstallTime"] = devJson.SetupDate;
        tableRow["installSite"] = devJson.SetupAddress;
        tableRow["YearExploitation"] = devJson.YearExploitation;
        tableRow["Crop"] = devJson.Crop;
        tableRow["Area"] = devJson.Area;
        tableRow["showSlave"] = "<img src='../Images/Detail.gif' onclick='javascript:ShowSlave(this)' />";
        tableRow["editDev"] = "<img src='../Images/Detail.gif' onclick='javascript:EditMonitor(" + devJson.ID + ")' />";
        tableRow["removeDev"] = "<img src='../Images/Delete.gif' onclick='javascript:DeleteMonitor(" + devJson.ID + ")' />";

        tableRow["StationTypeStr"] = devJson.StationTypeStr;
        tableRow["MainDevNum"] = devJson.MainDevNum;
        var SlaveList = '<table width="100%" class="gridtable"><tr><th>从站名称</th><th>从站类型</th><th>从站编号</th><th>从站地址码</th></tr>';
        for (var j = 0; j < devJson.SlaveList.length; j++) {
            SlaveList += '<tr><td>' + devJson.SlaveList[j].名称 + '</td><td>' + devJson.SlaveList[j].DeviceType + '</td><td>' + devJson.SlaveList[j].编号 + '</td><td>' + devJson.SlaveList[j].StationCode + '</td></tr>';
        }
        SlaveList += "</table>";
        tableRow["SlaveList"] = SlaveList;
        tableData.push(tableRow);

        if (flag) {
            var devObj = {};
            devObj["id"] = devJson.ID;
            devObj["text"] = devJson.名称;
            comboBoxData.push(devObj);
        }
    }
    if (flag) {
        $("#cbb_DevCombobox").combobox({
            data: comboBoxData
        });
    }
    
    $('#tbDevInfos').datagrid({ loadFilter: pagerFilter }).datagrid('loadData', tableData);
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

function Btn_Query_Click() {
    var devIds = $.QueryCombobox("cbb_DevCombobox");
    if (devIds == "" || devIds == null) {
        $.messager.alert("提示信息", "请选择设备名称！");
        return;
    }
    devIds = devIds.split(",");
    ReLoadTableData(devIds,false);
}

//绑定所属级别的下拉框
function LoadLevelComboboxData() {
    $.ajax(
    {
        url: "../WebServices/ManageNodeService.asmx/GetManageNodeInfos",
        type: "GET",
        data: { "mnID": mnId, "loginIdentifer": window.parent.guid, "isRecursive": true, "isExport": false },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result)//登录成功
            {
                levelJson = data.ManageNodes;
                var comboBoxDataLevel = [];

                for (i = 0; i < levelJson.length; i++) {
                    var levelObj = {};
                    levelObj["id"] = levelJson[i].ID;
                    levelObj["text"] = levelJson[i].名称;
                    comboBoxDataLevel.push(levelObj);
                }
                $("#cbb_DevManager").combobox({
                    data: comboBoxDataLevel
                });
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function LoadQuotaComboboxData() {
    $.ajax(
    {
        url: "../WebServices/QuotaManageService.asmx/GetUnitQuotasByType",
        type: "GET",
        data: { 'unitQuotaType': "作物", "loginIdentifer": window.parent.guid },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result)//登录成功
            {
                var recoreJson = data.UnitQuotas;
                var comboBoxDataQuota = [];
                for (var i = 0; i < recoreJson.length; i++) {
                    var QuotaObj = {};
                    QuotaObj["id"] = recoreJson[i].ID;
                    QuotaObj["text"] = recoreJson[i].名称 + " - " + recoreJson[i].单位定额;
                    comboBoxDataQuota.push(QuotaObj);
                }
                $("#cbb_Crop").combobox({
                    data: comboBoxDataQuota
                });
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function LoadDeviceTypeCodeComboboxData() {
    $.ajax(
    {
        url: "../WebServices/SystemService.asmx/GetDeviceTypeCode",
        type: "GET",
        data: { "loginIdentifer": window.parent.guid },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result)//登录成功
            {
                var recoreJson = data.DeviceTypeCode;
                var comboBoxDataDeviceTypeCode = [];
                for (var i = 0; i < recoreJson.length; i++) {
                    var DeviceTypeCodeObj = {};
                    DeviceTypeCodeObj["id"] = recoreJson[i].ID;
                    DeviceTypeCodeObj["text"] = recoreJson[i].K + " - " + recoreJson[i].V;
                    comboBoxDataDeviceTypeCode.push(DeviceTypeCodeObj);
                }
                $("#cbb_DeviceTypeCode").combobox({
                    data: comboBoxDataDeviceTypeCode
                });
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function btn_OK_Click() {
    var devJsonStr = "";
    var devid = $("#txt_ID").val();
    var devname = $("#txt_DevName").val();
    var devmanagerId = $("#cbb_DevManager").combobox('getValue');
    var devmanagerName = $("#cbb_DevManager").combobox('getText');
    var devnum = $("#txt_DevNum").val();
    var phonenum = $("#txt_PhoneNum").val();
    var installtime = $("#txt_InstallTime").val();
    var installposition = $("#txt_InstallPosition").val();
    var devposition = $("#txt_DevPosition").val();
    var pumptype = $("#txt_PumpType").val();
    var metertype = $("#txt_MeterType").val();
    var electrictype = $("#txt_ElectricType").val();
    var YearExploitation = $("#txt_YearExploitation").val();
    var AlertAvailableWater = $("#txt_AlertAvailableWater").val();
    var DeviceTypeCode = $("#cbb_DeviceTypeCode").combobox('getValue');
    var MeterPulse = $("#txt_MeterPulse").val();
    var AlertWaterLevel = $("#txt_AlertWaterLevel").val();
    var AlertAvailableElectric = $("#txt_AlertAvailableElectric").val();
    var Crop = $("#cbb_Crop").combobox('getValue');
    var Area = $("#txt_Area").val();
    var StationType = $("#cbb_StationType").combobox('getValue');
    //var StationCode = $("#cbb_StationCode").combobox('getValue');
    var StationCode = $("#txt_StationCode").val();
    //var Frequency = $("#cbb_Frequency").combobox('getValue');
    var MainDevNum = $("#txt_MainDevNum").val();
    var DeviceType = $("#cbb_DeviceType").combobox('getValue');
    var RemoteStation = $("#txt_RemoteStation").val();

    //写死的信息
    var isPoll = "是";//是否轮询
    var pollInternal = "600";//轮询间隔

    //进行验证
    if (devname.trim() == null || devname.trim() == "") {
        $.messager.alert("提示信息", "设备名称不能为空！");
        return;
    }
    if (devnum.trim() == "") {
        $.messager.alert("提示信息", "设备编码不能为空！");
        return;
    }
    var reNum = /^\d*$/;
    if (!reNum.test(devnum)) {
        $.messager.alert("提示信息", "设备编码必须为数字！");
        return;
    }
    if (parseInt(devnum.trim())>254) {
        $.messager.alert("提示信息", "设备编码不能大于254！");
        return;
    }

    if (!(/^1[3|4|5|7|8][0-9]\d{4,8}$/.test(phonenum))) {
        $.messager.alert("提示信息", "手机号码不规范！");
        return;
    }
    if (phonenum.length < 11) {
        $.messager.alert("提示信息", "手机号码必须是11位有效数字！");
        return;
    }
    /*
    devJsonStr = "[{ID:'" + devid + "',测点名称:'" + devname + "',管理ID:'" + devmanagerId + "',经纬度:'" + devposition + "',手机卡号:'" + phonenum
	                          + "',编码:'" + devnum + "',安装时间:'" + installtime + "',安装位置:'" + installposition + "',水泵型号:'" + pumptype + "',水表型号:'" + metertype + "',电表型号:'" + electrictype + "',测点类型:'设备灌溉',通讯设备名称:'" +
	                          devmanagerName + "_" + devname + "',设备地址:'101',是否轮询:'" + isPoll + "',轮询间隔:'" + pollInternal
	                          + "',通讯协议:'设备灌溉水资源',传输协议:'设备灌溉水资源',传输协议参数:'" + phonenum + ",1',通道类型:'DC_TCP_FromServer',通道服务参数:'9998'}]";
    */
    devJsonStr = "{";
    devJsonStr += "'ID'" + ":'" + devid + "',";
    devJsonStr += "'名称'" + ":'" + devname + "',";
    devJsonStr += "'经纬度'" + ":'" + devposition + "',";
    devJsonStr += "'手机卡号'" + ":'" + phonenum + "',";
    devJsonStr += "'编码'" + ":'" + devnum + "',";
    devJsonStr += "'安装时间'" + ":'" + installtime + "',";
    devJsonStr += "'安装位置'" + ":'" + installposition + "',";
    devJsonStr += "'年可开采水量'" + ":'" + YearExploitation + "',";
    devJsonStr += "'可用水量提醒'" + ":'" + AlertAvailableWater + "',";
    devJsonStr += "'可用电量提醒'" + ":'" + AlertAvailableElectric + "',";
    devJsonStr += "'流量计类型'" + ":'" + DeviceTypeCode + "',";
    devJsonStr += "'电表脉冲数'" + ":'" + MeterPulse + "',";
    devJsonStr += "'水位报警值'" + ":'" + AlertWaterLevel + "',";
    devJsonStr += "'作物'" + ":'" + Crop + "',";
    devJsonStr += "'面积'" + ":'" + Area + "',";
    devJsonStr += "'管理ID'" + ":'" + devmanagerId + "',";
    devJsonStr += "'站类型'" + ":'" + StationType + "',";
    devJsonStr += "'地址码'" + ":'" + StationCode + "',";
    //devJsonStr += "'通信频率'" + ":'" + Frequency + "',";
    devJsonStr += "'主站编码'" + ":'" + MainDevNum + "',";
    devJsonStr += "'设备类型'" + ":'" + DeviceType + "',";
    devJsonStr += "'水位仪编码'" + ":'" + RemoteStation + "'";
    devJsonStr += "}";
    $.ShowMask("请稍等……");

    if (operateType == "MODIFY") {
        $.ajax({
            url: "../WebServices/DeviceNodeService.asmx/ModifyDevice",
            data: { "loginIdentifer": window.parent.guid, 'deviceJSONString': devJsonStr },
            type: "get",
            dataType: "text",
            cache: false,
            success: function (responseText) {
                var data = eval("(" + $.xml2json(responseText) + ")");
                if (data.Result) {
                    $.HideMask();
                    $.messager.alert("提示信息", "修改设备信息成功！");
                    LoadTree("divAreaTree", mnId, true, false);
                    LoadTableData(devmanagerId);
                    LoadComboboxData();
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
    else {
        $.ajax({
            url: "../WebServices/DeviceNodeService.asmx/AddDevice",
            data: { "loginIdentifer": window.parent.guid, 'deviceJSONString': devJsonStr },
            type: "get",
            dataType: "text",
            cache: false,
            success: function (responseText) {
                var data = eval("(" + $.xml2json(responseText) + ")");
                if (data.Result) {
                    $.HideMask();
                    $.messager.alert("提示信息", "添加设备信息成功！");
                    LoadTree("divAreaTree", mnId, true, false);
                    LoadTableData(devmanagerId);
                    LoadComboboxData();
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
}


    function Open() {
        //显示弹出窗口
        divFlyBar.style.display = "block";
        document.getElementById("pickCoordinate").src = "PickLongitudeAndLattude.html?id=txt_DevPosition&2";
        $("#divFlyBar").css("z-index", 99999);
    }
    function Close() {
        divFlyBar.style.display = "none";
    }

    function DeleteMonitor(deviceid) {
        var pid = deviceNodes[deviceid].attributes["uid"];
        $.messager.confirm("提示", "确定要删除此记录吗？？", function (e) {
            if (e)
                $.ajax({
                    url: "../WebServices/DeviceNodeService.asmx/DeleteDevice",
                    data: { "devID": deviceid, "loginIdentifer": window.parent.guid },
                    type: "get",
                    dataType: "text",
                    cache: false,
                    success: function (responseText) {
                        var data = eval("(" + $.xml2json(responseText) + ")");
                        if (data.Result) {
                            $.messager.alert("提示信息", "删除" + data.Message);
                            LoadTree("divAreaTree", mnId, true, false);
                            LoadTableData(pid);
                            LoadComboboxData();
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
                    }
                });
        });
    }

    function ShowSlave(obj) {
        $('#dlgSlave').dialog({ title: "查看从站列表", closed: false });
        var strSlaveList = $(obj).parent().parent().parent().find(".datagrid-cell-c2-SlaveList").html();
        $("#divlist").html(strSlaveList);
    }