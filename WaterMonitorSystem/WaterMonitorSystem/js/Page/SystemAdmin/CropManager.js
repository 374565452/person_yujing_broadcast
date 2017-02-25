// JScript 文件
var dicDeviceAppGroups = {};
//表格数据
var tableData = [];
var comboBoxData = [];
//当前登录操作员管理ID
var mnId = "";
//操作类型
var operateType = "Add";
//左侧树形选中节点的管理ID
var currentSelManageId;
//用于存储当前编辑的管理的ID
var operateManageId;
//编辑数据
var editJson = {};
$(document).ready(function () {
    var defaultTheme = $.cookie("psbsTheme");
    if (defaultTheme != null && defaultTheme != "default") {
        var link = $(document).find('link:first');
        link.attr('href', '../App_Themes/easyui/themes/' + defaultTheme + '/easyui.css');
    }
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
                currentSelManageId = mnId;
                LoadTableData();
                LoadManagerCombobox();
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function LoadTableData() {
    $.ShowMask("数据加载中，请稍等……");
    $.ajax(
    {
        url: "../WebServices/QuotaManageService.asmx/GetUnitQuotasByType?t=" + Math.random(),
        type: "get",
        data: { 'unitQuotaType': "作物", "loginIdentifer": window.parent.guid },
        cache: false,
        dataType: "text",
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result)//成功
            {
                tableData = [];
                var recoreJson = data.UnitQuotas;
                for (var i = 0; i < recoreJson.length; i++) {
                    editJson[recoreJson[i].ID] = recoreJson[i];
                    var tableRow = {};
                    tableRow["agrotypeID"] = recoreJson[i].ID;
                    tableRow["waterNum"] = recoreJson[i].单位定额;
                    tableRow["agroType"] = recoreJson[i].名称;
                    tableRow["editAgro"] = "<img src='../Images/Detail.gif' onclick='javascript:EditAgro(" + recoreJson[i].ID + ")' />";
                    tableRow["removeAgro"] = "<img src='../Images/Delete.gif' onclick='javascript:DeleteAgro(" + recoreJson[i].ID + ")' />";
                    tableData.push(tableRow);
                }
                $('#tbAgroType').datagrid({ loadFilter: pagerFilter }).datagrid('loadData', tableData);
                $.HideMask();
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

//清空弹出窗口中的文本信息
function ClearManageDetailInfo() {
    $("#txt_AgroTypeName").val("");
    $("#txt_waterNum").val("");
}
function EditAgro(agroid) {
    operateType = "Modify";
    $('#dlgAgro').dialog({ title: "修改作物类型信息", closed: false });
    $("#txt_ID").val(agroid);
    $("#txt_AgroTypeName").val(editJson[agroid].名称);
    $("#txt_waterNum").val(editJson[agroid].单位定额);
}

function DeleteAgro(agroid) {
    $.messager.confirm("提示", "确定要删除此记录吗？？", function (e) {
        if (e)
            $.ajax({
                url: "../WebServices/QuotaManageService.asmx/DeleteUnitQuota",
                data: { "unitQuotaId": agroid, "loginIdentifer": window.parent.guid },
                type: "get",
                dataType: "text",
                cache: false,
                success: function (responseText) {
                    var data = eval("(" + $.xml2json(responseText) + ")");
                    if (data.Result) {
                        LoadTableData();
                        LoadManagerCombobox();
                        $.messager.alert("提示信息", data.Message);
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
                }
            });
    });
}
function btn_Add_Click() {
    operateType = "Add";
    ClearManageDetailInfo();
    $('#dlgAgro').dialog({ title: "添加作物类型信息", closed: false });
}

function btn_OK_Click() {
    var _Id = $("#txt_ID").val();
    var _Name = $("#txt_AgroTypeName").val();
    var _ModeId = 1;
    var _UnitQuota = $("#txt_waterNum").val();
    if (_Name.trim() == "") {
        $.messager.alert("提示信息", "请输入作物类型！");
        return;
    }
    if (_UnitQuota.trim() == "") {
        $.messager.alert("提示信息", "请输入亩均用水量！");
        return;
    }
    var re = /^\d+(?=\.{0,1}\d+$|$)/
    if (_UnitQuota.trim() != "") {
        if (!re.test(_UnitQuota)) {
            $.messager.alert("提示信息", "请输入正确的数字");
            $("#txt_waterNum").val("");
            $("#txt_waterNum").focus();
            return;
        }
    }
    var mJsonObj = "{ID:'" + _Id + "',名称:'" + _Name + "',定额方式ID:'" + _ModeId + "',单位定额:'" + _UnitQuota + "'}";
    $.ShowMask("请稍等……");
    if (operateType == "Add") {
        $.ajax(
        {
            url: "../WebServices/QuotaManageService.asmx/AddUnitQuota",
            type: "GET",
            data: { 'unitQuotaJson': mJsonObj, 'loginIdentifer': window.parent.guid },
            dataType: "text",
            cache: false,
            success: function (responseText) {
                var data = eval("(" + $.xml2json(responseText) + ")");
                if (data.Result)//登录成功
                {
                    LoadTableData();
                    LoadManagerCombobox();
                    $.HideMask();
                    $('#dlgAgro').dialog("close");
                    $.messager.alert("提示信息", "添加成功！");
                } else {
                    $.HideMask();
                    $.messager.alert("信息提示", data.Message);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $.HideMask();
                $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
            }
        });
    }
    else if (operateType == "Modify") {
        $.ajax(
        {
            url: "../WebServices/QuotaManageService.asmx/ModifyUnitQuotas",
            type: "POST",
            data: { 'unitQuotaJson': mJsonObj, 'loginIdentifer': window.parent.guid },
            dataType: "text",
            cache: false,
            success: function (responseText) {
                var data = eval("(" + $.xml2json(responseText) + ")");
                if (data.Result)//登录成功
                {
                    LoadTableData();
                    LoadManagerCombobox();
                    $.HideMask();
                    $('#dlgAgro').dialog("close");
                    $.messager.alert("提示信息", "修改成功！");
                } else {
                    $.HideMask();
                    $.messager.alert("信息提示", data.Message);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $.HideMask();
                $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
            }
        });
    }
}

function btn_Query_Click() {
    var unitQuotaId = $.QueryCombobox("cbb_DevCombobox");
    if (unitQuotaId.indexOf("all") >= 0) {
        unitQuotaId = unitQuotaId.substr(0, unitQuotaId.length - 3);
    }
    if (unitQuotaId == "" || unitQuotaId == null) {
        $.messager.alert("提示信息", "请选择作物类型！");
        return;
    }
    $.ShowMask("数据加载中，请稍等……");
    $.ajax(
    {
        url: "../WebServices/QuotaManageService.asmx/GetUnitQuotasByIds",
        type: "get",
        data: { 'unitQuotaIds': unitQuotaId, "loginIdentifer": window.parent.guid },
        cache: false,
        dataType: "text",
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result)//成功
            {
                tableData = [];
                var recoreJson = data.UnitQuotas;
                for (var i = 0; i < recoreJson.length; i++) {
                    editJson[recoreJson[i].ID] = recoreJson[i];
                    var tableRow = {};
                    tableRow["agrotypeID"] = recoreJson[i].ID;
                    tableRow["waterNum"] = recoreJson[i].单位定额;
                    tableRow["agroType"] = recoreJson[i].名称;
                    tableRow["editAgro"] = "<img src='../Images/Detail.gif' onclick='javascript:EditAgro(" + recoreJson[i].ID + ")' />";
                    tableRow["removeAgro"] = "<img src='../Images/Delete.gif' onclick='javascript:DeleteAgro(" + recoreJson[i].ID + ")' />";
                    tableData.push(tableRow);
                }
                
                $('#tbAgroType').datagrid({ loadFilter: pagerFilter }).datagrid('loadData', tableData);
                $.HideMask();
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

function LoadManagerCombobox() {
    $.ajax(
        {
            url: "../WebServices/QuotaManageService.asmx/GetUnitQuotasByType",
            type: "GET",
            data: { "loginIdentifer": window.parent.guid, "unitQuotaType": "作物" },
            dataType: "text",
            cache: false,
            success: function (responseText) {
                var allid = "";
                var data = eval("(" + $.xml2json(responseText) + ")");
                if (data.Result)//登录成功
                {
                    levelJson = data.UnitQuotas;
                    var comboBoxDataLevel = [];
                    for (i = 0; i < levelJson.length; i++) {
                        var levelObj = {};
                        levelObj["id"] = levelJson[i].ID;
                        levelObj["text"] = levelJson[i].名称;
                        comboBoxDataLevel.push(levelObj);
                        if (allid != "") {
                            allid += ",";
                        }
                        allid += levelJson[i].ID;
                    }
                    var allObj = {};
                    allObj["id"] = allid + "all";
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

