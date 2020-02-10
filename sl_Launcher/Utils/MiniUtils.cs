using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace StarLight.Launcher.Utils
{
    public class MiniUtils
    {
        /// <summary> 
        /// 读取Url文件 
        /// </summary> 
        /// <param name="url">文件url(前面加Http://或https://)</param> 
        /// <param name="filePath">保存文件名</param>
        /// <param name="dirPath">保存路径(后面加\)</param>
        public static void ReadWebFile(string url, string dirPath, string filePath)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            WebClient client = new WebClient();
            client.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            client.Headers.Add("UserAgent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            Directory.CreateDirectory(dirPath);
            client.DownloadFile(url, dirPath + filePath);
        }
        ///<summary>
        ///生成随机字符串 
        ///</summary>
        ///<param name="count">生成的数量</param>
        public static string GetRandomString(int count)
        {
            string checkCode = String.Empty;     //存放随机码的字符串   
            int number = 0;
            System.Random random = new Random();

            for (int i = 0; i < count; i++) //产生4位校验码   
            {
                number = random.Next();
                number = number % 36;
                if (number < 10)
                {
                    number += 48;    //数字0-9编码在48-57   
                }
                else
                {
                    number += 55;    //字母A-Z编码在65-90   
                }

                checkCode += ((char)number).ToString();
            }
            return checkCode;
        }

        /// <summary>
        /// 执行bat文件
        /// </summary>
        /// <param name="batName">文件名</param>
        /// <param name="Arguments">参数</param>
        public static void LaunchBat(string batName,string Arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.FileName = batName;
            startInfo.Arguments = Arguments;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(startInfo);

        }
    }
}