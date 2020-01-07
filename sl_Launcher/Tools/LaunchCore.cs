﻿using KMCCC.Launcher;
using KMCCC.Authentication;

namespace StarLight.Launcher.Tools
{
    public class Launch
    {
        /// <summary>
        /// 离线启动
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="MaxMemory">最大内存</param>
        /// <returns></returns>
        public static LaunchResult Offline(string UserName, int MaxMemory)
        {
            var ver = MainWindow.Core.GetVersion("StarLight-Client");
            var result = MainWindow.Core.Launch(new LaunchOptions
            {
                Version = ver, //Ver为Versions里你要启动的版本名字
                MaxMemory = MaxMemory, //最大内存，int类型
                Authenticator = new OfflineAuthenticator(UserName), //离线启动
                //Authenticator = new YggdrasilLogin("邮箱", "密码", true), // 正版启动，最后一个为是否twitch登录
            }); ; ; ;
            return result;
        }
        /// <summary>
        /// 正版启动
        /// </summary>
        /// <param name="Account">正版账号</param>
        /// <param name="Password">正版密码</param>
        /// <param name="MaxMemory">最大内存</param>
        /// <returns></returns>
        public static LaunchResult Mojang(string Account, string Password, int MaxMemory)
        {
            var ver = MainWindow.Core.GetVersion("StarLight-Client");
            var result = MainWindow.Core.Launch(new LaunchOptions
            {
                Version = ver, //Ver为Versions里你要启动的版本名字
                MaxMemory = MaxMemory, //最大内存，int类型
                //Authenticator = new OfflineAuthenticator(UserName), //离线启动
                Authenticator = new YggdrasilLogin(Account, Password, false), // 正版启动，最后一个为是否twitch登录
            });
            return result;
        }
    }
}
