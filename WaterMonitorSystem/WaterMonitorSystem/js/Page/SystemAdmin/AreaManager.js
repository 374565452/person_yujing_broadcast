// JScript 文件
var dicDeviceAppGroups = {};
//表格数据
var tableData = [];
var comboBoxData=[];
//当前登录操作员管理ID
var mnId = "";
//操作类型
var operateType = "Add";
//用于存储所有级别的节点
var levelJson;
//左侧树形选中节点的管理ID
var currentSelManageId;
//用于存储级别信息表中倒数第二级的级别ID(即管理的最后一级别)
var lastLevelId="";
//是否显示直接子管理
var isContainsChildManage = true;
//用于存储当前编辑的管理的ID
var operateManageId;

$(document).ready(function(){
    var defaultTheme = $.cookie("psbsTheme");
    if(defaultTheme != null && defaultTheme != "default")
    {
        var link = $(document).find('link:first');
	    link.attr('href', '../App_Themes/easyui/themes/'+defaultTheme+'/easyui.css');
    }
    $.ShowMask("数据加载中，请稍等……");
    GetSystemInfo();
    $('#btn_Add').linkbutton({ disabled: true });
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
	            LoadTree("divAreaTree", mnId, false, false);
	            LoadTableData();
	            LoadComboboxData();
	            //绑定所属级别下拉框
                LoadLevelComboboxData();
	            TreeBindSelect();
            }
        },
        error:function (XMLHttpRequest, textStatus, errorThrown)
        {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function LoadTableData(treeid)
{
    if(!manageNodeLoaded)
    {
        window.setTimeout("LoadTableData(" + treeid + ")", 500);
        return;
    }

    if (treeid) {
        var node = $('#divAreaTree').tree('find', "mn_" + treeid);
        $('#divAreaTree').tree('select', node.target);
        return;
    }
    tableData = [];
    var manJson;
    for (var key in manageNodes) 
    {
        manJson = manageNodes[key].attributes["manage"];
        var tableRow = {};
        tableRow["manageId"] = manJson.ID;
        tableRow["LevelName"] = manJson.级别名称;
        tableRow["manageName"] = manJson.名称;
        tableRow["manageCode"] = typeof (manJson.编码) == "undefined" ? "" : manJson.编码;
        tableRow["editYearExploitation"] = "<img src='../Images/Detail.gif' onclick='javascript:EditYearExploitation(" + manJson.ID + ")' />";
        tableRow["editManage"] = "<img src='../Images/Detail.gif' onclick='javascript:EditArea("+manJson.ID+")' />";
        tableRow["removeManage"] = "<img src='../Images/Delete.gif' onclick='javascript:DeleteArea("+manJson.ID+")' />";
        tableData.push(tableRow);
    }
    $('#tbManageInfos').datagrid({ loadFilter: pagerFilter }).datagrid('loadData', tableData);
    $.HideMask();
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
    if(!manageNodeLoaded)
    {
        window.setTimeout("LoadComboboxData()",500);
        return;
    }
    comboBoxData=[];
    for (i = 0; i < manageJson.length; i++) 
    {
        var manageObj = {};
        manageObj["id"] = manageJson[i].ID;
        manageObj["text"] = manageJson[i].名称;
        comboBoxData.push(manageObj);
    }
    $("#cbb_ManageCombobox").combobox({
        data:comboBoxData
    });
}

//点击左侧树形重新加载右侧列表
function TreeBindSelect()
{
    $('#divAreaTree').tree({
        onSelect: function(node){
	        var manageIds = GetManageIDsByNode(node);
	        currentSelManageId=node.attributes["manage"].ID;
	        ReLoadTableData(manageIds);
	        if(lastLevelId==node.attributes["manage"].级别ID)
	        {
	            $('#btn_Add').linkbutton({disabled:true});
	        }
	        else
	        {
	            $('#btn_Add').linkbutton({disabled:false});
	        }
        }
    });
}
//根据传进来的管理ID查询相应信息
function ReLoadTableData(manageIds)
{
    tableData = [];
    var manJson;
    for (var i = 0; i < manageIds.length; i++) 
    {
        var tableRow = {};
        manJson = manageNodes[manageIds[i]].attributes["manage"];
        tableRow["manageId"] = manJson.ID;
        tableRow["LevelName"] = manJson.级别名称;
        tableRow["manageName"] = manJson.名称;
        tableRow["manageCode"] = typeof (manJson.编码) == "undefined" ? "" : manJson.编码;
        tableRow["editYearExploitation"] = "<img src='../Images/Detail.gif' onclick='javascript:EditYearExploitation(" + manJson.ID + ")' />";
        tableRow["editManage"] = "<img src='../Images/Detail.gif' onclick='javascript:EditArea("+manJson.ID+")' />";
        tableRow["removeManage"] = "<img src='../Images/Delete.gif' onclick='javascript:DeleteArea("+manJson.ID+")' />";
        tableData.push(tableRow);
    }
    $('#tbManageInfos').datagrid({ loadFilter: pagerFilter }).datagrid('loadData', tableData);
    $.HideMask();
}

//点击工具栏的查询按钮的方法
function Btn_Query_Click()
{
    $.ShowMask("数据加载中，请稍等……");
    var manIds = [];
    var manComboboxObj = $("#cbb_ManageCombobox");
    var value = manComboboxObj.combobox("getValue");
    //值为空时采用模糊获取ID
    if(value == null || value == "")
    {
        var text = manComboboxObj.combobox("getText");
        //文本为空时按Combobox加载数据获取管理ID
        if($.trim(text) != "")
        {
            var comboboxData = manComboboxObj.combobox("getData");
            var opts = manComboboxObj.combobox('options');
            for(var i=0; i<comboboxData.length; i++)
            {
                if(comboboxData[i][opts.textField].indexOf(text)>-1)
                {
                    manIds.push(comboboxData[i][opts.valueField]);
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
        var _manNode=manageNodes[value];
        var manageIdsArray=GetManageIDsByNode(_manNode);
        if(manageIdsArray!=null && manageIdsArray.length>0)
        {
            for(var i=0;i<manageIdsArray.length;i++)
            {
                manIds.push(manageIdsArray[i]);
            }
        }
    }
    if(manIds.length == 0)
    {
        $.HideMask();
        $.messager.alert("提示信息","查询结果为空");
    }
    ReLoadTableData(manIds)
}

function GetManageIDsByNode(node) {
    var manageIds = [];

    manageIds.push(node.attributes.mid);

    for (var i = 0; i < node.children.length; i++) {
        if (node.children[i].attributes["nodeType"] == "manage" && node.children[i].attributes["uid"] == node.attributes.mid) {
            manageIds = manageIds.concat(GetManageIDsByNode(node.children[i]));
        }
    }
    return manageIds;
}

function EditArea(areaid)
{
    operateManageId = areaid;
    operateType = "Modify";
 		        LoadManageDetailInfo(areaid);  
 		        $('#dlgManage').dialog({title:'编辑区域信息',closed:false});
}

function EditYearExploitation(areaid) {
    operateManageId = areaid;
    $('#dlgManage2').dialog({ title: '年可开采水量设置', closed: false });
}

function DeleteArea(areaid)
{
    operateManageId= areaid;
    $.messager.confirm('提示信息', '您确定要删除该区域吗?', function(r){
        if (r)
        {
            $.ajax({ 
                url:"../WebServices/ManageNodeService.asmx/DeleteMangeNode",
                type:"POST",
                data:{"managerId":operateManageId,'loginIdentifer':window.parent.guid},
                dataType:"text",
                cache:false,
                success:function(responseText)
                {
                    var data = eval("("+$.xml2json(responseText)+")");
                    if(data.Result)//登录成功
                    {
                        $.messager.alert('提示信息','删除成功！');
                        for(var i=0; i<manageJson.length; i++)
                        {
                            if(manageJson[i].ID == operateManageId)
                            {
                                manageJson.splice(i,1);
                                break;
                            }
                        }
                        delete manageNodes[operateManageId];
                        LoadTableData();
                        var treeNode = $("#divAreaTree").tree("find","mn_"+operateManageId);
                        $("#divAreaTree").tree("remove",treeNode.target);
                    }
                    else
                    {
                        $.messager.alert('提示信息',data.Message,'warning');
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

function RemoveTR()
{
    for(var i=3;i<$("#tbDevice tr").length;i++)
    {
        $("#tbDevice tr:eq("+i+")").remove(); 
    }

    if($("#tbDevice tr").length>3)
    {
        RemoveTR();
    }
}

//绑定双击详细弹出的窗口
function LoadManageDetailInfo(manageId)
{
    var manJson = manageNodes[manageId].attributes["manage"];
    
    //绑定前先删除固定行之外的其他行
    RemoveTR();
    //先绑定级别辅助信息中的级别描述
    var _aidObj=eval(manJson.级别描述);
    if(_aidObj!=null && _aidObj!="")
    {
        var tableObj=$("#tbDevice");
        var count=0;
        var trNew=$("<tr></tr>");
        for(var i=0;i<_aidObj.length;i++)
        {
            var tdNew=$("<td style='height:26px;width:75px;' align='right'><span>"+_aidObj[i].量名+"：</span></td>");
            tdNew.appendTo(trNew);
            var tdNewValue="";
            if(_aidObj[i].控件类型=="T")
            {
                tdNewValue=$("<td align='left'><input id='txt_"+_aidObj[i].量名+"' class='easyui-validatebox textbox'type='text' data-options='required:true,validType:\"length[1,25]\"' style='height: 21px;' /></td>");
            }
            else if(_aidObj[i].控件类型.indexOf("C")>=0)
            {
                var optionStr="";
                var _comValue=_aidObj[i].控件类型.split('C');
                if(_comValue!=null && _comValue.length>1)
                {
                    var _valueArray=_comValue[1].split('-');
                    if(_valueArray!=null && _valueArray.length>0)
                    {
                        var comObj=document.getElementById("cbb_"+_aidObj[i].量名);
                        for(var val=0;val<_valueArray.length;val++)
                        {
                            optionStr+="<option value='"+_valueArray[val]+"'>"+_valueArray[val]+"</option>";
                        }
                    }
                }                              
                tdNewValue=$("<td align='left'><input id='cbb_"+_aidObj[i].量名+"' class='easyui-combobox' data-options='valueField: \"id\",textField: \"text\"' style='width:150px' /></td>");                            
            }
            else if(_aidObj[i].控件类型=="D")
            {
                tdNewValue=$("<td align='left'><input id='txt_"+_aidObj[i].量名+"' type='text' style='width: 140px' maxlength='19' value='"+new Date().Format("yyyy-MM-dd")+"' onfocus='WdatePicker({dateFmt:\"yyyy-MM-dd\"})' /></td>");
            }
            tdNewValue.appendTo(trNew);
            
            var showStr="";
            if(_aidObj[i].必填项!="" || _aidObj[i].必填项=="是")
            {
                showStr+="<span>*</span>";
            }
            if(_aidObj[i].规则提示!="")
            {
                showStr+="&nbsp;<span style='color:red;'>"+_aidObj[i].规则提示+"</span>";
            }
            var tdShow=$("<td align='left'>"+showStr+"</td>");
            tdShow.appendTo(trNew);
            
            trNew.appendTo(tableObj);
            trNew=$("<tr></tr>");
        }
    }
    
    $("#txt_ManageName").textbox("setText",manJson.名称);
    
    //将级别下拉框中的项选中
    $('#cbb_LevelManager').combobox({disabled:true});
 	$("#cbb_LevelManager").combobox('select',manJson.级别ID);
 	$('#cb_manageCombotree').combotree({ disabled: true });
    //绑定上级区域下拉框中的树形
 	GetComboTreeData(manJson.级别ID);
    //将上级区域下拉框中的项选中
    if(manJson.上级ID!=0)
    {
        var _tree = $('#cb_manageCombotree').combotree('tree');
        var node = _tree.tree('find', "mn_"+manJson.上级ID);
        _tree.tree('select', node.target);
        $('#cb_manageCombotree').combotree('setValue', "mn_"+manJson.上级ID);
    }
    
    //判断界面上是否有动态添加的辅助信息，如果有则根据读取出来的规则进行判断
    if(_aidObj!=null)
    {
        for(var i=0;i<_aidObj.length;i++)
        {
            var valName=_aidObj[i].量名;
            if(_aidObj[i].控件类型=="T")
            {
                //$("#txt_" + _aidObj[i].量名).val(manJson.辅助信息[valName]);
                $("#txt_" + _aidObj[i].量名).val(manJson.编码);
            }
            if(_aidObj[i].控件类型=="C")
            {
                $("#cbb_"+_aidObj[i].量名).val(manJson.辅助信息[valName]);
            }
            if(_aidObj[i].控件类型=="D")
            {
                $("#txt_"+_aidObj[i].量名).val(manJson.辅助信息[valName]);
            }
        }
    }
}

//绑定所属级别的下拉框
function LoadLevelComboboxData()
{
    $.ajax(
    { 
        url:"../WebServices/ManageNodeService.asmx/GetLevelInfos",
        type:"GET",
        data:{"loginIdentifer":window.parent.guid},
        dataType:"text",
        cache:false,
        success:function(responseText)
        {
            var data = eval("("+$.xml2json(responseText)+")");
            if(data.Result)//登录成功
            {
                levelJson=data.LevelNodes;
                var comboBoxDataLevel=[];
                //用于存储管理的最后一个级别ID
                var levelNum=0;
                
                for (i = 0; i < levelJson.length; i++) 
                {
                    var levelObj = {};
                    levelObj["id"] = levelJson[i].ID;
                    levelObj["text"] = levelJson[i].级别名称;
                    comboBoxDataLevel.push(levelObj);
                    
                    //如果levelNum为0，说明是第一次循环；如果levelNum中的值小于当前级别ID，则将当前级别ID赋给levelNum
                    if(levelNum==0 || levelNum<levelJson[i].ID)
                    {
                        levelNum=levelJson[i].ID;
                    }
                }
                if(levelNum!=0)
                {
                    lastLevelId=levelNum;
                }
                
                $("#cbb_LevelManager").combobox({
                    data:comboBoxDataLevel
                });
            }
        },
        error:function (XMLHttpRequest, textStatus, errorThrown)
        {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}
//绑定上级区域的下拉框================（没用到此方法）==================
function LoadHigherComboboxData(manageId)
{
    var manJson = manageNodes[manageId].attributes["manage"];
    var comboBoxDataHigher=[];
    for (i = 0; i < manageJson.length; i++) 
    {
        if(manageJson[i].上级ID<manJson.上级ID)
        {
            var manageObj = {};
            manageObj["id"] = manageJson[i].ID;
            manageObj["text"] = manageJson[i].名称;
            comboBoxDataHigher.push(manageObj);
        }
    }
    $("#cbb_HighManager").combobox({
        data:comboBoxDataHigher
    });
}

//绑定上级区域下拉框中的树形
function GetComboTreeData(levelId)
{
    if(!manageNodeLoaded)
    {
        window.setTimeout("GetComboTreeData('"+levelId+"')",200);
        return;
    }
    
    comboTreeData = [];
    var manageJson = window.manageJson;
    //复制一份
    var manageNodes = {};
    // 挂载节点
    for (i = 0; i < manageJson.length; i++) 
    {
        if(manageJson[i].级别ID>levelId)
        {
            continue;
        }
        if(operateType == "Modify")
        {
            if(manageJson[i].级别ID==levelId)
            {
                continue;
            }
        }
        //节点附加属性
        var mInfo = 
        {
            mid : manageJson[i].ID,
            nodeType : 'manage',
            uid : manageJson[i].上级ID,
            manage:manageJson[i]
        };
        var treeNode = 
        {
            "id" : "mn_" + manageJson[i].ID,
            "text" : manageJson[i].名称,
            "attributes":mInfo,
            "children":[]
        }
        manageNodes[manageJson[i].ID] = treeNode;
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
    $("#cb_manageCombotree").combotree("loadData",comboTreeData);
}

//清空弹出窗口中的文本信息
function ClearManageDetailInfo()
{
    $("#txt_ManageName").textbox("setText","");
}

function btn_Add_Click()
{
    if(currentSelManageId==null ||currentSelManageId =="")
    {
        $.messager.alert("提示信息", "请选择要添加下级管理的节点！");
        return;
    }//绑定前先删除固定行之外的其他行
    RemoveTR();
    operateType = "Add";
    ClearManageDetailInfo();
    $('#dlgManage').dialog({title:"添加区域信息",closed:false});
    
    //绑定上级区域下拉框中的树形并将选中的显示在下拉框中
    var manJson=manageNodes[currentSelManageId].attributes["manage"];
    GetComboTreeData(manJson.级别ID);
    $('#cb_manageCombotree').combotree({ disabled: true });
    $('#cb_manageCombotree').combotree('setValue', "mn_"+currentSelManageId);
    //将所属级别下拉框的选中
    $('#cbb_LevelManager').combobox({ disabled: true });
    
    var downLevelId="";
    for (i = 0; i < levelJson.length; i++) 
    {
        //得到传进来的级别的下级Id
        if(downLevelId=="" && levelJson[i].ID>manJson.级别ID)
        {
            downLevelId = levelJson[i].ID;
            //绑定此级别的辅助信息
            var _aidObj = eval(manJson.级别描述2);
            if(_aidObj!=null && _aidObj!="")
            {
                var tableObj=$("#tbDevice");
                var count=0;
                var trNew=$("<tr></tr>");
                for(var i=0;i<_aidObj.length;i++)
                {
                    var tdNew=$("<td style='height:26px;' align='right'>"+_aidObj[i].量名+"：</td>");
                    tdNew.appendTo(trNew);
                    var tdNewValue="";
                    if(_aidObj[i].控件类型=="T")
                    {
                        tdNewValue=$("<td align='left'><input id='txt_"+_aidObj[i].量名+"' class='easyui-validatebox textbox'type='text' data-options='required:true,validType:\"length[1,25]\"' style='height: 21px;' /></td>");
                    }
                    else if(_aidObj[i].控件类型.indexOf("C")>=0)
                    {
                        var optionStr="";
                        var _comValue=_aidObj[i].控件类型.split('C');
                        if(_comValue!=null && _comValue.length>1)
                        {
                            var _valueArray=_comValue[1].split('-');
                            if(_valueArray!=null && _valueArray.length>0)
                            {
                                var comObj=document.getElementById("cbb_"+_aidObj[i].量名);
                                for(var val=0;val<_valueArray.length;val++)
                                {
                                    optionStr+="<option value='"+_valueArray[val]+"'>"+_valueArray[val]+"</option>";
                                }
                            }
                        }
                        tdNewValue=$("<td align='left'><input id='cbb_"+_aidObj[i].量名+"' class='easyui-combobox' data-options='valueField: \"id\",textField: \"text\"' style='width:150px' /></td>");                            
                    }
                    else if(_aidObj[i].控件类型=="D")
                    {
                        tdNewValue=$("<td align='left'><input id='txt_"+_aidObj[i].量名+"' type='text' style='width: 140px' maxlength='19' value='"+new Date().Format("yyyy-MM-dd")+"' onfocus='WdatePicker({dateFmt:\"yyyy-MM-dd\"})' /></td>");
                    }
                    tdNewValue.appendTo(trNew);
                    
                    var showStr="";
                    if(_aidObj[i].必填项!="" || _aidObj[i].必填项=="是")
                    {
                        showStr+="<span>*</span>";
                    }
                    if(_aidObj[i].规则提示!="")
                    {
                        showStr+="&nbsp;<span style='color:red;'>"+_aidObj[i].规则提示+"</span>";
                    }
                    var tdShow=$("<td>"+showStr+"</td>");
                    tdShow.appendTo(trNew);
                    
                    trNew.appendTo(tableObj);
                    trNew=$("<tr></tr>");
                }
            }
            break;
        }
    }
    $("#cbb_LevelManager").combobox('select',downLevelId);
}

function btn_OK_Click()
{
    var _mName = $("#txt_ManageName").textbox('getText');
    var _levelValue=$("#cbb_LevelManager").combobox("getValue");
    var currentSelMId = $("#cb_manageCombotree").combotree("getValue");
    if (currentSelMId == undefined) {
        currentSelMId = 0;
    }
    if(_mName.trim()=="")
    {
        $.messager.alert("提示信息", "区域名称不能为空！");
        return;
    }
    if(currentSelMId!=null && currentSelMId!="")
    {
        currentSelMId=currentSelMId.replace('mn_','');
    }
    
    //判断界面上是否有动态添加的辅助信息，如果有则根据读取出来的规则进行判断
    var _devAidInfo = null;
    for (i = 0; i < levelJson.length; i++) 
    {
        if(levelJson[i].ID==_levelValue)
        {
            _devAidInfo = eval(levelJson[i].级别描述);
            break;
        }
    }
    
    var _aidStr="";
    if (_devAidInfo != null) {
        for (var i = 0; i < _devAidInfo.length; i++) {
            var _val = "";
            if (_devAidInfo[i].控件类型 == "T") {
                _val = document.getElementById("txt_" + _devAidInfo[i].量名).value;
                if (_devAidInfo[i].必填项 == "是" && (_val == "" || _val == null)) {
                    $.messager.alert("提示信息", _devAidInfo[i].量名 + "为必填项！");
                    return;
                }
                //判断是否是数字
                if (_devAidInfo[i].规则提示.indexOf("数字") >= 0 && _devAidInfo[i].规则提示.indexOf("非数字") < 0) {
                    var reNum = /^\d*$/;
                    if (!(reNum.test(_val) && _val.length == _devAidInfo[i].规则提示[0])) {
                        $.messager.alert("提示信息", _devAidInfo[i].量名 + "必须是" + _devAidInfo[i].规则提示[0] + "位数字!");
                        return;
                    }
                }
            }
            if (_devAidInfo[i].控件类型.indexOf("C") >= 0) {
                _val = document.getElementById("cbb_" + _devAidInfo[i].量名).value;
            }
            if (_devAidInfo[i].控件类型 == "D") {
                _val = document.getElementById("txt_" + _devAidInfo[i].量名).value;
            }

            if (_val != "" && _val != null) {
                _aidStr = _aidStr + _devAidInfo[i].量名 + ":'" + _val + "',";
            }
        }
    }
    var mJsonObj="{ID:'"+operateManageId+"',名称:'"+_mName+"',级别ID:'"+_levelValue+"',上级ID:'"+currentSelMId+"',"+_aidStr+"描述:''}";
    $.ShowMask("请稍等……");
    if(operateType=="Add")
    {
        $.ajax(
        { 
            url:"../WebServices/ManageNodeService.asmx/AddMangeNode",
            type:"POST",
            data:{'manageJSONString':mJsonObj,'loginIdentifer':window.parent.guid},
            dataType:"text",
            cache:false,
            success:function(responseText)
            {
                var data = eval("("+$.xml2json(responseText)+")");
                if(data.Result)//登录成功
                {
	                LoadTree("divAreaTree", mnId, false, false);
	                LoadTableData(operateManageId);
	                LoadComboboxData();
	                $.HideMask();
                    $('#dlgManage').dialog("close");
                    $.messager.alert("提示信息", data.Message);
                } else {
                    $.HideMask();
                    $.messager.alert("提示信息", data.Message);
                }
            },
            error:function (XMLHttpRequest, textStatus, errorThrown)
            {
                $.HideMask();
                $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
            }
        });
    }
    else if(operateType=="Modify")
    {
        $.ajax(
        { 
            url:"../WebServices/ManageNodeService.asmx/ModifyMangeNode",
            type:"POST",
            data:{'manageJSONString':mJsonObj,'loginIdentifer':window.parent.guid},
            dataType:"text",
            cache:false,
            success:function(responseText)
            {
                var data = eval("(" + $.xml2json(responseText) + ")");
                if(data.Result)//登录成功
                {
	                LoadTree("divAreaTree", mnId, false, false);
	                LoadTableData(operateManageId);
	                LoadComboboxData();
	                $.HideMask();
                    $('#dlgManage').dialog("close");
                    $.messager.alert("提示信息", data.Message);
                } else {
                    $.HideMask();
                    $.messager.alert("提示信息",data.Message);
                }
            },
            error:function (XMLHttpRequest, textStatus, errorThrown)
            {
                $.HideMask();
                $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
            }
        });
    }
}

function btn_OK2_Click() {
    alert(operateManageId);
    alert($("#tbYearExploitation tr").length);
}

