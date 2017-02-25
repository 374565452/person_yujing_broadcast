// JScript 文件

//登录标示
var loginIdentifer = window.parent.guid;
var Title = "";
var Id ="";
var unitname="";

$(document).ready(function () 
{
    $.ShowMask("数据加载中，请稍等……");
    GetSystemInfo();  
    
    $("#ccb_VideoFocus").combobox({data:[{'id':"0",'text':"是"},{'id':"1",'text':"否"}]}); 
    $("#cbb_StartVoice").combobox({data:[{'id':"0",'text':"是"},{'id':"1",'text':"否"}]});   
    $("#cbb_VideoFactory").combobox({data:[{'id':"1",'text':"海康威视"},{'id':"2",'text':"华迈视频"}]});
});

//从服务器取得系统运行状态信息
function GetSystemInfo() 
{
    $.ajax(
    {
        url: "../WebServices/SystemService.asmx/GetSystemStateInfo",
        type: "GET",
        data: { "loginIdentifer": window.parent.guid },
        dataType: "text",
        cache: false,
        success: function(responseText)
        {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result)//登录成功
            {
                mnId = data.SysStateInfo.当前登录操作员管理ID;
                manageNodeLoaded = false;
                deviceNodeLoaded = false;
                LoadWaterUserTree("VideoAreaTree", mnId, false, false); 
                TreeBindSelect();
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) 
        {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

//点击左侧树形重新加载右侧Combobox
function TreeBindSelect() 
{
    $('#VideoAreaTree').tree({
        onSelect: function (node) {
            $("#cbb_VideoNameCombobox").combobox({data:[]});
            if (node.attributes["nodeType"] == "manage") 
            {
                if(node.attributes["manage"]["级别名称"]=="村庄")
                {
                    $("#btn_Add").linkbutton({
                        disabled: false
                    });
                    unitname=node.attributes["manage"].名称;
                }
                else
                {
                    $("#btn_Add").linkbutton({
                        disabled: true
                    });
                }
             }
            if (node.id.indexOf("cz") >= 0) {
                villageid = node.attributes["mid"];
                LoadVideoInfo("../WebServices/VideoInfoService.asmx/GetVideoRecordsByVillageId", { "loginIdentifer": loginIdentifer, "villageId": villageid, "isExport": false });
            }
        }
    });
}

function LoadVideoInfo(url,data)
{
    $("#tbVideoInfos").datagrid('loadData',[]);   
    $.ajax(
    {
        url:url,
        type:"GET",
        data:data,
        dataType:"text",
        cache:false,
        success:function(responseText)
        {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if(data.Result)
            {
                var tableData=[];
                var cbbData=[];
                var VideoJson = data.Records;
                for(var i in VideoJson)
                {
                    var tableRow = {};
                    var cbbItem = {};
                    cbbItem["id"] = VideoJson[i]["ID"];
                    cbbItem["text"] = VideoJson[i]["名称"];
                    tableRow["ID"] = VideoJson[i]["ID"];
                    tableRow["Caption"] = VideoJson[i]["单位"];
                    tableRow["videoName"] = VideoJson[i]["名称"];
                    tableRow["IPAddress"] = VideoJson[i]["IP地址"];
                    tableRow["PortNum"] = VideoJson[i]["端口号"];
                    tableRow["videoFactory"] = VideoJson[i]["视频厂家"];
                    tableRow["videoNum"] = VideoJson[i]["设备序列号"];
                    tableRow["videoPanel"] = VideoJson[i]["视频通道"];
                    tableRow["OnFocus"] = VideoJson[i]["是否定焦"];
                    tableRow["userName"] = VideoJson[i]["用户名"];
                    tableRow["Password"] = VideoJson[i]["密码"];
                    tableRow["Edit"] = "<img src='../Images/Detail.gif' onclick='EditVideoRecord(" + VideoJson[i]["ID"]+")'/>";
                    tableRow["Cancel"] = "<img src='../Images/Delete.gif' onclick = 'DeleteVideoRecord(" + VideoJson[i]["ID"]+")'/>";
                    tableData.push(tableRow);
                    cbbData.push(cbbItem);                    
                }
                $.HideMask();
                $("#cbb_VideoNameCombobox").combobox({data: cbbData});
                $("#tbVideoInfos").datagrid('loadData',tableData);    
            }
            else
            {
                $.HideMask();
                if(data.Message == "未登录")
                {
                    $.messager.alert("提示信息","您未登录，请先登录！");
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

function Btn_Query_Click()
{
    $("#tbVideoInfos").datagrid('loadData',[]);   
    $.ShowMask("数据加载中，请稍等……");
    var videoId = $("#cbb_VideoNameCombobox").combobox('getValue');
    $.ajax(
      {
            url: "../WebServices/VideoInfoService.asmx/GetVideoRecordByVideoId",
            type: "GET",
            data: {'loginIdentifer':loginIdentifer,'VideoId': videoId},
            dataType: "text",
            cache: false,
            success: function (responseText) 
            {
                var tableData=[];
                var data = eval("(" + $.xml2json(responseText) + ")");
                if(data.Result)
                {
                    var tableRow={};
                    var VideoJson = data.Records;
                    tableRow["ID"] = VideoJson["ID"];
                    tableRow["Caption"] = VideoJson["单位"];
                    tableRow["videoName"] = VideoJson["名称"];
                    tableRow["IPAddress"] = VideoJson["IP地址"];
                    tableRow["PortNum"] = VideoJson["端口号"];
                    tableRow["videoFactory"] = VideoJson["视频厂家"];
                    tableRow["videoNum"] = VideoJson["设备序列号"];
                    tableRow["videoPanel"] = VideoJson["视频通道"];
                    tableRow["OnFocus"] = VideoJson["是否定焦"];
                    tableRow["userName"] = VideoJson["用户名"];
                    tableRow["Password"] = VideoJson["密码"];
                    tableRow["Edit"] = "<img src='../Images/Detail.gif' onclick='EditVideoRecord(" + VideoJson["ID"]+")'/>";
                    tableRow["Cancel"] = "<img src='../Images/Delete.gif' onclick = 'DeleteVideoRecord(" + VideoJson["ID"]+")'/>";
                    tableData.push(tableRow);
                    $.HideMask();   
                    $("#tbVideoInfos").datagrid('loadData',tableData); 
                }
                else
                {
                    $.HideMask();
                    if(data.Message == "未登录")
                    {
                        $.messager.alert("提示信息","您未登录，请先登录！");
                        window.location.href = "../default.html";
                    }
                    else if(data.Message == "登录超时")
                    {
                        $.messager.alert("提示信息","登录超时，请重新登录！");
                        window.location.href = "../default.html";
                    }
                    else
                    {
                        $.messager.alert("提示信息", data.Message);
                    }   
                }
            },
            error:function (XMLHttpRequest, textStatus, errorThrown) 
            {
                $.HideMask();
                $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
            }
       });
        
}

function Btn_Add_Click()
{
    $("#AddVideoData").dialog({closed:false});
    Title = "添加视频信息";
    $("#txt_unitName").textbox('setText',unitname);
    $("#txt_userName").textbox('setText',"");
    $("#txt_Password").textbox('setText',"");
    $("#txt_Name").textbox('setText',"");
    $("#txt_Port").textbox('setText',"");
    $("#txt_IPAddress").textbox('setText',"");
    $("#cbb_VideoFactory").combobox('setText',"");
    $("#txt_Videopanel").textbox('setText',"");
    $("#ccb_VideoFocus").combobox('setText',"否");
    $("#txt_Num").textbox('setText',""); 
    $("#txt_VoicePanel").textbox('setText',"0");
    $("#cbb_StartVoice").combobox('setText',"否");  
}

function EditVideoRecord(videoId)
{
    Id = videoId;
    Title = "编辑视频信息";
    $.ajax(
      {
            url: "../WebServices/VideoInfoService.asmx/GetVideoRecordByVideoId",
            type: "GET",
            data: {'loginIdentifer':loginIdentifer,'VideoId': videoId},
            dataType: "text",
            cache: false,
            success: function (responseText) 
            {
                var data = eval("(" + $.xml2json(responseText) + ")");
                if(data.Result)
                {
                    var VideoJson = data.Records;
                    $("#txt_unitName").textbox('setText',VideoJson["单位"]);
                    $("#txt_userName").textbox('setText',VideoJson["用户名"]);
                    $("#txt_Password").textbox('setText',VideoJson["密码"]);
                    $("#txt_Name").textbox('setText',VideoJson["名称"]);
                    $("#txt_Port").textbox('setText',VideoJson["端口号"]);
                    $("#txt_IPAddress").textbox('setText',VideoJson["IP地址"]);
                    $("#cbb_VideoFactory").combobox('setText',VideoJson["视频厂家"]);
                    $("#txt_Videopanel").textbox('setText',VideoJson["视频通道"]);
                    $("#ccb_VideoFocus").combobox('setText',VideoJson["是否定焦"]);
                    $("#txt_Num").textbox('setText',VideoJson["设备序列号"]); 
                    $("#txt_VoicePanel").textbox('setText',VideoJson["音频通道"]);    
                    $("#cbb_StartVoice").combobox('setText',VideoJson["启用音频"]); 
                    $("#AddVideoData").dialog({closed:false});  
                }
                else
                {
                    $.messager.alert("提示信息", data.Message);   
                }
            },
            error:function (XMLHttpRequest, textStatus, errorThrown) 
            {
                $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
            }
       });   
}
function DeleteVideoRecord(videoId)
{
    if(confirm("是否删除视频信息"))
    {
        $.ajax(
        {
            url: "../WebServices/VideoInfoService.asmx/DeleteVideoInfoById",
            type: "GET",
            data: { 'loginIdentifer': loginIdentifer, 'VideoID': videoId},
            dataType: "text",
            cache: false,
            success: function (responseText) 
            {
                var data = eval("(" + $.xml2json(responseText) + ")");
                if (data.Result) 
                {
                    LoadVideoInfo("../WebServices/VideoInfoService.asmx/GetVideoRecordsByVillageId", { "loginIdentifer": window.parent.guid, "villageId": villageid, "isExport": false });
                    $.messager.alert("提示信息", "删除视频信息成功");
                }
                else
                {
                    $.messager.alert("提示信息", data.Message);
                } 
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) 
            {
                $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
            }
        });  
    }
    else
    {
        return;
    }

}
function Btn_OK_Save()
{
    $.ShowMask("数据加载中，请稍等……");
    var SelectNode = $("#VideoAreaTree").tree("getSelected");
    var VillageId = SelectNode.attributes["mid"];
    var strVideo ="{";
    if(Title == "添加视频信息")
    {
        strVideo += "'管理ID" + "':'" + VillageId + "','";
        strVideo += "名称" +"':'" + $("#txt_Name").textbox("getText") +"','" ;
        strVideo += "IP地址" + "':'"+ $("#txt_IPAddress").textbox("getText") +"','";
        strVideo += "端口号" + "':'" + $("#txt_Port").textbox("getText") + "','";
        strVideo += "用户名" +"':'" +  $("#txt_userName").textbox("getText") + "','";
        strVideo += "密码" + "':'" + $("#txt_Password").textbox("getText") + "','";
        strVideo += "视频厂家" + "':'" +  $("#cbb_VideoFactory").combobox("getText") + "','";
        strVideo += "视频通道" + "':'" +  $("#txt_Videopanel").textbox("getText") + "','";
        strVideo += "设备序列号" + "':'" +  $("#txt_Num").textbox("getText") + "','";
        strVideo += "是否定焦" + "':'" + $("#ccb_VideoFocus").combobox("getText") + "','";
        strVideo += "音频通道" + "':'" + $("#txt_VoicePanel").textbox("getText") + "','";
        strVideo += "启用音频" + "':'" + $("#cbb_StartVoice").combobox("getText") + "'";
        strVideo += "}";
        $.ajax(
        {
            url:"../WebServices/VideoInfoService.asmx/AddVideo",
            type:"GET",
            data:{'loginIdentifer':loginIdentifer,'VideoJson':strVideo},
            dataType:"text",
            cache:false,
            success:function(responseText)
            {
                var data = eval("(" + $.xml2json(responseText) + ")");
                if(data.Result)
                { 
                    $("#AddVideoData").dialog({closed:true});                     
                    LoadVideoInfo("../WebServices/VideoInfoService.asmx/GetVideoRecordsByVillageId", { "loginIdentifer": window.parent.guid, "villageId": villageid, "isExport": false });
                    $.HideMask();
                    $.messager.alert("提示信息", data.Message);
                }
                else
                {
                    $("#AddVideoData").dialog({closed:true}); 
                    $.HideMask(); 
                    $.messager.alert("提示信息", data.Message);
                   
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) 
            {
                $.HideMask();
                $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
            }
        });
    }
    if(Title == "编辑视频信息")
    {
        strVideo += "'ID" + "':'" + Id + "','";
        strVideo += "管理ID" + "':'" + VillageId + "','";
        strVideo += "名称" +"':'" + $("#txt_Name").textbox("getText") +"','" ;
        strVideo += "IP地址" + "':'"+ $("#txt_IPAddress").textbox("getText") +"','";
        strVideo += "端口号" + "':'" + $("#txt_Port").textbox("getText") + "','";
        strVideo += "用户名" +"':'" +  $("#txt_userName").textbox("getText") + "','";
        strVideo += "密码" + "':'" + $("#txt_Password").textbox("getText") + "','";
        strVideo += "视频厂家" + "':'" +  $("#cbb_VideoFactory").combobox("getText") + "','";
        strVideo += "视频通道" + "':'" +  $("#txt_Videopanel").textbox("getText") + "','";
        strVideo += "设备序列号" + "':'" +  $("#txt_Num").textbox("getText") + "','";
        strVideo += "是否定焦" + "':'" + $("#ccb_VideoFocus").combobox("getText") + "','";
        strVideo += "音频通道" + "':'" + $("#txt_VoicePanel").textbox("getText") + "','";
        strVideo += "启用音频" + "':'" + $("#cbb_StartVoice").combobox("getText") + "'";
        strVideo += "}";
        $.ajax(
        {
            url:"../WebServices/VideoInfoService.asmx/ModifyVideo",
            type:"GET",
            data:{'loginIdentifer':loginIdentifer,'VideoJson':strVideo},
            dataType:"text",
            cache:false,
            success:function(responseText)
            {
                var data = eval("(" + $.xml2json(responseText) + ")");
                if(data.Result)
                {
                    $("#AddVideoData").dialog({closed:true}); 
                    //更新页面待确定
                    LoadVideoInfo("../WebServices/VideoInfoService.asmx/GetVideoRecordsByVillageId", { "loginIdentifer": window.parent.guid, "villageId": villageid, "isExport": false });
                    $.HideMask();
                    $.messager.alert("提示信息",data.Message);
                }
                else
                {
                    $("#AddVideoData").dialog({closed:true}); 
                    $.HideMask();
                    $.messager.alert("提示信息", data.Message);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) 
            {
                $.HideMask();
                $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
            }
        });
    }
    Title ="";
}
function Btn_Cancel_Exit()
{
    $("#AddVideoData").dialog({closed:true});
}
