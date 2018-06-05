using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Management;
using System.Windows.Forms;

namespace CS.ProcessEx
{
    /// <summary>
    /// 进程控制类
    /// </summary>
    public  class ProcessManager
    {


        public static void KillProcess(string ip,string user,string password, string processName)
        {

            
            //var processor = Process.GetProcessesByName(processName, ip);

            //if (processor != null && processor.Length > 0)
            //{
               
            //    processor[0].Kill();
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}


            ConnectionOptions options = new ConnectionOptions();
            ManagementScope scope = new ManagementScope(string.Format("\\\\{0}\\{1}\\{2}",ip,user,password), options);
            scope.Connect();
            ObjectQuery query = new ObjectQuery(" SELECT * FROM Win32_Process"); // SELECT
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            ManagementObjectCollection queryCollection = searcher.Get();
            foreach (ManagementObject m in queryCollection)
            {
                // Display the remote computer information
                MessageBox.Show("Process Name : " + m["Name"]);
                MessageBox.Show("User Name : " + m["ProcessID"]);
                MessageBox.Show("ProcessID : " + m["ParentProcessID"]);
            }


        }
    }
}
