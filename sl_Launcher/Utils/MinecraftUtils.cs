using System;
using System.Runtime.InteropServices;

namespace StarLight.Launcher.Utils
{
    public class MinecraftUtils
    {
        /// <summary>
        /// 函数功能：
        /// 该函数获得一个顶层窗口的句柄，该窗口的类名和窗口名与给定的字符串相匹配。
        /// 这个函数不查找子窗口。在查找时不区分大小写。
        /// </summary>
        /// <param name="lpClassName">类名</param>
        /// <param name="lpWindowName">窗口名</param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(
                   string lpClassName,
                   string lpWindowName
        );
        /// <summary>
        /// 函数功能：
        /// 该函数设置一个指定窗口的标题
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="lpString"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "SetWindowText")]
        public static extern int SetWindowText(
            IntPtr hwnd,
            string lpString
        );
    }
}
