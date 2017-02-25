using System.Management;
using System.Net;

namespace Common
{
    public class WinInfo
    {
        /// <summary>
        /// 获取主机域名
        /// </summary>
        /// <returns></returns>
        public static string Get_HostName()
        {
            return Dns.GetHostName();
        }

        /// <summary>
        /// 获取CPU编号
        /// </summary>
        /// <returns></returns>
        public static string Get_CPUID()
        {
            try
            {
                //需要在解决方案中引用System.Management.DLL文件
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                string strCpuID = null;
                foreach (ManagementObject mo in moc)
                {
                    strCpuID = mo.Properties["ProcessorId"].Value.ToString();
                    break;
                }
                return strCpuID;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 获取第一分区硬盘编号
        /// </summary>
        /// <returns></returns>
        public static string GetHardDiskID()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");
                string strHardDiskID = null;
                foreach (ManagementObject mo in searcher.Get())
                {
                    strHardDiskID = mo["SerialNumber"].ToString().Trim();
                    break;
                }
                return strHardDiskID;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 获取网卡的MAC地址
        /// </summary>
        /// <returns></returns>
        public static string GetNetCardMAC()
        {
            try
            {
                string stringMAC = "";
                ManagementClass MC = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection MOC = MC.GetInstances();
                foreach (ManagementObject MO in MOC)
                {
                    if ((bool)MO["IPEnabled"] == true)
                    {
                        stringMAC += MO["MACAddress"].ToString();
                    }
                }
                return stringMAC;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 获取当前网卡IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetNetCardIP()
        {
            try
            {
                string stringIP = "";
                ManagementClass MC = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection MOC = MC.GetInstances();
                foreach (ManagementObject MO in MOC)
                {
                    if ((bool)MO["IPEnabled"] == true)
                    {
                        string[] IPAddresses = (string[])MO["IPAddress"];
                        if (IPAddresses.Length > 0)
                        {
                            stringIP = IPAddresses[0].ToString();
                        }
                    }
                }
                return stringIP;
            }
            catch
            {
                return "";
            }
        }
    }
}
