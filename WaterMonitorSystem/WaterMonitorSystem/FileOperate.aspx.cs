using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WaterMonitorSystem.Src;

namespace WaterMonitorSystem
{
    public partial class FileOperate1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string loginIdentifer = Request["loginIdentifer"] ?? "";
            LoginUser loginUser = GlobalAppModule.GetLoginUser(loginIdentifer);
            if (loginUser == null)
            {
                this.Response.Write("<script>alert('未登录')</script>");
                this.Response.End();
            }
            if (loginUser.LoginTimeout)
            {
                this.Response.Write("<script>alert('登录超时')</script>");
                this.Response.End();
            }
            loginUser.LastOperateTime = DateTime.Now;
            CommonUtil.WaitMainLibInit();

            string t = Request["t"] ?? "";
            if (t != "1" && t != "2")
            {
                this.Response.Write("<script>alert('参数非法')</script>");
                this.Response.End();
            }

            string str = "";
            try
            {
                DateTime UploadTime = DateTime.Parse(Request["UploadTime"].ToString());
                string UserName = Request["UserName"] ?? "";
                string deviceFullNo = Request["deviceFullNo"] ?? "";
                string FileName = Request["FileName"] ?? "";

                string rootpath = "";
                string filename = "";
                string path = "";
                if (t == "1")
                {
                    rootpath = "UploadFiles/Device/";
                    path = Server.MapPath("~/" + rootpath + deviceFullNo + "/");
                    filename = FileName;
                }
                else if (t == "2")
                {
                    rootpath = "UploadFiles/User/";
                    path = Server.MapPath("~/" + rootpath + UserName + "/" + UploadTime.ToString("yyyyMMddHHmmss") + "_" + deviceFullNo + "_");
                    filename = FileName;
                }

                string fullfilename = path + filename;
                string method = Request["Method"] ?? "";
                switch (method)
                {
                    case "DownloadFile":
                        if (filename != "")
                        {
                            System.IO.FileInfo file = new System.IO.FileInfo(fullfilename);
                            if (file.Exists)
                            {
                                this.Response.ContentType = "application/ms-download";
                                this.Response.Clear();
                                this.Response.AddHeader("Content-Type", "application/octet-stream");
                                this.Response.Charset = "utf-8";
                                this.Response.AddHeader("Content-Disposition", "attachment;filename=" + System.Web.HttpUtility.UrlEncode(filename));
                                this.Response.AddHeader("Content-Length", file.Length.ToString());
                                this.Response.WriteFile(file.FullName);
                                this.Response.Flush();
                                this.Response.Clear();
                                this.Response.End();
                            }
                            else
                            {
                                str = "文件不存在，无法下载！";
                            }
                        }
                        else
                        {
                            str = "非法请求，无法下载！";
                        }
                        break;
                    case "DeleteFile":
                        if (filename != "")
                        {
                            System.IO.FileInfo file = new System.IO.FileInfo(fullfilename);
                            if (file.Exists)
                            {
                                file.Delete();
                                str = "删除成功！";
                            }
                            else
                            {
                                str = "文件不存在，无法删除！";
                            }
                        }
                        else
                        {
                            str = "非法请求，无法删除！";
                        }
                        break;
                    default:
                        str = "缺少参数！";
                        break;
                }
            }
            catch (Exception ex)
            {
                str = "错误！" + ex.Message;
            }

            this.Response.Write("<script>alert('" + str + "');</script>");
            this.Response.End();
        }
    }
}