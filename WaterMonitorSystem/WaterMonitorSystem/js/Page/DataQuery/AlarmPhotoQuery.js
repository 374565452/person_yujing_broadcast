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

var dicColumnNames = {};

$(document).ready(function () {
    var defaultTheme = $.cookie("psbsTheme");
    if (defaultTheme != null && defaultTheme != "default") {
        var link = $(document).find('link:first');
        link.attr('href', '../App_Themes/easyui/themes/' + defaultTheme + '/easyui.css');
    }
    InitTimeControl();
    GetSystemInfo();
});

//初始化时间框
function InitTimeControl() {
    var e = new Date();
    $("#txt_StartTime").val(e.Format("yyyy-MM-dd 00:00"));
    $("#txt_EndTime").val(e.DateAdd('d', 1).Format("yyyy-MM-dd HH:mm"));
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
                LoadTree("divAreaTree", mnId, true, false);
                //LoadUserCombobox();   
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(errorThrown + "\r\n" + XMLHttpRequest.responseText);
        }
    });
}

//查询的按钮事件
function Btn_Query_Click() {
    var devIDs = "";
    var treeObj = $("#divAreaTree");
    var checkedNodes = treeObj.tree('getSelected');
    if (checkedNodes == null) {
        alert("请选择测点！");
        return;
    }
    if (checkedNodes.attributes.nodeType == "manage") {
        alert("请选择测点！");
        return;
    }
    //获取起始时间
    var startTime = $("#txt_StartTime").val();
    //获取结束时间
    var endTime = $("#txt_EndTime").val();
    //for (var i = 0; i < checkedNodes.length; i++) {
    //    var checkedNode = checkedNodes[i];
    //    if (checkedNode.attributes["nodeType"] == "manage") {
    //        continue;
    //    }
    //    if (devIDs != "") {
    //        devIDs += ",";
    //    }
    //    devIDs += checkedNode.attributes["did"];

    //}
    
    //数据量大，禁止起止时间跨年
    var str1 = startTime.replace(/-/g,"/");
    var str2=endTime.replace(/-/g,"/");
    var startDate = new Date(str1);
    var endDate=new Date(str2);
    
    if(endDate.getFullYear()-startDate.getFullYear()!=0)
    {
       alert("查询范围请选择在同一年");
        $(".mod_gallerylist .layout_default").remove();
       return;
    }
    devIDs = checkedNodes.attributes.did;
    LoadTableData(devIDs, startTime, endTime);
}

function LoadTableData(_devIds, _startTime, _endTime) {
    $.ajax(
        {
            url: "../WebServices/DataQueryService.asmx/AlarmPhotoTimeList",
            type: "GET",
            data: { deviceIDs: _devIds, startTime: _startTime, endTime: _endTime },
            dataType: "text",
            cache: false,
            success: function (responseText) {
                var data = eval("(" + $.xml2json(responseText) + ")");
                if (data.Result)//登录成功
                {
                    var htmlstr1 = "";
                    var htmlstr = "";
                    if (data.Record.length == 0) {
                        alert("该测点没有图片！");
                        return;
                    }
                    for (var i = 0; i < data.Record[0].length; i++) {
                        htmlstr += "<div class='layout_default'><p class='info'><span class='date'></span><span class='author'></span></p><div class='image_container'><a href='" + data.Record[0][i][1] + "' rel='lightbox[ostec]' title='" + data.Record[0][i][4] + "&nbsp;&nbsp;"+data.Record[0][i][5] + "'><img src='" + data.Record[0][i][1] + "' alt='" + data.Record[0][i][4] + "'/></a></div><div class='meta'><a href='javascript:void(0)'>" + data.Record[0][i][4] + "</a></div></div>"
                    }
                    $(".mod_gallerylist .layout_default").remove();
                    $(".mod_gallerylist").append(htmlstr);
                    Mediabox.scanPage = function () {
                        var links = $$("a").filter(function (el) {
                            return el.rel && el.rel.test(/^lightbox/i);
                        });
                        $$(links).mediabox({/* Put custom options here */ }, null, function (el) {
                            var rel0 = this.rel.replace(/[[]|]/gi, " ");
                            var relsize = rel0.split(" ");
                            return (this == el) || ((this.rel.length > 8) && el.rel.match(relsize[1]));
                        });
                    };
                    window.addEvent("domready", Mediabox.scanPage);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown + "\r\n" + XMLHttpRequest.responseText);
            }
        });
}

//function LoadDeviceCombobox() {
//    $.ajax(
//        {
//            url: "../WebServices/DeviceNodeService.asmx/GetDeviceNodeInfosByMnId",
//            type: "GET",
//            data: { 'managerId': mnId, "loginIdentifer": window.parent.guid, "isRecursive": true, "isExport": false },
//            dataType: "text",
//            cache: false,
//            success: function (responseText) {
//                var data = eval("(" + $.xml2json(responseText) + ")");
//                if (data.Result)//登录成功
//                {
//                    levelJson = data.UserNodes;
//                    var comboBoxDataLevel = [];

//                    for (i = 0; i < levelJson.length; i++) {
//                        var levelObj = {};
//                        levelObj["id"] = levelJson[i].ID;
//                        levelObj["text"] = levelJson[i].用户名;
//                        comboBoxDataLevel.push(levelObj);
//                    }
//                    $("#cbb_DevCombobox").combobox({
//                        data: comboBoxDataLevel
//                    });
//                }
//            },
//            error: function (XMLHttpRequest, textStatus, errorThrown) {
//                alert(errorThrown + "\r\n" + XMLHttpRequest.responseText);
//            }
//        });
//}
