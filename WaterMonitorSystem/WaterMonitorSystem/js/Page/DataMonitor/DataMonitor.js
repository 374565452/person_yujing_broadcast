// JScript 文件

var dicDeviceAppGroups = {};
var mnId = "";
$(document).ready(function(){
    var defaultTheme = $.cookie("psbsTheme");
    if(defaultTheme != null && defaultTheme != "default")
    {
        var link = $(document).find('link:first');
	    link.attr('href', '../App_Themes/easyui/themes/'+defaultTheme+'/easyui.css');
	}
    GetSystemInfo();
    GetDeviceAppGroup();
});

//从服务器取得系统运行状态信息
function GetSystemInfo()
{
    $.ajax(
    { 
        url:"../WebServices/SystemService.asmx/GetSystemStateInfo",
        type:"GET",
        data:{},
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
                GetManageNode(mnId);
	            GetDeviceNode(mnId);
            }
        },
        error:function (XMLHttpRequest, textStatus, errorThrown)
        {
            alert(errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function GetDeviceAppGroup()
{
    $.ajax({
        url:"../WebServices/GlobalAppService.asmx/GetDeviceAppGroups",
        type:"GET",
        data:{},
        dataType:"text",
        cache:false,
        success:function(responseText)
        {
            var deviceAppGroups = eval("("+$.xml2json(responseText)+")");
            if(deviceAppGroups.length == 0)
            {
                ShowPageInTab("实时监测",true);
            }
            else
            {
                for(var i=0;i<deviceAppGroups.length;i++)
                {
                    dicDeviceAppGroups[deviceAppGroups[i].名称] = deviceAppGroups[i];
                    if(i == 0)
                    {
                        ShowPageInTab(deviceAppGroups[i].名称,true);
                    }
                    else
                    {
                        ShowPageInTab(deviceAppGroups[i].名称,false);
                    }
                }
            }
        },
        error:function (XMLHttpRequest, textStatus, errorThrown)
        {
            alert(errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function ShowPageInTab(groupName,isSelected)
{
    //根据标题获取标签页
    var tabPanel = $("#divTabs").tabs('getTab',groupName);
    //判断获取的标签页是否为空，为空则新建，否则直接选中
    if(tabPanel == null)
    {
        $("#divTabs").tabs('add',{
            title:groupName,
            selected:isSelected,
            //标签页不允许关闭
            closable:false
        });
        $("#divTabs").tabs('getTab',groupName).css("overflow","hidden");
    }
    else
    {
        $("#divTabs").tabs('select',groupName);
    }
}

function TabOnSelect(title,index)
{
    var tabPanel = $("#divTabs").tabs("getTab",index);
    if(tabPanel.html() == "")
    {
        var monitorUrl = typeof(dicDeviceAppGroups[title]) == "undefined" ? "MonitorList.html" : dicDeviceAppGroups[title].实时监测Url;
        tabPanel.html("<iframe style='width:100%;height:100%' frameborder='no' src='"+monitorUrl+"?groupName="+title+"'></iframe>");
    }
}