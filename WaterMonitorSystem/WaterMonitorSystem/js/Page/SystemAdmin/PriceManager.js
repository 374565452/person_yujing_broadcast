// JScript 文件
//当前登录操作员管理ID
var mnId = "";
//水价阶梯数量
var waterStepsCount = "";
//电价阶梯数量
var powerStepsCount = "";
//水价阶梯类型
var waterStepsType = "";
//电价阶梯类型
var powerStepsType = "";
//水价阶梯定额
var waterPercent = [];
//电价阶梯定额
var powerPercent = [];
//水价设置
var waterColumns = [];
//电价设置
var powerColumns = [];

//登录标识
var loginIdentifer = window.parent.guid;
//价格id
var priceID = "";


//-------------

$(document).ready(function () {
    var defaultTheme = $.cookie("psbsTheme");
    if (defaultTheme != null && defaultTheme != "default") {
        var link = $(document).find('link:first');
        link.attr('href', '../App_Themes/easyui/themes/' + defaultTheme + '/easyui.css');
    }
    $.ShowMask("数据加载中，请稍等……");
    //读取价格配置文件
    LoadPriceConfig();
});
//获取价格设置信息，加载显示列、数据列表
function LoadPriceConfig() {
    $.ajax({
        url: "../WebServices/PriceManageService.asmx/LoadPriceConfig?t=" + Math.random(),
        type: "Get",
        data: { "loginIdentifer": loginIdentifer },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var res = eval("(" + $.xml2json(responseText) + ")");
            if (res.Result == true) {
                var data = res.PriceConfig;
                //水价阶梯数量
                waterStepsCount = data.WaterStepsCount;
                //电价阶梯数量
                powerStepsCount = data.PowerStepsCount;
                //水价阶梯类型
                waterStepsType = data.WaterStepsType;
                //电价阶梯类型
                powerStepsType = data.PowerStepsType;
                //水价设置
                waterColumns = data.WaterColumns;
                //电价设置
                powerColumns = data.PowerColumns;

                for (var i = 1; i < waterColumns.length; i++) {
                    $("#waterPriceName" + i)[0].innerText = waterColumns[i].title + ":";
                }
                for (var i = 1; i < powerColumns.length; i++) {
                    $("#powerPriceName" + i)[0].innerText = powerColumns[i].title + ":";
                }

                //添加ID列、名称列
                waterColumns.push({ field: 'id', title: 'ID', hidden: 'true' });
                powerColumns.push({ field: 'id', title: 'ID', hidden: 'true' });
                //       
                //       waterColumns.push({field:'name',title:'名称',width:100,fixed:true});
                //       powerColumns.push({field:'name',title:'名称',width:100,fixed:true});

                //添加水价tab页中编辑、删除操作列，并设置按钮
                waterColumns.push({
                    field: 'edit', title: '编辑', width: 100,
                    formatter: function (val, row, index) { return '<Image src="../Images/Detail.gif" onclick="Btn_WaterEdit_Click(' + index + ')"/>'; }
                });

                waterColumns.push({
                    field: 'delete', title: '删除', width: 100,
                    formatter: function (val, row, index) { return '<Image src="../Images/Delete.gif" onclick="Btn_WaterDelete_Click(' + index + ')"/>'; }
                });

                //设置水价datagrid数据源
                $("#waterTab").datagrid({
                    columns: [waterColumns]
                });

                //添加电价tab页中编辑、删除操作列，并设置按钮
                powerColumns.push({
                    field: 'edit', title: '编辑', width: 100,
                    formatter: function (val, row, index) { return '<Image src="../Images/Detail.gif" onclick="Btn_PowerEdit_Click(' + index + ')"/>'; }
                });

                powerColumns.push({
                    field: 'delete', title: '删除', width: 100,
                    formatter: function (val, row, index) { return '<Image src="../Images/Delete.gif" onclick="Btn_PowerDelete_Click(' + index + ')"/>'; }
                });

                //设置电价datagrid数据源
                $("#powerTab").datagrid({
                    columns: [powerColumns]
                });

                //加载水价阶梯定额
                for (var i = 1; i < waterColumns.length; i++) {
                    waterPercent.push(waterColumns[i].percent);
                }
                //加载电价阶梯定额
                for (var i = 1; i < powerColumns.length; i++) {
                    powerPercent.push(powerColumns[i].percent);
                }

                //加载数据列表
                LoadWaterPriceInfos(loginIdentifer);
                LoadPowerPriceInfos(loginIdentifer);

            }
            else {
                $.HideMask();
                $.messager.alert("提示信息", "加载失败" + data.Message);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.HideMask();
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}


//加载数据库水价价格信息
function LoadWaterPriceInfos(loginIdentifer) {
    $.ajax({
        url: "../WebServices/PriceManageService.asmx/GetWaterPriceInfos?t=" + Math.random(),
        type: "GET",
        data: { "loginIdentifer": loginIdentifer },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result == true) {
                waterRecords = data.PriceInfos;
                var waterDatas = [];
                for (var i = 0; i < waterRecords.length; i++) {
                    var dataRow = {};
                    dataRow["ID"] = waterRecords[i].ID;
                    dataRow["name"] = waterRecords[i].名称;
                    for (var j = 1; j <= waterStepsCount; j++) {
                        switch (j) {
                            case 1:
                                dataRow["waterStep" + j] = waterRecords[i].一阶价格;
                                break;
                            case 2:
                                dataRow["waterStep" + j] = waterRecords[i].二阶价格;
                                break;
                            case 3:
                                dataRow["waterStep" + j] = waterRecords[i].三阶价格;
                                break;
                            case 4:
                                dataRow["waterStep" + j] = waterRecords[i].四阶价格;
                                break;
                            default:
                                break;
                        }
                    }
                    waterDatas.push(dataRow);

                }
                $("#waterTab").datagrid({ data: waterDatas });
                $.HideMask();
            }
            else {
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
//加载数据库电价价格信息
function LoadPowerPriceInfos(loginIdentifer) {
    $.ajax({
        url: "../WebServices/PriceManageService.asmx/GetPowerPriceInfos?t=" + Math.random(),
        type: "GET",
        data: { "loginIdentifer": loginIdentifer },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result == true) {
                var powerRecords = data.PriceInfos;
                var powerDatas = [];

                for (var i = 0; i < powerRecords.length; i++) {
                    var dataRow = {};
                    dataRow["ID"] = powerRecords[i].ID;
                    dataRow["name"] = powerRecords[i].名称;
                    for (var j = 1; j <= powerStepsCount; j++) {
                        switch (j) {
                            case 1:
                                dataRow["powerStep" + j] = powerRecords[i].一阶价格;
                                break;
                            case 2:
                                dataRow["powerStep" + j] = powerRecords[i].二阶价格;
                                break;
                            case 3:
                                dataRow["powerStep" + j] = powerRecords[i].三阶价格;
                                break;
                            case 4:
                                dataRow["powerStep" + j] = powerRecords[i].四阶价格;
                                break;
                            default:
                                break;
                        }

                    }
                    dataRow["edit"] = "<img src='../Images/Detail.gif' onclick='Btn_PowerEdit_Click()'/>";
                    dataRow["delete"] = "<img src='../Images/delete.gif' onclick='Btn_PowerDelete_Click()'/>";
                    powerDatas.push(dataRow);
                }
                $("#powerTab").datagrid({ data: powerDatas });
                $.HideMask();
            }
            else {
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

//添加水价价格信息
function Btn_WaterAdd_Click() {
    //弹出对话框
    $("#waterDialog").dialog({ closed: false });
    $("#waterDialog").dialog({ title: "添加水价价格信息" });

    $("#name").textbox('setText', "");
    $("#price1").textbox('setText', "");
    $("#price2").textbox('setText', "");
    $("#price3").textbox('setText', "");
    $("#price4").textbox('setText', "");

    for (var i = waterStepsCount + 1; i <= 4; i++) {
        $("#row" + i).css("display", "none");
    }
    //根据阶梯数量设置弹出框大小     
    AdjustSize("water", waterStepsCount);
}

function AdjustSize(priceType, stepsCount) {
    //根据阶梯数量设置弹出框大小
    switch (stepsCount) {
        case 1:
            $("#" + priceType + "Fieldset").css('height', 65);
            $("#" + priceType + "Dialog").dialog({ width: 350, height: 150 });
            break;
        case 2:

            $("#" + priceType + "Fieldset").css('height', 95);
            $("#" + priceType + "Dialog").dialog({ width: 350, height: 180 });
            break;
        case 3:

            $("#" + priceType + "Fieldset").css('height', 125);
            $("#" + priceType + "Dialog").dialog({ width: 350, height: 210 });
            break;
        case 4:

            $("#" + priceType + "Fieldset").css('height', 155);
            $("#" + priceType + "Dialog").dialog({ width: 350, height: 240 });
            break;
        default:
            break;
    }

}

//添加电价价格信息
function Btn_PowerAdd_Click() {
    $("#powerDialog").dialog({ title: "添加电价价格信息" });

    $("#powerName").textbox('setText', "");
    $("#powerPrice1").textbox('setText', "");
    $("#powerPrice2").textbox('setText', "");
    $("#powerPrice3").textbox('setText', "");
    $("#powerPrice4").textbox('setText', "");

    for (var i = powerStepsCount + 1; i <= 4; i++) {
        $("#powerRow" + i).css("display", "none");
    }
    //根据阶梯数量设置弹出框大小     
    AdjustSize("power", powerStepsCount);
    //弹出对话框
    $("#powerDialog").dialog({ closed: false });
}

//保存电价价格信息
function Btn_PowerSave_Click() {
    //验证数据完整性
    for (var i = 1; i <= powerStepsCount; i++) {
        if ($("#powerPrice" + i).textbox("getText") == "") {
            $.messager.alert("提示信息", "请将信息填写完整");
            return;
        }
        //判断输入的是否为正数
        if (isNaN($("#powerPrice" + i).textbox("getText")) || $("#powerPrice" + i).textbox("getText") < 0) {
            $.messager.alert("提示信息", "请输入正数");
            $("#powerPrice" + i).textbox('setText', "");
            return;
        }
    }
    if ($("#powerName").textbox("getText") == "") {
        $.messager.alert("提示信息", "请将信息填写完整");
        return;
    }
    //价格信息json对象
    var priceJson = "{";
    priceJson += "'名称':'" + $("#powerName").textbox("getText") + "',";

    priceJson += "'价格类型':'2',";
    priceJson += "'阶梯类型':'" + powerStepsType + "',";

    for (var i = 1; i <= powerStepsCount; i++) {
        switch (i) {
            case 1:
                priceJson += "'一阶水量':'" + powerPercent[(i - 1)] + "',";
                priceJson += "'一阶价格':'" + $("#powerPrice" + i).textbox("getText") + "',";
                break;
            case 2:
                priceJson += "'二阶水量':'" + powerPercent[(i - 1)] + "',";
                priceJson += "'二阶价格':'" + $("#powerPrice" + i).textbox("getText") + "',";
                break;
            case 3:
                priceJson += "'三阶水量':'" + powerPercent[(i - 1)] + "',";
                priceJson += "'三阶价格':'" + $("#powerPrice" + i).textbox("getText") + "',";
                break;
            case 4:
                priceJson += "'四阶水量':'" + powerPercent[(i - 1)] + "',";
                priceJson += "'四阶价格':'" + $("#powerPrice" + i).textbox("getText") + "',";
                break;
            default:
                break;
        }
    }
    priceJson += "'阶梯数量':'" + powerStepsCount + "'";
    if ($("#powerDialog").panel('options').title == "添加电价价格信息") {
        priceJson += "}";
        //添加价格信息
        $.ajax({
            url: "../WebServices/PriceManageService.asmx/AddPriceInfo",
            type: "GET",
            data: { "loginIdentifer": loginIdentifer, "priceJson": priceJson },
            dataType: "text",
            cache: false,
            success: function (responseText) {
                var data = eval("(" + $.xml2json(responseText) + ")");
                if (data.Result == true) {
                    $.messager.alert("提示信息", "价格信息添加成功");
                    $("#powerDialog").dialog({ closed: true });
                    //更新主页面
                    LoadWaterPriceInfos(loginIdentifer);
                    LoadPowerPriceInfos(loginIdentifer);
                }
                else {
                    $.messager.alert("提示信息", data.Message);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
            }
        });
    }
    else {
        priceJson += ",'ID':'" + priceID + "'}";
        //修改价格信息
        $.ajax({
            url: "../WebServices/PriceManageService.asmx/ModifyPriceInfo",
            type: "GET",
            data: { "loginIdentifer": loginIdentifer, "priceJson": priceJson },
            dataType: "text",
            cache: false,
            success: function (responseText) {
                var data = eval("(" + $.xml2json(responseText) + ")");
                if (data.Result == true) {
                    $.messager.alert("提示信息", "价格信息修改成功");
                    $("#powerDialog").dialog({ closed: true });
                    //更新主页面
                    LoadWaterPriceInfos(loginIdentifer);
                    LoadPowerPriceInfos(loginIdentifer);
                    //显示电价tab页
                    $("#priceTabs").tabs('select', '电价');
                }
                else {
                    $.messager.alert("提示信息", data.Message);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
            }
        });
    }
}

//保存水价价格信息
function Btn_WaterSave_Click() {
    //验证数据完整性
    for (var i = 1; i <= waterStepsCount; i++) {
        if ($("#price" + i).textbox("getText") == "") {
            $.messager.alert("提示信息", "请将信息填写完整");
            return;
        }
        //判断输入的是否为正数
        if (isNaN($("#price" + i).textbox("getText")) || $("#price" + i).textbox("getText") < 0) {
            $.messager.alert("提示信息", "请输入正数");
            $("#price" + i).textbox('setText', "");
            return;
        }
    }
    if ($("#name").textbox("getText") == "") {
        $.messager.alert("提示信息", "请将信息填写完整");
        return;
    }

    //价格信息json对象
    var priceJson = "{";
    priceJson += "'名称':'" + $("#name").textbox("getText") + "',";

    priceJson += "'价格类型':'1',";
    priceJson += "'阶梯类型':'" + waterStepsType + "',";

    for (var i = 1; i <= waterStepsCount; i++) {
        switch (i) {
            case 1:
                priceJson += "'一阶水量':'" + waterPercent[(i - 1)] + "',";
                priceJson += "'一阶价格':'" + $("#price" + i).textbox("getText") + "',";
                break;
            case 2:
                priceJson += "'二阶水量':'" + waterPercent[(i - 1)] + "',";
                priceJson += "'二阶价格':'" + $("#price" + i).textbox("getText") + "',";
                break;
            case 3:
                priceJson += "'三阶水量':'" + waterPercent[(i - 1)] + "',";
                priceJson += "'三阶价格':'" + $("#price" + i).textbox("getText") + "',";
                break;
            case 4:
                priceJson += "'四阶水量':'" + waterPercent[(i - 1)] + "',";
                priceJson += "'四阶价格':'" + $("#price" + i).textbox("getText") + "',";
                break;
            default:
                break;
        }
    }
    priceJson += "'阶梯数量':'" + waterStepsCount + "'";
    if ($("#waterDialog").panel('options').title == "添加水价价格信息") {
        priceJson += "}";
        //添加价格信息
        $.ajax({
            url: "../WebServices/PriceManageService.asmx/AddPriceInfo",
            type: "GET",
            data: { "loginIdentifer": loginIdentifer, "priceJson": priceJson },
            dataType: "text",
            cache: false,
            success: function (responseText) {
                var data = eval("(" + $.xml2json(responseText) + ")");
                if (data.Result == true) {
                    $.messager.alert("提示信息", "价格信息添加成功");
                    $("#waterDialog").dialog({ closed: true });
                    //更新主页面
                    LoadWaterPriceInfos(loginIdentifer);
                    LoadPowerPriceInfos(loginIdentifer);
                }
                else {
                    $.messager.alert("提示信息", data.Message);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
            }
        });
    }
    else {
        //获取价格信息ID
        priceJson += ",'ID':'" + priceID + "'}";
        //保存价格信息
        $.ajax({
            url: "../WebServices/PriceManageService.asmx/ModifyPriceInfo",
            type: "GET",
            data: { "loginIdentifer": loginIdentifer, "priceJson": priceJson },
            dataType: "text",
            cache: false,
            success: function (responseText) {
                var data = eval("(" + $.xml2json(responseText) + ")");
                if (data.Result == true) {
                    $.messager.alert("提示信息", "价格信息修改成功");
                    $("#waterDialog").dialog({ closed: true });
                    //更新主页面
                    LoadWaterPriceInfos(loginIdentifer);
                    LoadPowerPriceInfos(loginIdentifer);
                }
                else {
                    $.messager.alert("提示信息", data.Message);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
            }
        });

    }
}

function Btn_WaterCancel_Click() {
    $("#waterDialog").dialog({ closed: true });
}
function Btn_PowerCancel_Click() {
    $("#powerDialog").dialog({ closed: true });
}


//编辑水价价格信息
function Btn_WaterEdit_Click(index) {

    $("#waterDialog").dialog({ title: "编辑水价价格信息" });
    //获取所选行数据
    $("#waterTab").datagrid('selectRow', index);
    var row = $("#waterTab").datagrid("getSelected");
    //数据载入到对话框中
    $("#name").textbox('setText', row.name);
    for (var i = 1; i <= waterStepsCount; i++) {
        $("#price" + i).textbox('setText', row["waterStep" + i]);
    }

    //价格ID
    priceID = row.ID;
    for (i = waterStepsCount + 1; i <= 4; i++) {
        $("#row" + i).css("display", "none");
    }
    //根据阶梯数量设置弹出框大小     
    AdjustSize("water", waterStepsCount);

    //弹出水价价格对话框
    $("#waterDialog").dialog({ closed: false });
}

//删除水价价格信息
function Btn_WaterDelete_Click(index) {
    //获取所选行数据
    $("#waterTab").datagrid('selectRow', index);
    var row = $("#waterTab").datagrid("getSelected");
    priceId = row.ID;
    //调用删除服务
    DeletePriceInfo();
    $("#priceTabs").tabs('select', '水价');
}

//编辑电价价格信息
function Btn_PowerEdit_Click(index) {
    //获取所选行的数据
    $("#powerTab").datagrid('selectRow', index);
    var row = $("#powerTab").datagrid("getSelected");

    $("#powerDialog").dialog({ title: "编辑电价价格信息" });
    //对话框中加载所选行数据
    $("#powerName").textbox('setText', row.name);
    for (var i = 1; i <= powerStepsCount; i++) {
        $("#powerPrice" + i).textbox('setText', row["powerStep" + i]);
    }

    //价格ID
    priceID = row.ID;
    for (var i = powerStepsCount + 1; i <= 4; i++) {
        $("#powerRow" + i).css("display", "none");
    }
    //根据阶梯数量设置弹出框大小     
    AdjustSize("power", powerStepsCount);

    //弹出编辑对话框
    $("#powerDialog").dialog({ closed: false });
}

//删除电价价格信息
function Btn_PowerDelete_Click(index) {
    //获取所选行数据
    $("#powerTab").datagrid('selectRow', index);
    var row = $("#powerTab").datagrid("getSelected");
    priceId = row.ID;
    //调用删除服务
    DeletePriceInfo();
    $("#priceTabs").tabs('select', '电价');
}

//删除价格信息
function DeletePriceInfo() {
    //弹出提示
    $.messager.confirm("提示信息", "确认要删除此价格信息?", function (r) {
        if (r) {
            //调用删除服务
            $.ajax({
                url: "../WebServices/PriceManageService.asmx/DeletePriceInfo",
                type: "GET",
                data: { "loginIdentifer": loginIdentifer, "priceId": priceId },
                dataType: "text",
                cache: false,
                success: function (responseText) {
                    var data = eval("(" + $.xml2json(responseText) + ")");
                    if (data.Result == true) {
                        $.messager.alert("提示信息", "价格信息删除成功");
                        //更新主页面
                        LoadWaterPriceInfos(loginIdentifer);
                        LoadPowerPriceInfos(loginIdentifer);
                    }
                    else {
                        $.messager.alert("提示信息", data.Message);
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
                }
            });
        }
    });
}
