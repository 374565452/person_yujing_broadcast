using System;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace WaterMonitorSystem.Src
{
    public class SysInfo
    {
        public static void SetFilePath(string filePath)
        {
            fileName = filePath + "Config/jssl.dll";
        }
        public static string fileName = "Config/jssl.dll";

        public static bool IsReg { get; set; }

        public static bool IsRegSuccess { get; set; }

        public static string RegStr { get; set; }

        public static string DRegStr { get; set; }

        public static string Cpuid()
        {
            try
            {
                ManagementObjectCollection instances = new ManagementClass("Win32_Processor").GetInstances();
                string str = "";
                foreach (ManagementObject obj2 in instances)
                {
                    str = obj2.Properties["Processorid"].Value.ToString();
                    break;
                }
                return str;
            }
            catch (Exception)
            {
                return "strCpuID";
            }
        }

        public static string Diskid()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");
                string str = "";
                foreach (ManagementObject obj2 in searcher.Get())
                {
                    str = obj2["SerialNumber"].ToString().Trim();
                    break;
                }
                return str;
            }
            catch (Exception)
            {
                return "strdiskid";
            }
        }

        public static string GetMacAddress()
        {
            try
            {
                //获取网卡硬件地址 
                string mac = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        mac = mo["MacAddress"].ToString();
                        break;
                    }
                }
                moc = null;
                mc = null;
                return mac;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }

        }

        public static string GetIPAddress()
        {
            try
            {
                //获取IP地址 
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        //st=mo["IpAddress"].ToString(); 
                        System.Array ar;
                        ar = (System.Array)(mo.Properties["IpAddress"].Value);
                        st = ar.GetValue(0).ToString();
                        break;
                    }
                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }
        }

        public static string GetRegStr1()
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            string IP = GetIPAddress().Trim();
            string MAC = GetMacAddress().Trim();
            string str = MAC + "|" + IP + "|" + RegStr + "1";
            return BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(str))).Replace("-", "");
        }

        public static string GetRegStr2()
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            string str = GetRegStr1() + "|" + RegStr + "2";
            return BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(str))).Replace("-", "");
        }
    }
}