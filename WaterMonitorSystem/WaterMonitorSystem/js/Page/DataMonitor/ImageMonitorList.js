// JScript 文件
//登录标识
var loginIdentifer=window.parent.guid;
//操作标识
var operateIdentifer="";
//查询图片结果数量
var totalRecord=0;
//设置图片列表中图片的宽、高
var imgWidth=200;
var imgHeight=200;
var attrColWidth=100;
var pagesize=0;
//定时更新变量
var pollingImgDatas = null;
var monitorRealName = "";

$(document).ready(function () {
    $.ShowMask("数据加载中，请稍等……");
    var defaultTheme = $.cookie("psbsTheme");
    if (defaultTheme != null && defaultTheme != "default") {
        var link = $(document).find('link:first');
        link.attr('href', '../App_Themes/easyui/themes/' + defaultTheme + '/easyui.css');
    }
    GetSystemInfo();
    LoadComboboxData();
    LoadQueryPhotos();

    $("#cbb_DevCombobox").combobox({
        onSelect: function (rec) {
            var node = $('#divAreaTree').tree('find', "dn_" + rec.id);
            $('#divAreaTree').tree('uncheck', $("#divAreaTree").tree('getRoot').target).tree('check', node.target);
        }
    });
    
    setPageSize();
 });
 
 //设置pagesize
 function setPageSize()
 {
      //获取图片列表区域的宽、高
    var imgAreaWidth=$("#imagesArea")[0].clientWidth;
    var imgAreaHeight=$("#imagesArea")[0].clientHeight;
    //根据列表区域大小、图片大小计算pagesize、及pageList
    var rowCount=(imgAreaHeight%imgHeight!=0)?(Math.floor(imgAreaHeight/imgHeight)+1):(imgAreaHeight/imgHeight);
    
    pagesize=(Math.floor(imgAreaWidth/(imgWidth+attrColWidth)))*rowCount;
    var pagelist=[pagesize,pagesize*2,pagesize*3];
    
    $('#pp').pagination({
      pageSize:pagesize,
      pageList:pagelist,
      onSelectPage: function(pageNumber, pageSize){//选择相应的页码时刷新显示内容列表。
		    $('#pp').pagination('refresh',{
			    pageNumber:pageNumber,
			    pageSize:pageSize
		    });
		   $(this).pagination('loading');
           QueryCurrentPageData(pageNumber,pageSize);
           $(this).pagination('loaded');
	    }
     });
 }

 function RefreshPage()
 {
      Btn_Query_Click();
 }

//从服务器取得系统运行状态信息
function GetSystemInfo() {
    $.ajax(
    {
        url: "../WebServices/SystemService.asmx/GetSystemStateInfo",
        type: "GET",
        data: { "loginIdentifer": loginIdentifer },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result)//登录成功
            {
                mnId = data.SysStateInfo.当前登录操作员管理ID;
                manageNodeLoaded = false;
                deviceNodeLoaded = false;
                monitorRealName = data.SysStateInfo.监测点级别名称;
                $('#divContainer').layout('panel', 'west').panel({ title: monitorRealName + "列表" });
                $("#wellname").text(monitorRealName);
                LoadTree("divAreaTree", mnId, true, true); 
                $.HideMask();
                pollingImgDatas = window.setInterval("RefreshPage()", 300000);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}
//页面加载完后查询图像
function LoadQueryPhotos(){
    if (!treeLoaded) {
        window.setTimeout("LoadQueryPhotos()", 500);
        return;
    }
    Btn_Query_Click();
}

//查询的按钮事件
function Btn_Query_Click() {
     
    operateIdentifer="";
    totalRecord=0;
    $.ShowMask("数据加载中，请稍等……");
    
    var devIDs="";
    var treeObj = $("#divAreaTree");
    var checkedNodes = treeObj.tree('getChecked');
    for (var i = 0; i < checkedNodes.length; i++) {
        var checkedNode = checkedNodes[i];
        if (checkedNode.attributes["nodeType"] == "manage") {
            continue;
        }
        if (devIDs != "") {
            devIDs += ",";
        }
        devIDs += checkedNode.attributes["did"];
    }
    
    $.ajax(
    {
        url: "../WebServices/DeviceMonitorService.asmx/GetDeviceRealTimeImagesCount",
        type: "GET",
        data: { 'loginIdentifer': loginIdentifer, 'devIDs': devIDs},
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                totalRecord = data.Count;
                operateIdentifer = data.Guid;
                
                if (totalRecord == 0) {
                    $.messager.alert("提示信息", "查询结果为空");
                }
                else {
                    $('#pp').pagination({
                    total:totalRecord,
                    pageSize:pagesize,
                    onSelectPage:function(pageNumber, pageSize){
                          $('#pp').pagination('refresh',{
			               pageNumber:pageNumber,
			               pageSize:pageSize
		                  });
                        $(this).pagination('loading');
                        QueryCurrentPageData(pageNumber,pageSize);
                        $(this).pagination('loaded');
                     }
                     });    
                    //获取pagination中的pagenumber、pageSize值
                    var pageOpt=$("#pp").pagination("options");
                    QueryCurrentPageData(pageOpt.pageNumber,pageOpt.pageSize);
                }
            }
            else {
                if (data.Message == "未登录" || data.Message == "登录超时") {
                    clearInterval(pollingImgDatas);
                }
                else
                {
                    $.messager.alert("提示信息", data.Message);
                }
            }
            $.HideMask();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.HideMask();
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}
    
 function  QueryCurrentPageData(pageNum,pageSize)
 {
    var startIndex=(pageNum-1)*pageSize+1;
    $.ajax({
        url: "../WebServices/DeviceMonitorService.asmx/GetDeviceRealTimeImageDatas",
        data: { "loginIdentifer": loginIdentifer,"operateIdentifer":operateIdentifer,"startIndex":startIndex,"count":pageSize},
        type: "Get",
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                  var jsaAllData=data.RealDatas;
                    if (jsaAllData.length == 0) {
                        $.HideMask();
                        alert("没有测点图片！");
                        return;
                    }
                     var url="";
                     var time="";
                     var id="";
                     var name="";
                     var manageName="";

                    //将图片显示到datagrid列表中
                     var htmlstr="";
                    for (var i = 0; i < jsaAllData.length; i++) 
                    {
                        url="";
                        time="";
                        id="";
                        name="";
                        manageName="";
                                               
                        url=jsaAllData[i].PhotoInfo["Url"];
                        time=jsaAllData[i].PhotoInfo["Time"];
                        id=jsaAllData[i].ID;
                        name=jsaAllData[i].Name;
                        manageName=jsaAllData[i].manageName;
                        var displayParam=data.DisplayParam;
                                              
                        htmlstr += "<div class='layout_default'>"+
                        "<div id='dataContainer'><table><tr><td>"+
                        "<div class='image_container'>"+
                        "<a href='" + (url==""?("javascript:void(0);"+(i+1)):url)+ "' rel='lightbox[ostec]' title='" + time + "&nbsp;&nbsp;"+name + "'>"+
                        "<img src='" + url + "' alt='" + time + "' width="+imgWidth+"px  height="+imgHeight+"px />"+
                        "</a>"+
                        "</div></td><td class='attrColumn' width="+attrColWidth+"px><table id='attrtable'  width=100% height="+
                        imgHeight+"px><tr><td ><p id='devName'>"+name+"</p>"+manageName+"</td></tr>";
                        //匹配显示的参数
                        var paramcount=0;
                        for(var j=0;j< displayParam.length;j++)
                        {
                           if(paramcount<2)
                           {
                               for(var k=0;k<jsaAllData[i].RealData.length;k++)
                               {
                                   var param=jsaAllData[i].RealData[k];
                                   if(param.Name==displayParam[j])
                                   {
                                       paramcount=paramcount+1;
                                       htmlstr +="<tr><td>"+displayParam[j]+":"+param.Value+"</td></tr>";
                                       break;
                                   }
                                }
                            }
                            else
                            {
                               break;
                            }
                        }
                         htmlstr +="<tr><td id='photoTime'>"+ time +"</td></tr>"
                         +"<tr><td ><input type='button'  onclick='TakePhoto("+id+")' value='拍照'></td></tr>"+
                         "</table></td></tr></table></div>"+
                        "</div>";
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
                
                $.HideMask();
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
  
function LoadComboboxData() {
    if (!deviceNodeLoaded) {
        window.setTimeout("LoadComboboxData()", 500);
        return;
    }
    comboBoxData = [];
    for (i = 0; i < deviceJson.length; i++) {
        var devObj = {};
        devObj["id"] = deviceJson[i].ID;
        devObj["text"] = deviceJson[i].名称;
        comboBoxData.push(devObj);
    }
    $("#cbb_DevCombobox").combobox({
        data: comboBoxData
    });
}

//执行拍照命令
function TakePhoto( devId)
{   
    //清空定时器
    clearInterval(pollingImgDatas);
    $.ajax({
        type: "GET",
        url: "../WebServices/DeviceMonitorService.asmx/ControlDevice",
        data: { 'loginIdentifer': window.parent.guid, 'ctrlName': "拍照", 'devID': devId },
        contentType: "content",
        dataType: "text",
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
               $.messager.alert("拍照成功");
               //更新页面
               Btn_Query_Click();
               //启动定时更新
               pollingImgDatas=window.setInterval("RefreshPage()", 300000);
            }
            else
            {
              $.messager.alert("提示信息", data.Message);
               //更新页面
               Btn_Query_Click();
               //启动定时更新
               pollingImgDatas=window.setInterval("RefreshPage()", 300000);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.reponseText);
        }
    });
}