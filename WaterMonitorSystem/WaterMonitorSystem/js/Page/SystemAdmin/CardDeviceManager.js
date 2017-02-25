// JScript 文件


//--------------------
//登录标示
var loginIdentifer = window.parent.guid;
//用于存储查询出来的用水户卡信息
var cardUserObj = {};

$(document).ready(function () {
    $.ShowMask("数据加载中，请稍等……");
    GetSystemInfo();
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
                TreeBindSelect();
                $.HideMask();
                LoadWaterUserTree("VillageTree", mnId, false, false);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

//点击左侧树形重新加载右侧Combobox
function TreeBindSelect() {
    $('#VillageTree').tree({
        onSelect: function (node) {
            $("#cbb_DeviceNoCombobox").combobox({ data: [] });
            
            if (node.id.indexOf("cz") >= 0) {
                villageid = node.attributes["mid"];
                LoadCardUserInfo("../WebServices/CardDeviceService.asmx/GetCardDevicesByVillageId", { "loginIdentifer": window.parent.guid, "villageId": villageid, "isExport": false });
            }
        }
    });
}

function LoadCardUserInfo(url, data) {
    $.ajax(
    {
        url: url,
        type: "GET",
        data: data,
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result)//登录成功
            {
                var cardDevices = data.CardDevices;
                var comboBoxDataLevel = [];
                var tableData = [];
                allcomboid = "";
                for (var i = 0; i < cardDevices.length; i++) {
                    var cardDevice = cardDevices[i];
                    //更新datagrid
                    var tableRow = {};
                    tableRow["ID"] = cardDevice["ID"];
                    tableRow["SerialNumber"] = cardDevice["SerialNumber"];
                    tableRow["DeviceNo"] = cardDevice["DeviceNo"];
                    tableRow["YearExploitation"] = cardDevice["YearExploitation"];
                    tableRow["AlertAvailableWater"] = cardDevice["AlertAvailableWater"];
                    tableRow["AlertAvailableElectric"] = cardDevice["AlertAvailableElectric"];
                    tableRow["StationType"] = cardDevice["StationType"];
                    tableRow["StationCode"] = cardDevice["StationCode"];
                    tableRow["OpenTime"] = cardDevice["OpenTime"];

                    tableRow["Edit"] = "<img src='../Images/Detail.gif' onclick='ShowCardDevice(" + cardDevice["ID"] + ")' />";
                    tableData.push(tableRow);

                    //更新combobox
                    var comboBoxData = {};
                    comboBoxData["id"] = cardDevice["ID"];
                    comboBoxData["text"] = cardDevice["DeviceNo"];
                    var flag = false;
                    for (var k = 0; k < comboBoxDataLevel.length; k++) {
                        if (comboBoxDataLevel[k].id == comboBoxData.id) {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag) {
                        comboBoxDataLevel.push(comboBoxData);
                        if (allcomboid != "") {
                            allcomboid += ",";
                        }
                        allcomboid += cardDevice["ID"];
                    }
                }
                var allObj = {};
                allObj["id"] = allcomboid + "all";
                allObj["text"] = "全部";
                allObj["selected"] = true;
                comboBoxDataLevel.unshift(allObj);
                $('#tbCardDeviceInfos').datagrid({ loadFilter: pagerFilter }).datagrid('loadData', tableData);
                $.HideMask();
                //$('#tbWaterUserInfos').datagrid('loadData', tableData)
                $("#cbb_DeviceNoCombobox").combobox({
                    data: comboBoxDataLevel
                });
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

function ShowCardDevice(CardDeviceID) {
    $('#ShowCardDeviceData').dialog({ closed: false });
    $('#ShowCardDeviceData').dialog({ title: "查看设备卡信息" });
    $.ajax(
    {
        url: "../WebServices/CardDeviceService.asmx/GetCardDeviceById",
        type: "GET",
        data: { 'loginIdentifer': loginIdentifer, 'cardDeviceID': CardDeviceID },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                var CardDeviceInfo = data.CardDevice;
                $("#txt_SerialNumber").html(CardDeviceInfo["SerialNumber"]);
                $("#txt_DeviceNo").html(CardDeviceInfo["DeviceNo"]);
                $("#txt_YearExploitation").html(CardDeviceInfo["YearExploitation"]);
                $("#txt_AlertAvailableWater").html(CardDeviceInfo["AlertAvailableWater"]);
                $("#txt_AlertAvailableElectric").html(CardDeviceInfo["AlertAvailableElectric"]);
                $("#txt_MeterPulse").html(CardDeviceInfo["MeterPulse"]);
                $("#txt_AlertWaterLevel").html(CardDeviceInfo["AlertWaterLevel"]);
                $("#txt_TypeCode").html(CardDeviceInfo["TypeCode"]);
                $("#txt_StationType").html(CardDeviceInfo["StationType"]);
                $("#txt_StationCode").html(CardDeviceInfo["StationCode"]);
                $("#txt_OpenTime").html(CardDeviceInfo["OpenTime"]);
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

function Btn_Query_Click() {
    $('#tbCardDeviceInfos').datagrid('loadData', { total: 0, rows: [] });
    var Ids = $.QueryCombobox("cbb_DeviceNoCombobox");

    if (Ids.indexOf("all") >= 0) {
        Ids = Ids.substr(0, Ids.length - 3);
    }
    else if (Ids == "" || Ids == null) {
        $.messager.alert("提示信息", "请选择设备！");
        return;
    }
    $.ShowMask("数据加载中，请稍等……");
    $.ajax(
    {
        url: "../WebServices/CardDeviceService.asmx/GetCardDeviceByIds",
        type: "GET",
        data: { 'loginIdentifer': loginIdentifer, 'Ids': Ids },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                var cardDevices = data.CardDevices;
                tableData = [];
                for (var i = 0; i < cardDevices.length; i++) {
                    var cardDevice = cardDevices[i];
                    //更新datagrid
                    var tableRow = {};
                    tableRow["ID"] = cardDevice["ID"];
                    tableRow["SerialNumber"] = cardDevice["SerialNumber"];
                    tableRow["DeviceNo"] = cardDevice["DeviceNo"];
                    tableRow["YearExploitation"] = cardDevice["YearExploitation"];
                    tableRow["AlertAvailableWater"] = cardDevice["AlertAvailableWater"];
                    tableRow["AlertAvailableElectric"] = cardDevice["AlertAvailableElectric"];
                    tableRow["StationType"] = cardDevice["StationType"];
                    tableRow["StationCode"] = cardDevice["StationCode"];
                    tableRow["OpenTime"] = cardDevice["OpenTime"];

                    tableRow["Edit"] = "<img src='../Images/Detail.gif' onclick='ShowCardDevice(" + cardDevice["ID"] + ")' />";
                    tableData.push(tableRow);
                }
                $('#tbCardDeviceInfos').datagrid({ loadFilter: pagerFilter }).datagrid('loadData', tableData);
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



