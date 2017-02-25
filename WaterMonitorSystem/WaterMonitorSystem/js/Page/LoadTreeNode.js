// JScript 文件

/*各项内容加载完成的标志*/
var manageNodeLoaded = false;
var deviceNodeLoaded = false;
var commanageNodeLoaded = false;
var comdeviceNodeLoaded = false;
var userNodeLoaded = false;
var waterUserLoaded = false;
var treeLoaded = false;

//管理节点信息
var manageJson = [];
var commanageJson = [];
//设备节点信息
var deviceJson = [];
var comdeviceJson = [];
//用水户节点信息
var waterUserJson = [];
//用户节点信息
var userJson = [];
//管理节点的树节点的集合
var manageNodes = {};
var commanageNodes = {};
//设备节点的树节点的集合
var deviceNodes = {};
var comdeviceNodes = {};
//用户节点的树节点的集合
var waterUserNodes = {};
//用户节点的树节点的集合
var userNodes = {};
//选中的管理节点的树节点的集合
var checkedManageIds = [];
//选中的设备节点的树节点的集合
var checkedDeviceIds = [];
//选中的设备节点的树节点的集合
var checkedWaterUserIds = [];
//选中的用户节点的树节点的集合
var checkedUserIds = [];
//村庄ID
var villageid = "";
//镇ID
var townid = "";
//所有用水户ID
var allid = "";
//所有正常用水户ID
var allnormalid = "";
//村庄节点的集合
var villageNodes = [];

//取管理节点
function GetManageNode(mid) {
    $.ajax(
    {
        url: "../WebServices/ManageNodeService.asmx/GetManageNodeInfos",
        type: "GET",
        data: { "mnID": mid, "loginIdentifer": window.parent.guid, "isRecursive": true, "isExport": false },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                manageNodes = {};
                manageJson = data.ManageNodes
                for (i = 0; i < manageJson.length; i++) {
                    //节点附加属性
                    var mInfo =
                    {
                        mid: manageJson[i].ID,
                        nodeType: 'manage',
                        uid: manageJson[i].上级ID,
                        manage: manageJson[i]
                    };
                    var treeNode =
                    {
                        "id": "mn_" + manageJson[i].ID,
                        "text": manageJson[i].名称,
                        "attributes": mInfo,
                        "children": []
                    }
                    // 索引之
                    manageNodes[manageJson[i].ID] = treeNode;
                }
                manageNodeLoaded = true;
            }
            else {
                $.messager.alert("提示信息", data.Message);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //alert(errorThrown + "\r\n" + XMLHttpRequest.responseText);
        }
    });
}

//取管理节点
function GetComManageNode(mid) {
    $.ajax(
    {
        url: "../WebServices/ManageNodeService.asmx/GetManageNodeInfos",
        type: "GET",
        data: { "mnID": mid, "loginIdentifer": window.parent.guid, "isRecursive": true, "isExport": false },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                commanageNodes = {};
                commanageJson = data.ManageNodes;
                for (i = 0; i < commanageJson.length; i++) {
                    //节点附加属性
                    var mInfo =
                    {
                        mid: commanageJson[i].ID,
                        nodeType: 'manage',
                        uid: commanageJson[i].上级ID,
                        manage: commanageJson[i]
                    };
                    var treeNode =
                    {
                        "id": "mn_" + commanageJson[i].ID,
                        "text": commanageJson[i].名称,
                        "attributes": mInfo,
                        "children": []
                    }
                    // 索引之
                    commanageNodes[commanageJson[i].ID] = treeNode;
                }
                commanageNodeLoaded = true;
            }
            else {
                $.messager.alert("提示信息", data.Message);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //alert(errorThrown + "\r\n" + XMLHttpRequest.responseText);
        }
    });
}

//取管理节点
function GetWaterManageNode(mid) {
    $.ajax(
    {
        url: "../WebServices/ManageNodeService.asmx/GetManageNodeInfos",
        type: "GET",
        data: { "mnID": mid, "loginIdentifer": window.parent.guid, "isRecursive": true, "isExport": false },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                manageNodes = {};
                manageJson = data.ManageNodes
                for (i = 0; i < manageJson.length; i++) {
                    //节点附加属性
                    var mInfo =
                    {
                        mid: manageJson[i].ID,
                        nodeType: 'manage',
                        uid: manageJson[i].上级ID,
                        manage: manageJson[i]
                    };
                    var treeNode = {};
                    if (manageJson[i].级别名称 == "村庄") {
                        treeNode = {
                            "id": "cz_" + manageJson[i].ID,
                            "text": manageJson[i].名称,
                            "attributes": mInfo,
                            "children": []
                        }

                    } else {
                        treeNode = {
                            "id": "mn_" + manageJson[i].ID,
                            "text": manageJson[i].名称,
                            "attributes": mInfo,
                            "children": []
                        }
                    }

                    // 索引之
                    manageNodes[manageJson[i].ID] = treeNode;
                }
                manageNodeLoaded = true;
            }
            else {
                $.messager.alert("提示信息", data.Message);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //alert(errorThrown + "\r\n" + XMLHttpRequest.responseText);
        }
    });
}

function GetStatisticManageNode(mid) {
    $.ajax(
    {
        url: "../WebServices/ManageNodeService.asmx/GetManageNodeInfos",
        type: "GET",
        data: { "mnID": mid, "loginIdentifer": window.parent.guid, "isRecursive": true, "isExport": false },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result) {
                manageNodes = {};
                villageNodes = [];
                manageJson = data.ManageNodes
                for (i = 0; i < manageJson.length; i++) {
                    //节点附加属性
                    var mInfo =
                    {
                        mid: manageJson[i].ID,
                        nodeType: 'manage',
                        uid: manageJson[i].上级ID,
                        manage: manageJson[i]
                    };
                    var treeNode = {};
                    if (manageJson[i].级别名称 == "乡镇") {
                        treeNode = {
                            "id": "z_" + manageJson[i].ID,
                            "text": manageJson[i].名称,
                            "attributes": mInfo,
                            "children": []
                        }
                    } else {
                        treeNode = {
                            "id": "mn_" + manageJson[i].ID,
                            "text": manageJson[i].名称,
                            "attributes": mInfo,
                            "children": []
                        }
                    }
                    if (manageJson[i].级别名称 == "村庄") {
                        villageNodes.push(treeNode)
                        continue;
                    }

                    // 索引之
                    manageNodes[manageJson[i].ID] = treeNode;
                }
                manageNodeLoaded = true;
            }
            else {
                $.messager.alert("提示信息", data.Message);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //alert(errorThrown + "\r\n" + XMLHttpRequest.responseText);
        }
    });
}

//取设备节点
function GetDeviceNode(mid) {
    $.ajax(
    {
        url: "../WebServices/DeviceNodeService.asmx/GetDeviceNodeInfosByMnId",
        type: "GET",
        data: { loginIdentifer: window.parent.guid, mnID: mid, isRecursive: true, isExport: false },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result)//登录成功
            {
                deviceNodes = {};
                deviceJson = data.DeviceNodes;
                for (i = 0; i < deviceJson.length; i++) {
                    //节点附加属性
                    var dInfo =
                    {
                        did: deviceJson[i].ID,
                        nodeType: 'device',
                        uid: deviceJson[i].管理ID,
                        device: deviceJson[i]
                    };
                    var treeNode =
                    {
                        "id": "dn_" + deviceJson[i].ID,
                        "text": "<span style='color:blue'>" + deviceJson[i].名称 + "</span>",
                        "attributes": dInfo,
                        "children": []
                    }

                    // 索引之
                    deviceNodes[deviceJson[i].ID] = treeNode;
                }
                deviceNodeLoaded = true;
            }
            else {
                $.messager.alert("提示信息", data.Message);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //alert(errorThrown + "\r\n" + XMLHttpRequest.responseText);
        }
    });
}

//取设备节点
function GetComDeviceNode(mid) {
    $.ajax(
    {
        url: "../WebServices/DeviceNodeService.asmx/GetDeviceNodeInfosByMnId",
        type: "GET",
        data: { loginIdentifer: window.parent.guid, mnID: mid, isRecursive: true, isExport: false },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result)//登录成功
            {
                comdeviceNodes = {};
                comdeviceJson = data.DeviceNodes;
                for (i = 0; i < comdeviceJson.length; i++) {
                    //节点附加属性
                    var dInfo =
                    {
                        did: comdeviceJson[i].ID,
                        nodeType: 'device',
                        uid: comdeviceJson[i].管理ID,
                        device: comdeviceJson[i]
                    };
                    var treeNode =
                    {
                        "id": "cd_" + comdeviceJson[i].ID,
                        "text": "<span style='color:blue'>" + comdeviceJson[i].名称 + "</span>",
                        "attributes": dInfo,
                        "children": []
                    }

                    // 索引之
                    comdeviceNodes[comdeviceJson[i].ID] = treeNode;
                }
                comdeviceNodeLoaded = true;
            }
            else {
                $.messager.alert("提示信息", data.Message);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //alert(errorThrown + "\r\n" + XMLHttpRequest.responseText);
        }
    });
}

//取设备节点
function GetGroundWaterDeviceNode(mid) {
    $.ajax(
    {
        url: "../WebServices/DeviceGroundWaterNodeService.asmx/GetDeviceNodeInfosByMnId",
        type: "GET",
        data: { loginIdentifer: window.parent.guid, mnID: mid, isRecursive: true, isExport: false },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result)//登录成功
            {
                deviceNodes = {};
                deviceJson = data.DeviceNodes;
                for (i = 0; i < deviceJson.length; i++) {
                    //节点附加属性
                    var dInfo =
                    {
                        did: deviceJson[i].ID,
                        nodeType: 'device',
                        uid: deviceJson[i].管理ID,
                        device: deviceJson[i]
                    };
                    var treeNode =
                    {
                        "id": "dn_" + deviceJson[i].ID,
                        "text": "<span style='color:blue'>" + deviceJson[i].名称 + "</span>",
                        "attributes": dInfo,
                        "children": []
                    }

                    // 索引之
                    deviceNodes[deviceJson[i].ID] = treeNode;
                }
                deviceNodeLoaded = true;
            }
            else {
                $.messager.alert("提示信息", data.Message);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //alert(errorThrown + "\r\n" + XMLHttpRequest.responseText);
        }
    });
}

//取用户节点
function GetUserNode(mid) {
    $.ajax(
    {
        url: "../WebServices/UserAdminService.asmx/GetUserInfos",
        type: "GET",
        data: { "managerId": mid, "loginIdentifer": window.parent.guid, "isRecursive": true, "isExport": false },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result)//登录成功
            {
                userNodes = {};
                userJson = data.UserNodes;
                for (i = 0; i < userJson.length; i++) {
                    //节点附加属性
                    var dInfo =
                    {
                        did: userJson[i].ID,
                        nodeType: 'user',
                        uid: userJson[i].ID,
                        user: userJson[i]
                    };
                    var treeNode =
                    {
                        "id": "dn_" + userJson[i].ID,
                        "text": "<span style='color:blue'>" + userJson[i].用户名 + "</span>",
                        "attributes": dInfo,
                        "children": []
                    }

                    // 索引之
                    userNodes[userJson[i].ID] = treeNode;
                }
                userNodeLoaded = true;
            }
            else {

                $.messager.alert("提示信息", data.Message);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //alert(errorThrown + "\r\n" + XMLHttpRequest.responseText);
        }
    });
}

function LoadTree(treeId, mnId, loadDevice, checkbox) {
    manageNodeLoaded = false;
    deviceNodeLoaded = false;
    treeLoaded = false;
    checkedManageIds = [];
    checkedDeviceIds = [];

    GetManageNode(mnId);
    if (loadDevice) {
        GetDeviceNode(mnId);
    }
    InitTree(treeId, loadDevice, checkbox);
}

function LoadStatisticTree(treeId, mnId, loadDevice, checkbox) {
    manageNodeLoaded = false;
    deviceNodeLoaded = false;
    treeLoaded = false;
    checkedManageIds = [];
    checkedDeviceIds = [];

    GetStatisticManageNode(mnId);
    if (loadDevice) {
        GetDeviceNode(mnId);
    }
    InitStatisticTree(treeId, loadDevice, checkbox);
}

function LoadWaterUserTree(treeId, mnId, loadWaterUser, checkbox) {
    manageNodeLoaded = false;
    waterUserLoaded = false;
    treeLoaded = false;
    checkedManageIds = [];
    checkedWaterUserIds = [];

    GetWaterManageNode(mnId);
    if (loadWaterUser) {
        GetWaterUser(mnId);
    }
    InitWaterUserTree(treeId, loadWaterUser, checkbox);
}

function LoadWaterUserTree2(treeId1, treeId2, mnId, loadWaterUser, checkbox) {
    manageNodeLoaded = false;
    waterUserLoaded = false;
    treeLoaded = false;
    checkedManageIds = [];
    checkedWaterUserIds = [];

    GetWaterManageNode(mnId);
    if (loadWaterUser) {
        GetWaterUser(mnId);
    }
    InitWaterUserTree2(treeId1, treeId2, loadWaterUser, checkbox);
}

function LoadUserTree(treeId, mnId, loadUser, checkbox) {
    manageNodeLoaded = false;
    userNodeLoaded = false;
    treeLoaded = false;
    checkedManageIds = [];
    checkedUserIds = [];

    GetManageNode(mnId);
    if (loadUser) {
        GetUserNode(mnId);
    }
    InitUserTree(treeId, loadUser, checkbox);
}

function LoadEventTree(treeId, mnId, loadDevice, checkbox) {
    manageNodeLoaded = false;
    deviceNodeLoaded = false;
    treeLoaded = false;
    checkedManageIds = [];
    checkedDeviceIds = [];

    GetManageNode(mnId);
    if (loadDevice) {
        GetDeviceNode(mnId);
    }
    InitEventTree(treeId, loadDevice, checkbox);
}

function LoadChartTree(treeId, mnId, loadDevice, checkbox) {
    manageNodeLoaded = false;
    deviceNodeLoaded = false;
    treeLoaded = false;
    checkedManageIds = [];
    checkedDeviceIds = [];

    GetManageNode(mnId);
    if (loadDevice) {
        GetDeviceNode(mnId);
    }
    InitChartTree(treeId, loadDevice, checkbox);
}

function LoadComTree(treeId, mnId, loadDevice, checkbox) {
    checkedManageIds = [];
    checkedDeviceIds = [];

    GetComManageNode(mnId);
    if (loadDevice) {
        GetComDeviceNode(mnId);
    }
    InitComTree(treeId, loadDevice, checkbox);
}

function LoadGroundWaterTree(treeId, mnId, loadDevice, checkbox) {
    checkedManageIds = [];
    checkedDeviceIds = [];

    GetManageNode(mnId);
    if (loadDevice) {
        GetGroundWaterDeviceNode(mnId);
    }
    InitTree(treeId, loadDevice, checkbox);
}

function InitTree(treeId, loadDevice, checkbox) {
    if (!manageNodeLoaded || (loadDevice && !deviceNodeLoaded)) {
        window.setTimeout("InitTree('" + treeId + "'," + loadDevice + "," + checkbox + ")", 500);
        return;
    }
    var treeData = [];
    // 挂载节点
    for (i = 0; i < manageJson.length; i++) {
        if (checkbox) {
            manageNodes[manageJson[i].ID]["checked"] = true;
            checkedManageIds.push(manageJson[i].ID);
        }
        // 添加到上级节点中
        if (manageJson[i].上级ID == '0' || manageNodes[manageJson[i].上级ID] == null) {
            // 根节点
            treeData.push(manageNodes[manageJson[i].ID]);
        }
        else {
            // 上级节点
            manageNodes[manageJson[i].上级ID].children.push(manageNodes[manageJson[i].ID]);
        }
    }
    if (loadDevice) {
        // 挂载节点
        for (i = 0; i < deviceJson.length; i++) {
            // 添加到上级节点中
            if (deviceJson[i].上级ID == '0') {
                // 根节点
                continue;
            }
            else {
                if (checkbox) {
                    deviceNodes[deviceJson[i].ID]["checked"] = true;
                    checkedDeviceIds.push(deviceJson[i].ID);
                }
                // 上级节点
                manageNodes[deviceJson[i].管理ID].children.push(deviceNodes[deviceJson[i].ID]);
            }
        }//结束挂载设备节点
    }
    $("#" + treeId).tree({
        checkbox: checkbox,
        data: treeData,
        onCheck: function (node, checked) {
            var nodes = $("#" + treeId).tree("getChecked");
            var checkedMnIDs = [];
            var checkedDevIDs = [];
            for (var i = 0; i < nodes.length; i++) {
                if (nodes[i].attributes["nodeType"] == "device") {
                    checkedDevIDs.push(nodes[i].attributes["did"])
                }
                else {
                    checkedMnIDs.push(nodes[i].attributes["mid"])
                }
            }
            checkedManageIds = checkedMnIDs;
            checkedDeviceIds = checkedDevIDs;
        }
    });
    treeLoaded = true;
}

function InitStatisticTree(treeId, loadDevice, checkbox) {
    if (!manageNodeLoaded || (loadDevice && !deviceNodeLoaded)) {
        window.setTimeout("InitStatisticTree('" + treeId + "'," + loadDevice + "," + checkbox + ")", 500);
        return;
    }
    var treeData = [];
    var czflag = false;
    // 挂载节点
    for (i = 0; i < manageJson.length; i++) {
        if (checkbox) {
            manageNodes[manageJson[i].ID]["checked"] = true;
            checkedManageIds.push(manageJson[i].ID);
        }
        // 添加到上级节点中
        if (manageJson[i].上级ID == '0' || manageNodes[manageJson[i].上级ID] == null) {
            // 根节点
            treeData.push(manageNodes[manageJson[i].ID]);
        }
        else {
            // 上级节点
            manageNodes[manageJson[i].上级ID].children.push(manageNodes[manageJson[i].ID]);
        }

        if (manageJson[i] != undefined) {
            if (manageNodes[manageJson[i].ID] != undefined) {
                if (manageNodes[manageJson[i].ID].id.indexOf("z_") >= 0) {
                    if (!czflag) {
                        townid = manageNodes[manageJson[i].ID].attributes.mid;
                        czflag = true;
                    }
                }
            }
        }

    }
    if (loadDevice) {
        // 挂载节点
        for (i = 0; i < deviceJson.length; i++) {
            // 添加到上级节点中
            if (deviceJson[i].上级ID == '0') {
                // 根节点
                continue;
            }
            else {
                if (checkbox) {
                    deviceNodes[deviceJson[i].ID]["checked"] = true;
                    checkedDeviceIds.push(deviceJson[i].ID);
                }
                // 上级节点
                manageNodes[deviceJson[i].管理ID].children.push(deviceNodes[deviceJson[i].ID]);
            }

        }//结束挂载设备节点
    }
    $("#" + treeId).tree({
        checkbox: checkbox,
        data: treeData
    });
    var selectnode = $('#divAreaTree').tree('find', "z_" + townid);
    $('#divAreaTree').tree('select', selectnode.target);
    treeLoaded = true;
}

function InitWaterUserTree(treeId, loadWaterUser, checkbox) {
    if (!manageNodeLoaded || (loadWaterUser && !waterUserLoaded)) {
        window.setTimeout("InitWaterUserTree('" + treeId + "'," + loadWaterUser + "," + checkbox + ")", 500);
        return;
    }
    var treeData = [];
    var czflag = false;
    // 挂载节点
    for (i = 0; i < manageJson.length; i++) {
        if (checkbox) {
            manageNodes[manageJson[i].ID]["checked"] = true;
            checkedManageIds.push(manageJson[i].ID);
        }
        // 添加到上级节点中
        if (manageJson[i].上级ID == '0' || manageNodes[manageJson[i].上级ID] == null) {
            // 根节点
            treeData.push(manageNodes[manageJson[i].ID]);
        }
        else {
            // 上级节点
            manageNodes[manageJson[i].上级ID].children.push(manageNodes[manageJson[i].ID]);
        }


        if (manageNodes[manageJson[i].ID].id.indexOf("cz") >= 0) {
            if (!czflag) {
                villageid = manageNodes[manageJson[i].ID].attributes.mid;
                czflag = true;
            }

        }
    }
    if (loadWaterUser) {
        // 挂载节点
        for (i = 0; i < waterUserJson.length; i++) {
            // 添加到上级节点中
            if (waterUserJson[i].管理ID == '0') {
                // 根节点
                continue;
            }
            else {
                if (checkbox) {
                    waterUserNodes[waterUserJson[i].ID]["checked"] = true;
                    checkedWaterUserIds.push(waterUserJson[i].ID);
                }
                // 上级节点
                manageNodes[deviceJson[i].管理ID].children.push(waterUserNodes[waterUserJson[i].ID]);
            }
        }//结束挂载设备节点
    }
    $("#" + treeId).tree({
        checkbox: checkbox,
        data: treeData
    });
    var selectnode = $("#" + treeId).tree('find', "cz_" + villageid);
    if (selectnode != null) {
        //alert("有村庄节点！");
        $("#" + treeId).tree('select', selectnode.target);
    }
    else {
        //alert("无村庄节点！");
    }
    treeLoaded = true;
}

function InitWaterUserTree2(treeId1, treeId2, loadWaterUser, checkbox) {
    if (!manageNodeLoaded || (loadWaterUser && !waterUserLoaded)) {
        window.setTimeout("InitWaterUserTree2('" + treeId1 + "','" + treeId2 + "'," + loadWaterUser + "," + checkbox + ")", 500);
        return;
    }
    var treeData = [];
    var czflag = false;
    // 挂载节点
    for (i = 0; i < manageJson.length; i++) {
        if (checkbox) {
            manageNodes[manageJson[i].ID]["checked"] = true;
            checkedManageIds.push(manageJson[i].ID);
        }
        // 添加到上级节点中
        if (manageJson[i].上级ID == '0' || manageNodes[manageJson[i].上级ID] == null) {
            // 根节点
            treeData.push(manageNodes[manageJson[i].ID]);
        }
        else {
            // 上级节点
            manageNodes[manageJson[i].上级ID].children.push(manageNodes[manageJson[i].ID]);
        }


        if (manageNodes[manageJson[i].ID].id.indexOf("cz") >= 0) {
            if (!czflag) {
                villageid = manageNodes[manageJson[i].ID].attributes.mid;
                czflag = true;
            }

        }
    }
    if (loadWaterUser) {
        // 挂载节点
        for (i = 0; i < waterUserJson.length; i++) {
            // 添加到上级节点中
            if (waterUserJson[i].管理ID == '0') {
                // 根节点
                continue;
            }
            else {
                if (checkbox) {
                    waterUserNodes[waterUserJson[i].ID]["checked"] = true;
                    checkedWaterUserIds.push(waterUserJson[i].ID);
                }
                // 上级节点
                manageNodes[deviceJson[i].管理ID].children.push(waterUserNodes[waterUserJson[i].ID]);
            }
        }//结束挂载设备节点
    }
    $("#" + treeId1).tree({
        checkbox: checkbox,
        data: treeData
    });
    var selectnode1 = $("#" + treeId1).tree('find', "cz_" + villageid);
    if (selectnode1 != null) {
        //alert("有村庄节点！");
        $("#" + treeId1).tree('select', selectnode1.target);
    }
    else {
        //alert("无村庄节点！");
    }

    $("#" + treeId2).tree({
        checkbox: checkbox,
        data: treeData
    });
    var selectnode2 = $("#" + treeId2).tree('find', "cz_" + villageid);
    if (selectnode2 != null) {
        //alert("有村庄节点！");
        $("#" + treeId2).tree('select', selectnode2.target);
    }
    else {
        //alert("无村庄节点！");
    }
    treeLoaded = true;
}

function InitChartTree(treeId, loadDevice, checkbox) {
    if (!manageNodeLoaded || (loadDevice && !deviceNodeLoaded)) {
        window.setTimeout("InitChartTree('" + treeId + "'," + loadDevice + "," + checkbox + ")", 500);
        return;
    }
    var treeData = [];
    // 挂载节点
    for (i = 0; i < manageJson.length; i++) {
        if (checkbox) {
            manageNodes[manageJson[i].ID]["checked"] = true;
            checkedManageIds.push(manageJson[i].ID);
        }
        // 添加到上级节点中
        if (manageJson[i].上级ID == '0' || manageNodes[manageJson[i].上级ID] == null) {
            // 根节点
            treeData.push(manageNodes[manageJson[i].ID]);
        }
        else {
            // 上级节点
            manageNodes[manageJson[i].上级ID].children.push(manageNodes[manageJson[i].ID]);
        }
    }
    if (loadDevice) {
        // 挂载节点
        for (i = 0; i < deviceJson.length; i++) {
            // 添加到上级节点中
            if (deviceJson[i].上级ID == '0') {
                // 根节点
                continue;
            }
            else {
                if (checkbox) {
                    deviceNodes[deviceJson[i].ID]["checked"] = true;
                    checkedDeviceIds.push(deviceJson[i].ID);
                }
                // 上级节点
                manageNodes[deviceJson[i].管理ID].children.push(deviceNodes[deviceJson[i].ID]);
            }
        }//结束挂载设备节点
    }
    $("#" + treeId).tree({
        checkbox: checkbox,
        data: treeData,
        onCheck: function (node, checked) {
            var nodes = $("#" + treeId).tree("getChecked");
            var checkedMnIDs = [];
            var checkedDevIDs = [];
            var ckids = "";
            for (var i = 0; i < nodes.length; i++) {
                if (nodes[i].attributes["nodeType"] == "device") {
                    checkedDevIDs.push(nodes[i].attributes["did"]);
                    ckids += nodes[i].attributes["did"] + ",";
                }
                else {
                    checkedMnIDs.push(nodes[i].attributes["mid"])
                }
            }
            checkedManageIds = checkedMnIDs;
            checkedDeviceIds = checkedDevIDs;
            ckids = ckids.substr(0, ckids.length - 1);
            $.get("../MainAshx/PeriodChartMonitorValue.ashx", { type: "json", ids: ckids },
  function (data) {
      if (data != null && data != "") {
          var Result = eval("(" + data + ")");
          var comboBoxDataLevel = [];

          for (i = 0; i < Result.length; i++) {
              var levelObj = {};
              if (i == 0) {
                  levelObj["id"] = Result[i];
                  levelObj["text"] = Result[i];
                  levelObj["selected"] = true
              } else {
                  levelObj["id"] = Result[i];
                  levelObj["text"] = Result[i];
              }
              comboBoxDataLevel.push(levelObj);
          }
          $("#ccb_MonitorValue").combobox({
              data: comboBoxDataLevel
          });
      }
  });

        }
    });
    var root = $("#" + treeId).tree('getRoot');
    $("#" + treeId).tree('uncheck', root.target);
    treeLoaded = true;
}

function InitEventTree(treeId, loadDevice, checkbox) {
    if (!manageNodeLoaded || (loadDevice && !deviceNodeLoaded)) {
        window.setTimeout("InitEventTree('" + treeId + "'," + loadDevice + "," + checkbox + ")", 500);
        return;
    }
    var treeData = [];
    // 挂载节点
    for (i = 0; i < manageJson.length; i++) {
        if (checkbox) {
            manageNodes[manageJson[i].ID]["checked"] = true;
            checkedManageIds.push(manageJson[i].ID);
        }
        // 添加到上级节点中
        if (manageJson[i].上级ID == '0' || manageNodes[manageJson[i].上级ID] == null) {
            // 根节点
            treeData.push(manageNodes[manageJson[i].ID]);
        }
        else {
            // 上级节点
            manageNodes[manageJson[i].上级ID].children.push(manageNodes[manageJson[i].ID]);
        }
    }
    if (loadDevice) {
        // 挂载节点
        for (i = 0; i < deviceJson.length; i++) {
            // 添加到上级节点中
            if (deviceJson[i].上级ID == '0') {
                // 根节点
                continue;
            }
            else {
                if (checkbox) {
                    deviceNodes[deviceJson[i].ID]["checked"] = true;
                    checkedDeviceIds.push(deviceJson[i].ID);
                }
                // 上级节点
                manageNodes[deviceJson[i].管理ID].children.push(deviceNodes[deviceJson[i].ID]);
            }
        }//结束挂载设备节点
    }
    $("#" + treeId).tree({
        checkbox: checkbox,
        data: treeData,
        onCheck: function (node, checked) {
            var nodes = $("#" + treeId).tree("getChecked");
            var checkedMnIDs = [];
            var checkedDevIDs = [];
            for (var i = 0; i < nodes.length; i++) {
                if (nodes[i].attributes["nodeType"] == "device") {
                    checkedDevIDs.push(nodes[i].attributes["did"])
                }
                else {
                    checkedMnIDs.push(nodes[i].attributes["mid"])
                }
            }
            checkedManageIds = checkedMnIDs;
            checkedDeviceIds = checkedDevIDs;
            LoadEventType(checkedDevIDs.join(","));
        }
    });
    treeLoaded = true;
    LoadEventType("");
}

function LoadEventType(deviceIds) {
    $.ajax(
    {
        url: "../WebServices/DataQueryService.asmx/GetEventTypes",
        type: "GET",
        data: { "loginIdentifer": window.parent.guid, "deviceIds": deviceIds },
        dataType: "text",
        cache: false,
        success: function (responseText) {
            var data = eval("(" + $.xml2json(responseText) + ")");
            if (data.Result)//登录成功
            {
                levelJson = data.EventTypes;
                var comboBoxDataLevel = [];
                for (i = 0; i < levelJson.length; i++) {
                    var levelObj = {};
                    levelObj["id"] = levelJson[i];
                    levelObj["text"] = levelJson[i];
                    comboBoxDataLevel.push(levelObj);
                }
                $("#cbb_DevCombobox").combobox({
                    data: comboBoxDataLevel
                }).combobox('select', '全部');
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert("提示信息", errorThrown + "<br/>" + XMLHttpRequest.responseText);
        }
    });
}

function InitUserTree(treeId, loadUser, checkbox) {
    if (!manageNodeLoaded || (loadUser && !userNodeLoaded)) {
        window.setTimeout("InitUserTree('" + treeId + "'," + loadUser + "," + checkbox + ")", 500);
        return;
    }
    var treeData = [];
    // 挂载节点
    for (i = 0; i < manageJson.length; i++) {
        if (checkbox) {
            manageNodes[manageJson[i].ID]["checked"] = true;
            checkedManageIds.push(manageJson[i].ID);
        }
        // 添加到上级节点中
        if (manageJson[i].上级ID == '0' || manageNodes[manageJson[i].上级ID] == null) {
            // 根节点
            treeData.push(manageNodes[manageJson[i].ID]);
        }
        else {
            // 上级节点
            manageNodes[manageJson[i].上级ID].children.push(manageNodes[manageJson[i].ID]);
        }
    }
    if (loadUser) {
        // 挂载节点
        for (i = 0; i < userJson.length; i++) {
            // 添加到上级节点中
            if (userJson[i].上级ID == '0') {
                // 根节点
                continue;
            }
            else {
                if (checkbox) {
                    userNodes[userJson[i].ID]["checked"] = true;
                    checkedUserIds.push(userJson[i].ID);
                }
                // 上级节点
                manageNodes[userJson[i].管理ID].children.push(userNodes[userJson[i].ID]);
            }
        }//结束挂载设备节点
    }
    $("#" + treeId).tree({
        checkbox: checkbox,
        data: treeData,
        onCheck: function (node, checked) {
            var nodes = $("#" + treeId).tree("getChecked");
            var checkedMnIDs = [];
            var checkedUserIDs = [];
            for (var i = 0; i < nodes.length; i++) {
                if (nodes[i].attributes["nodeType"] == "device") {
                    checkedUserIDs.push(nodes[i].attributes["did"])
                }
                else {
                    checkedMnIDs.push(nodes[i].attributes["mid"])
                }
            }
            checkedManageIds = checkedMnIDs;
            checkedUserIds = checkedUserIDs;
            if (nodes.length > 1) {
                $("#cbb_DevCombobox").combobox('setValue', '');
            }
        }
    });
    treeLoaded = true;
}

//通过管理节点查找其下属的设备节点
function GetDeviceIDsByNode(node) {
    var devIds = [];
    if (node.attributes["nodeType"] == "device") {
        //添加设备ID
        devIds.push(node.attributes.did);
    }
    else {
        for (var i = 0; i < node.children.length; i++) {
            if (node.children[i].attributes["nodeType"] == "device") {
                //添加设备ID
                devIds.push(node.children[i].attributes.did);
            }
            else {
                devIds = devIds.concat(GetDeviceIDsByNode(node.children[i]));
            }
        }
    }
    return devIds;
}

//通过管理节点查找其下属的用户节点
function GetUserIDsByNode(node) {
    var devIds = [];
    if (node.attributes["nodeType"] == "user") {
        //添加设备ID
        devIds.push(node.attributes.did);
    }
    else if (node.attributes["nodeType"] == "manage") {
        for (var i = 0; i < node.children.length; i++) {
            if (node.children[i].attributes["nodeType"] == "user") {
                //添加设备ID
                devIds.push(node.children[i].attributes.did);
            }
            else {
                devIds = devIds.concat(GetUserIDsByNode(node.children[i]));
            }
        }
    }
    return devIds;
}

//通过管理节点查找其下属的管理节点
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

function InitComTree(treeId, loadDevice, checkbox) {
    if (!commanageNodeLoaded || (loadDevice && !comdeviceNodeLoaded)) {
        window.setTimeout("InitComTree('" + treeId + "'," + loadDevice + "," + checkbox + ")", 500);
        return;
    }
    var treeData = [];
    // 挂载节点
    for (i = 0; i < commanageJson.length; i++) {
        if (checkbox) {
            commanageNodes[commanageJson[i].ID]["checked"] = true;
            checkedManageIds.push(commanageJson[i].ID);
        }
        // 添加到上级节点中
        if (commanageJson[i].上级ID == '0' || commanageNodes[commanageJson[i].上级ID] == null) {
            // 根节点
            treeData.push(commanageNodes[commanageJson[i].ID]);
        }
        else {
            // 上级节点
            commanageNodes[commanageJson[i].上级ID].children.push(commanageNodes[commanageJson[i].ID]);
        }
    }
    if (loadDevice) {
        // 挂载节点
        for (i = 0; i < comdeviceJson.length; i++) {
            // 添加到上级节点中
            if (comdeviceJson[i].上级ID == '0') {
                // 根节点
                continue;
            }
            else {
                if (checkbox) {
                    comdeviceNodes[comdeviceJson[i].ID]["checked"] = true;
                    checkedDeviceIds.push(comdeviceJson[i].ID);
                }
                // 上级节点
                commanageNodes[comdeviceJson[i].管理ID].children.push(comdeviceNodes[comdeviceJson[i].ID]);
            }
        }//结束挂载设备节点
    }
    $("#" + treeId).combotree({
        data: treeData,
        multiple: true,
        onLoadSuccess: function () {
            $("#" + treeId).combotree('clear');
        }
    })
}

