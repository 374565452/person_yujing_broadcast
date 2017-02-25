// JScript 文件

//去掉字符串左右空格
String.prototype.trim = function () { return this.replace(/(^\s*)|(\s*$)/g, ""); }
//去掉字符串左边空格
String.prototype.ltrim = function () { return this.replace(/^\s+/, ""); }
//去掉字符串右边空格
String.prototype.rtrim = function () { return this.replace(/\s+$/, ""); }
//判断此字符串中是否包含目标字符串
String.prototype.Contains = function (substring) {
    var index = this.indexOf(substring);
    if (index == -1) {
        return false;
    }
    else {
        return true;
    }
}
//得到字符串的字节长度
String.prototype.GetByteLength = function () {
    var totalLength = 0;
    var charCode;
    for (var i = 0; i < this.length; i++) {
        charCode = this.charCodeAt(i);
        if (charCode < 0x007f) {
            totalLength++;
        }
        else if ((0x0080 <= charCode) && (charCode <= 0x07ff)) {
            totalLength += 2;
        }
        else if ((0x0800 <= charCode) && (charCode <= 0xffff)) {
            totalLength += 3;
        }
    }
    return totalLength;
}

function DoClick(obj) {
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

function ShowMsg(message, millisecond) {
    try {
        if (millisecond != null && parseInt(millisecond) != 0) {
            window.setTimeout(function () { alert(message); }, parseInt(millisecond));
        }
        else {
            alert(message);
        }
    }
    catch (err) {
        alert(message);
    }
}

function ResetCover(outDiv, coverDiv) {
    var myCover = document.getElementById(coverDiv);
    var myOutDiv = document.getElementById(outDiv);

    myOutDiv.style.top = document.documentElement.scrollTop;
    myOutDiv.style.left = document.documentElement.scrollLeft;

    if (document.documentElement.scrollWidth > document.body.scrollWidth) {
        myCover.style.width = document.documentElement.scrollWidth;
    }
    else {
        myCover.style.width = document.body.scrollWidth;
    }
    if (document.documentElement.scrollHeight > document.body.scrollHeight) {
        myCover.style.height = document.documentElement.scrollHeight;
    }
    else {
        myCover.style.height = document.body.scrollHeight;
    }
    myOutDiv.style.display = "block";
}

function HideCover(outDiv) {
    document.getElementById(outDiv).style.display = "none";
}

var _psExtend_MouseMove = false; //移动标记
var _psExtend_mouseStartX, _psExtend_mouseStartY; //鼠标离控件左上角的相对位置
/***********************************************
 * 将指定的元素显示在指定的元素中心，支持拖动  *
 * elementId	  要显示的元素ID			   *
 * parentId       父元素ID                     *
 * leftCompensate left补偿                     *
 * topCompensate  top补偿                      *
 * draggable      是否支持拖动                 *
 ***********************************************/
function ElementShowCenter(elementId, parentId, leftCompensate, topCompensate, draggable) {
    var element = $("#" + elementId);
    var parent = $("#" + parentId);

    if (element.width() >= parent.width()) {
        element.css("left", leftCompensate + "px");
    }
    else {
        element.css("left", (leftCompensate + (parent.width() - element.width()) / 2) + "px");
    }
    if (element.height() >= parent.height()) {
        element.css("top", topCompensate + "px");
    }
    else {
        element.css("top", (topCompensate + (parent.height() - element.height()) / 2) + "px");
    }
    element.show();
    if (!draggable) {
        return;
    }
    element.mousedown(function (e) {
        if (_psExtend_GetMouseButton(e) == 0) {
            _psExtend_MouseMove = true;
            _psExtend_mouseStartX = e.pageX - parseInt(element.css("left"));
            _psExtend_mouseStartY = e.pageY - parseInt(element.css("top"));
            element.fadeTo(20, 0.8); //点击后开始拖动并透明显示
        }
    });
    $(document).mousemove(function (e) {
        if (_psExtend_MouseMove) {
            var x = e.pageX - _psExtend_mouseStartX; //移动时根据鼠标位置计算控件左上角的绝对位置
            var y = e.pageY - _psExtend_mouseStartY;
            element.css({ top: y, left: x }); //控件新位置
        }
    }).mouseup(function () {
        if (_psExtend_MouseMove) {
            element.fadeTo("fast", 1); //松开鼠标后停止移动并恢复成不透明
        }
        _psExtend_MouseMove = false;
    });
}

var _psExtend_IsIE678 = function (ua) {
    function check(r) {
        return r.test(ua);
    }
    var isOpera = check(/opera/),
        isIE = !isOpera && check(/msie/),
        isIE6 = isIE && check(/msie 6/),
        isIE7 = isIE && check(/msie 7/),
        isIE8 = isIE && check(/msie 8/);

    return isIE6 || isIE7 || isIE8;
}(navigator.userAgent.toLowerCase());

function _psExtend_GetMouseButton(e) {
    var code = e.button;
    var ie678Map = {
        1: 0,
        4: 1,
        2: 2
    }
    if (_psExtend_IsIE678) {
        return ie678Map[code];
    }
    return code;
}


Date.prototype.Format = function (formatStr) {
    var str = formatStr;
    var Week = ['日', '一', '二', '三', '四', '五', '六'];

    str = str.replace(/yyyy|YYYY/, this.getFullYear());
    str = str.replace(/yy|YY/, (this.getYear() % 100) > 9 ? (this.getYear() % 100).toString() : '0' + (this.getYear() % 100));

    str = str.replace(/MM/, this.getMonth() > 8 ? (this.getMonth() + 1).toString() : '0' + (this.getMonth() + 1));
    str = str.replace(/M/g, (this.getMonth() + 1));

    str = str.replace(/w|W/g, Week[this.getDay()]);

    str = str.replace(/dd|DD/, this.getDate() > 9 ? this.getDate().toString() : '0' + this.getDate());
    str = str.replace(/d|D/g, this.getDate());

    str = str.replace(/hh|HH/, this.getHours() > 9 ? this.getHours().toString() : '0' + this.getHours());
    str = str.replace(/h|H/g, this.getHours());
    str = str.replace(/mm/, this.getMinutes() > 9 ? this.getMinutes().toString() : '0' + this.getMinutes());
    str = str.replace(/m/g, this.getMinutes());

    str = str.replace(/ss|SS/, this.getSeconds() > 9 ? this.getSeconds().toString() : '0' + this.getSeconds());
    str = str.replace(/s|S/g, this.getSeconds());

    return str;
}
Date.prototype.DateAdd = function (strInterval, Number) {
    var dtTmp = this;
    switch (strInterval) {
        case 's': return new Date(Date.parse(dtTmp) + (1000 * Number));
        case 'n': return new Date(Date.parse(dtTmp) + (60000 * Number));
        case 'h': return new Date(Date.parse(dtTmp) + (3600000 * Number));
        case 'd': return new Date(Date.parse(dtTmp) + (86400000 * Number));
        case 'w': return new Date(Date.parse(dtTmp) + ((86400000 * 7) * Number));
        case 'q': return new Date(dtTmp.getFullYear(), (dtTmp.getMonth()) + Number * 3, dtTmp.getDate(), dtTmp.getHours(), dtTmp.getMinutes(), dtTmp.getSeconds());
        case 'm': return new Date(dtTmp.getFullYear(), (dtTmp.getMonth()) + Number, dtTmp.getDate(), dtTmp.getHours(), dtTmp.getMinutes(), dtTmp.getSeconds());
        case 'y': return new Date((dtTmp.getFullYear() + Number), dtTmp.getMonth(), dtTmp.getDate(), dtTmp.getHours(), dtTmp.getMinutes(), dtTmp.getSeconds());
    }
}

/* 
* 获得时间差,时间格式为 年-月-日 小时:分钟:秒 或者 年/月/日 小时：分钟：秒 
* 其中，年月日为全格式，例如 ： 2010-10-12 01:00:00 
* 返回精度为：秒，分，小时，天 
*/
function GetDateDiff(startTime, endTime, diffType) {
    //将xxxx-xx-xx的时间格式，转换为 xxxx/xx/xx的格式 
    startTime = startTime.replace(/\-/g, "/");
    endTime = endTime.replace(/\-/g, "/");
    //将计算间隔类性字符转换为小写 
    diffType = diffType.toLowerCase();
    var sTime = new Date(startTime); //开始时间 
    var eTime = new Date(endTime); //结束时间 
    //作为除数的数字 
    var divNum = 1;
    switch (diffType) {
        case "second":
            divNum = 1000;
            break;
        case "minute":
            divNum = 1000 * 60;
            break;
        case "hour":
            divNum = 1000 * 3600;
            break;
        case "day":
            divNum = 1000 * 3600 * 24;
            break;
        default:
            break;
    }
    return parseInt((eTime.getTime() - sTime.getTime()) / parseInt(divNum));
}

/*
* 秒数格式化为时分秒，秒数格式一定要为整数
*/
function FormatSeconds(i) {

    if (!isInteger(i)) {
        return '-';
    }

    var v = parseInt(i);
    if (v < 60) {
        return v + "秒";
    } else if (v < 3600) {
        var m = Math.floor(v / 60);
        var s = (v % 60);
        return m + "分" + (s > 0 ? s + "秒" : "");
    }
    else if (v > 3600) {
        var h = Math.floor(v / 3600);
        var m = Math.floor((v % 3600) / 60);
        var s = (v % 60);
        return h + "小时" + (m > 0 ? m + "分" : "") + (s > 0 ? s + "秒" : "");
    }
}

/* 判断对象是不是整数 */
function isInteger(obj) {
    if (parseInt(obj) == obj) {
        return true;
    }
    return false;
}

/*******************************************************
 * ResMsg 操作结果对象								   *
 * result  操作是否成功      类型:bool     默认值:true *
 * message 操作结果提示信息  类型:string   默认值:""   *
 * vlaue   操作结果值对象    类型:object   默认值:null *
 *******************************************************/
function ResMsg() {
    this.result = true;
    this.message = "";
    this.value = null;
}

/*******************************************************
 * HashTable javascript自定义哈希表					   *
 *******************************************************/
function HashTable() {
    //哈希表对象
    this._hashValue = new Object();
    //哈希表中键值对的数量
    this._iCount = 0;
    /********************************************
	 * 向哈希表中添加键值对                     *
	 * strKey 关键字,类型必须是string           *
	 * value  值，当值为"undefined"时默认为null *
	 ********************************************/
    this.add = function (strKey, value) {
        var rm = new ResMsg();
        if (typeof (strKey) == "string") {
            this._hashValue[strKey] = (typeof (value) != "undefined" ? value : null);
            this._iCount++;
        }
        else {
            rm.result = false;
            rm.message = "关键字必须是字符串";
        }
        return rm;
    }
    /********************************************
	 * 根据关键字得到值                         *
	 * key 关键字,类型必须是string              *
	 ********************************************/
    this.getValueByKey = function (key) {
        var rm = new ResMsg();
        if (typeof (key) == "string") {
            if (typeof (this._hashValue[key]) != 'undefined') {
                rm.value = this._hashValue[key];
            }
            else {
                rm.result = false;
                rm.message = "关键字不存在";
            }
        }
        else {
            rm.result = false;
            rm.message = "关键字必须是字符串";
        }
        return rm;
    }
    /********************************************
	 * 根据索引得到值                           *
	 * index 索引,类型必须是number              *
	 ********************************************/
    this.getValueByIndex = function (index) {
        var rm = new ResMsg();
        if (typeof (index) == "number") {
            if (index < 0 || index >= this._iCount) {
                rm.result = false;
                rm.message = "索引超出范围";
                return rm;
            }
            var i = 0;
            for (var key in this._hashValue) {
                if (i == index) {
                    rm.value = this._hashValue[key];
                    return rm;
                }
                i++;
            }
        }
        else {
            rm.result = false;
            rm.message = "索引必须是数字";
        }
        return rm;
    }
    /********************************************
	 * 判断哈希表中是否包含该关键字             *
	 * key 关键字,类型必须是string              *
	 ********************************************/
    this.containsKey = function (key) {
        var rm = new ResMsg();
        if (typeof (key) == "string") {
            if (typeof (this._hashValue[key]) == 'undefined') {
                rm.result = false;
                rm.message = "关键字不存在";
            }
        }
        else {
            rm.result = false;
            rm.message = "关键字必须是字符串";
        }
        return rm;
    }
    this.getKeysToArray = function () {
        var keys = [];
        for (var key in this._hashValue) {
            keys.push(key);
        }
        return keys;
    }
    /********************************************
	 * 根据索引得到关键字                       *
	 * index 索引,类型必须是number              *
	 ********************************************/
    this.getKeyByIndex = function (index) {
        var rm = new ResMsg();
        if (typeof (index) == "number") {
            if (index < 0 || index >= this._iCount) {
                rm.result = false;
                rm.message = "索引超出范围";
                return rm;
            }
            var i = 0;
            for (var key in this._hashValue) {
                if (i == index) {
                    rm.value = key;
                    return rm;
                }
                i++;
            }
        }
        else {
            rm.result = false;
            rm.message = "索引必须是数字";
        }
        return rm;
    }
    /********************************************
	 * 得到哈希表中键值对的数量                 *
	 ********************************************/
    this.count = function () {
        return this._iCount;
    }
    /********************************************
	 * 从哈希表中删除指定的键值对               *
	 * key 关键字,类型必须是string              *
	 ********************************************/
    this.remove = function (key) {
        var rm = new ResMsg();
        if (typeof (key) == "string") {
            if (typeof (this._hashValue[key]) == 'undefined') {
                rm.result = false;
                rm.message = "关键字不存在";
                return rm;
            }
            for (var strKey in this._hashValue) {
                if (key == strKey) {
                    delete this._hashValue[key];
                    this._iCount--;
                }
            }
        }
        else {
            rm.result = false;
            rm.message = "关键字必须是字符串";
        }
        return rm;
    }

    this.clear = function () {
        for (var key in this._hashValue) {
            delete this._hashValue[key];
        }
        this._iCount = 0;
    }
}

function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}


/****************************************************************
 *扩展jQuery方法，获取URL请求参数                               *
 *Get object of URL parameters var allVars = $.getUrlVars();    *
 *Getting URL var by its name var byName = $.getUrlVar('name'); *
 ****************************************************************/
$.extend({
    getUrlVars: function () {
        var vars = [], hash;
        var href = window.location.href;
        var hashes = href.slice(href.indexOf('?') + 1).split('&');
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
        }
        return vars;
    },
    getUrlVar: function (name) {
        return $.getUrlVars()[name];
    },
    ShowMask: function (msg, parent) {
        var parentObj;
        if (typeof (parent) != "undefined") {
            parentObj = parent;
        }
        else {
            parentObj = $("body");
        }
        if (!parentObj.children("div.datagrid-mask").length) {
//            $("<div class=\"datagrid-mask\" style=\"display:block\"></div>").appendTo(parentObj);
//            var msg = $("<div class=\"datagrid-mask-msg\" style=\"display:block;left:50%\"></div>").html(msg).appendTo(parentObj);
//            msg._outerHeight(40);
//            msg.css({ marginLeft: (-msg.outerWidth() / 2), lineHeight: (msg.height() + "px") });
            $("<div class='datagrid-mask'></div>").css({ display:"block","z-index":"999999" }).appendTo(parentObj); 
			$("<div class='datagrid-mask-msg' style='font-size:16px;z-index:999999'></div>").html(msg).appendTo(parentObj).css({ display:"block", left: (parentObj.width() - $("div.datagrid-mask-msg", parentObj).outerWidth()) / 2, top: (parentObj.height() - $("div.datagrid-mask-msg", parentObj).outerHeight()) / 2 }); 
        }
    },
    HideMask: function (parent) {
        var parentObj;
        if (typeof (parent) != "undefined") {
            parentObj = parent;
        }
        else {
            parentObj = $("body");
        }
        if (parentObj.children("div.datagrid-mask").length) {
            parentObj.children("div.datagrid-mask-msg").remove();
            parentObj.children("div.datagrid-mask").remove();
        }
    },
    //扩展复选框
    CheckBoxCombobox: function (id) {
        $('#' + id).combobox({
            valueField: 'id',
            textField: 'text',
            multiple: true,
            formatter: function (row) {
                var opts = $(this).combobox('options');
                return '<input type="checkbox" class="combobox-checkbox">' + row[opts.textField]
            },
            onLoadSuccess: function () {
                var opts = $(this).combobox('options');
                var target = this;
                var values = $(target).combobox('getValues');
                $.map(values, function (value) {
                    var el = opts.finder.getEl(target, value);
                    el.find('input.combobox-checkbox')._propAttr('checked', true);
                })
            },
            onSelect: function (row) {
                var opts = $(this).combobox('options');
                var el = opts.finder.getEl(this, row[opts.valueField]);
                el.find('input.combobox-checkbox')._propAttr('checked', true);
                if ($(this).combobox("getText").indexOf("全部") >= 0) {
                    var selectitemstext = $(this).combobox("getText").split(",");
                    if (selectitemstext[selectitemstext.length - 1].indexOf("全部") >= 0) {
                        var selectids = $(this).combobox("getValues");
                        for (var i = 0; i < selectids.length - 1; i++) {
                            $(this).combobox('unselect', selectids[i]);
                        }
                        $(this).combobox('select', row[opts.valueField]);
                    } else {
                        var selectids = $(this).combobox("getValues");
                        for (var i = 0; i < selectids.length - 1; i++) {
                            $(this).combobox('unselect', selectids[i]);
                        }
                        $(this).combobox('select', row[opts.valueField]);
                    }
                } else {
                    var selectitemstext = $(this).combobox("getText").split(",");
                    if (selectitemstext[selectitemstext.length - 1].indexOf("全部") >= 0) {
                        $(this).combobox('select', row[opts.valueField]);
                    } else {
                        $(this).combobox('select', row[opts.valueField]);
                    }
                }
            },
            onUnselect: function (row) {
                var opts = $(this).combobox('options');
                var el = opts.finder.getEl(this, row[opts.valueField]);
                el.find('input.combobox-checkbox')._propAttr('checked', false);
            }
        });
    },
    QueryCombobox: function (id) {
        var checkedNodeValues = []
        var devComboboxObj = $("#" + id);
        var value = devComboboxObj.combobox("getValue");
        //值为空时采用模糊获取ID
        if (value == null) {
            var text = devComboboxObj.combobox("getText");
            //文本为空时按Combobox加载数据获取设备ID
            if ($.trim(text) != "") {
                var comboboxData = devComboboxObj.combobox("getData");
                var opts = devComboboxObj.combobox('options');
                for (var i = 0; i < comboboxData.length; i++) {
                    if (comboboxData[i][opts.textField].indexOf(text) > -1) {
                        checkedNodeValues.push(comboboxData[i][opts.valueField]);
                    }
                }
            }
            return checkedNodeValues.join(",");
        }
        else {
            checkedNodeValues.push(value);
            return checkedNodeValues.join(",");
        }
    },
    QueryCheckBoxCombobox: function (id) {
        var checkedNodeValues = []
        var devComboboxObj = $("#" + id);
        var value = devComboboxObj.combobox("getValues");
        //值为空时采用模糊获取ID
        if (value.length == 0) {
            var text = devComboboxObj.combobox("getText");
            //文本为空时按Combobox加载数据获取设备ID
            if ($.trim(text) != "") {
                var comboboxData = devComboboxObj.combobox("getData");
                var opts = devComboboxObj.combobox('options');
                for (var i = 0; i < comboboxData.length; i++) {
                    if (comboboxData[i][opts.textField].indexOf(text) > -1) {
                        checkedNodeValues.push(comboboxData[i][opts.valueField]);
                    }
                }
            }
            return checkedNodeValues.join(",");
        }
        else {
            checkedNodeValues.push(value);
            return checkedNodeValues.join(",");
        }
    },
    CheckTime: function (_startTime, _endTime) {
        if (_startTime == "" || _startTime==null) {
            $.messager.alert("提示信息","请选择开始时间！");
            return false;
        }
        if (_endTime == "" || _endTime==null) {
            $.messager.alert("提示信息", "请选择结束时间！");
            return false;
        }
        var arrdate = _startTime.substr(0, 10).split("-");
        var arrtime = _startTime.substr(11, 5).split(":");
        var starttime = new Date(arrdate[0], arrdate[1] - 1, arrdate[2], arrtime[0], arrtime[1]);
        var starttimes = starttime.getTime();
        var earrdate = _endTime.substr(0, 10).split("-");
        var earrtime = _endTime.substr(11, 5).split(":");
        var endtime = new Date(earrdate[0], earrdate[1] - 1, earrdate[2], earrtime[0], earrtime[1]);
        var endimes = endtime.getTime();
        if (starttimes > endimes) {
            $.messager.alert("提示信息", "开始时间不能大于结束时间");
            return false;
        } else {
            return true;
        }
    }
});

if (window.jQuery) (function ($) {
    // Add function to jQuery namespace
    $.extend({
        // converts xml documents and xml text to json object
        xml2json: function (xml, extended) {
            if (!xml) {
                return {}; // quick fail
            }
            //### PARSER LIBRARY
            // Core function
            function parseXML(node, simple) {
                if (!node) {
                    return null;
                }
                var txt = '', obj = null, att = null;
                var nt = node.nodeType, nn = jsVar(node.localName || node.nodeName);
                var nv = node.text || node.nodeValue || '';
                /*DBG*/ //if(window.console) console.log(['x2j',nn,nt,nv.length+' bytes']);
                if (node.childNodes) {
                    if (node.childNodes.length > 0) {
                        /*DBG*/ //if(window.console) console.log(['x2j',nn,'CHILDREN',node.childNodes]);
                        $.each(node.childNodes, function (n, cn) {
                            var cnt = cn.nodeType, cnn = jsVar(cn.localName || cn.nodeName);
                            var cnv = cn.text || cn.nodeValue || '';
                            /*DBG*/ //if(window.console) console.log(['x2j',nn,'node>a',cnn,cnt,cnv]);
                            if (cnt == 8) {
                                /*DBG*/ //if(window.console) console.log(['x2j',nn,'node>b',cnn,'COMMENT (ignore)']);
                                return; // ignore comment node
                            }
                            else if (cnt == 3 || cnt == 4 || !cnn) {
                                // ignore white-space in between tags
                                if (cnv.match(/^\s+$/)) {
                                    /*DBG*/ //if(window.console) console.log(['x2j',nn,'node>c',cnn,'WHITE-SPACE (ignore)']);
                                    return;
                                };
                                /*DBG*/ //if(window.console) console.log(['x2j',nn,'node>d',cnn,'TEXT']);
                                txt += cnv.replace(/^\s+/, '').replace(/\s+$/, '');
                                // make sure we ditch trailing spaces from markup
                            }
                            else {
                                /*DBG*/ //if(window.console) console.log(['x2j',nn,'node>e',cnn,'OBJECT']);
                                obj = obj || {};
                                if (obj[cnn]) {
                                    /*DBG*/ //if(window.console) console.log(['x2j',nn,'node>f',cnn,'ARRAY']);
                                    if (!obj[cnn].length) {
                                        obj[cnn] = myArr(obj[cnn]);
                                    }
                                    obj[cnn][obj[cnn].length] = parseXML(cn, true/* simple */);
                                    obj[cnn].length = obj[cnn].length;
                                }
                                else {
                                    /*DBG*/ //if(window.console) console.log(['x2j',nn,'node>g',cnn,'dig deeper...']);
                                    obj[cnn] = parseXML(cn);
                                };
                            };
                        });
                    };//node.childNodes.length>0
                };//node.childNodes
                if (node.attributes) {
                    if (node.attributes.length > 0) {
                        /*DBG*/ //if(window.console) console.log(['x2j',nn,'ATTRIBUTES',node.attributes])
                        att = {}; obj = obj || {};
                        $.each(node.attributes, function (a, at) {
                            var atn = jsVar(at.name), atv = at.value;
                            att[atn] = atv;
                            if (obj[atn]) {
                                /*DBG*/ //if(window.console) console.log(['x2j',nn,'attr>',atn,'ARRAY']);
                                if (!obj[atn].length) {
                                    obj[atn] = myArr(obj[atn]);//[ obj[ atn ] ];
                                }
                                obj[atn][obj[atn].length] = atv;
                                obj[atn].length = obj[atn].length;
                            }
                            else {
                                /*DBG*/ //if(window.console) console.log(['x2j',nn,'attr>',atn,'TEXT']);
                                obj[atn] = atv;
                            };
                        });
                        //obj['attributes'] = att;
                    };//node.attributes.length>0
                };//node.attributes
                if (obj) {
                    obj = $.extend((txt != '' ? new String(txt) : {}),/* {text:txt},*/ obj || {}/*, att || {}*/);
                    txt = (obj.text) ? (typeof (obj.text) == 'object' ? obj.text : [obj.text || '']).concat([txt]) : txt;
                    if (txt) {
                        obj.text = txt;
                    }
                    txt = '';
                };
                var out = obj || txt;
                //console.log([extended, simple, out]);
                if (extended) {
                    if (txt) {
                        out = {};//new String(out);
                    }
                    txt = out.text || txt || '';
                    if (txt) {
                        out.text = txt;
                    }
                    if (!simple) {
                        out = myArr(out);
                    }
                };
                return out;
            };// parseXML
            // Core Function End
            // Utility functions
            var jsVar = function (s) {
                return String(s || '').replace(/-/g, "_");
            };
            var isNum = function (s) {
                return (typeof s == "number") || String((s && typeof s == "string") ? s : '').test(/^((-)?([0-9]*)((\.{0,1})([0-9]+))?$)/);
            };
            var myArr = function (o) {
                if (!o.length) {
                    o = [o];
                    o.length = o.length;
                }
                // here is where you can attach additional functionality, such as searching and sorting...
                return o;
            };
            // Utility functions End
            //### PARSER LIBRARY END

            // Convert plain text to xml
            if (typeof xml == 'string') {
                xml = $.text2xml(xml);
            }

            // Quick fail if not xml (or if this is a node)
            if (!xml.nodeType) {
                return;
            }
            if (xml.nodeType == 3 || xml.nodeType == 4) {
                return xml.nodeValue;
            }

            // Find xml root node
            var root = (xml.nodeType == 9) ? xml.documentElement : xml;

            // Convert xml to json
            var out = parseXML(root, true /* simple */);

            // Clean-up memory
            xml = null; root = null;

            // Send output
            return out;
        },

        // Convert text to XML DOM
        text2xml: function (str) {
            // NOTE: I'd like to use jQuery for this, but jQuery makes all tags uppercase
            //return $(xml)[0];
            var out;
            try {
                var xml = (/msie/.test(navigator.userAgent.toLowerCase())) ? new ActiveXObject("Microsoft.XMLDOM") : new DOMParser();
                xml.async = false;
            }
            catch (e) {
                throw new Error("XML Parser could not be instantiated")
            };
            try {
                if (/msie/.test(navigator.userAgent.toLowerCase())) {
                    out = (xml.loadXML(str)) ? xml : false;
                }
                else {
                    out = xml.parseFromString(str, "text/xml");
                }
            }
            catch (e) {
                throw new Error("Error parsing XML string")
            };
            return out;
        }
    }); // extend $
})(jQuery);

// JScript 文件

function OnlyNum()//限制输入为数字
{
	event = (event)?event:window.event
	var keyCode = -1;
    if(event.keyCode)
    {
        keyCode = event.keyCode;
    }
    else
    {
		keyCode = event.which;
    }
    if( (keyCode >47 && keyCode < 58) || (keyCode>=96 && keyCode<=105) || event.keyCode==46 || keyCode==8 )
    {
        event.returnValue =true;
        return true;
    }
    else
    {
        event.returnValue=false;
        return false;
    }
}

function OnlyNumDot()//限制输入为数字和小数点
{
	event = (event)?event:window.event
	var keyCode = -1;
    if(event.keyCode)
    {
        keyCode = event.keyCode;
    }
    else
    {
		keyCode = event.which;
    }
    if((keyCode >47 && keyCode < 58)||(keyCode>=96 && keyCode<=105) || event.keyCode==46 || event.keyCode==8 )
    {
        event.returnValue =true;
        return true;
    }
    else
    {
        event.returnValue=false;
        return false;
    }
}
function OnlyNumDotDashes()//限制输入为数字和小数点和负号
{
	event = (event)?event:window.event
	var keyCode = -1;
    if(event.keyCode)
    {
        keyCode = event.keyCode;
    }
    else
    {
		keyCode = event.which;
    }
    if((keyCode >47 && keyCode < 58) || (keyCode>=96 && keyCode<=105) ||(keyCode==46 || keyCode==45) || (keyCode==189 || keyCode==109 )|| (keyCode==190 || keyCode==110) || (keyCode==8 || keyCode==37 || keyCode==39))
    {
        event.returnValue =true;
        return true;
    }
    else
    {
        event.returnValue=false;
        return false;
    }
}

function OnlyNumDashes()//限制输入为数字和负号
{
	event = (event)?event:window.event
	var keyCode = -1;
    if(event.keyCode)
    {
        keyCode = event.keyCode;
    }
    else
    {
		keyCode = event.which;
    }
    if((keyCode>47&&keyCode<58) || (keyCode>=96 && keyCode<=105) || keyCode==45 || keyCode==8 )
    {
        event.returnValue=true;
        return true;
    }
    else
    {
        event.returnValue=false;
        return false;
    }
}

function test_email(strEmail) 
{ 
  var myReg = /^[_a-z0-9]+@([_a-z0-9]+\.)+[a-z0-9]{2,3}$/; 
  if(!myReg.test(strEmail)) 
  {
    alert('请填写信息');
    return false;
  }
    else
    {
    return true;
    }

 }
 function xIsNull(strText)
 {
    var myReg=/^S+$/;
    if(!myReg.test(strText))
    {
        alert('不能为空！');
        return false;
    }
    else 
    {
        return true;
    }
 } 