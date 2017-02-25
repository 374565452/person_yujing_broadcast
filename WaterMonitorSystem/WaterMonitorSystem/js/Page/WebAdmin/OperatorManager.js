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

var currenteditmid = "";
$(document).ready(function () {
    var defaultTheme = $.cookie("psbsTheme");
    if (defaultTheme != null && defaultTheme != "default") {
        var link = $(document).find('link:first');
        link.attr('href', '../App_Themes/easyui/themes/' + defaultTheme + '/easyui.css');
    }
    $.ShowMask("数据加载中，请稍等……");
    GetSystemInfo();
    LoadUserRoleComboboxData();
    $("#cbb_PointerList").combotree({
        onCheck: function (node) {
            ckcdids = '';
            ckglids = '';
            var t = $("#cbb_PointerList").combotree('tree');
            var ckarr = t.tree('getChecked', ['checked', 'indeterminate']);

            $.each(ckarr, function (n, value) {
                if (value.id.indexOf('cd') >= 0) {
                    ckcdids += value.id.replace('cd_', '') + ',';
                }
                if (value.id.indexOf('mn') >= 0) {
                    ckglids += value.id.replace('mn_', '') + ',';
                }
            });
            if (ckcdids.length > 0) {
                ckcdids = ckcdids.substr(0, ckcdids.length - 1);
            }
            if (ckglids.length > 0) {
                ckglids = ckglids.substr(0, ckglids.length - 1);
            }
        }
    });
    $("#ccb_CustomerPointer").combobox({
        onSelect: function (rec) {
            if (rec.text == "是") {
                $("#cbb_PointerList").combotree("enable");
            } else {
                $("#cbb_PointerList").combotree("disable");
            }
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
                currentSelManageId = mnId;
                manageNodeLoaded = false;
                deviceNodeLoaded = false;
                TreeBindSelect();
                LoadTree("divAreaTree", mnId, false, false);
                managerRealName = data.SysStateInfo.监测点管理名称;
                $('#divContainer').layout('panel', 'west').panel({ title: managerRealName + "列表" });
                LoadTableData();
                GetComboTreeData();
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
        url: "../WebServices/UserAdminService.asmx/GetUserInfos?t=" + Math.random(),
        type: "get",
        data: { 'managerId': currentSelManageId, "loginIdentifer": window.parent.guid, "isRecursive": true, "isExport": false },
        cache: false,
        dataType: "text",
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result)//成功
            {
                tableData = [];
                _excelURL = data.ExcelURL;
                var recoreJson = data.UserNodes;
                var comboBoxDataLevel = [];
                for (var i = 0; i < recoreJson.length; i++) {
                    editJson[recoreJson[i].ID] = recoreJson[i];
                    var tableRow = {};
                    tableRow["userName"] = recoreJson[i].用户名;
                    tableRow["roleName"] = recoreJson[i].角色名称;
                    tableRow["unitName"] = recoreJson[i].管理名称;
                    tableRow["editUser"] = "<img src='../Images/Detail.gif' onclick='javascript:EditUser(" + recoreJson[i].ID + ")' />";
                    tableRow["removeUser"] = "<img src='../Images/Delete.gif' onclick='javascript:DeleteUser(" + recoreJson[i].ID + ")' />";
                    tableData.push(tableRow);


                    var levelObj = {};
                    levelObj["id"] = recoreJson[i].ID;
                    levelObj["text"] = recoreJson[i].用户名;
                    comboBoxDataLevel.push(levelObj);

                }
                $("#cbb_DevCombobox").combobox({
                    data: comboBoxDataLevel
                });
                $('#tbUserInfos').datagrid({ loadFilter: pagerFilter }).datagrid('loadData', tableData);
                $.HideMask();
            } else {
                $.HideMask();
                $.messager.alert("提示信息", data.Result);
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

//点击左侧树形重新加载右侧列表
function TreeBindSelect() {
    $('#divAreaTree').tree({
        onSelect: function (node) {
            var manageIds = GetManageIDsByNode(node);
            currentSelManageId = node.attributes["manage"].ID;
            LoadTableData();
        }
    });
}

//绑定所属角色的下拉框
function LoadUserRoleComboboxData() {
    $.ajax(
    {
        url: "../WebServices/RoleRightService.asmx/GetUserRoles",
        type: "GET",
        data: { 'loginIdentifer': window.parent.guid },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result)//登录成功
            {
                userRoleJsons = data.UserRoles;
                var comboBoxDataLevel = [];

                for (i = 0; i < userRoleJsons.length; i++) {
                    var userRoleObj = {};
                    userRoleObj["id"] = userRoleJsons[i].ID;
                    userRoleObj["text"] = userRoleJsons[i].Name;
                    comboBoxDataLevel.push(userRoleObj);
                }
                $("#ccb_RoleName").combobox({
                    data: comboBoxDataLevel
                });
                $("#ccb_RoleName").combobox('select', userRoleJsons[0].ID);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

//绑定上级区域下拉框中的树形
function GetComboTreeData() {
    LoadComTree("cbb_PointerList", mnId, true, true);
}

//清空弹出窗口中的文本信息
function ClearManageDetailInfo() {
    $("#txt_UserName").val("");
    $("#ccb_CustomerPointer").combobox('setValue', '否');
    $("#cbb_PointerList").combotree({
        disabled: true
    })
}
function EditUser(userid) {
    operateType = "Modify";
    $('#dlgUser').dialog({ title: "修改用户信息", closed: false });
    currenteditmid = editJson[userid].管理ID;
    $("#txt_ID").val(userid);
    $("#txt_UserName").val(editJson[userid].用户名);
    $("#ccb_RoleName").combobox('select', editJson[userid].角色ID);
    //    $("#txt_UnitName").val(editJson[userid].管理名称);
    $("#txt_UnitName").textbox("setText", editJson[userid].管理名称);
    $("#ccb_CustomerPointer").combobox('select', editJson[userid].自定义测点 == "是" ? "是" : "否");
    if (editJson[userid].自定义测点 == "是") {
        var ids = "";
        var cdids = editJson[userid].设备ID列表;
        var glids = editJson[userid].管理ID列表;
        if (cdids.indexOf(",") >= 0) {
            var arrcdids = cdids.split(',');
            for (var i = 0; i < arrcdids.length; i++) {
                ids += "cd_" + arrcdids[i] + ",";
            }
        }
        else {
            ids += "cd_" + cdids + ",";
        }
        $("#cbb_PointerList").combotree({
            disabled: false
        }).combotree('setValues', ids.substr(0, ids.length - 1));
        ckcdids = '';
        ckglids = '';
        var t = $("#cbb_PointerList").combotree('tree');
        var ckarr = t.tree('getChecked', ['checked', 'indeterminate']);

        $.each(ckarr, function (n, value) {
            if (value.id.indexOf('cd') >= 0) {
                ckcdids += value.id.replace('cd_', '') + ',';
            }
            if (value.id.indexOf('mn') >= 0) {
                ckglids += value.id.replace('mn_', '') + ',';
            }
        });
        if (ckcdids.length > 0) {
            ckcdids = ckcdids.substr(0, ckcdids.length - 1);
        }
        if (ckglids.length > 0) {
            ckglids = ckglids.substr(0, ckglids.length - 1);
        }
    } else {
        $("#cbb_PointerList").combotree({
            disabled: true
        });
    }
}

function DeleteUser(userid) {
    $.messager.confirm("提示", "确定要删除此记录吗？？", function (e) {
        if (e)
            $.ajax({
                url: "../WebServices/UserAdminService.asmx/DeleteUser",
                data: { "id": userid, "loginIdentifer": window.parent.guid },
                type: "get",
                dataType: "text",
                cache: false,
                success: function (responseText) {
                    var data = eval("(" + $.xml2json(responseText) + ")");
                    if (data.Result) {
                        LoadTree("divAreaTree", mnId, false, false);
                        LoadTableData();
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
    if (currentSelManageId == null || currentSelManageId == "") {
        $.messager.alert("提示信息", "请选择要添加下级管理的节点！");
        return;
    }
    operateType = "Add";
    ClearManageDetailInfo();
    $('#dlgUser').dialog({ title: "添加用户信息", closed: false });
    $.ajax(
    {
        url: "../WebServices/ManageNodeService.asmx/GetManageNodeInfo",
        type: "GET",
        data: { 'mnID': currentSelManageId, 'loginIdentifer': window.parent.guid },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result)//登录成功
            {
                levelJson = data.ManageNode;
                //                $("#txt_UnitName").val(levelJson.名称);
                $("#txt_UnitName").textbox("setText", levelJson.名称);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function btn_OK_Click() {
    var _userID = $("#txt_ID").val();
    var _userName = $("#txt_UserName").val();
    var _roleID = $("#ccb_RoleName").combobox("getValue");
    //var _unitName = $("#txt_UnitName").val();
    var _unitName = $("#txt_UnitName").textbox("getText");
    var _customerPoint = $("#ccb_CustomerPointer").combobox('getValue');
    if (_userName.trim() == "") {
        $.messager.alert("提示信息", "请输入用户名！");
        return;
    }

    if (_customerPoint == "是") {
        if (ckcdids.length == 0) {
            $.messager.alert("提示信息", "请选择测点！");
            return;
        }
    }

    var mJsonObj = "";
    if (operateType == "Add") {
        mJsonObj = "{ID:'" + _userID + "',用户名:'" + _userName + "',角色ID:'" + _roleID + "',管理ID:'" + currentSelManageId + "',自定义测点:'" + _customerPoint + "','管理ID列表':'" + ckglids + "','设备ID列表':'" + ckcdids + "'}";
    } else {
        mJsonObj = "{ID:'" + _userID + "',用户名:'" + _userName + "',角色ID:'" + _roleID + "',管理ID:'" + currenteditmid + "',自定义测点:'" + _customerPoint + "','管理ID列表':'" + ckglids + "','设备ID列表':'" + ckcdids + "'}";
    }
    $.ShowMask("请稍等……");
    if (operateType == "Add") {
        $.ajax(
        {
            url: "../WebServices/UserAdminService.asmx/AddUser",
            type: "GET",
            data: { 'userJSONString': mJsonObj, 'loginIdentifer': window.parent.guid },
            dataType: "text",
            cache: false,
            success: function (responseText) {
                var data = eval("(" + $.xml2json(responseText) + ")");
                if (data.Result)//登录成功
                {
                    //LoadTree("divAreaTree", mnId, false, false);
                    LoadTableData();
                    $.HideMask();
                    $('#dlgUser').dialog("close");
                    $.messager.alert("提示信息", data.Message);
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
    else if (operateType == "Modify") {
        $.ajax(
        {
            url: "../WebServices/UserAdminService.asmx/ModifyUser",
            type: "POST",
            data: { 'userJSONString': mJsonObj, 'loginIdentifer': window.parent.guid },
            dataType: "text",
            cache: false,
            success: function (responseText) {
                var data = eval("(" + $.xml2json(responseText) + ")");
                if (data.Result)//登录成功
                {
                    //LoadTree("divAreaTree", mnId, false, false);
                    LoadTableData();
                    $.HideMask();
                    $('#dlgUser').dialog("close");
                    $.messager.alert("提示信息", data.Message);
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
}

function btn_OutExcel_Click() {
    $.ajax(
    {
        url: "../WebServices/UserAdminService.asmx/GetUserInfos",
        type: "get",
        data: { 'managerId': mnId, "loginIdentifer": window.parent.guid, "isRecursive": true, "isExport": true },
        cache: false,
        dataType: "text",
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result)//成功
            {
                _excelURL = data.ExcelURL;
                if (_excelURL != null && _excelURL != "") {
                    window.location.href = _excelURL;
                }
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function Btn_Query_Click() {

    var operid = $.QueryCombobox("cbb_DevCombobox");
    if (operid == null || operid == "") {
        $.messager.alert("提示信息", "请选择操作员名称！");
        return;
    }
    LoadTableDataByIds(operid);
}

function LoadTableDataByIds(operid) {
    $.ShowMask("数据加载中，请稍等……");

    $.ajax(
    {
        url: "../WebServices/UserAdminService.asmx/GetUserInfosByIds",
        type: "get",
        data: { 'ids': operid, "loginIdentifer": window.parent.guid },
        cache: false,
        dataType: "text",
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result)//成功
            {
                tableData = [];
                _excelURL = data.ExcelURL;
                var recoreJson = data.UserNodes;
                for (var i = 0; i < recoreJson.length; i++) {
                    editJson[recoreJson[i].ID] = recoreJson[i];
                    var tableRow = {};
                    tableRow["userName"] = recoreJson[i].用户名;
                    tableRow["roleName"] = recoreJson[i].角色名称;
                    tableRow["unitName"] = recoreJson[i].管理名称;
                    tableRow["editUser"] = "<img src='../Images/Detail.gif' onclick='javascript:EditUser(" + recoreJson[i].ID + ")' />";
                    tableRow["removeUser"] = "<img src='../Images/Delete.gif' onclick='javascript:DeleteUser(" + recoreJson[i].ID + ")' />";
                    tableData.push(tableRow);
                }
                $('#tbUserInfos').datagrid({ loadFilter: pagerFilter }).datagrid('loadData', tableData);
                $.HideMask();
            } else {
                $.HideMask();
                $.messager.alert("提示信息", data.Result);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.HideMask();
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });

}

