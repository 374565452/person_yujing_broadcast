// JScript 文件

var map;
//带有经纬度的设备信息
var deviceInfo;
var deviceInfos={};
//用于存储实时数据
var deviceData=[];
//用于存储实时数据返回来的列名
var Columns={};
//  标注点数组
var baseData=[]; 
//地图xml数据
var mapDataInfo;
//用于存储选中的设备ID
var selDevIds=[];
var allDevIds = "";

var dicDeviceAppGroups = {};
var mnId = "";
var monitorInfoShow = true;
var deviceInfoLoaded = false;
var iframeLoaded = false;
$(document).ready(function(){
    var defaultTheme = $.cookie("psbsTheme");
    if(defaultTheme != null && defaultTheme != "default")
    {
        var link = $(document).find('link:first');
	    link.attr('href', '../App_Themes/easyui/themes/'+defaultTheme+'/easyui.css');
	}
    GetSystemInfo();
    GetDeviceAppGroup();
    InitBDMap();
    resizeDivTabs();
});

$(window).resize(function(){
    resizeDivTabs();
});

function ShowAndHideInfo() {
    if (monitorInfoShow) {
        $("#divTabContainer").animate({ left: ($(window).width() - 25) }, 'slow');
        document.getElementById("imgRealTimeData").src = "../Images/实时数据-1.png";
    }
    else {
        $("#divTabContainer").animate({ left: ($(window).width() - 405) }, 'slow');
        document.getElementById("imgRealTimeData").src = "../Images/实时数据.png";
    }
    monitorInfoShow = !monitorInfoShow
}

function resizeDivTabs() {
    if (monitorInfoShow) {
        $("#divTabContainer").css("left", ($(window).width() - $("#divTabContainer").width()) + "px");
    }
    else {
        $("#divTabContainer").css("left", ($(window).width() - 20) + "px");
    }
    $("#divTabContainer").css("top", ($(window).height() - $("#divTabContainer").height()) / 2 + "px");
}


function InitBDMap() {
    $.ajax(
    {
        url: "../WebServices/GlobalViewService.asmx/GetMapConfig",
        type: "GET",
        data: {},
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                window.searchClass = new SearchClass();
                mapDataInfo = data.MapConfig;
                var mapBaseConfig = data.MapConfig.基本设置;
                var centerPoint = mapBaseConfig.中心坐标.split("|");
                var initZoom = mapBaseConfig.初始缩放级别;
                var minZoom = mapBaseConfig.最小缩放级别;
                var maxZoom = mapBaseConfig.最大缩放级别;

                var centerPointLat_wgs = parseFloat(centerPoint[1]); //中心纬度GPS
                var centerPointLng_wgs = parseFloat(centerPoint[0]); //中心经度
                var centerpointGGStr = GPS.gcj_encrypt(centerPointLat_wgs, centerPointLng_wgs);
                var centerPointGGLat = centerpointGGStr.lat; //中心纬度Google
                var centerPointGGLng = centerpointGGStr.lon; //中心经度
                var centerpointBDStr = GPS.bd_encrypt(centerPointGGLat, centerPointGGLng);
                var centerPointBDLat = centerpointBDStr.lat; //中心纬度baidu
                var centerPointBDLng = centerpointBDStr.lon; //中心经度
                var centerPointBD = new BMap.Point(centerPointBDLng, centerPointBDLat);

                // 创建Map实例
                map = new BMap.Map("allmap", { 'minZoom': minZoom, 'maxZoom': maxZoom, mapType: BMAP_HYBRID_MAP });
                map.centerAndZoom(centerPointBD, initZoom);
                map.enableScrollWheelZoom();
                map.addControl(new BMap.NavigationControl());
                map.addControl(new BMap.MapTypeControl());

                /*
                var bdary = new BMap.Boundary();
                bdary.get("乌拉特前旗", function (rs) {       //获取行政区域
                    var count = rs.boundaries.length; //行政区域的点有多少个
                    for (var i = 0; i < count; i++) {
                        var ply = new BMap.Polyline(rs.boundaries[i], { strokeWeight: 4, strokeColor: "#ff0000" }); //建立多边形覆盖物
                        map.addOverlay(ply);  //添加覆盖物
                    }
                });
                */

                InitDeviceInfo();
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            map = new BMap.Map("allmap");            // 创建Map实例
            map.centerAndZoom(new BMap.Point(118.18, 39.65), 13);
            map.enableScrollWheelZoom();
            map.addControl(new BMap.NavigationControl());
            map.addControl(new BMap.MapTypeControl());
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });

}

//生成图例div
function createLegend() {
    var divLegend = document.createElement("div");
    divLegend.id = "divLegend";
    // Div的样式 请使用标准的CSS样式 例如：height:100px;width:100px;必须给定的值width height position:absolute;z-index:10;
    divLegend.style = "left:" + (parseInt(mapsize.width) - parseInt(legend.Width) - 2) + "px;top:2px;height:" + legend.Height + ";width:" + legend.Width +
        ";filter:alpha(opacity=" + legend.BackOpacity + ");opacity:" + legend.BackOpacity / 100 + ";background-color:" + legend.BackColor + ";border-color:" + legend.BorderColor
        + ";border:solid;border-width:1px;position:absolute;Z-index:10;";

    var tableHtml = '<table cellpadding="1" cellspacing="5" width="100%" height="100%">';
    LegendUnits = legend.LegendUnits;
    for (var i = 0; i < LegendUnits.length; i++) {
        str += '<tr><td style="text-align: center;">';
        str += '<img src="Images/' + LegendUnits[i].PicFileName + '" width="' + LegendUnits[i].Width + '" height="' + LegendUnits[i].Height + '" align="absMiddle" /></td>';
        str += '<td>';
        str += '<span style="color: ' + LegendUnits[i].LabelForeColor + '; background-color: ' + LegendUnits[i].LabelBackColor + '">' + LegendUnits[i].StatesKey + '</span>';
        str += '</td></tr>';
    }
    str += '</table>';
    divLegend.innerHTML = tableHtml;
    document.body.appendChild(divLegend);
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
                GetManageNode(mnId);
                GetDeviceNode(mnId);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function GetDeviceAppGroup() {
    $.ajax({
        url: "../WebServices/GlobalAppService.asmx/GetDeviceAppGroups",
        type: "GET",
        data: {},
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var deviceAppGroups = eval("(" + $.xml2json(responseText) + ")");
            if (deviceAppGroups.length == 0) {
                ShowPageInTab("实时监测", true);
            }
            else {
                for (var i = 0; i < deviceAppGroups.length; i++) {
                    dicDeviceAppGroups[deviceAppGroups[i].名称] = deviceAppGroups[i];
                    if (i == 0) {
                        ShowPageInTab(deviceAppGroups[i].名称, true);
                    }
                    else {
                        ShowPageInTab(deviceAppGroups[i].名称, false);
                    }
                }
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function ShowPageInTab(groupName, isSelected) {
    //根据标题获取标签页
    var tabPanel = $("#divTabs").tabs('getTab', groupName);
    //判断获取的标签页是否为空，为空则新建，否则直接选中
    if (tabPanel == null) {
        $("#divTabs").tabs('add', {
            title: groupName,
            selected: isSelected,
            //标签页不允许关闭
            closable: false
        });
        $("#divTabs").tabs('getTab', groupName).css("overflow", "hidden");
    }
    else {
        $("#divTabs").tabs('select', groupName);
    }
}

function TabOnSelect(title, index) {
    var tabPanel = $("#divTabs").tabs("getTab", index);
    if (tabPanel.html() == "") {
        var monitorUrl = typeof (dicDeviceAppGroups[title]) == "undefined" ? "BDMap_Monitor.html" : dicDeviceAppGroups[title].电子地图Url;
        var userStationParams = typeof (dicDeviceAppGroups[title]) == "undefined" ? "" : dicDeviceAppGroups[title].用户站参数;
        tabPanel.html("<iframe name='MonitorIFrame' style='width:100%;height:100%' frameborder='no' src='" + monitorUrl + "?usps=" + userStationParams + "' onload='MonitorIFrameLoaded();'></iframe>");
    }
}

function MonitorIFrameLoaded() {
    iframeLoaded = true;
    window.frames["MonitorIFrame"].GetMonitorDataFromParent();
}

//获取此登录用户所能看到的设备信息
function InitDeviceInfo() {
    $.ajax({
        url: "../WebServices/DeviceNodeService.asmx/GetDeviceNodeInfosByLoginUser",
        type: "GET",
        data: { "loginIdentifer": window.parent.guid },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result)//登录成功
            {
                deviceInfo = data.DeviceNodes;
                deviceInfos = {};
                allDevIds = "";
                for (var i = 0; i < deviceInfo.length; i++) {
                    deviceInfos[deviceInfo[i].ID] = deviceInfo[i];
                    if (i > 0) {
                        allDevIds += ",";
                    }
                    allDevIds += deviceInfo[i].ID;
                }

                deviceInfoLoaded = true;
                DrawPoint();
                setInterval(DrawPoint, 60000);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

///画点方法
function DrawPoint(searchText) {
    //循环deviceInfo中的所有测点
    var devIdStr = "";
    if (selDevIds == null || selDevIds.length == 0) {
        devIdStr = allDevIds;
    }
    else {
        for (var i = 0; i < selDevIds.length; i++) {
            if (i > 0) {
                devIdStr += ",";
            }
            devIdStr += selDevIds[i];
        }
    }
    $.ajax({
        url: "../WebServices/DeviceMonitorService.asmx/GetDeviceRealTimeDatas",
        type: "GET",
        data: { "loginIdentifer": window.parent.guid, "devIDs": devIdStr },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result)//登录成功
            {
                //======================向地图中添点开始======================//
                deviceData = data.DevDatas;
                Columns = data.Columns;
                baseData = [];
                for (var i = 0; i < deviceData.length; i++) {
                    //用于存储弹出窗的html脚本;
                    var htmlStr = "<table class='pstbl' cellpadding='0' cellspacing='0' style='font-size:10pt;width:500px'>";
                    var _devId = deviceData[i].操作.Value;
                    //var templateId = deviceInfos[_devId].用户站参数;
                    var templateId = 1;
                    //通讯状态默认值
                    var commState = "未知";
                    //设备状态默认值
                    var devState = "未知";
                    //用于存储地图上点的状态图片
                    var devImg = "大圈灰.gif";
                    for (var j = 0; j < mapDataInfo.用户模板.length; j++) {
                        if (mapDataInfo.用户模板[j].ID == templateId) {
                            var xmlStateValue = mapDataInfo.用户模板[j].状态量;
                            var valueName = mapDataInfo.用户模板[j].采集量.split(',');
                            var rowIndex = 2;
                            var tdIndex = 0;
                            var htmlNew = "";
                            for (var k = 0; k < valueName.length; k++) {
                                var currentValName = valueName[k];
                                if (deviceData[i][currentValName] != null) {
                                    var currentValVal = deviceData[i][currentValName].Value;
                                    if (k == 0) {
                                        var commStateImg = "中断.png";
                                        var devStateImg = "中断.png";
                                        commState = deviceData[i].通讯状态.Value;
                                        devState = deviceData[i].运行状态.Value;
                                        if (commState == "全部正常") {
                                            commStateImg = "正常.png";
                                        }
                                        /*
                                        if (devState == "全部正常") {
                                            devStateImg = "正常.png";
                                        }
                                        */
                                        htmlStr += "<tr><td class='alt' style='width:120px;text-align:left;'>通讯状态</td><td style='width:130px'><Img style='height:20px;widht:20px;' src='../images/" + commStateImg +
                                                 "'/></td><td class='alt' style='width:120px;text-align:left;'>运行状态</td><td style='width:130px'>" + devState + "</tr>";
                                    }
                                                                        
                                    htmlNew += "<td class='alt' style='width:120px;text-align:left;'>" + currentValName + "</td><td style='width:130px'>" + currentValVal + "</td>";
                                    tdIndex++;
                                    if (tdIndex % 2 == 0) {
                                        rowIndex++;
                                        if (rowIndex % 2 == 1) {
                                            htmlStr = htmlStr + "<tr>" + htmlNew + "</tr>";
                                        }
                                        else {
                                            htmlStr = htmlStr + "<tr>" + htmlNew + "</tr>";
                                        }
                                        htmlNew = "";
                                    }
                                    if (k == valueName.length - 1) {
                                        if (tdIndex % 2 != 0) {
                                            htmlNew += "<td style='width:120px'>&nbsp;</td><td style='width:130px'>&nbsp;</td>";
                                            tdIndex++;
                                        }
                                        rowIndex++;
                                        if (rowIndex % 2 == 1) {
                                            htmlStr = htmlStr + "<tr>" + htmlNew + "</tr>";
                                        }
                                        else {
                                            htmlStr = htmlStr + "<tr>" + htmlNew + "</tr>";
                                        }
                                    }

                                }//循环valueName结束
                                //根据配置的状态量来确定点的图片
                                /*
                                if (deviceData[i][xmlStateValue] != null) {
                                    var stateInfo = mapDataInfo.用户模板[j].状态;
                                    for (var state = 0; state < stateInfo.length; state++) {
                                        if (deviceData[i][xmlStateValue].Value == stateInfo[state].状态信息) {
                                            devImg = stateInfo[state].状态图片;
                                            break;
                                        }
                                    }
                                }
                                */
                                if (deviceData[i].运行状态.Value == "水泵工作") {
                                    devImg = "大圈绿.gif";
                                } else if (deviceData[i].运行状态.Value == "水泵停机") {
                                    devImg = "大圈红.gif";
                                } else {
                                    devImg = "大圈灰.gif";
                                }
                            }
                            //如果最后一个量名在取得的实时数据不存在，
                            if (tdIndex % 2 != 0) {
                                htmlNew += "<td>&nbsp;</td><td>&nbsp;</td>";
                                rowIndex++;
                                if (rowIndex % 2 == 1) {
                                    htmlStr = htmlStr + "<tr class='alt'>" + htmlNew + "</tr>";
                                }
                                else {
                                    htmlStr = htmlStr + "<tr>" + htmlNew + "</tr>";
                                }
                            }
                            break;
                        }
                    }//循环mapDataInfo结束
                    htmlStr += "</table>";
                    //组织JSon数据存入baseData中（格式：var BASEDATA = [{title:"aa",content:"bb",point:"经度|纬度",isOpen:0,icon:{图片路径或图片大小等信息}},{}]）
                    var pointObj = {};
                    pointObj['title'] = deviceInfos[_devId].名称;
                    pointObj['iwTitle'] = deviceInfos[_devId].管理名称 + "(" + deviceInfos[_devId].名称 + ")";
                    pointObj['commState'] = deviceData[i].通讯状态.Value;
                    pointObj['devState'] = deviceData[i].运行状态.Value;
                    pointObj['ID'] = _devId;
                    //pointObj['TemplateID']=deviceInfos[_devId].模板信息ID;
                    pointObj['TemplateID'] = 1;
                    pointObj['content'] = htmlStr;
                    pointObj['point'] = deviceInfos[_devId].LON + "|" + deviceInfos[_devId].LAT;
                    pointObj['isOpen'] = 0;
                    pointObj['icon'] = "../Images/" + devImg;
                    baseData.push(pointObj);
                }
                //添点
                searchClass.setData(baseData);
                addMarker(baseData);//向地图中添加marker
                //======================向地图中添点结束======================//

                if (searchText) {
                    search(searchText);
                    window.frames["MonitorIFrame"].GetMonitorDataFromParent(true);
                }
                else {
                    if (iframeLoaded) {
                        window.frames["MonitorIFrame"].GetMonitorDataFromParent(false);
                    }
                }
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

window.search = function (_value) {
    var s_v, t_v, d_v = _value;
    searchClass.trim(t_v) == "" && (t_v = "single");
    var dd;
    if (searchClass.trim(d_v) == "") {
        dd = baseData;
    }
    else {
        dd = searchClass.search({ k: "title", d: d_v, t: 'more', s: '' });
    }
    addMarker(dd);//向地图中添加marker
}

//创建marker
window.addMarker = function (data) {
    map.clearOverlays();
    /*
    var bdary = new BMap.Boundary();
    bdary.get("乌拉特前旗", function (rs) {       //获取行政区域
        var count = rs.boundaries.length; //行政区域的点有多少个
        for (var i = 0; i < count; i++) {
            var ply = new BMap.Polyline(rs.boundaries[i], { strokeWeight: 4, strokeColor: "#ff0000" }); //建立多边形覆盖物
            map.addOverlay(ply);  //添加覆盖物
        }
    });
    */
    for (var i = 0; i < data.length; i++) {
        var json = data[i];
        var p0 = json.point.split("|")[0];
        var p1 = json.point.split("|")[1];

        var PointLat_wgs = parseFloat(p1); //纬度GPS
        var PointLng_wgs = parseFloat(p0); //经度
        var pointGGStr = GPS.gcj_encrypt(PointLat_wgs, PointLng_wgs);
        var PointGGLat = pointGGStr.lat; //纬度Google
        var PointGGLng = pointGGStr.lon; //经度
        var pointBDStr = GPS.bd_encrypt(PointGGLat, PointGGLng);
        var PointBDLat = pointBDStr.lat; //纬度baidu
        var PointBDLng = pointBDStr.lon; //经度
        var PointBD = new BMap.Point(PointBDLng, PointBDLat);

        var point = PointBD;
        //        var iconImg = createIcon(json.icon);
        var iconImg = new BMap.Icon(json.icon, new BMap.Size(15, 15));
        iconImg.setImageSize(new BMap.Size(15, 15))
        var marker = new BMap.Marker(point, { icon: iconImg });
        var iw = createInfoWindow(i);
        var nameLength = json.title.length;
        //        var py=json.title.length*2;
        document.getElementById("titleDiv").innerHTML = json.title;
        var py = $("#titleDiv").width();
        var label = new BMap.Label(json.title, { "offset": new BMap.Size(-(py / 2 - 9), -20) });
        marker.setLabel(label);
        map.addOverlay(marker);
        label.setStyle({
            borderColor: "#808080",
            color: "#333",
            cursor: "pointer"
        });

        (function () {
            var _json = json;
            var _iw = createInfoWindow(_json);
            var _marker = marker;
            _marker.addEventListener("click", function () {
                realDevId = _json.ID;
                //alert(realDevId);
                var s = _iw.getContent();
                //s += "<div>开泵，关泵，历史数据，报警数据</div>";
                var DeviceInfo = [];
                $.ajax({
                    url: "../WebServices/DeviceMonitorService.asmx/GetDeviceInfoForWell",
                    data: { "loginIdentifer": window.parent.guid, "devID": realDevId },
                    type: "Get",
                    dataType: "text",
                    async: false,
                    cache: false,
                    success: function (responseText) {
                        var data = eval("(" + $.xml2json(responseText) + ")");
                        DeviceInfo = data.DeviceInfo;

                        var trbtn = "";
                        var count = 0;
                        var iskb = 0;
                        var isgb = 0;
                        if (DeviceInfo.SupportControl) {
                            for (var i = 0; i < DeviceInfo.ControlNames.length; i++) {
                                if (DeviceInfo.ControlNames[i] == "开泵") {
                                    iskb = 1;
                                    continue;
                                }
                                if (DeviceInfo.ControlNames[i] == "关泵") {
                                    isgb = 1;
                                    continue;
                                }
                                count++;
                                if (count % 5 == 1)
                                { trbtn += "<div style=\"text-align:center;\">"; }
                                trbtn += "<button onclick='ControlDevice(\"" + DeviceInfo.ControlNames[i] + "\"," + realDevId + ")' class='psbutton'>" + DeviceInfo.ControlNames[i] + "</button>";
                                if (count % 5 == 0)
                                { trbtn += "</div>"; }
                            }
                        }
                        if (DeviceInfo.SupportParam) {
                            count++;
                            if (count % 5 == 1)
                            { trbtn += "<div style=\"text-align:center;\">"; }
                            trbtn += "<button onclick='OpenSetParam(" + realDevId + ",\"" + json.iwTitle + "\")' class='psbutton'>参数设置</button>";
                            if (count % 5 == 0)
                            { trbtn += "</div>"; }
                        }

                        count++;
                        if (count % 5 == 1)
                        { trbtn += "<div style=\"text-align:center;\">"; }
                        trbtn += "<button onclick=\"LinkAlarmData(" + realDevId + ")\" class='psbutton' />报警数据</button>";
                        if (count % 5 == 0)
                        { trbtn += "</div>"; }

                        count++;
                        if (count % 5 == 1)
                        { trbtn += "<div style=\"text-align:center;\">"; }
                        trbtn += "<button onclick=\"LinkEventData(" + realDevId + ")\" class='psbutton' />事件数据</button>";
                        if (count % 5 == 0)
                        { trbtn += "</div>"; }

                        if (DeviceInfo.SupportWaterView) {
                            count++;
                            if (count % 5 == 1)
                            { trbtn += "<div style=\"text-align:center;\">"; }
                            trbtn += "<button onclick='OpenSetWater(" + realDevId + ",\"" + json.iwTitle + "\")' class='psbutton'>水文查询</button>";
                            if (count % 5 == 0)
                            { trbtn += "</div>"; }
                            count++;
                            if (count % 5 == 1)
                            { trbtn += "<div style=\"text-align:center;\">"; }
                            trbtn += "<button onclick='OpenSetWaterParam(" + realDevId + ",\"" + json.iwTitle + "\")' class='psbutton'>水文参数</button>";
                            if (count % 5 == 0)
                            { trbtn += "</div>"; }
                        }

                        if (count % 5 != 0)
                        { trbtn += "</div>"; }

                        count = 0;
                        if (iskb == 1)
                        {
                            count++;
                            if (count % 5 == 1)
                            { trbtn += "<div style=\"text-align:center;\">"; }
                            trbtn += "<button onclick='ControlDevice(\"" + "开泵" + "\"," + realDevId + ")' class='psbutton2'>" + "紧急开泵" + "</button>";
                            if (count % 5 == 0)
                            { trbtn += "</div>"; }

                        }

                        if (isgb == 1) {
                            count++;
                            if (count % 5 == 1)
                            { trbtn += "<div style=\"text-align:center;\">"; }
                            trbtn += "<button onclick='ControlDevice(\"" + "关泵" + "\"," + realDevId + ")' class='psbutton2'>" + "紧急关泵" + "</button>";
                            if (count % 5 == 0)
                            { trbtn += "</div>"; }
                        }

                        count++;
                        if (count % 5 == 1)
                        { trbtn += "<div style=\"text-align:center;\">"; }
                        trbtn += "<button onclick='OpenSendFile(\"" + json.iwTitle + "\")' class='psbutton'>文件语音</button>";
                        if (count % 5 == 0)
                        { trbtn += "</div>"; }                        
                        
                        if (count % 5 != 0)
                        { trbtn += "</div>"; }

                        if (s.indexOf("psbutton") == -1)
                            s += trbtn;
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {

                    }
                });
                _iw.setContent(s);
                this.openInfoWindow(_iw);
            });

            _iw.addEventListener("open", function () {
                _marker.getLabel().hide();
            });

            _iw.addEventListener("close", function () {
                _marker.getLabel().show();
            });

            label.addEventListener("click", function () {
                _marker.openInfoWindow(_iw);
            });
            /*
            if (!!json.isOpen) {
                label.hide();
                _marker.openInfoWindow(_iw);
            }
            */
            _marker.addEventListener("dblclick", function () {
                pointDoubleClick(_json.TemplateID, _json.ID);
            });
        })()
    }
}
var realDevId = "";
//创建InfoWindow
function createInfoWindow(json) {
    var iw = new BMap.InfoWindow("<b class='iw_poi_title' title='" + json.iwTitle + "详细信息'><span style='color:blue;font-size:10pt;'>" + json.iwTitle + "</span><span style='font-size:10pt;'>详细信息</span></b><div class='iw_poi_content' style='padding:5px;'>" + json.content + "</div>", { enableMessage: false });
    return iw;
}

function SearchClass(data) {
    this.datas = data;
}

SearchClass.prototype.search = function (rule) {
    if (this.datas == null) { alert("数据不存在!"); return false; }
    if (this.trim(rule) == "" || this.trim(rule.d) == "" || this.trim(rule.k) == "" || this.trim(rule.t) == "") { alert("请指定要搜索内容!"); return false; }
    var reval = [];
    var datas = this.datas;
    var len = datas.length;
    var me = this;
    var ruleReg = new RegExp(this.trim(rule.d));
    var hasOpen = false;

    var addData = function (data, isOpen) {
        // 第一条数据打开信息窗口
        if (isOpen && !hasOpen) {
            hasOpen = true;
            data.isOpen = 1;
        } else {
            data.isOpen = 0;
        }
        reval.push(data);
    }
    var getData = function (data, key) {
        var ks = me.trim(key).split(/\./);
        var i = null, s = "data";
        if (ks.length == 0) {
            return data;
        } else {
            for (var i = 0; i < ks.length; i++) {
                s += '["' + ks[i] + '"]';
            }
            return eval(s);
        }
    }
    for (var cnt = 0; cnt < len; cnt++) {
        var data = datas[cnt];

        //判断实时层中是否勾选了在线、不在线的复选框
        var devStateNormalChecked = window.frames["MonitorIFrame"].document.getElementById("devStateNormal").checked;
        var devStateAlarmChecked = window.frames["MonitorIFrame"].document.getElementById("devStateAlarm").checked;

        if (devStateNormalChecked && !devStateAlarmChecked) {
            if (data.commState != "全部正常" || data.devState != "全部正常") {
                continue;
            }
        }
        else if (!devStateNormalChecked && devStateAlarmChecked) {
            if (data.commState == "全部正常" && data.devState == "全部正常") {
                continue;
            }
        }

        var d = getData(data, rule.k);
        if (rule.t == "single" && rule.d == d) {
            addData(data, true);
        } else if (rule.t != "single" && ruleReg.test(d)) {
            addData(data, true);
        } else if (rule.s == "all") {
            addData(data, false);
        }
    }
    return reval;
}
SearchClass.prototype.setData = function (data) {
    this.datas = data;
}
SearchClass.prototype.trim = function (str) {
    if (str == null) { str = ""; } else { str = str.toString(); }
    return str.replace(/(^[\s\t\xa0\u3000]+)|([\u3000\xa0\s\t]+$)/g, "");
}

function SetCenter(lon, lat) {
    map.setCenter(new BMap.Point(lon, lat));
}

function SetZoom(zoom) {
    map.setZoom(zoom);
}

function CenterAndZoom(lon, lat, zoom) {
    var PointLat_wgs = parseFloat(lat); //纬度GPS
    var PointLng_wgs = parseFloat(lon); //经度
    var pointGGStr = GPS.gcj_encrypt(PointLat_wgs, PointLng_wgs);
    var PointGGLat = pointGGStr.lat; //纬度Google
    var PointGGLng = pointGGStr.lon; //经度
    var pointBDStr = GPS.bd_encrypt(PointGGLat, PointGGLng);
    var PointBDLat = pointBDStr.lat; //纬度baidu
    var PointBDLng = pointBDStr.lon; //经度
    var PointBD = new BMap.Point(PointBDLng, PointBDLat);

    map.centerAndZoom(PointBD, zoom);
}


//详细数据的设备ID
var realDevId = "";
function openControl(marker, json) {
    realDevId = json.ID;
    var DeviceInfo = [];
    $.ajax({
        url: "../WebServices/DeviceMonitorService.asmx/GetDeviceInfoForWell",
        data: { "loginIdentifer": window.parent.guid, "devID": realDevId },
        type: "Get",
        dataType: "text",
        async: false,
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            DeviceInfo = data.DeviceInfo;

            var trbtn = "";
            var count = 0;
            if (DeviceInfo.SupportControl) {
                for (var i = 0; i < DeviceInfo.ControlNames.length; i++) {
                    count++;
                    if (count % 2 == 1)
                    { trbtn += "<div>"; }
                    trbtn += "<button onclick='ControlDevice(\"" + DeviceInfo.ControlNames[i] + "\"," + realDevId + ")' class='psbutton'>" + DeviceInfo.ControlNames[i] + "</button>";
                    if (count % 2 == 0)
                    { trbtn += "</div>"; }
                }
            }
            if (DeviceInfo.SupportParam) {
                count++;
                if (count % 2 == 1)
                { trbtn += "<div>"; }
                trbtn += "<button onclick='OpenSetParam(" + realDevId + ",\"" + json.iwTitle + "\")' class='psbutton'>参数设置</button>";
                if (count % 2 == 0)
                { trbtn += "</div>"; }
            }
            if (DeviceInfo.SupportWaterView) {
                count++;
                if (count % 2 == 1)
                { trbtn += "<div>"; }
                trbtn += "<button onclick='OpenSetWater(" + realDevId + ",\"" + json.iwTitle + "\")' class='psbutton'>水位查询</button>";
                if (count % 2 == 0)
                { trbtn += "</div>"; }
                count++;
                if (count % 2 == 1)
                { trbtn += "<div>"; }
                trbtn += "<button onclick='OpenSetWaterParam(" + realDevId + ",\"" + json.iwTitle + "\")' class='psbutton'>水文参数</button>";
                if (count % 2 == 0)
                { trbtn += "</div>"; }
            }

            if (count % 2 == 1)
            { trbtn += "</div>"; }

            var iw = new BMap.InfoWindow("<b class='iw_poi_title' title='" + json.iwTitle + "控制'><span style='color:blue;font-size:10pt;'>" + json.iwTitle + "</span><span style='font-size:10pt;'>控制</span></b><div class='iw_poi_content' style='padding:5px;'>" +
                trbtn +
                "<div><button onclick=\"LinkAlarmData(" + realDevId + ")\" class='psbutton' />报警数据</button><button onclick=\"LinkEventData(" + realDevId + ")\" class='psbutton' />事件数据</button></div>" +
                "</div>", { enableMessage: false });
            marker.openInfoWindow(iw);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            var iw = new BMap.InfoWindow("<b class='iw_poi_title' title='" + json.iwTitle + "控制'><span style='color:blue;font-size:10pt;'>" + json.iwTitle + "</span><span style='font-size:10pt;'>控制</span></b><div class='iw_poi_content' style='padding:5px;'>" + realDevId + "</div>", { enableMessage: false });;
            marker.openInfoWindow(iw);
        }
    });   
}

function LinkEventData(deviceID) {
    window.parent.showM("事件查询", "?devid=" + deviceID);
}

function LinkAlarmData(deviceID) {
    window.parent.showM("报警查询", "?devid=" + deviceID);
}

function OpenSendFile(diviceName) {
    $('#dlgSendFile').dialog({ closed: false, title: (diviceName + "文件语音") });
}

function OpenSetParam(deviceID, diviceName) {
    $('#dlgParam').dialog({ closed: false, title: (diviceName + "参数设置") });
    $("#txt_Time").val("");
    $("#txt_Exploit").val("");
}

function OpenSetWater(deviceID, diviceName) {
    $('#dlgParamWater').dialog({ closed: false, title: (diviceName + "水文查询") });
}

function OpenSetWaterParam(deviceID, diviceName) {
    $('#dlgParamWater2').dialog({ closed: false, title: (diviceName + "水文参数") });
}

function ReadParam(paramName, paramTextId) {
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

function SetParam(paramName, paramTextId) {
    var paramTextObj = $("#" + paramTextId);
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
    if (ctrlName == "设置地址") {
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

function ControlDevice3(ctrlName) {
    if (ctrlName != "时段数据查询" && ctrlName != "实时数据查询") {
        $.messager.alert("提示信息", "非法操作！" + ctrlName);
        return;
    }

    var d1 = $("#Text1").val();
    var d2 = $("#Text2").val();
    var d3 = $("#Select1").combobox("getValue");
    var d4 = $("#Text3").val();
    var d5 = $("#Text4").val();
    var d6 = $("#Text5").val();

    //alert(d1 + " | " + d2 + " | " + d3);

    $.ShowMask("正在" + ctrlName + "，请稍等……");
    $.ajax({
        type: "GET",
        url: "../WebServices/DeviceMonitorService.asmx/ControlDevice3",
        data: { 'loginIdentifer': window.parent.guid, 'ctrlName': ctrlName, 'devID': realDevId, 'BeginTime': d1, 'EndTime': d2, 'Identifier': d3, 'step1': d4, 'step2': d5, 'step3': d6 },
        contentType: "content",
        dataType: "text",
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            $.HideMask();
            if (data.Result) {
                $.messager.alert("提示信息", ctrlName + "成功！");
                $("#waterInfo").val(data.Info)
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
