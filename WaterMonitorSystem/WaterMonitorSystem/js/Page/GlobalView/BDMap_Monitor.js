// JScript 文件

var comboTreeData=[];
var comboBoxData=[];
var queryDevIds = [];
var groupUSPs = "";
var groupUSPsObj = {};
$(document).ready(function(){
    var defaultTheme = $.cookie("psbsTheme");
    if(defaultTheme != null && defaultTheme != "default")
    {
        var link = $(document).find('link:first');
	    link.attr('href', '../App_Themes/easyui/themes/'+defaultTheme+'/easyui.css');
	}
    groupUSPs = $.getUrlVar("usps");
    if(groupUSPs != "")
    {
        var usps = groupUSPs.split(',');
        for(var i=0;i<usps.length;i++)
        {
            groupUSPsObj[usps[i]]=usps[i];
        }
    }
    //GetComboTreeData(groupUSPs);
    GetComboboxData(groupUSPs);
});

function GetComboboxData(groupUSPs)
{
    if(!window.parent.deviceInfoLoaded)
    {
        window.setTimeout("GetComboboxData('"+groupUSPs+"')",200);
        return;
    }
     //父页面设备信息集合
    var deviceJson = window.parent.deviceInfo;
    for (i = 0; i < deviceJson.length; i++) 
    {
        if(groupUSPs != "" && typeof(groupUSPsObj[deviceJson[i].用户站参数]) == "undefined")
        {
            continue;
        }
        var devObj = {};
        devObj["id"] = deviceJson[i].ID;
        devObj["text"] = deviceJson[i].名称;
        comboBoxData.push(devObj);
    }
    $("#cb_devCombobox").combobox({
        data:comboBoxData
    });
}

function GetComboTreeData(groupUSPs)
{
    if(!window.parent.manageNodeLoaded || !window.parent.deviceNodeLoaded)
    {
        window.setTimeout("GetComboTreeData('"+groupUSPs+"')",200);
        return;
    }
    comboTreeData = [];
    var manageJson = window.parent.manageJson;
    //复制一份
    var manageNodes = $.extend(true,{},window.parent.manageNodes);
    // 挂载节点
    for (i = 0; i < manageJson.length; i++) 
    {
        // 添加到上级节点中
        if (manageJson[i].上级ID == '0' || manageNodes[manageJson[i].上级ID]==null) 
        {
            // 根节点
            comboTreeData.push(manageNodes[manageJson[i].ID]);
        } 
        else 
        {
            // 上级节点
            manageNodes[manageJson[i].上级ID].children.push(manageNodes[manageJson[i].ID]);
        }
    }
    
    var deviceJson = window.parent.deviceJson;
    var deviceNodes = window.parent.deviceNodes;
    // 挂载节点
    for (i = 0; i < deviceJson.length; i++) 
    {
        // 添加到上级节点中
        if (deviceJson[i].管理ID == '0' || (groupUSPs != "" && typeof(groupUSPsObj[deviceJson[i].用户站参数]) == "undefined")) 
        {
            // 根节点
            continue;
        }
        else 
        {
            // 上级节点
            manageNodes[deviceJson[i].管理ID].children.push(deviceNodes[deviceJson[i].ID]);
        }
    }
    $("#cb_devCombotree").combotree("loadData",comboTreeData);
}

function QueryTypeChange()
{
    checkedNodeValues = [];
    var cbText = $(this).combobox("getText");
    if(cbText == "模糊查询")
    {
        $("#sDevCombobox").show();
        $("#sDevCombotree").css("visibility","hidden");
        $("#cb_devCombobox").combobox("clear");
    }
    else
    {
        $("#sDevCombobox").hide();
        $("#sDevCombotree").css("visibility","visible");
        $("#cb_devCombotree").combotree("clear");
    }
    $("#btnQuery").linkbutton({text:cbText})
}

var checkedNodeValues = [];
function ComboboxTreeOnCheck(node, checked)
{
    var t = $('#cb_devCombotree').combotree('tree');
    var treeNodes = t.tree('getChecked');
    checkedNodeValues = [];
    var checkText = "";
    for(var i=0;i<treeNodes.length;i++)
    {
        if(treeNodes[i].attributes["nodeType"]=="device")
        {
            checkedNodeValues.push(treeNodes[i].attributes["did"]);
            if(checkText != "")
            {
                checkText += "," + treeNodes[i].text;
            }
            else
            {
                checkText = treeNodes[i].text;
            }
        }
    }
    $('#cb_devCombotree').combotree('setText',checkText);
}

var isOnLine = null;
function BtnQueryClick()
{
    window.parent.selDevIds=[];
    //用于存储是否勾选了在线、不在线的复选框
    isOnLine="";
    if($("#cb_QueryType").combobox('getText')=="模糊查询")
    {
        checkedNodeValues = []
        var devComboboxObj = $("#cb_devCombobox");
        var value = devComboboxObj.combobox("getValue");
        //值为空时采用模糊获取ID
        if(value == null || value == "")
        {
            var text = devComboboxObj.combobox("getText");
            text = $.trim(text);
            if(text == "")
            {
                alert("请输入查询条件");
                return;
            }
            var comboboxData = devComboboxObj.combobox("getData");
            var opts = devComboboxObj.combobox('options');
            for(var i=0; i<comboboxData.length; i++)
            {
                if(comboboxData[i][opts.textField].indexOf(text) < 0)
                {
                    continue;
                }
                checkedNodeValues.push(comboboxData[i][opts.valueField]);
                window.parent.selDevIds.push(comboboxData[i][opts.valueField]);
            }
        }
        else
        {
            checkedNodeValues.push(value);
            window.parent.selDevIds.push(value);
        }
        
        var devStateNormalChecked = document.getElementById("devStateNormal").checked;
        var devStateAlarmChecked = document.getElementById("devStateAlarm").checked;
        //获取是否选中在线不在线的复选框
        if(devStateNormalChecked && !devStateAlarmChecked)
        {
            isOnLine = "normal";
        }
        else if(!devStateNormalChecked && devStateAlarmChecked)
        {
            isOnLine="alarm";
        }
        
        //地图也随之定位到此测点的位置
        window.parent.DrawPoint(devComboboxObj.combobox("getText"));
    }
    else
    {
        //alert(checkedNodeValues);
    }
}

function GetMonitorDataFromParent(isSearch)
{
    var devIdsObj = {};
    if(isSearch)
    {
        var devIds =window.parent.selDevIds;
        if(devIds==null || devIds.length==0)
        {
            return;
        }
        for(var j=0;j<devIds.length;j++)
        {
            devIdsObj[devIds[j]] = devIds[j];
        }
    }
    
    var gridcolumns = [];
    var devDatas = window.parent.deviceData;
    var submapDataInfo = window.parent.mapDataInfo;
    var tableData = [];
    var gridColumnsFlag = true;
    for (var i = 0; i < devDatas.length; i++) {
        
        if(isSearch && devIdsObj[devDatas[i].操作.Value] == undefined)
        {
            continue;
        }

        //先判断通讯状态和设备状态是否都是正常的
        var isContinue = true;
        if(isOnLine!=null)
        {
            for(var colName in devDatas[i])
            {
                if(isOnLine=="normal" && ((colName == "通讯状态" && devDatas[i][colName].Value != "全部正常") ||
                (colName == "设备状态" && devDatas[i][colName].Value != "全部正常")))
                {
                    isContinue=false;
                    break;
                }
                else if(isOnLine=="alarm" && ((colName == "通讯状态" && devDatas[i][colName].Value == "全部正常") ||
                    (colName == "设备状态" && devDatas[i][colName].Value == "全部正常")))
                {
                    isContinue=false;
                    break;
                }
            }
        }
        if(!isContinue)
        {
            continue;
        }
        var tableRow = {};
        for (var item in devDatas[i]) {
            if (item == "通讯状态") {
                if (devDatas[i][item].Value == "全部正常") {
                    devDatas[i][item].Value = "<Img style='height:18px;width:18px;' title='通讯状态:\r\n正常' src='../images/正常.png'/>";
                } else {
                    devDatas[i][item].Value = "<Img style='height:18px;width:18px;' src='../images/中断.png'/>";
                }
            }
            else if (item == "设备状态") {
                if (devDatas[i][item].Value == "全部正常") {
                    devDatas[i][item].Value = "<Img style='height:18px;width:18px;' src='../images/正常.png'/>";
                } else {
                    devDatas[i][item].Value = "<Img style='height:18px;width:18px;' src='../images/中断.png'/>";
                }
            } else if (item == "运行状态") {
                if (devDatas[i][item].Value == "水泵工作") {
                    devDatas[i][item].Value = "<Img style='height:18px;width:18px;' src='../images/大圈绿.gif'/>";
                } else if (devDatas[i][item].Value == "水泵停机") {
                    devDatas[i][item].Value = "<Img style='height:18px;width:18px;' src='../images/大圈红.gif'/>";
                } else {
                    devDatas[i][item].Value = "<Img style='height:18px;width:18px;' src='../images/大圈灰.gif'/>";
                }
            } else if (item == "通讯") {
                if (devDatas[i]["通讯状态"] == "全部正常") {
                    var q = (devDatas[i]["信号强度"].Value != null ? devDatas[i]["信号强度"].Value : "--");
                    if (q == "--" || q == "未知") {
                        devDatas[i][item].Value = "<Img style='height:18px;width:18px;' src='../images/signal4.png'/>";
                    }
                    if (q < 15) {
                        devDatas[i][item].Value = "<Img style='height:18px;width:18px;' src='../images/signal1.png'/>";
                    }
                    if (q >= 15 && q <= 25) {
                        devDatas[i][item].Value = "<Img style='height:18px;width:18px;' src='../images/signal3.png'/>";
                    }
                    if (q > 25) {
                        devDatas[i][item].Value = "<Img style='height:18px;width:18px;' src='../images/signal4.png'/>";
                    }
                }
                else {
                    devDatas[i][item].Value = "<Img style='height:18px;width:18px;' src='../images/signal0.png'/>";
                }
            }
            var _devId = devDatas[i].操作.Value;
            //var templateId = window.parent.deviceInfos[_devId].用户站参数;
            var templateId = 1;
            if (gridColumnsFlag && item != "操作" && (item == "村庄" || item == "设备" || item == "通讯状态" || item == "设备状态")) {
                gridcolumns.push({ field: devDatas[i][item].Field, title: window.parent.Columns[item].HeadText, width: 70 });
            }
            for (var j = 0; j < submapDataInfo.用户模板.length; j++) {
                if (submapDataInfo.用户模板[j].ID == templateId) {
                    var xmlStateValue = submapDataInfo.用户模板[j].状态量;
                    var valueName = submapDataInfo.用户模板[j].采集量.split(',');
                    for (var k = 0; k < valueName.length; k++) {
                        if (item == "村庄" || item == "设备" || item == "通讯状态" || item == "设备状态" || item == valueName[k]) {
                            tableRow[devDatas[i][item].Field] = devDatas[i][item].Value;
                        }
                        
                        if (gridColumnsFlag && item != "操作" && (item == valueName[k])) {
                            if (item == "经纬度") {
                                gridcolumns.push({ field: devDatas[i][item].Field, title: window.parent.Columns[item].HeadText, width: 0 });
                            } else if (item == "开泵时间" || item == "关泵时间") {
                                gridcolumns.push({ field: devDatas[i][item].Field, title: window.parent.Columns[item].HeadText, width: 120 });
                            } else {
                                gridcolumns.push({ field: devDatas[i][item].Field, title: window.parent.Columns[item].HeadText, width: 100 });
                            }
                        }
                    }
                }
            }
        }
        gridColumnsFlag = false;
        tableData.push(tableRow);
    }
    $("#tbMonitorDats").datagrid({
        columns: [gridcolumns],
        onDblClickRow: function (rowIndex, rowData) {
            setTimeout(function () {
                if (rowData["jwd4"]) {
                    var ll = rowData["jwd4"];
                    var centerPoint = rowData["jwd4"].split("|");
                    if (window.parent.CenterAndZoom)
                        window.parent.CenterAndZoom(centerPoint[0], centerPoint[1], 12);
                }
            }, 100);
        }
    }).datagrid("loadData", tableData)
}

function BtnResetClick()
{
    checkedNodeValues = [];
    isOnLine = null;
    var text = $("#cb_QueryType").combobox("getText");
    if(text == "模糊查询")
    {
        $("#cb_devCombobox").combobox("clear");
    }
    else
    {
        $("#cb_devCombotree").combotree("clear");
    }
    document.getElementById("devStateNormal").checked="checked";
    document.getElementById("devStateAlarm").checked="checked";
    //点击重置按钮后将实时层的列表默认显示所有
    window.parent.selDevIds=[];
    //加载地图上所有的点
    window.parent.DrawPoint();
}