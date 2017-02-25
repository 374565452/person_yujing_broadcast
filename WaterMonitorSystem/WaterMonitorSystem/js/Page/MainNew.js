// JScript 文件
//用于存储主菜单对应的子菜单的html
var menuFObj = {};
//用于存储菜单对象
var menuData;
//用于存储子菜单对应的父菜单的索引
var menuCObj = {};

//登录标识
var guid = $.cookie("psbsLogin");
var mnId = "";
var alarmnameflag = false;

var divContainerWidth;
$(document).ready(function () {
    //alert("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
    if (window.name == "") {
        try {
            //使浏览器窗口最大化
            window.moveTo(0, 0);
            window.resizeTo(screen.availWidth, screen.availHeight);
        } catch (err) { }
    }
    //加载皮肤
    LoadThemes();
    //初始化全局报警容位置
    $("#divAlarmContainer").css("top", ($(window).height() - 320) + "px");
    //调整布局大小
    resizeDivContainer();
    //获取系统信息
    GetSystemInfo();
    //加载全局报警模块
    GlobalAlarmLoad();
    //获取主菜单
    GetMainMenu();
});

function show(obj) {
    $('#divPos').html("首页 >> " + $.trim($(obj).parent().parent().parent().find(".divMenuTitle").text()) + " >> " + $.trim($(obj).text()));
}

$(window).resize(function () {
    resizeDivContainer();
});

//调整布局大小和全局报警窗口位置
function resizeDivContainer() {
    var divObj = $("#divContainer");
    var windowObj = $(window);
    //获取布局容器宽高
    var dWidth = divObj.width();
    var dHeight = divObj.height();
    //获取浏览器窗口宽高
    var wWidth = windowObj.width();
    var wHeight = windowObj.height();
    //判断是否有滚动条
    var hasScoll = false;
    if (dWidth > wWidth || dHeight > wHeight) {
        hasScoll = true;
    }
    //保证布局窗口最小为800 x 600
    wWidth = wWidth < 800 ? 800 : wWidth;
    wHeight = wHeight < 600 ? 600 : wHeight;
    //调整布局容器宽高
    $("#divContainer").layout('resize', {
        width: wWidth + "px",
        height: wHeight + "px"
    });
    divContainerWidth = wWidth;

    $("#topMenu").width(wWidth - $("#topLogo").width() - $("#topUser").width() - 5);

    $("#rightFrame").height($("#divCenter").height() - $("#divPlace").height());
    $("#rightFrame").width($("#divCenter").width());

    //调整全局报警窗口位置
    $("#divAlarmContainer").css("top", (wHeight - 320) + "px");
    //如果有滚动条的话，过50ms重新调整位位置
    if (hasScoll) {
        window.setTimeout(resizeDivContainer, 50);
    }
}

var isPollingSystemInfo = false;
//30秒钟检查一次系统信息
var pollingSystemInfo = window.setInterval("GetSystemInfo()", 30000);
//从服务器取得系统运行状态信息
function GetSystemInfo() {
    if (isPollingSystemInfo) {
        return;
    }
    isPollingSystemInfo = true;
    $.ajax(
    {
        url: "WebServices/SystemService.asmx/GetSystemStateInfo",
        type: "GET",
        data: { "loginIdentifer": guid },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                $("#td_CurrentUser").text("当前用户：" + data.SysStateInfo.当前登录操作员名称);
                $("#td_State").text("通讯服务器：" + data.SysStateInfo.通讯服务器连接状态);
                window.document.title = data.SysStateInfo.系统名称;
                mnId = data.SysStateInfo.当前登录操作员管理ID;
                roleId = data.SysStateInfo.当前登录操作员角色ID;
                var unitname = data.SysStateInfo.监测点管理名称;
                var devname = data.SysStateInfo.监测点级别名称;
                if (!alarmnameflag) {
                    $("#unitName").html(unitname + "名称");
                    $("#devName").html(devname + "名称");
                    alarmnameflag = true;
                }
            }
            else {
                if (pollingSystemInfo != null) {
                    window.clearInterval(pollingSystemInfo);
                }
                if (pollingGlobalAlarm != null) {
                    window.clearInterval(pollingGlobalAlarm);
                }
                if (data.Message == "未登录" || data.Message == "登录超时") {
                    $.messager.alert("提示信息", "您未登录过或登录超时，请先登录！", "info", function () {
                        window.location.href = "default.html";
                    });
                }
                else if (data.Message.Contains == "用户不存在") {
                    $.messager.alert("提示信息", "当前登录用户不存在，可能已被管理员删除！", "info", function () {
                        window.location.href = "default.html";
                    });
                }
                else {
                    $.messager.alert("提示信息", data.Message, "info", function () {
                        window.location.href = "default.html";
                    });
                }
            }
            isPollingSystemInfo = false;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            isPollingSystemInfo = false;
            //alert(errorThrown + "\r\n" + XMLHttpRequest.responseText);
        }
    });
}

var oldMenuTitle = "";
//获取当前登录用户主菜单
function GetMainMenu() {
    $("#divMenu").html("");
    $.ajax(
    {
        url: "WebServices/MenuService.asmx/GetMainMenu",
        type: "GET",
        data: { "loginIdentifer": guid },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            menuData = eval("(" + $.xml2json(responseText) + ")");
            var menu = "";
            if (menuData != null && menuData.length > 0) {
                for (var i = 0; i < menuData.length; i++) {
                    var menuson = "";
                    if (menuData[i].子菜单 != null && menuData[i].子菜单.length > 0) {
                        for (var j = 0; j < menuData[i].子菜单.length; j++) {
                            menuson += "<div><a href=\"" + menuData[i].子菜单[j].入口URL + "\" target=\"rightFrame\">" + menuData[i].子菜单[j].菜单名称 + "</a></div>";
                        }
                        menu += "<div class=\"divMenuList\"><div class=\"divMenuTitle\">" + menuData[i].菜单名称 + "</div><div class=\"divMenuSon\">" + menuson + "</div></div>";
                    }
                }
                $("#divMenu").html(menu);
            }

            var menutitle = $('.divMenuTitle');
            menutitle.click(function () {
                var $div = $(this).next('.divMenuSon');
                $('.divMenuSon').slideUp();
                if ($div.is(':visible')) {
                    $(this).next('.divMenuSon').slideUp();
                } else {
                    $(this).next('.divMenuSon').slideDown();
                }
            });

            if (menutitle.length > 0) { menutitle.eq(0).click(); }

            var as = $(".divMenuSon div a");
            as.hover(function () {
                $(this).css("color", "#fff");
            }, function () {
                $(this).css("color", "#000");
            });

            as.click(function () {
                show(this);
            });

            if (as.length > 0) {
                as.eq(0).click();
                $("#rightFrame").attr("src", as.eq(0).attr("href"));
            }

            var topa = $("#topMenu .item a");
            for (var i = 0; i < topa.length; i++) {
                var txt = $.trim(topa.eq(i).find(".mtitle").text());
                for (var j = 0; j < as.length; j++) {
                    if (as.eq(j).text() == txt) {
                        topa.eq(i).parent().show();
                        break;
                    }
                }
            }

            $("#topMenu .item a").click(function () {
                $("#topMenu .item a.selected").removeClass("selected")
                $(this).addClass("selected");
                var txt = $.trim($(this).find(".mtitle").text());

                for (var j = 0; j < as.length; j++) {
                    if (as.eq(j).text() == txt) {
                        as.eq(j).click();
                        $("#rightFrame").attr("src", as.eq(j).attr("href"));
                        /*
                        var $div = as.eq(j).parent().parent();
                        var MenuTitle = $.trim($div.parent().find(".divMenuTitle").text());
                        if (MenuTitle != oldMenuTitle) {
                            oldMenuTitle = MenuTitle;
                            $('.divMenuSon').slideUp();
                        }
                        $div.slideDown();
                        */
                        break;
                    }
                }
            })
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //alert(errorThrown + "\r\n" + XMLHttpRequest.responseText);
        }
    });
}

function showM(v,p) {
    var as = $(".divMenuSon div a");
    for (var j = 0; j < as.length; j++) {
        if (as.eq(j).text() == v) {
            as.eq(j).click();
            $("#rightFrame").attr("src", as.eq(j).attr("href").split("?")[0] + p);
            break;
        }
    }
}


var waitting = 1;
var secondLeft = waitting;
var timer;
var number;
var oldobj;
function getObject(objectId)//获取id的函数               
{
    if (document.getElementById && document.getElementById(objectId)) {
        // W3C DOM              
        return document.getElementById(objectId);
    } else if (document.all && document.all(objectId)) {
        // MSIE 4 DOM              
        return document.all(objectId);
    } else if (document.layers && document.layers[objectId]) {
        // NN 4 DOM.. note: this won't find nested layers              
        return document.layers[objectId];
    } else {
        return false;
    }
}

function del() {

    return false;
}


function showMyWindow(title, href, width, height, modal, minimizable, maximizable) {
    $('#myWindow').window({
        title: title,
        width: width === undefined ? 600 : width,
        height: height === undefined ? 450 : height,
        content: '<iframe scrolling="yes" frameborder="0"  src="' + href + '" style="width:100%;height:98%;"></iframe>',
        //        href: href === undefined ? null : href, 
        modal: modal === undefined ? true : modal,
        minimizable: minimizable === undefined ? false : minimizable,
        maximizable: maximizable === undefined ? false : maximizable,
        shadow: false,
        cache: false,
        closed: false,
        collapsible: false,
        resizable: false,
        loadingMessage: '正在加载数据，请稍等片刻......'
    });
}

//退出系统
function LogoutSystem() {
    if (!confirm("确定退出系统吗？"))
    {
        return false;
    }
    $.ShowMask("正在退出，请稍等……");
    $.ajax(
    {
        url: "WebServices/UserAdminService.asmx/Logout",
        type: "GET",
        data: { "loginIdentifer": window.parent.guid },
        success: function (data) {
            if (typeof (url) == "undefined" || url == "") {
                $.cookie("psbsLogin", data.Guid, { expires: -1, path: '/' });
                window.location.href = "default.html";
            }
            else {
                window.location.href = url;
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function LoadThemes() {
    var themes = [
			{ value: 'default', text: '默认' },
			{ value: 'metro-blue', text: 'Win8蓝' },
			{ value: 'metro-gray', text: 'Win8灰' },
			{ value: 'metro-green', text: 'Win8绿' },
			{ value: 'metro-orange', text: 'Win8橙' },
			{ value: 'metro-red', text: 'Win8红' },
			{ value: 'ui-sunny', text: '阳光' }
    ];
    $('#cb-theme').combobox({
        value: 'default',
        data: themes,
        editable: false,
        panelHeight: 'auto',
        onChange: onChangeTheme
    });
    var defaultTheme = $.cookie("psbsTheme");
    if (defaultTheme != null && defaultTheme != "default") {
        $("#cb-theme").combobox('setValue', defaultTheme);
    }
}

function onChangeTheme(theme) {
    var link = $(document).find('link:first');
    link.attr('href', 'App_Themes/easyui/themes/' + theme + '/easyui.css');

    var frameObjs = window.frames;
    for (var i = 0; i < frameObjs.length; i++) {
        var childLink = $(frameObjs[i].document).find('link:first');
        childLink.attr('href', '../App_Themes/easyui/themes/' + theme + '/easyui.css');

        var grandchildFrames = frameObjs[i].frames;
        for (var j = 0; j < grandchildFrames.length; j++) {
            $(grandchildFrames[j].document).find('link:first').attr('href', '../App_Themes/easyui/themes/' + theme + '/easyui.css');
        }
    }
    $.cookie("psbsTheme", theme, { expires: 7, path: '/' });
}



function ShowModifyPwd() {
    $('#dlgModifyPwd').dialog({ closed: false });
}

// JScript 文件
function ClearPwd() {
    $("#txt_OldPassword").val("");
    $("#txt_NewPassword").val("");
    $("#txt_ConfirmPassword").val("");
}

function ModifyPwd() {
    var oldPwd = $("#txt_OldPassword").val();
    var newPwd = $("#txt_NewPassword").val();
    var confirmPwd = $("#txt_ConfirmPassword").val();
    if (oldPwd == newPwd) {
        $.messager.alert("提示信息", "旧密码与新密码一致");
        $("#txt_NewPassword").focus();
        return;
    }
    if (newPwd != confirmPwd) {
        $.messager.alert("提示信息", "新密码与确认密码不一致");
        $("#txt_ConfirmPassword").focus();
        return;
    }
    $.ShowMask("正在修改密码，请稍等……");
    $.ajax(
    {
        url: "WebServices/UserAdminService.asmx/ModifyPassWord",
        type: "POST",
        data: { "loginIdentifer": guid, "id": "", "oldPwd": oldPwd, "newPwd": newPwd },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                $.HideMask();
                $.messager.alert("提示信息", "密码修改成功，需重新登录", "info", function () {
                    LogoutSystem();
                });
            }
            else {
                $.messager.alert("提示信息", data.Message);
                $.HideMask();
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
            $.HideMask();
        }
    });
}