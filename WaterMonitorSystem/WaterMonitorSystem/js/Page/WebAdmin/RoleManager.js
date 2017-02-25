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
                LoadMenuList();
                LoadTableData();
                
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function LoadMenuList()
{
    $.ShowMask("数据加载中，请稍等……");

    $.ajax(
    {
        url: "../WebServices/MenuService.asmx/GetMenuAll",
        type: "GET",
        data: { 'loginIdentifer': window.parent.guid },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            menuData = eval("(" + $.xml2json(responseText) + ")");
            var menu = "";
            if (menuData != null && menuData.length > 0) {
                for (var i = 0; i < menuData.length; i++) {
                    if (menuData[i].子菜单 != null && menuData[i].子菜单.length > 0) {
                        menu += "<div class=\"menuM\">" + menuData[i].菜单名称 + "</div>";
                        menu += "<div class=\"menuS\">";
                        for (var j = 0; j < menuData[i].子菜单.length; j++) {
                            menu += "<div class=\"item\"><input id=\"ckb_" + menuData[i].子菜单[j].菜单ID + "\" type=\"checkbox\" value=\"" + menuData[i].子菜单[j].菜单ID + "\" /><label for=\"ckb_" + menuData[i].子菜单[j].菜单ID + "\">" + menuData[i].子菜单[j].菜单名称 + "</label></div>";
                        }
                        menu += "<div style=\"clear:both;\"></div></div>";
                    }
                }
                $("#MenuList").html(menu);
            }
            $.HideMask();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.HideMask();
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function LoadTableData() {
    $.ShowMask("数据加载中，请稍等……");

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
                tableData = [];
                userRoleJsons = data.UserRoles;
                for (i = 0; i < userRoleJsons.length; i++) {
                    var tableRow = {};
                    tableRow["Id"] = userRoleJsons[i].ID;
                    tableRow["RoleName"] = userRoleJsons[i].Name;
                    tableRow["UserCount"] = userRoleJsons[i].UserCount;
                    tableRow["editMenu"] = "<img src='../Images/Detail.gif' onclick='javascript:EditMenu(" + userRoleJsons[i].ID + ")' />";
                    tableRow["editRole"] = "<img src='../Images/Detail.gif' onclick='javascript:EditRole(" + userRoleJsons[i].ID + ",\"" + userRoleJsons[i].Name + "\")' />";
                    tableRow["deleteRole"] = "<img src='../Images/Delete.gif' onclick='javascript:DeleteRole(" + userRoleJsons[i].ID + "," + userRoleJsons[i].UserCount + ")' />";

                    tableData.push(tableRow);
                }
                $('#tbUserInfos').datagrid({ loadFilter: pagerFilter }).datagrid('loadData', tableData);
                $.HideMask();
            }
            else {
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

function DeleteRole(roleId, num)
{
    $.messager.confirm("提示", "确定要删除此记录吗？？", function (e) {
        if (e) {
            if (num > 0) {
                $.messager.alert("提示信息", "无法删除有用户数量的角色！");
                return;
            }

            $.ajax({
                url: "../WebServices/RoleRightService.asmx/DeleteRole",
                data: { "roleId": roleId, "loginIdentifer": window.parent.guid },
                type: "get",
                dataType: "text",
                cache: false,
                success: function (responseText) {
                    var data = eval("(" + $.xml2json(responseText) + ")");
                    if (data.Result) {
                        LoadTableData();
                        $.messager.alert("提示信息", data.Message);
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
                }
            });
        }
    });
}

function EditMenu(roleId) {
    $("#txt_ID").val(roleId);
    $("input[id^=ckb_]").prop('checked', false);
    $.ShowMask("加载角色权限，请稍等……");
    $.ajax(
    {
        url: "../WebServices/MenuService.asmx/GetMenuByRoleId",
        type: "GET",
        data: { 'roleId': roleId },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            menuData = eval("(" + $.xml2json(responseText) + ")");
            var menu = "";
            if (menuData != null && menuData.length > 0) {
                for (var i = 0; i < menuData.length; i++) {
                    if (menuData[i].子菜单 != null && menuData[i].子菜单.length > 0) {
                        for (var j = 0; j < menuData[i].子菜单.length; j++) {
                            //menu += menuData[i].子菜单[j].菜单ID + ",";
                            $("#ckb_" + menuData[i].子菜单[j].菜单ID).prop('checked', true);
                        }
                    }
                }
            }
            //alert("用户菜单：" + menu);
            $.HideMask();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.HideMask();
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });

    $('#dlgMenu').dialog({ title: "编辑权限信息", closed: false });
}

function btnMenu_OK_Click() {
    $.messager.confirm("提示", "确定修改角色权限吗？？", function (e) {
        if (e) {
            var _roleID = $("#txt_ID").val();
            var val = "";
            var objs = $("input[id^=ckb_]:checked");
            for (var i = 0; i < objs.length; i++) {
                val += objs.eq(i).val() + ",";
            }

            $.ShowMask("正在修改角色权限，请稍等……");
            $.ajax(
            {
                url: "../WebServices/RoleRightService.asmx/SetRoleMenu",
                type: "GET",
                data: { 'loginIdentifer': window.parent.guid, 'roleId': _roleID, 'menuIds': val },
                dataType: "text",
                cache: false,
                success: function (responseText) {
                    var data = eval("(" + $.xml2json(responseText) + ")");
                    if (data.Result) {
                        $.HideMask();
                        $('#dlgMenu').dialog("close");
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
    });
}

function EditRole(roleid, name) {
    operateType = "Modify";
    $('#dlgRole').dialog({ title: "修改角色信息", closed: false });
    $("#txt_ID").val(roleid);
    $("#txt_RoleName").textbox("setText", name);
}

function btn_Add_Click() {
    operateType = "Add";
    $('#dlgRole').dialog({ title: "添加角色信息", closed: false });
    $("#txt_ID").val(0);
    $("#txt_RoleName").textbox("setText", '');
}

function btn_OK_Click() {
    var _roleID = $("#txt_ID").val();
    var _roleName = $("#txt_RoleName").val();

    if (_roleName.trim() == "") {
        $.messager.alert("提示信息", "请输入角色名称！");
        return;
    }

    var mJsonObj = "{ID:'" + _roleID + "',角色名:'" + _roleName + "'}";

    $.ShowMask("请稍等……");
    if (operateType == "Add") {
        $.ajax(
        {
            url: "../WebServices/RoleRightService.asmx/AddRole",
            type: "GET",
            data: { 'roleJSONString': mJsonObj, 'loginIdentifer': window.parent.guid },
            dataType: "text",
            cache: false,
            success: function (responseText) {
                var data = eval("(" + $.xml2json(responseText) + ")");
                if (data.Result)//登录成功
                {
                    LoadTableData();
                    $.HideMask();
                    $('#dlgRole').dialog("close");
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
            url: "../WebServices/RoleRightService.asmx/ModifyRole",
            type: "POST",
            data: { 'roleJSONString': mJsonObj, 'loginIdentifer': window.parent.guid },
            dataType: "text",
            cache: false,
            success: function (responseText) {
                var data = eval("(" + $.xml2json(responseText) + ")");
                if (data.Result)//登录成功
                {
                    LoadTableData();
                    $.HideMask();
                    $('#dlgRole').dialog("close");
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