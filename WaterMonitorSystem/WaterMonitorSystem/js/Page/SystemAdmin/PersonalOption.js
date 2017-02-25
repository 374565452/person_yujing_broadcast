// JScript 文件
//--------------------------声明全局变量-------------------------------
//用于存储当前选中的单元格的ID
var currentSelTd=null;
//用于存储规则ID
var ruleId=null;

$(document).ready(function(){
    var defaultTheme = $.cookie("psbsTheme");
    if(defaultTheme != null && defaultTheme != "default")
    {
        var link = $(document).find('link:first');
	    link.attr('href', '../App_Themes/easyui/themes/'+defaultTheme+'/easyui.css');
    }
    $.ShowMask("数据加载中，请稍等……");
	
	InitPersonRule("实时列表","tb_Monitor_Content","chk_AllMonitor");
	InitPersonRule("历史记录","tb_History_Content","chk_AllHistory");
	InitPersonRule("事件记录","tb_Event_Content","chk_AllEvent");
	InitPersonRule("报表统计","tb_Report_Content","chk_AllReport");
	$.HideMask();

});

function InitPersonRule(_ruleType,_controlObj,_chkBoxControl)
{
    $.ajax(
    { 
        url:"../WebServices/PersonOptionService.asmx/GetPersonOptionColunms",
        type:"GET",
        data:{"loginIdentifer":window.parent.guid,"ruleType":_ruleType},
        dataType:"text",
        cache:false,
        success:function(responseText)
        {
            var data = eval("("+$.xml2json(responseText)+")");
            if(data.Result)//登录成功
            {
                //获取tb_Content 的table对象
                var tbContentObj=$("#"+_controlObj);
                tbContentObj.html("");
                //用于计算选中的复选框的个数
                var checkCount=0;
                var ruleArray=data.PersonOptionColunms;
                for(var i=0;i<ruleArray.length;i++)
                {
                    ruleId=ruleArray[i].ID;
                    var isCheck="";
                    if(ruleArray[i].是否显示)
                    {
                        isCheck="checked='checked'";
                        checkCount++;
                    }
                    //var trNew = $("<tr id=' "+i+"' onclick='SelectMove_Monitor(td_"+i+");'></tr>");
                    var trNew = $("<tr id='tr_"+i+"' onclick='SelectMove_Monitor(this);'></tr>");
                    var tdNew = $("<td id='td_"+i+"' align='left' height='25px'><input type='checkbox' id='cb_"+i+"'"+isCheck+" style='vertical-align: middle'/><span>"+ruleArray[i].名称+"</span></td>");
                    tdNew.appendTo(trNew);
                    trNew.appendTo(tbContentObj);
                }
                if(checkCount==ruleArray.length)
                {
                    document.getElementById(_chkBoxControl).checked=true;
                }
            }
        },
        error:function (XMLHttpRequest, textStatus, errorThrown)
        {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function SelectMove_Monitor(_trObject)
{
    if(currentSelTd!=null)
    {
        currentSelTd.style.backgroundColor="#F5F9FF";
    }
    currentSelTd=_trObject;
    _trObject.style.backgroundColor="red";
    
     //获取当前的行号
    var row = currentSelTd.parentElement.rowIndex;
    //得到table的总行数
    var rowAll=_trObject.parentElement.parentElement.rows.length;
    if(row==0)
    {
        $("#btn_Up").attr("src","../Images/UpNo.png");
        $("#btn_Up").attr("disabled",true);
        $("#btn_Down").attr("src","../Images/Down.png");
        $("#btn_Down").attr("disabled",false);
    }
    else if(row==rowAll-1)
    {
        $("#btn_Up").attr("src","../Images/Up.png");
        $("#btn_Up").attr("disabled",false);
        $("#btn_Down").attr("src","../Images/DownNo.png");
        $("#btn_Down").attr("disabled",true);
    }
    else
    {
        $("#btn_Up").attr("src","../Images/Up.png");
        $("#btn_Up").attr("disabled",false);
        $("#btn_Down").attr("src","../Images/Down.png");
        $("#btn_Down").attr("disabled",false);
    }
}

//全选的复选框选中或取消事件
function CheckedAllValueName(_chkControlId,_tbControlId)
{
//    var checkObj=document.getElementById("chk_AllMonitor");
//    var checkControls=$("#tb_Monitor_Content").find("input[type=checkbox]");
    var checkObj=document.getElementById(_chkControlId);
    var checkControls=$("#"+_tbControlId).find("input[type=checkbox]");
    for(var i=0;i<checkControls.length;i++)
    {
        checkControls[i].checked=checkObj.checked;
    }
}

function btn_UP_Click(_tbContent)
{
    if(currentSelTd==null)
    {
        alert("请选中要移动的量！");
    }
    //获取当前的行号
    var row = currentSelTd.rowIndex;
    //得到table的总行数
    var count=document.getElementById(_tbContent).rows.length;

    //上一行的名称列值
    var htm1=document.getElementById(_tbContent).rows[row-1].cells[0].innerHTML;
    var ishtm1=$("#"+_tbContent+" tr:eq("+(row-1)+") td:eq(0) input")[0].checked;
    
    // 当前行的名称列值
    var htm2=currentSelTd.innerHTML;
    var ishtm2=$("#"+_tbContent+" tr:eq("+row+") td:eq(0) input")[0].checked;
    
    document.getElementById(_tbContent).rows[row-1].cells[0].innerHTML=htm2;
    $("#"+_tbContent+" tr:eq("+(row-1)+") td:eq(0) input")[0].checked=ishtm2;
    
    currentSelTd.cells[0].innerHTML=htm1;
    $("#"+_tbContent+" tr:eq("+row+") td:eq(0) input")[0].checked=ishtm1;
    //将选中的行也上移
    SelectMove_Monitor($("#"+_tbContent).find("#tr_"+(row-1))[0]);
}

function btn_Down_Click(_tbContent)
{
    if(currentSelTd==null)
    {
        alert("请选中要移动的量！");
    }
    //获取当前的行号
    var row = currentSelTd.rowIndex;
    //得到table的总行数
    var count=document.getElementById(_tbContent).rows.length;

    //下一行的名称列值
    var htm1=document.getElementById(_tbContent).rows[row+1].cells[0].innerHTML;
    var ishtm1=$("#"+_tbContent+" tr:eq("+(row+1)+") td:eq(0) input")[0].checked;

    // 当前行的名称列值
    var htm2=currentSelTd.innerHTML;
    var ishtm2=$("#"+_tbContent+" tr:eq("+row+") td:eq(0) input")[0].checked;
    
    document.getElementById(_tbContent).rows[row+1].cells[0].innerHTML=htm2;
    $("#"+_tbContent+" tr:eq("+(row+1)+") td:eq(0) input")[0].checked=ishtm2;
    
    currentSelTd.cells[0].innerHTML=htm1;
    $("#"+_tbContent+" tr:eq("+row+") td:eq(0) input")[0].checked=ishtm1;
    
    //将选中的行也上移
    SelectMove_Monitor($("#"+_tbContent).find("#tr_"+(row+1))[0]);
}

function UpMouseOver(_btnControl)
{
    $("#"+_btnControl).attr("src","../Images/UpBlue.png");
}

function UpMouseOut(_btnControl)
{
    $("#"+_btnControl).attr("src","../Images/Up.png");
}

function DownMouseOver(_btnControl)
{
    $("#"+_btnControl).attr("src","../Images/DownBlue.png");
}

function DownMouseOut(_btnControl)
{
    $("#"+_btnControl).attr("src","../Images/Down.png");
}

//点击保存将规则添加到数据库中
function SaveOptionRule(_ruleType,_tbControlId)
{
    var checkControls=$("#"+_tbControlId).find("input[type=checkbox]");
    var spanControls=$("#"+_tbControlId).find("span");
    
    var _jsonStr="{'规则':[";
    
    for(var i=0;i<checkControls.length;i++)
    {
        if(i>0)
        {
            _jsonStr+=",";
        }
        _jsonStr+="{";
        _jsonStr+="'ID':'"+ruleId+"',";
        _jsonStr+="'量名称':'"+spanControls[i].innerHTML+"',";
        _jsonStr+="'是否显示':'"+checkControls[i].checked+"'";
        _jsonStr+="}";
    }
    _jsonStr+="]}";
    
    var url="";
    var data="";
    var _operateType="NEW";
    if(ruleId!=null && ruleId!="")
    {
        _operateType="MODIFY";
        url="../WebServices/PersonOptionService.asmx/ModifyPersonOption";
        data={"loginIdentifer":window.parent.guid,"ruleId":ruleId,"personOptionJson":_jsonStr};
    }
    else
    {
        url="../WebServices/PersonOptionService.asmx/AddPersonOption";
        data={"loginIdentifer":window.parent.guid,"ruleType":_ruleType,"personOptionJson":_jsonStr};
    }

    $.ajax(
    { 
        url:url,
        type:"GET",
        data:data,
        dataType:"text",
        cache:false,
        success:function(responseText)
        {
            var data = eval("("+$.xml2json(responseText)+")");
            if(data.Result)//登录成功
            {
                alert(data.Message);
            }
        },
        error:function (XMLHttpRequest, textStatus, errorThrown)
        {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}