//// JScript 文件
//var dicDeviceAppGroups = {};
////表格数据
//var tableData = [];
//var comboBoxData=[];
//当前登录操作员管理ID
var mnId = "";
////操作类型
//var operateType = "Add";
////用于存储所有级别的节点
//var levelJson;
////左侧树形选中节点的管理ID
//var currentSelManageId;
////用于存储级别信息表中倒数第二级的级别ID(即管理的最后一级别)
//var lastLevelId="";
////是否显示直接子管理
//var isContainsChildManage = true;
////用于存储当前编辑的管理的ID
//var operateManageId;

$(document).ready(function(){
    var defaultTheme = $.cookie("psbsTheme");
    if(defaultTheme != null && defaultTheme != "default")
    {
        var link = $(document).find('link:first');
	    link.attr('href', '../App_Themes/easyui/themes/'+defaultTheme+'/easyui.css');
	}
	InitTimeControl();
    GetSystemInfo();
});

//初始化时间框
function InitTimeControl()
{
    var e=new Date();
    $("#txt_StartTime").val(e.Format("yyyy-MM-dd 00:00"));
    $("#txt_EndTime").val(e.Format("yyyy-MM-dd HH:mm"));
}

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
//                manageNodeLoaded = false;
//                deviceNodeLoaded = false;
// 	            LoadTree("areaTree", mnId, false, true);
 	            LoadUserTree("areaTree",mnId,true,true);
//	            LoadTableData();
//	            LoadComboboxData();
//	            //绑定所属级别下拉框
//                LoadLevelComboboxData();
//	            TreeBindSelect();
//	            TableCellDbClick();    
            }
        },
        error:function (XMLHttpRequest, textStatus, errorThrown)
        {
            alert(errorThrown + "\r\n" + XMLHttpRequest.responseText);
        }
    });
}
