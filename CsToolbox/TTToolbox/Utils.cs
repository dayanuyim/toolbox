using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Management;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Win32;

namespace TTToolbox{
    public class Utils
    {
        public static T Max<T>(T value1, T value2) where T : IComparable
        {
            return value1.CompareTo(value2) > 0 ? value1 : value2;
        }

        public static T Min<T>(T value1, T value2) where T : IComparable
        {
            return value1.CompareTo(value2) < 0 ? value1 : value2;
        }


        //斷行函式，將過長的字串換行
        public static string MutilLines(string str, int len){
            if (str.Length <= len) return str;  //for effective

            StringBuilder txt = new StringBuilder();
            for(int i = 0; i < str.Length; i+= len){
                int rest = str.Length - i;
                if (rest <= len)
                    txt.Append(str.Substring(i, rest));
                else
                    txt.Append(str.Substring(i, len)).AppendLine();
            }
            return txt.ToString();
        }

        //本月第一天的12:00:00
        public static DateTime FirstDayOfMonth(DateTime date){
            const string fmt = "yyyyMMdd";
            return DateTime.ParseExact(date.ToString("yyyyMM" + "01"), fmt, CultureInfo.CurrentCulture);
        }

        //本月末日的12:00:00 
        public static DateTime LastDayOfMonth(DateTime date){
            return FirstDayOfMonth(date.AddMonths(1)).AddDays(-1);
        }

        private static DateTime unix_epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public static DateTime DateTimeFromUnixTimeStamp(double unix_timestamp)
        {
            //return unix_epoch.AddSeconds(unix_timestamp).ToLocalTime(); ;
            return unix_epoch.AddSeconds(unix_timestamp);
        }

        public static double DateTimeToUnixTimestamp(DateTime time)
        {
            //return (time - unix_epoch.ToLocalTime()).TotalSeconds;
            return (time - unix_epoch).TotalSeconds;
        }


        public static DateTime GetLastDayOfWeek(DateTime dt, DayOfWeek dow)
        {
            int off_day = ((int)dt.DayOfWeek - (int)dow + 7) % 7;
            return dt.Date.AddDays(-off_day);
        }

        public static double TimeSpanDivide(TimeSpan a, TimeSpan b)
        {
            return a.TotalSeconds / b.TotalSeconds;
        }

        public static TimeSpan TimeSpanDivide(TimeSpan a, double times)
        {
            return TimeSpan.FromSeconds(a.TotalSeconds / times);
        }

        public static TimeSpan TimeSpanTimes(TimeSpan a, double times)
        {
            return TimeSpan.FromSeconds(a.TotalSeconds * times);
        }


        // GetServiceController
        public static ServiceController GetServiceSontroller(string service_name)
        {
            if (service_name.Length == 0) return null;

            ServiceController[] services = ServiceController.GetServices(System.Environment.MachineName);
            foreach (ServiceController sc in services)
            {
                if (sc.ServiceName == service_name) return sc;
                //Console.WriteLine(sc.ServiceName + "/" + sc.DisplayName + ": " + sc.Status);
            }

            return null;
        }

        //Get Service's Description
        public static String GetServiceDescription(string service_name)
        {
            ManagementObject wmiService = new ManagementObject("Win32_Service.Name='" + service_name + "'");
            wmiService.Get();
            return wmiService["Description"].ToString();
        }

        public static String GetServicePath(string service_name)
        {
            RegistryKey hklm = Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Services\" + service_name);
            return (string) hklm.GetValue("ImagePath");
        }

        public static bool IsIPv4(String ip){
            try{
                string[] num = ip.Split(new char[]{'.'});
                if (num.Length != 4) return false;

                foreach(string n in num)
                    if (Convert.ToUInt32(n) > 255) return false;

                return true;
            }catch{}

            return false;
        }

        public static bool IsPort(String port){
            try{
                int pp = Convert.ToInt32(port);
                return (pp >= 1 && pp <= 65535);
            }catch{}

            return false;
        }

        public static string HexString(byte[] data)
        {
            char[] lookup = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
            int i = 0, p = 0, l = data.Length;
            char[] c = new char[l * 2];
            byte d;
            while (i < l)
            {
                d = data[i++];
                c[p++] = lookup[d / 0x10];
                c[p++] = lookup[d % 0x10];
            }
            return new string(c, 0, c.Length);
        }
    
        public static string MD5(string txt)
        {
            return MD5(Encoding.Default.GetBytes(txt));
        }
        public static string MD5(byte[] txt)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] hash = md5.ComputeHash(txt);
            return HexString(hash);
        }

        public static string SaltedMD5(string txt)
        {
            string salt = "MD5System.String";

            //to bytes
            byte[] bsalt = Encoding.Default.GetBytes(salt);
            byte[] btxt = Encoding.Default.GetBytes(txt);
            byte[] dest = new byte[bsalt.Length + btxt.Length];

            //cat
            bsalt.CopyTo(dest, 0);
            btxt.CopyTo(dest, bsalt.Length);

            //caculate
            return MD5(dest);
        }

        public static Process RunningInstance()
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            //遍历与当前进程名称相同的进程列表   
            foreach (Process process in processes)
            {
                //如果实例已经存在则忽略当前进程   
                if (process.Id != current.Id)
                {
                    //保证要打开的进程同已经存在的进程来自同一文件路径 
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName)
                    {
                        //返回已经存在的进程 
                        return process;

                    }
                }
            }
            return null;
        }


        public static void HandleRunningInstance(Process instance)
        {
            MessageBox.Show(instance.ProcessName + " 運行中", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ShowWindowAsync(instance.MainWindowHandle, WS_SHOWNORMAL);  //调用api函数，正常显示窗口 
            SetForegroundWindow(instance.MainWindowHandle); //将窗口放置最前端 
        }
        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(System.IntPtr hWnd, int cmdShow);
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(System.IntPtr hWnd);
        private const int WS_SHOWNORMAL = 1; 
    }
        
 
    public class Chronograph{
        DateTime begin_;
        public Chronograph(){
            begin_ = DateTime.Now;
        }

        public TimeSpan Watch(){
            return DateTime.Now - begin_;
        }
    }
}
