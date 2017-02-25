using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace DTUGateWay
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            SysInfo.IsReg = false;
            SysInfo.DRegStr = "jssl160721";
            SysInfo.RegStr = SysInfo.DRegStr;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DtuMain());
        }
    }
}
