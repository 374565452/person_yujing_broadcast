////用于存储查询的历史记录条数的结果集
//var historyRecordCounts;
////当前页码
//var currentPageNumber;
////每页显示的条数
//var currentPageSize;
var usrIDs="";
var datas=[];

////全部用户集合
//var listonit;
////匹配用户集合
//var listnew;

////combobox的textupdate事件
//$("#cbb_UserCombobox");//待修改


function Btn_Query_Click()
{
    usrIDs="";
    datas=[];
    var startTime=$("#txt_StartTime").val();
    var endTime=$("#txt_EndTime").val();
    
    var treeObj;
    treeObj=$("#areaTree");
    var checkedNodes=treeObj.tree("getChecked");
    for (var i=0;i<checkedNodes.length;i++)
    {
        var checkedNode=checkedNodes[i];
        if(checkedNode.attributes["nodeType"]=="manage")
        {
            continue;
        }
        if(usrIDs!="")
        {
           usrIDs=usrIDs+",";
        }
        usrIDs=usrIDs+checkedNode.attributes["userid"];
    }
    
    $.ajax(
        {
            url:"../WebServices/DataQueryService.asmx/LoginRecordsQuery",
            type:"GET",
            data:{'usrIDs':usrIDs,'startTime':$("#txt_StartTime").val(),'endTime':$("#txt_EndTime").val()},
            datatype:"text",
            cache:false,
            success:function(responseText)
            {
               var data=eval("("+$.xml2json(responseText)+")");
               var records=data.Record;
               for(var i=0;i<records.length;i++)
               {
                   var dataRow={};
                   dataRow["name"]=records[i].用户名;
                   dataRow["recordtime"]=records[i].记录时间;
                   dataRow["logininfo"]=records[i].登录信息;
                   datas.push(dataRow);
               }
               $("#tbOperateInfos").datagrid({
               data:datas
               });
            },
            error:function (XMLHttpRequest, textStatus, errorThrown)
            {
                //alert(errorThrown + "\r\n" + XMLHttpRequest.responseText);
            }
        });
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

function Btn_Excel_Click()
{
    usrIDs="";
    datas=[];
    var startTime=$("#txt_StartTime").val();
    var endTime=$("#txt_EndTime").val();
    
    var treeObj;
    treeObj=$("#areaTree");
    var checkedNodes=treeObj.tree("getChecked");
    for (var i=0;i<checkedNodes.length;i++)
    {
        var checkedNode=checkedNodes[i];
        if(checkedNode.attributes["nodeType"]=="manage")
        {
            continue;
        }
        if(usrIDs!="")
        {
           usrIDs=usrIDs+",";
        }
        usrIDs=usrIDs+checkedNode.attributes["userid"];
    }
   //请求
   $.ajax(
   {
     url:"../WebServices/DataQueryService.asmx/ExcelExport",
     type:"GET",
     data:{"usrIDs":usrIDs,"startTime":startTime,"endTime":endTime},
     datatype:"text",
     cache:false,
     success:function(responseText)
     {
         var data=eval("("+$.xml2json(responseText)+")");
         
         //接收的最新时间范围对应的数据
          var records=data["Records"];
          
         //将数据添加到页面上
         for(var i=0;i<records.length;i++)
         {
           var dataRow={};
           dataRow["name"]=records[i].用户名;
           dataRow["recordtime"]=records[i].记录时间;
           dataRow["logininfo"]=records[i].登录信息;
           datas.push(dataRow);
         }
         $("#tbOperateInfos").datagrid({data:datas});
         location.href=data["ExcelUrl"];
     },
     error:function(XMLHttpRequest,textStatus,errorThrown)
     {
         alert(errorThrown+"\r\n"+XMLHttpRequest.responseText);
     }
   });
}