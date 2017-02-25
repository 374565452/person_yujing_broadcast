// JScript 文件
var dicDeviceAppGroups = {};
//表格数据
var tableData = [];
var comboBoxData=[];
//组网方式
var networks = [];
//数据通道
var dataChannels = [];
var mnId = "";
var operateType = "Add";
$(document).ready(function(){
    var defaultTheme = $.cookie("psbsTheme");
    if(defaultTheme != null && defaultTheme != "default")
    {
        var link = $(document).find('link:first');
	    link.attr('href', '../App_Themes/easyui/themes/'+defaultTheme+'/easyui.css');
	}
    GetSystemInfo();
    //获取组网方式
    GetNetworks();
    //获取数据通道
    GetDataChannels();
});

//从服务器取得系统运行状态信息
function GetSystemInfo()
{
    $.ajax(
    { 
        url:"../WebServices/SystemService.asmx/GetSystemStateInfo",
        type:"GET",
        data:{"loginIdentifer":window.parent.guid},
        dataType:"text",
        cache:false,
        success:function(responseText)
        {
            var data = eval("("+$.xml2json(responseText)+")");
            if(data.Result)//登录成功
            {
                mnId = data.SysStateInfo.当前登录操作员管理ID;
                manageNodeLoaded = false;
                deviceNodeLoaded = false;
	            LoadTree("divAreaTree", mnId, true, false);
	            LoadTableData();
	            LoadComboboxData();
	            TreeBindSelect();
	            TableCellDbClick();
            }
        },
        error:function (XMLHttpRequest, textStatus, errorThrown)
        {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function GetNetworks()
{
    $.ajax(
    { 
        url:"../WebServices/DataChannelService.asmx/GetNetworks",
        type:"GET",
        data:{"loginIdentifer":window.parent.guid},
        dataType:"text",
        cache:false,
        success:function(responseText)
        {
            var data = eval("("+$.xml2json(responseText)+")");
            if(data.Result)//登录成功
            {
                networks = data.Networks;
                var networkComboBoxData=[];
                var selectedValue = "";
                for (i = 0; i < networks.length; i++) 
                {
                    var devObj = {};
                    devObj["id"] = networks[i].ID;
                    devObj["text"] = networks[i].Text;
                    networkComboBoxData.push(devObj);
                    if(data.Networks[i].Selected)
                    {
                        selectedValue = networks[i].ID;
                    }
                }
                $("#cbb_Network").combobox({
                    data:networkComboBoxData,
                    value:selectedValue,
                    onChange:cbb_Network_Change
                });
            }
        },
        error:function (XMLHttpRequest, textStatus, errorThrown)
        {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function GetDataChannels()
{
    $.ajax(
    { 
        url:"../WebServices/DataChannelService.asmx/GetDataChannels",
        type:"GET",
        data:{"loginIdentifer":window.parent.guid},
        dataType:"text",
        cache:false,
        success:function(responseText)
        {
            var data = eval("("+$.xml2json(responseText)+")");
            if(data.Result)
            {
                dataChannels = data.DataChannels;
                var dataChannelComboBoxData=[];
                var selectedValue = "";
                for (i = 0; i < dataChannels.length; i++) 
                {
                    var devObj = {};
                    devObj["id"] = dataChannels[i].ID;
                    devObj["text"] = dataChannels[i].Text;
                    dataChannelComboBoxData.push(devObj);
                }
                if(dataChannels.length > 0)
                {
                    selectedValue = dataChannels[0].ID;
                }
                $("#cbb_DataChannel").combobox({
                    data:dataChannelComboBoxData,
                    value:selectedValue
                });
            }
        },
        error:function (XMLHttpRequest, textStatus, errorThrown)
        {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function TreeBindSelect()
{
    $('#divAreaTree').tree({
        onSelect: function(node){
	        var devIds = GetDeviceIDsByNode(node);
	        ReLoadTableData(devIds);
        }
    });
}

function TableCellDbClick()
{
    $('#tbDevInfos').datagrid({
	    onDblClickCell: function(index,field,value){
	        if(field == "editDev")
 		    {
 		        operateType = "Modify";
 		        var selectedRow = $(this).datagrid("getSelected");
 		        var devId = selectedRow["devId"]
 		        LoadDeviceDetailInfo(devId);
 		        $('#divDeviceTabs').tabs("select",0);
                $("#btn_Next").linkbutton({text:"下一步"});
 		        $('#dlgDevice').dialog({title:'编辑终端信息',closed:false});
 		    }
 		    else if(field == "removeDev")
 		    {
 		        var selectedRow = $(this).datagrid("getSelected");
 		        var devId = selectedRow["devId"]
 		        $.messager.confirm('提示信息', '您确定要删除该终端吗?', function(r){
	                if (r)
	                {
	                    
		                $.ajax({ 
                            url:"../WebServices/DeviceNodeService.asmx/DeleteDeviceNode",
                            type:"POST",
                            data:{"loginIdentifer":window.parent.guid, "devID":devId},
                            dataType:"text",
                            cache:false,
                            success:function(responseText)
                            {
                                var data = eval("("+$.xml2json(responseText)+")");
                                if(data.Result)//登录成功
                                {
                                    $.messager.alert('提示信息','删除成功！');
                                    $("#tbDevInfos").datagrid("deleteRow",index);
                                    for(var i=0; i<deviceJson.length; i++)
                                    {
                                        if(deviceJson[i].ID == devId)
                                        {
                                            deviceJson.splice(i,1);
                                            break;
                                        }
                                    }
                                    delete deviceNodes[devId];
                                    var treeNode = $("#divAreaTree").tree("find","dn_"+devId);
                                    $("#divAreaTree").tree("remove",treeNode.target);
                                }
                                else
                                {
                                    switch(data.Message)
                                    {
                                        case "noLogin":
                                            $.messager.alert('提示信息','您未登录或登录超时，请重新登录！','warning');
                                            window.location.href = "../default.html";
                                            break;
                                        case "noRight":
                                            $.messager.alert('提示信息','您没有权限进行此操作，请联系管理员！','warning');
                                            break;
                                        default:
                                            $.messager.alert('提示信息',data.Message,'warning');
                                            break;
                                    }
                                }
                            },
                            error:function (XMLHttpRequest, textStatus, errorThrown)
                            {
                                $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
                            }
                        });
	                }
                });
 		    }
 	    }
    });
}

function cbb_Network_Change(newValue, oldValue)
{
//    var ddlTransArgeement = document.getElementById("ddl_TransArgeement");
//    var networkingMode = document.getElementById("ddl_NetworkingMode").value;
//    if(networkingMode !=1 && networkingMode !=6 && networkingMode != 7)
//    {
//        $("#trTransIdentify").show();
//        $("#trIpAndPort").hide();
//    }
//    else
//    {
//        $("#trTransIdentify").hide();
//        $("#trIPAndPort").show();
//        ddlTransArgeement.options.add(new Option("无协议","无协议"));
//    }
//    var ddlServerList = document.getElementById("ddl_ServerList");

//    ddlServerList.options.add(new Option("新建...","0"));
//    switch(networkingMode)
//    {
//        case "1":
//            for(var dataChannel in dataChannels)
//            {
//                if(dataChannel.ChannelType == "DC_COM" && dataChannel.Text.Contains("串口直连"))
//                {
//                    ddlServerList.options.add(new Option(dataChannels[dcID].显示名称,dataChannels[dcID].ID))
//                }
//            }
//            $("#TelephoneTD").show();
//            $("#trIPAndPort").hide();
//            document.getElementById("ddl_TransArgeement").value="无协议";
//            break;
//        case "2":
//            for(var dcID in dataChannels)
//            {
//                if(dataChannels[dcID].通道类型 == "DC_COM" && dataChannels[dcID].显示名称.Contains("移动专网"))
//                {
//                    ddlServerList.options.add(new Option(dataChannels[dcID].显示名称,dataChannels[dcID].ID))
//                }
//            }
//            break;
//        case "3":
//            break;
//        case "4":
//            for(var dcID in dataChannels)
//            {
//                if(dataChannels[dcID].通道类型 == "DC_TCP_FromServer")
//                {
//                    ddlServerList.options.add(new Option(dataChannels[dcID].显示名称,dataChannels[dcID].ID))
//                }
//            }
//            break;
//        case "5":
//            for(var dcID in dataChannels)
//            {
//                if(dataChannels[dcID].通道类型 == "DC_UDP_FromServer")
//                {
//                    ddlServerList.options.add(new Option(dataChannels[dcID].显示名称,dataChannels[dcID].ID))
//                }
//            }
//            break;
//        case "6":
//        case "7":
//            var dcType = "DC_TCP_Client";
//            if(networkingMode == "7")
//            {
//                dcType = "DC_UDP_Client";
//            }
//            for(var dcID in dataChannels)
//            {
//                if(dataChannels[dcID].通道类型 == dcType)
//                {
//                    ddlServerList.options.add(new Option(dataChannels[dcID].显示名称,dataChannels[dcID].ID))
//                }
//            }
//            document.getElementById("ddl_TransArgeement").value="无协议";
//            $("#TelephoneTD").hide();
//            $("#trIPAndPort").show();
//            break;
//        default:
//            break;
//    }
//    if(networkingMode != "6" && networkingMode != "7" && ddlServerList.options.length>1)
//    {
//        ddlServerList.options[1].selected=true;
//    }
//    ddl_ServerList_SelectChanged();
}

function LoadTableData(showLastPage)
{
    if(!deviceNodeLoaded)
    {
        window.setTimeout("LoadTableData("+showLastPage+")",500);
        return;
    }
    tableData = [];
    var devJson;
    for (var key in deviceNodes) 
    {
        devJson = deviceNodes[key].attributes["device"];
        var tableRow = {};
        tableRow["devId"] = devJson.ID;
        tableRow["mnName"] = devJson.管理名称;
        tableRow["devName"] = devJson.名称;
        tableRow["devType"] = devJson.模板信息名称;
        tableRow["transIdentify"] = devJson.传输设备[0].传输标识;
        tableRow["installTime"] = typeof(devJson.辅助信息.安装时间)=="undefined"?"":devJson.辅助信息.安装时间;
        tableRow["installSite"] = typeof(devJson.辅助信息.安装位置)=="undefined"?"":devJson.辅助信息.安装位置;
        tableRow["editDev"] = "<img src='../Images/Detail.gif' onclick='' />";
        tableRow["removeDev"] = "<img src='../Images/Delete.gif' onclick='' />";
        tableData.push(tableRow);
    }
    $('#tbDevInfos').datagrid({loadFilter:pagerFilter}).datagrid('loadData', tableData);
    if(showLastPage)
    {
        var pager = $('#tbDevInfos').datagrid("getPager");
        var pagerOptions = pager.pagination("options");
        var pageCount = parseInt((pagerOptions["total"]+pagerOptions["pageSize"]-1)/pagerOptions["pageSize"]);
        if(pageCount > 1)
        {
            pager.pagination("select",pageCount);
        }
    }
}

function ReLoadTableData(devIds)
{
    tableData = [];
    var devJson;
    for (var i = 0; i < devIds.length; i++) 
    {
        var tableRow = {};
        devJson = deviceNodes[devIds[i]].attributes["device"];
        tableRow["devId"] = devJson.ID;
        tableRow["mnName"] = devJson.管理名称;
        tableRow["devName"] = devJson.名称;
        tableRow["devType"] = devJson.模板信息名称;
        tableRow["transIdentify"] = devJson.传输设备[0].传输标识;
        tableRow["installTime"] = typeof(devJson.辅助信息.安装时间)=="undefined"?"":devJson.辅助信息.安装时间;
        tableRow["installSite"] = typeof(devJson.辅助信息.安装位置)=="undefined"?"":devJson.辅助信息.安装位置;
        tableRow["editDev"] = "<img src='../Images/Detail.gif' />";
        tableRow["removeDev"] = "<img src='../Images/Delete.gif' />";
        tableData.push(tableRow);
    }
    $('#tbDevInfos').datagrid({loadFilter:pagerFilter}).datagrid('loadData', tableData);
}

function pagerFilter(data){
    if (typeof data.length == 'number' && typeof data.splice == 'function'){	// is array
	    data = {
		    total: data.length,
		    rows: data
	    }
    }
    var dg = $(this);
    var opts = dg.datagrid('options');
    var pager = dg.datagrid('getPager');
    pager.pagination({
	    onSelectPage:function(pageNum, pageSize){
		    opts.pageNumber = pageNum;
		    opts.pageSize = pageSize;
		    pager.pagination('refresh',{
			    pageNumber:pageNum,
			    pageSize:pageSize
		    });
		    dg.datagrid('loadData',data);
	    }
    });
    if (!data.originalRows){
	    data.originalRows = (data.rows);
    }
    var start = (opts.pageNumber-1)*parseInt(opts.pageSize);
    var end = start + parseInt(opts.pageSize);
    data.rows = (data.originalRows.slice(start, end));
    return data;
}

function LoadComboboxData()
{
    if(!deviceNodeLoaded)
    {
        window.setTimeout("LoadComboboxData()",500);
        return;
    }
    comboBoxData=[];
    for (i = 0; i < deviceJson.length; i++) 
    {
        var devObj = {};
        devObj["id"] = deviceJson[i].ID;
        devObj["text"] = deviceJson[i].名称;
        comboBoxData.push(devObj);
    }
    $("#cbb_DevCombobox").combobox({
        data:comboBoxData
    });
}

function LoadDeviceDetailInfo(devId)
{

}

function ClearDeviceDetailInfo()
{

}

function Btn_Query_Click()
{
    var devIds = [];
    var devComboboxObj = $("#cbb_DevCombobox");
    var value = devComboboxObj.combobox("getValue");
    //值为空时采用模糊获取ID
    if(value == null || value == "")
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
                    devIds.push(comboboxData[i][opts.valueField]);
                }
            }
        }
        else
        {
            LoadTableData();
            return;
        }
    }
    else
    {
        devIds.push(value);
    }
    if(devIds.length == 0)
    {
        $.messager.alert("提示信息","查询结果为空");
    }
    ReLoadTableData(devIds);
}

function btn_Add_Click()
{
    operateType = "Add";
    ClearDeviceDetailInfo();
    $('#divDeviceTabs').tabs("select",0);
    $("#btn_Next").linkbutton({text:"下一步"});
    $('#dlgDevice').dialog({title:"添加终端信息",closed:false});
}

function btn_Back_Click()
{
    var tabsObj = $('#divDeviceTabs');
    var tab = tabsObj.tabs('getSelected');
    var index = tabsObj.tabs('getTabIndex',tab);
    if (index != 0)
    {
        $("#btn_Next").linkbutton({text:"下一步"});
        tabsObj.tabs("select",index-1);
    }
}

function btn_Next_Click()
{
    var tabsObj = $('#divDeviceTabs');
    var tabsConut = tabsObj.tabs('tabs').length;
    var tab = tabsObj.tabs('getSelected');
    var index = tabsObj.tabs('getTabIndex',tab);
    if (index == (tabsConut-1))
    {
        var deviceJSON = {};
        var deviceJSONString = JSON.stringify(deviceJSON);
        
        $.ajax(
        { 
            url:"../WebServices/DeviceNodeService.asmx/AddDeviceNode",
            type:"POST",
            data:{"loginIdentifer":window.parent.guid, 'deviceJSONString':deviceJSONString},
            dataType:"json",
            cache:false,
            success:function(data)
            {
                if(data.Result)//登录成功
                {
                    LoadTree("divAreaTree", mnId, true, false);
                    LoadTableData(true);
                    LoadComboboxData();
                    $('#dlgDevice').dialog("close");
                }
            },
            error:function (XMLHttpRequest, textStatus, errorThrown)
            {
                $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
            }
        });
    }
    else
    {
        if((index+1) == (tabsConut-1))
        {
            $("#btn_Next").linkbutton({text:"确定"});
        }
        else
        {
            $("#btn_Next").linkbutton({text:"下一步"});
        }
        tabsObj.tabs("select",index+1);
    }
}