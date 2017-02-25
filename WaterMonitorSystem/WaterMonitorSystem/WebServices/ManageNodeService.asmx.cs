using Common;
using DTU.GateWay.Protocol;
using Maticsoft.Model;
using Module;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Services;
using WaterMonitorSystem.Src;

namespace WaterMonitorSystem.WebServices
{
    /// <summary>
    /// ManageNodeService 的摘要说明
    /// </summary>
    [Serializable, WebService(Description = "提供区域节点操作服务，包括获取区域信息、添加区域、修改区域、删除区域", Name = "区域节点服务", Namespace = "http://www.data86.net/"), WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1), ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class ManageNodeService : System.Web.Services.WebService
    {
        private HttpContext context = HttpContext.Current;

        public ManageNodeService()
        {
            this.context.Response.Buffer = true;
            this.context.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1.0);
            this.context.Response.Expires = 0;
            this.context.Response.CacheControl = "no-cache";
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>添加管理节点信息</span><br/><p style='text-indent:15px'>输入参数：要添加的管理节点的JSON字符串，返回数据格式：{'Result':bool,'Message':string}</p>")]
        public string AddMangeNode(string loginIdentifer, string manageJSONString)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                obj2["Message"] = "未登录";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                obj2["Message"] = "登录超时";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            loginUser.LastOperateTime = DateTime.Now;
            CommonUtil.WaitMainLibInit();
            JavaScriptObject obj3 = (JavaScriptObject)JavaScriptConvert.DeserializeObject(manageJSONString);
            if (obj3 == null)
            {
                obj2["Message"] = "参数格式错误";
                return JavaScriptConvert.SerializeObject(obj2);
            }

            District district = new District();
            district.LON = 0;
            district.LAT = 0;
            district.ParentId = long.Parse(obj3["上级ID"].ToString());
            district.DistrictName = obj3["名称"].ToString();
            district.DistrictType = int.Parse(obj3["级别ID"].ToString());
            district.DistrictCode = obj3["编码"].ToString();

            if (DistrictModule.ExistsDistrictName(district.ParentId, district.DistrictName))
            {
                obj2["Result"] = false;
                obj2["Message"] = "同一级别名称重复！";
                return JavaScriptConvert.SerializeObject(obj2);
            }

            if (DistrictModule.ExistsDistrictCode(district.ParentId, district.DistrictCode))
            {
                obj2["Result"] = false;
                obj2["Message"] = "同一级别编码重复！";
                return JavaScriptConvert.SerializeObject(obj2);
            }

            string msg = DistrictModule.AddDistrictInfo(district);
            //ResMsg msg = DevicesManager.AddManageNode(node);
            if (msg == "添加成功")//msg.Result
            {
                GlobalAppModule.IsInitMainLib = true;
                DistrictModule.UpdateDistrictInfo();
                GlobalAppModule.IsInitMainLib = false;

                Thread parameterThread = new Thread(new ParameterizedThreadStart(TcpRunThread.ParameterRun));
                parameterThread.Start(ProtocolConst.WebToGateUpdateCache + ProtocolConst.UpdateCache_District + "01" + "0".PadLeft(16, '0'));  

                obj2["Result"] = true;
                obj2["Message"] = "成功";
            }
            else
            {
                //obj2["Message"] = msg.Message;
                obj2["Message"] = msg;
            }

            try
            {
                SysLog log = new SysLog();
                log.LogUserId = loginUser.UserId;
                log.LogUserName = loginUser.LoginName;
                log.LogAddress = ToolsWeb.GetIP(context.Request);
                log.LogTime = DateTime.Now;
                log.LogType = "添加区域";
                log.LogContent = msg + "|" + ModelHandler<District>.ToString(district);
                SysLogModule.Add(log);
            }
            catch { }

            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>修改管理节点信息</span><br/><p style='text-indent:15px'>输入参数：loginIdentifer=登录用户标识，manageJSONString=要修改的管理节点的JSON字符串；返回数据格式：{'Result':bool,'Message':string}</p>")]
        public string ModifyMangeNode(string loginIdentifer, string manageJSONString)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                obj2["Message"] = "未登录";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                obj2["Message"] = "登录超时";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            loginUser.LastOperateTime = DateTime.Now;
            CommonUtil.WaitMainLibInit();
            JavaScriptObject obj3 = (JavaScriptObject)JavaScriptConvert.DeserializeObject(manageJSONString);
            if (obj3 == null)
            {
                obj2["Message"] = "参数格式错误";
                return JavaScriptConvert.SerializeObject(obj2);
            }

            District district = new District();
            district.Id = long.Parse(obj3["ID"].ToString());
            district.LON = 0;
            district.LAT = 0;
            district.ParentId = long.Parse(obj3["上级ID"].ToString());
            district.DistrictName = obj3["名称"].ToString();
            district.DistrictType = int.Parse(obj3["级别ID"].ToString());
            district.DistrictCode = obj3["编码"].ToString();

            if (DistrictModule.ExistsDistrictName(district.Id, district.ParentId, district.DistrictName))
            {
                obj2["Result"] = false;
                obj2["Message"] = "同一级别名称重复！";
                return JavaScriptConvert.SerializeObject(obj2);
            }

            if (DistrictModule.ExistsDistrictCode(district.Id, district.ParentId, district.DistrictCode))
            {
                obj2["Result"] = false;
                obj2["Message"] = "同一级别编码重复！";
                return JavaScriptConvert.SerializeObject(obj2);
            }

            string msg = DistrictModule.ModifyDistrictInfo(district);
            //ResMsg msg = DevicesManager.ModifyManageNode(node);
            if (msg == "修改成功")//msg.Result
            {
                GlobalAppModule.IsInitMainLib = true;
                DistrictModule.UpdateDistrictInfo();
                GlobalAppModule.IsInitMainLib = false;

                Thread parameterThread = new Thread(new ParameterizedThreadStart(TcpRunThread.ParameterRun));
                parameterThread.Start(ProtocolConst.WebToGateUpdateCache + ProtocolConst.UpdateCache_District + "01" + "0".PadLeft(16, '0'));  

                obj2["Result"] = true;
                obj2["Message"] = "成功";
            }
            else
            {
                //obj2["Message"] = msg.Message;
                obj2["Message"] = msg;
            }
            try
            {
                SysLog log = new SysLog();
                log.LogUserId = loginUser.UserId;
                log.LogUserName = loginUser.LoginName;
                log.LogAddress = ToolsWeb.GetIP(context.Request);
                log.LogTime = DateTime.Now;
                log.LogType = "修改区域";
                log.LogContent = msg + "|" + ModelHandler<District>.ToString(district);
                SysLogModule.Add(log);
            }
            catch { }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>删除指定的管理节点信息</span><br/><p style='text-indent:15px'>输入参数：loginIdentifer=登录用户标识，managerId=要删除的管理的ID；返回数据格式：{'Result':bool,'Message':string}</p>")]
        public string DeleteMangeNode(string loginIdentifer, string managerId)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                obj2["Message"] = "未登录";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                obj2["Message"] = "登录超时";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            loginUser.LastOperateTime = DateTime.Now;
            CommonUtil.WaitMainLibInit();
            District district = DistrictModule.ReturnDistrictInfo(long.Parse(managerId));
            if (district == null)
            {
                obj2["Message"] = "传入的区域ID不存在";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (district.ParentId == 0)
            {
                obj2["Message"] = "根节点不允许删除";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            //有设备的节点不能删除
            if (DeviceModule.GetDevicesForManageID(district.Id).Count > 0)
            {
                obj2["Message"] = "此区域下有设备，无法删除";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            string msg = DistrictModule.DeleteDistrictNode(long.Parse(managerId));
            //ResMsg msg = DevicesManager.DeleteManageNode(managerId);
            if (msg == "删除成功")//msg.Result
            {
                GlobalAppModule.IsInitMainLib = true;
                DistrictModule.UpdateDistrictInfo();
                GlobalAppModule.IsInitMainLib = false;

                Thread parameterThread = new Thread(new ParameterizedThreadStart(TcpRunThread.ParameterRun));
                parameterThread.Start(ProtocolConst.WebToGateUpdateCache + ProtocolConst.UpdateCache_District + "01" + "0".PadLeft(16, '0'));  

                obj2["Result"] = true;
                obj2["Message"] = "成功";
            }
            else
            {
                //obj2["Message"] = msg.Message;
                obj2["Message"] = msg;
            }
            try
            {
                SysLog log = new SysLog();
                log.LogUserId = loginUser.UserId;
                log.LogUserName = loginUser.LoginName;
                log.LogAddress = ToolsWeb.GetIP(context.Request);
                log.LogTime = DateTime.Now;
                log.LogType = "删除区域";
                log.LogContent = msg + "|" + ModelHandler<District>.ToString(district);
                SysLogModule.Add(log);
            }
            catch { }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>获取所有的级别信息</span><br/><p style='text-indent:15px'>输入参数：loginIdentifer=登录用户标识；返回数据格式：{'Result':false,'Message':'','LevelNodes':object</p>")]
        public string GetLevelInfos(string loginIdentifer)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("LevelNodes", array);
            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                obj2["Message"] = "未登录";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                obj2["Message"] = "登录超时";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            loginUser.LastOperateTime = DateTime.Now;
            CommonUtil.WaitMainLibInit();
            string levelID = DistrictModule.ReturnDistrictInfo(SysUserModule.GetUser(loginUser.UserId).DistrictId).DistrictType.ToString();
            string[] allLevelID = DistrictModule.GetAllLevelID();
            if (allLevelID != null)
            {
                for (int i = 0; i < (allLevelID.Length - 1); i++)
                {
                    try
                    {
                        if (Convert.ToInt32(allLevelID[i]) < Convert.ToInt32(levelID))
                        {
                            continue;
                        }
                    }
                    catch
                    {
                    }
                    array.Add(this.LevelInfoToJson(allLevelID[i]));
                }
                obj2["Result"] = true;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        private JavaScriptObject LevelInfoToJson(string levelId)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("ID", levelId);
            obj2.Add("级别名称", DistrictModule.GetLevelName(levelId));
            JavaScriptArray array = new JavaScriptArray();
            string levelDescription = DistrictModule.GetLevelDescription(levelId);
            if (levelDescription != "")
            {
                string[] strArray = levelDescription.Split(new char[] { ',' });
                if ((strArray != null) && (strArray.Length > 0))
                {
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        string[] strArray2 = strArray[i].Split(new char[] { '.' });
                        if (strArray2.Length != 0)
                        {
                            JavaScriptObject item = new JavaScriptObject();
                            item.Add("量名", strArray2[0]);
                            item.Add("控件类型", strArray2[1]);
                            item.Add("规则提示", strArray2[2]);
                            item.Add("必填项", strArray2[3]);
                            item.Add("衍生相关量", strArray2[4]);
                            array.Add(item);
                        }
                    }
                }
            }
            obj2.Add("级别描述", array);
            obj2.Add("显示别名", "");
            return obj2;
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>根据管理ID获取管理节点信息</span><br/><p style='text-indent:15px'>输入参数：loginIdentifer=登录用户标识，mnID=管理ID；返回数据格式：{'Result':bool,'Message':string,'ManageNode':object</p>")]
        public string GetManageNodeInfo(string loginIdentifer, string mnID)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("ManageNode", new JavaScriptObject());
            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                obj2["Message"] = "未登录";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                obj2["Message"] = "登录超时";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            loginUser.LastOperateTime = DateTime.Now;
            CommonUtil.WaitMainLibInit();
            District node = DistrictModule.ReturnDistrictInfo(long.Parse(mnID));
            if (node != null)
            {
                obj2["ManageNode"] = this.ManageNodeToJson(node);
                obj2["Result"] = true;
            }
            else
            {
                obj2["Message"] = "ID为" + mnID + "区域不存在";
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>根据区域ID获取区域节点信息</span><br/><p style='text-indent:15px'>输入参数：loginIdentifer=登录用户标识，mnID=区域ID；返回数据格式：{'Result':bool,'Message':string,'ManageNodes':array</p>")]
        public string GetManageNodeInfos(string loginIdentifer, string mnID, bool isRecursive, bool isExport)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("ManageNodes", array);
            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                obj2["Message"] = "未登录";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                obj2["Message"] = "登录超时";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            loginUser.LastOperateTime = DateTime.Now;
            CommonUtil.WaitMainLibInit();
            District node = DistrictModule.ReturnDistrictInfo(long.Parse(mnID));
            if (node == null)
            {
                obj2["Message"] = "ID为" + mnID + "区域不存在";
            }
            else
            {
                if (isRecursive)
                {
                    this.GetAllNextManageInfo(long.Parse(mnID), ref array);
                }
                else
                {
                    foreach (long str in DistrictModule.GetChildrenDistrictID(node.Id))
                    {
                        node = DistrictModule.ReturnDistrictInfo(str);
                        if (node != null)
                        {
                            array.Add(this.ManageNodeToJson(node));
                        }
                    }
                }
                obj2["Result"] = true;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

        private void GetAllNextManageInfo(long manageId, ref JavaScriptArray jsa)
        {
            District node = DistrictModule.ReturnDistrictInfo(manageId);
            if (node != null)
            {
                jsa.Add(this.ManageNodeToJson(node));
                foreach (long str in DistrictModule.GetChildrenDistrictID(node.Id))
                {
                    node = DistrictModule.ReturnDistrictInfo(str);
                    if (node != null)
                    {
                        this.GetAllNextManageInfo(node.Id, ref jsa);
                    }
                }
            }
        }

        private JavaScriptObject ManageNodeToJson(District node)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptObject obj3 = new JavaScriptObject();
            obj2.Add("ID", node.Id);
            obj2.Add("上级ID", node.ParentId);
            obj2.Add("上级名称", DistrictModule.GetDistrictName(node.ParentId));
            obj2.Add("级别ID", node.DistrictType);
            obj2.Add("级别名称", DistrictModule.GetLevelName(node.DistrictType.ToString()));
            obj2.Add("编码", node.DistrictCode);
            JavaScriptArray array = new JavaScriptArray();
            
            string levelDescription = DistrictModule.GetLevelDescription(node.DistrictType.ToString());
            if (levelDescription != "")
            {
                string[] strArray = levelDescription.Split(new char[] { ',' });
                if ((strArray != null) && (strArray.Length > 0))
                {
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        string[] strArray2 = strArray[i].Split(new char[] { '.' });
                        if (strArray2.Length != 0)
                        {
                            JavaScriptObject item = new JavaScriptObject();
                            item.Add("量名", strArray2[0]);
                            item.Add("控件类型", strArray2[1]);
                            item.Add("规则提示", strArray2[2]);
                            item.Add("必填项", strArray2[3]);
                            item.Add("衍生相关量", strArray2[4]);
                            array.Add(item);
                        }
                    }
                }
            }

            obj2.Add("级别描述", array);

            JavaScriptArray array2 = new JavaScriptArray();
            string levelDescription2 = DistrictModule.GetLevelDescription((node.DistrictType+1).ToString());
            if (levelDescription2 != "")
            {
                string[] strArray = levelDescription2.Split(new char[] { ',' });
                if ((strArray != null) && (strArray.Length > 0))
                {
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        string[] strArray2 = strArray[i].Split(new char[] { '.' });
                        if (strArray2.Length != 0)
                        {
                            JavaScriptObject item = new JavaScriptObject();
                            item.Add("量名", strArray2[0]);
                            item.Add("控件类型", strArray2[1]);
                            item.Add("规则提示", strArray2[2]);
                            item.Add("必填项", strArray2[3]);
                            item.Add("衍生相关量", strArray2[4]);
                            array2.Add(item);
                        }
                    }
                }
            }
            obj2.Add("级别描述2", array2);

            obj2.Add("名称", node.DistrictName);
            obj2.Add("描述", "");
            obj2.Add("辅助信息", "");
            /*
            foreach (KeyValuePair<string, string> pair in node.AidInfo)
            {
                obj3.Add(pair.Key, pair.Value.Replace("\r\n", "<br>"));
            }
             * */
            return obj2;
        }

        [WebMethod(Description = "<span style='font-size:10pt;font-weight:bold;line-height:30px'>获取级别信息</span><br/><p style='text-indent:15px'>输入参数：loginIdentifer=登录用户标识；返回数据格式：{'Result':false,'Message':'','LevelNodes':object</p>")]
        public string GetSimpleManageNodeInfosByMnID(string loginIdentifer, string mnID)
        {
            JavaScriptObject obj2 = new JavaScriptObject();
            JavaScriptArray array = new JavaScriptArray();
            obj2.Add("Result", false);
            obj2.Add("Message", "");
            obj2.Add("ManageNodes", array);
            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                obj2["Message"] = "未登录";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            if (loginUser.LoginTimeout)
            {
                obj2["Message"] = "登录超时";
                return JavaScriptConvert.SerializeObject(obj2);
            }
            loginUser.LastOperateTime = DateTime.Now;
            CommonUtil.WaitMainLibInit();
            District node = DistrictModule.ReturnDistrictInfo(long.Parse(mnID));
            if (node == null)
            {
                obj2["Message"] = "ID为" + mnID + "管理不存在";
            }
            else
            {
                foreach (District nodeChild in DistrictModule.GetChildrenDistrict(node.Id))
                {
                    JavaScriptObject item = new JavaScriptObject();
                    item.Add("ID", nodeChild.Id);
                    item.Add("Name", nodeChild.DistrictName);
                    array.Add(item);
                }
                obj2["Result"] = true;
            }
            return JavaScriptConvert.SerializeObject(obj2);
        }

    }
}
