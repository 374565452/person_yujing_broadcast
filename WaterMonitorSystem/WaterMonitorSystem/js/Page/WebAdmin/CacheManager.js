// JScript 文件
var dicDeviceAppGroups = {};
//表格数据
var tableData = [];
var comboBoxData = [];
//当前登录操作员管理ID
var mnId = "";
//操作类型
var operateType = "Add";
//用于存储所有级别的节点
var userRoleJsons;
//左侧树形选中节点的管理ID
var currentSelManageId;
//用于存储级别信息表中倒数第二级的级别ID(即管理的最后一级别)
var lastLevelId = "";
//是否显示直接子管理
var isContainsChildManage = true;
//用于存储当前编辑的管理的ID
var operateManageId;
//测点列表
var ckcdids = '';
//管理列表
var ckglids = '';
//编辑数据
var editJson = {};

var managerRealName = "";

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
                manageNodeLoaded = false;
                deviceNodeLoaded = false;
                managerRealName = data.SysStateInfo.监测点管理名称;
                LoadTableData();
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function LoadTableData() {
    $.ShowMask("数据加载中，请稍等……");

    var tableRow = {};
    tableRow["name"] = "基础信息";
    tableRow["Info"] = "";
    tableData.push(tableRow);

    tableRow = {};
    tableRow["name"] = "用水户";
    tableRow["Info"] = "";
    tableData.push(tableRow);

    tableRow = {};
    tableRow["name"] = "用户卡";
    tableRow["Info"] = "";
    tableData.push(tableRow);

    tableRow = {};
    tableRow["name"] = "设备";
    tableRow["Info"] = "";
    tableData.push(tableRow);

    tableRow = {};
    tableRow["name"] = "区域";
    tableRow["Info"] = "";
    tableData.push(tableRow);

    $('#tbUserInfos').datagrid('loadData', tableData).datagrid({
        onSelect: function (rowIndex, rowData) {
            $("#CacheName").html(rowData.name);
        }
    });
    $.HideMask();
}

function Btn_RefreshCache_Click() {
    var row = $('#tbUserInfos').datagrid('getSelected');
    if (row) {
        var index = $('#tbUserInfos').datagrid('getRowIndex', row);

        if (confirm("确定刷新缓存【" + row.name + "】吗？")) {
            $('#tbUserInfos').datagrid('getRows')[index].Info = "正在刷新【" + row.name + "】";
            $('#tbUserInfos').datagrid('updateRow', { index: index });

            $.ajax(
            {
                url: "../WebServices/SystemService.asmx/RefreshCache",
                type: "GET",
                data: { "loginIdentifer": window.parent.guid, "CacheName": row.name },
                dataType: "text",
                cache: false,
                success: function (responseText) {
                    var data = eval("(" + $.xml2json(responseText) + ")");
                    if (data.Result) {
                        $('#tbUserInfos').datagrid('getRows')[index].Info = "缓存【" + row.name + "】刷新成功！";
                    }
                    else {
                        $('#tbUserInfos').datagrid('getRows')[index].Info = "缓存【" + row.name + "】刷新失败！" + data.Message;
                    }
                    $('#tbUserInfos').datagrid('updateRow', { index: index });
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    $.messager.alert("提示信息1", errorThrown + "<br/>" + XMLHttpRequest.responseText);
                    $('#tbUserInfos').datagrid('getRows')[index].Info = "缓存【" + row.name + "】刷新出错！";
                    $('#tbUserInfos').datagrid('updateRow', { index: index });
                }
            });

        } else {
            $('#tbUserInfos').datagrid('getRows')[index].Info = "不刷新【" + row.name + "】";
            $('#tbUserInfos').datagrid('updateRow', { index: index });
        }
        
    }
    else {
        $.messager.alert("提示信息", "请选择一行记录！");
    }
}

