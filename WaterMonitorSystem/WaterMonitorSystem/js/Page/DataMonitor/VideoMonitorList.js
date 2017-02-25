//// JScript 文件

var DocumentWidth = 0;
var DocumentHeight = 0;
//村id
var villageid="";
//视频Json数组
var VideoRecordJson="";

var totalRecord=0;

$(document).ready(function(){
    $.ShowMask("数据加载中，请稍等……");
    var defaultTheme = $.cookie("psbsTheme");
    if(defaultTheme != null && defaultTheme != "default")
    {
        var link = $(document).find('link:first');
	    link.attr('href', '../App_Themes/easyui/themes/'+defaultTheme+'/easyui.css');
	}
	DocumentWidth = $("#divVideoRecord")[0].clientWidth;
	DocumentHeight = $("#divVideoRecord")[0].clientHeight;
    GetSystemInfo();
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
                LoadWaterUserTree("divAreaTree", mnId, false, false);
                TreeBindSelect();
            }
        },
        error:function (XMLHttpRequest, textStatus, errorThrown)
        {
            $.message.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

//点击左侧树形重新加载右侧Combobox
function TreeBindSelect() 
{
    $('#divAreaTree').tree({
        onSelect: function (node) {  
            if (node.id.indexOf("cz") >= 0) {
                villageid = node.attributes["mid"];
                LoadVideoRecord("../WebServices/VideoInfoService.asmx/GetVideoRecordCount", { "loginIdentifer": window.parent.guid, "villageId": villageid, "isExport": false });
            }
        }
    });
}

function LoadVideoRecord(url,data)
{
    $.ajax(
    {
        url:url,
        type:"GET",
        data:data,
        dataType:"text",
        cache:false,
        success:function(responseText)
        {
            var VideoRecord = "";
            var data = eval("(" + $.xml2json(responseText) + ")");
            if(data.Result)
            {
                 totalRecord = data.Count;
                 $('#pp').pagination({
                    total:totalRecord,
                    pageSize:1,
                    pageList:[1,4,9],
                    onSelectPage:function(pageNumber, pageSize){
                        $(this).pagination('loading');
                        QueryCurrentPageData(villageid,pageNumber,pageSize);
                        $(this).pagination('loaded');
                    }
                });

                if(totalRecord==0)
                {
                    $.HideMask();
                    $.message.alert("提示信息","查询结果为空");
                    return;
                }
                QueryCurrentPageData(villageid,1,1);               
            }
            else
            {
                $.HideMask();
				$("#divVideoRecord").empty();
                if(data.Message == "未登录")
                {
                    $.Messager.alert("提示信息","您未登录，请先登录！");
                    window.location.href = "../default.html";
                }
                else if(data.Message == "登录超时")
                {
                    $.messager.alert("提示信息","登录超时，请重新登录！");
                    window.location.href = "../default.html";
                }
                else
                {
                    $.messager.alert("提示信息",data.Message);
                }
            }
         },
         error: function (XMLHttpRequest, textStatus, errorThrown) 
        {
            $.HideMask();
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function QueryCurrentPageData(villageid,pageNumber,pageSize)
{
    var startNumber = (pageNumber - 1) * pageSize+1;
    var endNumber = pageNumber * pageSize;
    $.ajax(
    { 
        url:"../WebServices/VideoInfoService.asmx/GetQueryPageVideoRecords",
        type:"GET",
        data:{"loginIdentifer": window.parent.guid,'VillageId':villageid,'startIndex':startNumber,'endIndex':endNumber},
        dataType:"text",
        cache:false,
        success:function (responseText) 
        {
            var data = eval("("+$.xml2json(responseText)+")");
            if(data.Result)
            {
                videoRecordJson = data.Records;
                $("#divVideoRecord").empty();
                var hcNetView =[];
                var width=0;
                var height = 0; 
                var padding = 20;
                if(pageSize==9)
                {
                    width = parseInt(DocumentWidth/3-100);
                    height = parseInt(DocumentWidth/3-100);
                }
                if(pageSize==4)
                {
                    width = parseInt(DocumentWidth/2-150);
                    height = parseInt(DocumentWidth/2-150);
                }
                if(pageSize==1)
                {
                    width = parseInt(DocumentWidth-500);
                    height = parseInt(DocumentWidth-500);
                }
                for(var i = 0;i<videoRecordJson.length;i++) 
                { 
                    var htmlstr="<div class='Layout_default'><object classid='clsid:0C011903-200D-447f-82E5-A04336113C21' codebase='VideoControll.dll#version=1,1,1,1' id='hcNetView"+i+"' style='width:"+ width +"px; height:"+ height +"px;'></object></div>";
                    $("#divVideoRecord").append(htmlstr);
                    hcNetView.push("hcNetView" + i);
                } 
                LoadVideo(hcNetView,videoRecordJson);               
            }
            else
            {
                $.HideMask();
                $.message.alert("提示信息",data.Message);
            }
          
        },
        error:function(XMLHttpRequest,textStatus,errorThrown)
        {
            $.HideMask();
            $.message.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function LoadVideo(hcNetView,videoRecordJson)
{
    try
    {
        document.getElementById(hcNetView[0]).StopView();
    } 
    catch(e)
    {
        if (confirm("需要下载视频控件才能显示视频，您确定要下载安装吗？")) 
        {
            DoClickByElementId("downHCNetView");
        }
    }
    for(var j=0;j<hcNetView.length;j++)
    {
        document.getElementById(hcNetView[j]).标题栏显示 = true;
        document.getElementById(hcNetView[j]).标题文本 = videoRecordJson[j]["名称"];
        document.getElementById(hcNetView[j]).设备序列号 = videoRecordJson[j]["设备序列号"];
        document.getElementById(hcNetView[j]).IP地址 = videoRecordJson[j]["IP地址"];
        document.getElementById(hcNetView[j]).数据传输端口 = videoRecordJson[j]["端口号"];
        document.getElementById(hcNetView[j]).视频通道号 = videoRecordJson[j]["视频通道"];
        document.getElementById(hcNetView[j]).用户名 = videoRecordJson[j]["用户名"];
        document.getElementById(hcNetView[j]).密码 = videoRecordJson[j]["密码"];
        if(videoRecordJson[j]["启用音频"] == "是")
        {
            document.getElementById(hcNetView[j]).启用语音功能 = true;
            document.getElementById(hcNetView[j]).音频通道号 = videoRecordJson[j]["音频通道"];
        }
        if(videoRecordJson[j]["是否定焦"] == "是")
        {
            document.getElementById(hcNetView[j]).定焦镜头 =true;
        }
        document.getElementById(hcNetView[j]).开启调试窗口 = false;
        document.getElementById(hcNetView[j]).LoadPropertyParams();
        document.getElementById(hcNetView[j]).DisplayInitParams();
    }
    $.HideMask();
}

function DoClickByElementId(objID) 
{
    var obj = document.getElementById(objID);
    if (document.all)
    {
        // For IE 
        obj.click();
    } 
    else if (document.createEvent) 
    {
        //FOR DOM2
        var ev = document.createEvent('MouseEvents');
        ev.initEvent('click', false, true);
        obj.dispatchEvent(ev);
    }
}