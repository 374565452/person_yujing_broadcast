if ($.fn.pagination){
	$.fn.pagination.defaults.beforePageText = '第';
	$.fn.pagination.defaults.afterPageText = '共{pages}页';
	$.fn.pagination.defaults.displayMsg = '显示{from}到{to},共{total}记录';
}
if ($.fn.datagrid){
	$.fn.datagrid.defaults.loadMsg = '';
}
if ($.fn.treegrid && $.fn.datagrid){
	$.fn.treegrid.defaults.loadMsg = $.fn.datagrid.defaults.loadMsg;
}
if ($.messager){
	$.messager.defaults.ok = '确定';
	$.messager.defaults.cancel = '取消';
}
if ($.fn.validatebox){
	$.fn.validatebox.defaults.missingMessage = '该输入项为必填项';
	$.fn.validatebox.defaults.rules.email.message = '请输入有效的电子邮件地址';
	$.fn.validatebox.defaults.rules.url.message = '请输入有效的URL地址';
	$.fn.validatebox.defaults.rules.length.message = '输入内容长度必须介于{0}和{1}之间';
	$.fn.validatebox.defaults.rules.remote.message = '请修正该字段';
}
if ($.fn.textbox) {
    $.fn.textbox.defaults.missingMessage = '该输入项为必填项';
}
if ($.fn.numberbox){
	$.fn.numberbox.defaults.missingMessage = '该输入项为必填项';
}
if ($.fn.combobox){
	$.fn.combobox.defaults.missingMessage = '该输入项为必填项';
}
if ($.fn.combotree){
	$.fn.combotree.defaults.missingMessage = '该输入项为必填项';
}
if ($.fn.combogrid){
	$.fn.combogrid.defaults.missingMessage = '该输入项为必填项';
}
if ($.fn.calendar){
	$.fn.calendar.defaults.weeks = ['日','一','二','三','四','五','六'];
	$.fn.calendar.defaults.months = ['一月','二月','三月','四月','五月','六月','七月','八月','九月','十月','十一月','十二月'];
}
if ($.fn.datebox){
	$.fn.datebox.defaults.currentText = '今天';
	$.fn.datebox.defaults.closeText = '关闭';
	$.fn.datebox.defaults.okText = '确定';
	$.fn.datebox.defaults.missingMessage = '该输入项为必填项';
	$.fn.datebox.defaults.formatter = function(date){
		var y = date.getFullYear();
		var m = date.getMonth()+1;
		var d = date.getDate();
		return y+'-'+(m<10?('0'+m):m)+'-'+(d<10?('0'+d):d);
	};
	$.fn.datebox.defaults.parser = function(s){
		if (!s) return new Date();
		var ss = s.split('-');
		var y = parseInt(ss[0],10);
		var m = parseInt(ss[1],10);
		var d = parseInt(ss[2],10);
		if (!isNaN(y) && !isNaN(m) && !isNaN(d)){
			return new Date(y,m-1,d);
		} else {
			return new Date();
		}
	};
}
if ($.fn.datetimebox && $.fn.datebox){
	$.extend($.fn.datetimebox.defaults,{
		currentText: $.fn.datebox.defaults.currentText,
		closeText: $.fn.datebox.defaults.closeText,
		okText: $.fn.datebox.defaults.okText,
		missingMessage: $.fn.datebox.defaults.missingMessage
	});
	$.fn.datetimebox.defaults.formatter = function(date){
		var y = date.getFullYear();
		var m = date.getMonth()+1;
		var d = date.getDate();
		var h = date.getHours();
		var min = date.getMinutes();
		var s = date.getSeconds();
		return y+'-'+(m<10?('0'+m):m)+'-'+(d<10?('0'+d):d)+" "+(h<10?('0'+h):h)+":"+(min<10?('0'+min):min)+":"+(s<10?('0'+s):s);
	};
	$.fn.datetimebox.defaults.parser = function(s){
		if (!s) return new Date();
		var ss = s.split('-');
		var y = parseInt(ss[0],10);
		var m = parseInt(ss[1],10);
		var d = parseInt(ss[2],10);
		var time = s.split(' ')[1].split(':')
		var h = parseInt(time[0],10);
		var min = parseInt(time[1],10);
		var s = parseInt(time[2],10);
		if (!isNaN(y) && !isNaN(m) && !isNaN(d) && !isNaN(h) && !isNaN(min) && !isNaN(s)){
			return new Date(y,m-1,d,h,min,s);
		} else {
			return new Date();
		}
	};
}

if ($.fn.tabs.methods){
	$.extend($.fn.tabs.methods, { 
		//显示遮罩
		loading: function (jq, msg)
		{ 
			return jq.each(function () { 
				var panel = $(this).tabs("getSelected"); 
				if (msg == undefined) { 
					msg ="正在加载数据，请稍候..."; 
				}
				var tabHeaderObj = $(this).find("div.tabs-header");
				if(tabHeaderObj)
				{
					$("<div class='datagrid-mask'></div>").css({ display:"block", top:tabHeaderObj.outerHeight(),width: panel.width(), height: panel.height() }).appendTo(panel); 
					$("<div class='datagrid-mask-msg' style='font-size:16px'></div>").html(msg).appendTo(panel).css({ display:"block", left: (panel.width() - $("div.datagrid-mask-msg", panel).outerWidth()) / 2, top: (tabHeaderObj.outerHeight() + (panel.height() - $("div.datagrid-mask-msg", panel).outerHeight()) / 2) }); 
				}
				else
				{
					$("<div class='datagrid-mask'></div>").css({ display:"block", width: panel.width(), height: panel.height() }).appendTo(panel); 
					$("<div class='datagrid-mask-msg' style='font-size:16px'></div>").html(msg).appendTo(panel).css({ display:"block", left: (panel.width() - $("div.datagrid-mask-msg", panel).outerWidth()) / 2, top: (panel.height() - $("div.datagrid-mask-msg", panel).outerHeight()) / 2 }); 
				}
			});
		}
		,
		//隐藏遮罩
		loaded: function (jq) { 
			return jq.each(function () { 
				var panel = $(this).tabs("getSelected"); 
				panel.find("div.datagrid-mask-msg").remove();
				panel.find("div.datagrid-mask").remove();
			});
		}
	});
}
