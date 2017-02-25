// JScript 文件

//存放处理报警时间的Cooike名字
var disposeAlarmTimeCookieName = "";
//检查报警记录的定时器
var pollingGlobalAlarm = null;
//是否正在检查报警记录
var isPollingGlobalAlarm = false;
//报警模块初始化
function GlobalAlarmLoad() {
    //为表格单绑定单元格点击事件、选中所有行和清理选中所有事件
    $("#tbAlarm").datagrid({
        onClickCell: function (rowIndex, field, value) {
            //点中处理单元格时移除该行
            if (field == "disposeAlarm") {
                $("#tbAlarm").datagrid("deleteRow", rowIndex);
                //设置全局报警提示文本和声音
                SetAlarmTextAndSound();
            }
        },
        onCheckAll: function (rows) {
            //表格左下角的全选复选框选中
            $("#checkAllTR").attr('checked', true);
        },
        onUncheckAll: function (rows) {
            //清理表格左下角的全选复选框选中
            $("#checkAllTR").attr('checked', false);
        }
    });

    //获取全局报警配置
    $.ajax(
    {
        url: "WebServices/GlobalAlarmService.asmx/GetGlobalAlarmConfig",
        type: "GET",
        data: { loginIdentifer: guid },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                var alarmConfig = data.AlarmConfig;
                if (alarmConfig.UseAlarm == "True") {
                    //启用报警，创建检查报警定时器
                    pollingGlobalAlarm = window.setInterval(GlobalAlarmAjax, 30 * 1000);
                }
                else {
                    //禁用报警，更改工具栏状态
                    $("#isPlaySound").attr('disabled', true);
                    $("#isAutoShow").attr('disabled', true);
                    $("#isUseAlarm").attr('checked', false);
                    $("#alarmText").text("未启用").css("color", "Green");
                }
                //判断是否启用声音
                if (alarmConfig.UseVoice == "False") {
                    $("#isPlaySound").attr('checked', false);
                }
                //判断全局报警窗口是否自动弹出
                if (alarmConfig.AutoPopup == "False") {
                    $("#isAutoShow").attr('checked', false);
                }
            }
            else {
                if (data.Message != "noLogin") {
                    $.messager.alert("提示信息", data.Message);
                }
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //alert(errorThrown + "\r\n" + XMLHttpRequest.responseText);
        }
    });
}
//从服务器获取全局报警信息
function GlobalAlarmAjax() {
    if (isPollingGlobalAlarm) {
        return;
    }
    isPollingGlobalAlarm = true;
    $.ajax(
    {
        url: "WebServices/GlobalAlarmService.asmx/GetGlobalAlarmInfos",
        type: "GET",
        data: { loginIdentifer: guid },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var resultObj = eval("(" + $.xml2json(responseText) + ")");
            disposeAlarmTimeCookieName = resultObj.CookieName;
            if (resultObj.AlarmRecords.length == 0) {
                isPollingGlobalAlarm = false;
                return;
            }
            try {
                var tbAlarmObj = $("#tbAlarm");
                var alarmRecords = resultObj.AlarmRecords;
                //循环报警记录
                for (var i = 0; i < alarmRecords.length; i++) {
                    //创建行对象
                    var trAlarm = {};
                    trAlarm["mnName"] = alarmRecords[i].单位名称;
                    trAlarm["devName"] = alarmRecords[i].测点名称;
                    trAlarm["alarmTime"] = alarmRecords[i].报警时间;
                    trAlarm["alarmType"] = alarmRecords[i].报警类型;
                    trAlarm["alarmDesc"] = alarmRecords[i].报警描述;
                    trAlarm["disposeAlarm"] = "<img id='dd' style='cursor:pointer;' src='Images/Delete.gif' />";
                    //向表格中追加行
                    tbAlarmObj.datagrid('appendRow', trAlarm);
                }
                //根据文本调整单位名称列宽
                tbAlarmObj.datagrid('autoSizeColumn', "mnName");
                //根据文本调整测点名称列宽
                tbAlarmObj.datagrid('autoSizeColumn', "devName");
                //根据文本调整报警类型列宽
                tbAlarmObj.datagrid('autoSizeColumn', "alarmTime");
                //根据文本调整报警描述列宽
                tbAlarmObj.datagrid('autoSizeColumn', "alarmDesc");
                //获取表格当前页所有行
                var rows = tbAlarmObj.datagrid('getRows');
                //滚动到最后一行
                tbAlarmObj.datagrid('scrollTo', (rows.length - 1));
                //判断是否自动弹出全局报警窗口
                if ($("#isAutoShow").attr("checked")) {
                    ShowGlobalAlarm();
                }
                //设置全局报警文本和声音
                SetAlarmTextAndSound();
            }
            catch (e) {
                e = null;
            }
            isPollingGlobalAlarm = false;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            isPollingGlobalAlarm = false;
            //alert(errorThrown + "\r\n" + XMLHttpRequest.responseText);
        }
    });
}

//启用报警复选框点击事件处理
function UseAlarmClick() {
    if ($("#isUseAlarm").attr('checked')) {
        $("#isUseAlarm").attr('checked', false);
        $("#isPlaySound").attr('disabled', true);
        $("#isAutoShow").attr('disabled', true);
        window.clearInterval(pollingGlobalAlarm);
        $("#AlarmImg").css("display", "none");
        $("#alarmText").text("未启用").css("color", "Green");
    }
    else {
        $("#isUseAlarm").attr('checked', true);
        $("#isPlaySound").attr('disabled', false);
        $("#isAutoShow").attr('disabled', false);
        pollingGlobalAlarm = window.setInterval(GlobalAlarmAjax, 30 * 1000);
        $("#AlarmImg").css("display", "none");
        $("#alarmText").text("正常").css("color", "Green");
    }
    SetGlobalAlarm();
}
//自动弹出复选框点击事件处理
function AutoShowClick() {
    if ($("#isAutoShow").attr('checked')) {
        $("#isAutoShow").attr('checked', false);
        HideGlobalAlarm();
    }
    else {
        $("#isAutoShow").attr('checked', true);
    }
    SetGlobalAlarm();
}

//启用声音复选框点击事件处理
function AlarmSoundClick() {
    if ($("#isPlaySound").attr('checked')) {
        $("#isPlaySound").attr('checked', false);
        RemoveAlarmSonud();
    }
    else {
        $("#isPlaySound").attr('checked', true);
        PlayAlarmSound();
    }
    SetGlobalAlarm();
}

//左下角全选复选框点击事件处理
function checkAll() {
    if ($("#checkAllTR").attr('checked')) {
        $("#checkAllTR").attr('checked', false);
        $('#tbAlarm').datagrid("uncheckAll");
    }
    else {
        $("#checkAllTR").attr('checked', true);
        $('#tbAlarm').datagrid("checkAll");
    }
}

//处理选中按钮点击事件处理
function DelCheckedTR() {
    var tbAlarmObj = $("#tbAlarm");
    var rows = tbAlarmObj.datagrid("getChecked");
    for (var i = 0; i < rows.length; i++) {
        var rowIndex = tbAlarmObj.datagrid("getRowIndex", rows[i]);
        tbAlarmObj.datagrid("deleteRow", rowIndex);
    }

    SetAlarmTextAndSound();

    var myDate = new Date();
    var year = myDate.getFullYear();
    var month = (myDate.getMonth() + 1).toString();
    if (month.length == 1) {
        month = "0" + month;
    }
    var date = myDate.getDate().toString();
    if (date.length == 1) {
        date = "0" + date;
    }
    var hour = myDate.getHours().toString();
    if (hour.length == 1) {
        hour = "0" + hour;
    }
    var minute = myDate.getMinutes().toString();
    if (minute.length == 1) {
        minute = "0" + minute;
    }
    var second = myDate.getSeconds().toString();
    if (second.length == 1) {
        second = "0" + second;
    }

    var timeStr = year + "-" + month + "-" + date + " " + hour + ":" + minute + ":" + second;
    $.cookie(encodeURI(disposeAlarmTimeCookieName), timeStr.toString(), { expires: 7, path: '/' });
}

//设置全局报警配置
function SetGlobalAlarm() {
    var strUseAalarm = 'false';
    var strUseVoice = 'false';
    var strAutoPopup = 'false';
    if ($("#isUseAlarm").attr('checked')) {
        strUseAalarm = 'true';
    }
    if ($("#isPlaySound").attr('checked')) {
        strUseVoice = 'true';
    }
    if ($("#isAutoShow").attr('checked')) {
        strAutoPopup = 'true';
    }
    $.ajax(
    {
        url: "WebServices/GlobalAlarmService.asmx/SetGlobalAlarmConfig",
        type: "POST",
        data: { loginIdentifer: guid, UseAlarm: strUseAalarm, UseVoice: strUseVoice, AutoPopup: strAutoPopup },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {

            }
            else {
                if (data.Message != "noLogin") {
                    $.messager.alert("提示信息", data.Message);
                }
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //alert(errorThrown + "\r\n" + XMLHttpRequest.responseText);
        }
    });
}

//设置全局报警提示文本和声音
function SetAlarmTextAndSound() {
    if ($("#tbAlarm").datagrid("getRows").length > 0) {
        $("#AlarmImg").css("display", "block");
        $("#alarmText").text("报警").css("color", "Red");
        if ($("#isPlaySound").attr('checked') && $("#alarmsound").attr("src") == null) {
            $("<bgsound id='alarmsound' src='alarm.wav' loop='-1'></bgsound>").appendTo($("#divAlarm"));
        }
        if ($("#tbAlarm").datagrid("getChecked").length == 0) {
            $("#checkAllTR").attr("checked", false);
            $("#tbAlarm").datagrid("uncheckAll");
        }
    }
    else {
        $("#AlarmImg").css("display", "none");
        $("#alarmText").text("正常").css("color", "Green");
        RemoveAlarmSonud();
        $("#checkAllTR").attr("checked", false);
        $("#tbAlarm").datagrid("uncheckAll");
    }
}

//播放声音
function PlayAlarmSound() {
    if ($("#tbAlarm").datagrid("getRows").length > 0 && $("#alarmsound").attr("src") == null) {
        $("<bgsound id='alarmsound' src='alarm.wav' loop='-1'></bgsound>").appendTo($("#divAlarm"));
    }
}

//停止播放声音
function RemoveAlarmSonud() {
    if ($("#alarmsound").attr("src") != null) {
        $("#alarmsound").remove();
    }
}

//显示和隐藏GlobalAlarm        
var globalAlarmIsShow = false;
function ShowAndHideGlobalAlarm() {
    if (globalAlarmIsShow) {
        $('#divAlarmContainer').css("visibility", "hidden");
    }
    else {
        $('#divAlarmContainer').css("visibility", "visible");
    }
    globalAlarmIsShow = !globalAlarmIsShow;
}

function ShowGlobalAlarm() {
    if (!globalAlarmIsShow) ShowAndHideGlobalAlarm();
}

function HideGlobalAlarm() {
    if (globalAlarmIsShow) ShowAndHideGlobalAlarm();
}
