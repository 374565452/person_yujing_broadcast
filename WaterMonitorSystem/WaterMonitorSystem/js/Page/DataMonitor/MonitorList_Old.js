// JScript 文件

var comboTreeData=[];
var comboBoxData=[];
var queryDevIds = [];
var groupName = "";
var groupUSPsObj = {};
$(document).ready(function(){
    var defaultTheme = $.cookie("psbsTheme");
    if(defaultTheme != null && defaultTheme != "default")
    {
        var link = $(document).find('link:first');
	    link.attr('href', '../App_Themes/easyui/themes/'+defaultTheme+'/easyui.css');
	}
	groupName = $.getUrlVar("groupName");
    if(groupName != "")
    {
        var groupObj = window.parent.dicDeviceAppGroups[groupName];
        if(typeof(groupObj) != "undefined")
        {
            groupUSPsObj["groupAll"] = groupObj.用户站参数
            {}
            if(groupObj["子类"].length > 1)
            {
                var tdHTML = ""
                for(var i=0;i<groupObj["子类"].length;i++)
                {
                    tdHTML+="<input id='cb_Group"+i+"' type="checkbox" style="margin:0; vertical-align:middle" /><span style="vertical-align:middle; font-size:10pt">在线</span>"
                }
            }
        }
        
        for(var i=0;i<usps.length;i++)
        {
            groupUSPsObj[usps[i]]=usps[i];
        }
    }
    GetComboTreeData(groupUSPs);
    GetComboboxData(groupUSPs);
});

function GetComboboxData(groupUSPs)
{
    if(!window.parent.deviceNodeLoaded)
    {
        window.setTimeout("GetComboboxData('"+groupUSPs+"')",200);
        return;
    }
     //父页面设备信息集合
    var deviceJson = window.parent.deviceJson;
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

function BtnQueryClick()
{
    if($("#cb_QueryType").combobox('getText')=="模糊查询")
    {
        checkedNodeValues = []
        var devComboboxObj = $("#cb_devCombobox");
        var value = devComboboxObj.combobox("getValue");
        //值为空时采用模糊获取ID
        if(value == null)
        {
            var text = devComboboxObj.combobox("getText");
            //文本为空时按Combobox加载数据获取设备ID
            if($.trim(text) != "")
            {
                var comboboxData = devComboboxObj.combobox("getData");
                var opts = devComboboxObj.combobox('options');
                for(var i=0; i<comboboxData.length; i++)
                {
                    if(comboboxData[i][opts.textField].indexOf(text)>-1)
                    {
                        checkedNodeValues.push(comboboxData[i][opts.valueField]);
                    }
                }
            }
        }
        else
        {
            checkedNodeValues.push(value);
        }
        alert(checkedNodeValues);
    }
    else
    {
        alert(checkedNodeValues);
    }
}

function BtnResetClick()
{
    checkedNodeValues = [];
    var text = $("#cb_QueryType").combobox("getText");
    if(text == "模糊查询")
    {
        $("#cb_devCombobox").combobox("clear");
    }
    else
    {
        $("#cb_devCombotree").combotree("clear");
    }
}