using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace UploadFileDemo
{
    /// <summary>
    /// uploadFile 的摘要说明
    /// </summary>
    public class uploadFile : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            UploadFileResult obj = uploadFileTask(request);

            response.Write(JavaScriptConvert.SerializeObject(obj));
            response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private UploadFileResult uploadFileTask(HttpRequest request)
        {
            //JavaScriptObject result = new JavaScriptObject();
            UploadFileResult result = new UploadFileResult();
            try
            {
                string fileName = request["f"];
                string fileData = request["u"];
                //Debug.Print(fileName + "=====" + fileData + "\n");
                string serverPath=System.Web.HttpContext.Current.Server.MapPath("/");
                //Debug.Print("the serverPath is " + serverPath+"\n");
                serverPath += "upload";
                if (!Directory.Exists(serverPath))
                {
                    Directory.CreateDirectory(serverPath);
                }
                string filePath = serverPath + "\\" + fileName;
                Debug.Print(filePath);
                FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                byte[] val = Convert.FromBase64String(fileData);
                fs.Write(val, 0, val.Length);
                fs.Flush();
                fs.Close();
                result.Result = true;
                result.Msg = "";
                //result.Add("result", true);
                //result.Add("msg", "");
            }
            catch (Exception e)
            {
                result.Result = false;
                result.Msg = e.Message;
               // result.Add("result", false);
                //result.Add("msg", e.Message);
            }
            return result;
        }

    }
}