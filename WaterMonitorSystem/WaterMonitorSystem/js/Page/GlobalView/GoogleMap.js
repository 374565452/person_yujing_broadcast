var map;

var dicDeviceAppGroups = {};
var mnId = "";
var monitorInfoShow = true;
var deviceInfoLoaded = false;
var iframeLoaded = false;
//地图xml数据
var mapDataInfo;
//用于存储实时数据
var deviceData = [];
//用于存储实时数据返回来的列名
var Columns = {};
//  标注点数组
var baseData = [];
//带有经纬度的设备信息
var deviceInfo;
var deviceInfos = [];
//用于存储选中的设备ID
var selDevIds = [];
var allDevIds = "";
//是否画图
var deviceInfoLoaded = false;

$(document).ready(function () {
    resizeDivTabs();
    GetSystemInfo();

    var str;
    var date = new Date();
    if (date.getMinutes() < 15)
        date = date.DateAdd('h', -10);
    else if (date.getMinutes() < 45)
        date = date.DateAdd('h', -9);
    else if (date.getMinutes() >= 45)
        date = date.DateAdd('h', -9);

    var str = date.Format("yyyyMMddHH4500000");
   // alert(str);
    $("#divWXYTInfo").html("<img src=\"http://i.weather.com.cn/i/product/pic/m/sevp_nsmc_wxcl_asc_e99_achn_lno_py_" + str + ".jpg\" style=\"width:100%;\" alt=\"" + str + "\" />");

    ShowAndHideInfo();
});

$(window).resize(function () {
    resizeDivTabs();
});

function resizeDivTabs() {
    if (monitorInfoShow) {
        $("#divTabContainer").css("left", ($(window).width() - $("#divTabContainer").width()) + "px");
    }
    else {
        $("#divTabContainer").css("left", ($(window).width() - 20) + "px");
    }
    $("#divTabContainer").css("top", ($(window).height() - $("#divTabContainer").height()) / 2 + "px");
}

function ShowAndHideInfo() {
    if (monitorInfoShow) {
        $("#divTabContainer").animate({ left: ($(window).width() - 25) }, 'slow');
        $("#imgRealTimeData").attr("src", "../Images/实时数据-1.png");
    }
    else {
        $("#divTabContainer").animate({ left: ($(window).width() - 405) }, 'slow');
        $("#imgRealTimeData").attr("src", "../Images/实时数据.png");
    }
    monitorInfoShow = !monitorInfoShow
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

                InitMap();

                ShowPageInTab("设备监控", true);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function InitMap() {
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

                mapDataInfo = data.MapConfig;
                var mapBaseConfig = data.MapConfig.基本设置;
                var centerPoint = mapBaseConfig.中心坐标.split("|");
                var initZoom = mapBaseConfig.初始缩放级别;
                var minZoom = mapBaseConfig.最小缩放级别;
                var maxZoom = mapBaseConfig.最大缩放级别;

                var centerpointGGStr = GPS.gcj_encrypt(parseFloat(centerPoint[1]), parseFloat(centerPoint[0]));

                var centerPointGGLat = centerpointGGStr.lat; //中心纬度Google
                var centerPointGGLng = centerpointGGStr.lon; //中心经度

                var centerPointGG = new google.maps.LatLng(centerPointGGLat, centerPointGGLng);

                // 创建Map实例
                var myOptions = {
                    zoom: parseInt(initZoom) - 1,
                    center: centerPointGG,
                    disableDefaultUI: false,
                    panControl: true,
                    zoomControl: true,
                    zoomControlOptions: { style: google.maps.ZoomControlStyle.LARGE, position: google.maps.ControlPosition.LEFT_TOP },                    streetViewControlOptions: { position: google.maps.ControlPosition.LEFT_TOP },                    scaleControl: true,
                    mapTypeControl: true,
                    mapTypeControlOptions: { position: google.maps.ControlPosition.RIGHT_TOP },
                    mapTypeId: google.maps.MapTypeId.HYBRID
                };
                map = new google.maps.Map(document.getElementById("allmap"), myOptions);
                /*
                var bdary = new BMap.Boundary();
                bdary.get("乌拉特前旗", function (rs) {       //获取行政区域
                    var count = rs.boundaries.length; //行政区域的覆盖物有多少个
                    for (var i = 0; i < count; i++) {
                        var ps = rs.boundaries[i].split(";");
                        var point_bdary = [];
                        for (var j = 0; j < ps.length; j++) {
                            point_bdary.push(ps[j]);
                        }
                        var g4_bdary = new GABT4(point_bdary);
                        g4_bdary.b2g(function (arr) {
                            if (arr.length > 0) {
                                var flightPlanCoordinates = [];
                                for (var k = 0; k < arr.length; k++) {
                                    var point_Polyline = new google.maps.LatLng(arr[k].split(",")[1], arr[k].split(",")[0]);
                                    flightPlanCoordinates.push(point_Polyline);
                                }

                                var flightPath = new google.maps.Polyline({
                                    path: flightPlanCoordinates,
                                    strokeColor: "#FF0000",
                                    strokeOpacity: 1.0,
                                    strokeWeight: 5
                                });

                                flightPath.setMap(map);
                            }
                        });
                    }
                });
                */
                window.searchClass = new SearchClass();

                InitDeviceInfo();
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
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
                deviceInfos = [];
                allDevIds = "";
                for (var i = 0; i < deviceInfo.length; i++) {
                    deviceInfos[deviceInfo[i].ID] = deviceInfo[i];
                    if (i > 0) {
                        allDevIds += ",";
                    }
                    allDevIds += deviceInfo[i].ID;
                }

                deviceInfoLoaded = false;
                DrawPoint();
                setInterval(DrawPoint, 60000);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

var marker_devices = [];
function DrawPoint(searchText) {
    if (!deviceInfoLoaded) {
        deviceInfoLoaded = true;
        $("#divInfoInfo").html("可以重新画！" + "数量：" + deviceInfos.length + " " + marker_devices.length + "。<br>" + new Date().Format("yyyy-MM-dd HH:mm:ss"));

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

                                    if (commState == "全部正常") {
                                        if (deviceData[i].设备类型.Value == "水泵") {
                                            if (deviceData[i].运行状态.Value == "水泵工作") {
                                                devImg = "正常.png";
                                            } else if (deviceData[i].运行状态.Value == "水泵停机") {
                                                devImg = "正常.png";
                                            } else {
                                                devImg = "中断.png";
                                            }
                                        } else {
                                            devImg = "blue20.png";
                                        }
                                    } else {
                                        devImg = "中断.png";
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
                        pointObj['devType'] = deviceData[i].设备类型.Value;
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

        deviceInfoLoaded = false;
    }
    else {
        $("#divInfoInfo").html("正在画图，无法重新画！" + new Date().Format("yyyy-MM-dd HH:mm:ss"));
    }
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

function TabOnSelect(title, index) {
    var tabPanel = $("#divTabs").tabs("getTab", index);
    if (tabPanel.html() == "") {
        var monitorUrl = "BDMap_Monitor.html";
        var userStationParams = "1";
        tabPanel.html("<iframe name='MonitorIFrame' style='width:100%;height:100%' frameborder='no' src='" + monitorUrl + "?usps=" + userStationParams + "' onload='MonitorIFrameLoaded();'></iframe>");
    }
}

function MonitorIFrameLoaded() {
    iframeLoaded = true;
    window.frames["MonitorIFrame"].GetMonitorDataFromParent();
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

var realDevId;
//创建marker
window.addMarker = function (data) {
    for (var i = 0; i < marker_devices.length; i++) {
        marker_devices[i].setMap(null);
    }
    marker_devices.length = 0;

    var infoWindow = new google.maps.InfoWindow();
    for (var i = 0; i < data.length; i++) {
        var json = data[i];
        var p0 = json.point.split("|")[0];
        var p1 = json.point.split("|")[1];

        var PointLat_wgs = parseFloat(p1); //纬度GPS
        var PointLng_wgs = parseFloat(p0); //经度
        var pointGGStr = GPS.gcj_encrypt(PointLat_wgs, PointLng_wgs);
        var PointGGLat = pointGGStr.lat; //纬度Google
        var PointGGLng = pointGGStr.lon; //经度

        var marker_device = new google.maps.Marker({
            position: new google.maps.LatLng(PointGGLat, PointGGLng),
            title: json.iwTitle + " " + json.ID,
            map: map,
            icon: json.icon,
            draggable: false
        });

        (function (marker, _json) {
            google.maps.event.addListener(marker, "click", function (e) {
                realDevId = _json.ID;
                var s = _json.content;

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
                        if (iskb == 1) {
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

                s = "<b class='iw_poi_title' title='" + _json.iwTitle + "详细信息'><span style='color:blue;font-size:10pt;'>" + _json.iwTitle + "</span><span style='font-size:10pt;'>详细信息</span></b><div class='iw_poi_content' style='padding:5px;'>" + s + "</div>"
                infoWindow.setContent(s);
                infoWindow.open(map, marker);
            });
        })(marker_device, json);

        marker_devices.push(marker_device);
    }
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