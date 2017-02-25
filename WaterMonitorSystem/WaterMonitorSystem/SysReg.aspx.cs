using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WaterMonitorSystem.Src;

namespace WaterMonitorSystem
{
    public partial class SysReg : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.TextBox1.Text = SysInfo.GetRegStr1();
                this.TextBox3.Text = SysInfo.RegStr;
                Label1.Text = "";
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (this.TextBox2.Text.Trim() == SysInfo.GetRegStr2())
            {
                FileHelper.writeFile(SysInfo.fileName, this.TextBox2.Text.Trim());
                SysInfo.IsRegSuccess = true;
                this.Response.Redirect(ResolveUrl("~/") + "default.html");
            }
            else
            {
                Label1.Text = "注册码不对";
            }
        }
    }
}