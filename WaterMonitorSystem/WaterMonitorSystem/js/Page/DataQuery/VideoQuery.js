// JScript 文件
var dicDeviceAppGroups = {};
//表格数据
var tableData = [];
var comboBoxData=[];
//当前登录操作员管理ID
var mnId = "";
//用于存储所有级别的节点
var levelJson;
//左侧树形选中节点的ID
var currentSelId;
//是否显示直接子管理
var isContainsChildManage = true;
//用于存储Excel文件的路径
var _excelURL="";


var treedata=[{
    "id": 1,
    "text": "天水市图像远程监测系统",
    "state": "open",
    "children": [{
        "id": 11,
        "text": "娘娘坝荣光村视频站"
    },{
        "id": 12,
        "text": "中梁水务站视频站"
    },{
        "id": 13,
        "text": "麦积藉河视频站"
    },{
        "id": 14,
        "text": "东柯河视频站"
    },{
        "id": 15,
        "text": "柳弯村视频站"
    },{
        "id": 16,
        "text": "鸳鸯镇渭河视频站"
    }]
}];

$(document).ready(function(){
    var defaultTheme = $.cookie("psbsTheme");
    if(defaultTheme != null && defaultTheme != "default")
    {
        var link = $(document).find('link:first');
	    link.attr('href', '../App_Themes/easyui/themes/'+defaultTheme+'/easyui.css');
	}
    GetSystemInfo();
    $("#divAreaTree").tree({
        onSelect: function (node) {
            if (node.id == 1) {
                window.location.reload();
            } else {
                $("#divVideQuery").empty();
                var htmlstr="<object classid='clsid:0C011903-200D-447f-82E5-A04336113C21' codebase='VideoControll.dll#version=1,1,1,1' id='hcNetView"+node.id+"' style='width: 95%; height: 90%;position:absolute;z-index:1;left:20px'></object>"
                $("#divVideQuery").append(htmlstr);
                startview(node.id);
            }
        }
    })
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
                $("#divAreaTree").tree({
                    data: treedata
                });
            }
        },
        error:function (XMLHttpRequest, textStatus, errorThrown)
        {
            alert(errorThrown + "\r\n" + XMLHttpRequest.responseText);
        }
    });
}

function DoClickByElementId(objID) {
    var obj = document.getElementById(objID);
    if (document.all) {
        // For IE 
        obj.click();
    } else if (document.createEvent) {
        //FOR DOM2
        var ev = document.createEvent('MouseEvents');
        ev.initEvent('click', false, true);
        obj.dispatchEvent(ev);
    }
}

function onPageLoad() {
    var hcNetViewObj11 = document.getElementById("hcNetView11");
    var hcNetViewObj12 = document.getElementById("hcNetView12");
    var hcNetViewObj13 = document.getElementById("hcNetView13");
    var hcNetViewObj14 = document.getElementById("hcNetView14");
    var hcNetViewObj15 = document.getElementById("hcNetView15");
    var hcNetViewObj16 = document.getElementById("hcNetView16");
    try {
        hcNetViewObj11.StopView();
        hcNetViewObj12.StopView();
        hcNetViewObj13.StopView();
        hcNetViewObj14.StopView();
        hcNetViewObj15.StopView();
        hcNetViewObj16.StopView();
    }
    catch (e) {
        if (confirm("需要下载视频控件才能显示视频，您确定要下载安装吗？")) {
            DoClickByElementId("downHCNetView");
        }
    }
    hcNetViewObj11.设备序列号 = "974C715303705";
    hcNetViewObj11.IP地址 = "111.11.166.141";
    hcNetViewObj11.数据传输端口 = 8100;
    hcNetViewObj11.视频通道号 = 0;
    hcNetViewObj11.用户名 = "admin";
    hcNetViewObj11.密码 = "123456";
    hcNetViewObj11.启用语音功能 = false;
    hcNetViewObj11.开启调试窗口 = false;
    hcNetViewObj11.定焦镜头 = false;
    hcNetViewObj11.LoadPropertyParams();
    hcNetViewObj11.DisplayInitParams();
    hcNetViewObj11.StartView();
    ////------------------------------
    hcNetViewObj12.设备序列号 = "2780A15303707";
    hcNetViewObj12.IP地址 = "111.11.166.142";
    hcNetViewObj12.数据传输端口 = 8100;
    hcNetViewObj12.视频通道号 = 0;
    hcNetViewObj12.用户名 = "admin";
    hcNetViewObj12.密码 = "123456";
    hcNetViewObj12.启用语音功能 = false;
    hcNetViewObj12.开启调试窗口 = false;
    hcNetViewObj12.定焦镜头 = false;
    hcNetViewObj12.LoadPropertyParams();
    hcNetViewObj12.DisplayInitParams();
    hcNetViewObj12.StartView();
    ////----------------------------------
    hcNetViewObj13.设备序列号 = "0913215303704";
    hcNetViewObj13.IP地址 = "111.11.166.147";
    hcNetViewObj13.数据传输端口 = 8100;
    hcNetViewObj13.视频通道号 = 0;
    hcNetViewObj13.用户名 = "admin";
    hcNetViewObj13.密码 = "123456";
    hcNetViewObj13.启用语音功能 = false;
    hcNetViewObj13.开启调试窗口 = false;
    hcNetViewObj13.定焦镜头 = false;
    hcNetViewObj13.LoadPropertyParams();
    hcNetViewObj13.DisplayInitParams();
    hcNetViewObj13.StartView();
    ////----------------------------------
    hcNetViewObj14.设备序列号 = "B7D1F15303703";
    hcNetViewObj14.IP地址 = "111.11.166.148";
    hcNetViewObj14.数据传输端口 = 8100;
    hcNetViewObj14.视频通道号 = 0;
    hcNetViewObj14.用户名 = "admin";
    hcNetViewObj14.密码 = "123456";
    hcNetViewObj14.启用语音功能 = false;
    hcNetViewObj14.开启调试窗口 = false;
    hcNetViewObj14.定焦镜头 = false;
    hcNetViewObj14.LoadPropertyParams();
    hcNetViewObj14.DisplayInitParams();
    hcNetViewObj14.StartView();
    ////--------------------------------
    hcNetViewObj15.设备序列号 = "EA52115303706";
    hcNetViewObj15.IP地址 = "111.11.166.149";
    hcNetViewObj15.数据传输端口 = 8100;
    hcNetViewObj15.视频通道号 = 0;
    hcNetViewObj15.用户名 = "admin";
    hcNetViewObj15.密码 = "123456";
    hcNetViewObj15.启用语音功能 = false;
    hcNetViewObj15.开启调试窗口 = false;
    hcNetViewObj15.定焦镜头 = false;
    hcNetViewObj15.LoadPropertyParams();
    hcNetViewObj15.DisplayInitParams();
    hcNetViewObj15.StartView();
    ////-----------------------------------
    hcNetViewObj16.设备序列号 = "EA52115303706";
    hcNetViewObj16.IP地址 = "111.11.166.149";
    hcNetViewObj16.数据传输端口 = 8100;
    hcNetViewObj16.视频通道号 = 0;
    hcNetViewObj16.用户名 = "admin";
    hcNetViewObj16.密码 = "123456";
    hcNetViewObj16.启用语音功能 = false;
    hcNetViewObj16.开启调试窗口 = false;
    hcNetViewObj16.定焦镜头 = false;
    hcNetViewObj16.LoadPropertyParams();
    hcNetViewObj16.DisplayInitParams();
    hcNetViewObj16.StartView();
}

function onPageUnLoad() {
    var hcNetViewObj11 = document.getElementById("hcNetView11");
    var hcNetViewObj12 = document.getElementById("hcNetView12");
    var hcNetViewObj13 = document.getElementById("hcNetView13");
    var hcNetViewObj14 = document.getElementById("hcNetView14");
    var hcNetViewObj15 = document.getElementById("hcNetView15");
    var hcNetViewObj16 = document.getElementById("hcNetView16");
    try {
        hcNetViewObj11.StopView();
        hcNetViewObj12.StopView();
        hcNetViewObj13.StopView();
        hcNetViewObj14.StopView();
        hcNetViewObj15.StopView();
        hcNetViewObj16.StopView();
    }
    catch (e) {

    }
}

function startview(treeid)
{
    var hcNetViewObj11 = document.getElementById("hcNetView11");
    var hcNetViewObj12 = document.getElementById("hcNetView12");
    var hcNetViewObj13 = document.getElementById("hcNetView13");
    var hcNetViewObj14 = document.getElementById("hcNetView14");
    var hcNetViewObj15 = document.getElementById("hcNetView15");
    var hcNetViewObj16 = document.getElementById("hcNetView16");
    if (treeid==11) {
        hcNetViewObj11.设备序列号 = "974C715303705";
        hcNetViewObj11.IP地址 = "111.11.166.141";
        hcNetViewObj11.数据传输端口 = 8100;
        hcNetViewObj11.视频通道号 = 0;
        hcNetViewObj11.用户名 = "admin";
        hcNetViewObj11.密码 = "123456";
        hcNetViewObj11.启用语音功能 = false;
        hcNetViewObj11.开启调试窗口 = false;
        hcNetViewObj11.定焦镜头 = false;
        hcNetViewObj11.LoadPropertyParams();
        hcNetViewObj11.DisplayInitParams();
        hcNetViewObj11.StartView();
    } else if (treeid == 12) {
        hcNetViewObj12.设备序列号 = "2780A15303707";
        hcNetViewObj12.IP地址 = "111.11.166.142";
        hcNetViewObj12.数据传输端口 = 8100;
        hcNetViewObj12.视频通道号 = 0;
        hcNetViewObj12.用户名 = "admin";
        hcNetViewObj12.密码 = "123456";
        hcNetViewObj12.启用语音功能 = false;
        hcNetViewObj12.开启调试窗口 = false;
        hcNetViewObj12.定焦镜头 = false;
        hcNetViewObj12.LoadPropertyParams();
        hcNetViewObj12.DisplayInitParams();
        hcNetViewObj12.StartView();
    } else if (treeid == 13) {
        hcNetViewObj13.设备序列号 = "0913215303704";
        hcNetViewObj13.IP地址 = "111.11.166.147";
        hcNetViewObj13.数据传输端口 = 8100;
        hcNetViewObj13.视频通道号 = 0;
        hcNetViewObj13.用户名 = "admin";
        hcNetViewObj13.密码 = "123456";
        hcNetViewObj13.启用语音功能 = false;
        hcNetViewObj13.开启调试窗口 = false;
        hcNetViewObj13.定焦镜头 = false;
        hcNetViewObj13.LoadPropertyParams();
        hcNetViewObj13.DisplayInitParams();
        hcNetViewObj13.StartView();
    } else if (treeid == 14) {
        hcNetViewObj14.设备序列号 = "B7D1F15303703";
        hcNetViewObj14.IP地址 = "111.11.166.148";
        hcNetViewObj14.数据传输端口 = 8100;
        hcNetViewObj14.视频通道号 = 0;
        hcNetViewObj14.用户名 = "admin";
        hcNetViewObj14.密码 = "123456";
        hcNetViewObj14.启用语音功能 = false;
        hcNetViewObj14.开启调试窗口 = false;
        hcNetViewObj14.定焦镜头 = false;
        hcNetViewObj14.LoadPropertyParams();
        hcNetViewObj14.DisplayInitParams();
        hcNetViewObj14.StartView();
    } else if (treeid == 15) {
        hcNetViewObj15.设备序列号 = "EA52115303706";
        hcNetViewObj15.IP地址 = "111.11.166.149";
        hcNetViewObj15.数据传输端口 = 8100;
        hcNetViewObj15.视频通道号 = 0;
        hcNetViewObj15.用户名 = "admin";
        hcNetViewObj15.密码 = "123456";
        hcNetViewObj15.启用语音功能 = false;
        hcNetViewObj15.开启调试窗口 = false;
        hcNetViewObj15.定焦镜头 = false;
        hcNetViewObj15.LoadPropertyParams();
        hcNetViewObj15.DisplayInitParams();
        hcNetViewObj15.StartView();
    } else if (treeid == 16) {
        hcNetViewObj16.设备序列号 = "EA52115303706";
        hcNetViewObj16.IP地址 = "111.11.166.149";
        hcNetViewObj16.数据传输端口 = 8100;
        hcNetViewObj16.视频通道号 = 0;
        hcNetViewObj16.用户名 = "admin";
        hcNetViewObj16.密码 = "123456";
        hcNetViewObj16.启用语音功能 = false;
        hcNetViewObj16.开启调试窗口 = false;
        hcNetViewObj16.定焦镜头 = false;
        hcNetViewObj16.LoadPropertyParams();
        hcNetViewObj16.DisplayInitParams();
        hcNetViewObj16.StartView();
    }
    
}
