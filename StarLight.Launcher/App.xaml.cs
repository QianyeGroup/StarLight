using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using KMCCC.Launcher;
using StarLight.Launcher.Tools;

namespace StarLight.Launcher
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static LauncherCore Core = LauncherCore.Create(gameRootPath: "Game");
    }
}
