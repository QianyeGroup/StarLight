﻿#region 系统引用
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Threading;
#endregion

#region 第三方引用
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using StarLight.Launcher.Utils;
using KMCCC.Launcher;
using MinecraftOutClient.Modules;
using ServerInfo = MinecraftOutClient.Modules.ServerInfo;
using System.Net;
using CL.IO.Zip;
#endregion

namespace StarLight.Launcher
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public static LauncherCore Core;
        #region 初始化窗口
        public MainWindow()
        {
            InitializeComponent();
            this.Title = "星际之光客户端 V" + GlobalVar.ThisVer;
            try
            {
                MiniUtils.ReadWebFile(GlobalVar.ResRootUrl + "data/url/Music.txt", @"Data\Url\", "Music.txt");
                MiniUtils.ReadWebFile(GlobalVar.ResRootUrl + "data/url/Image.txt", @"Data\Url\", "Image.txt");
                MiniUtils.ReadWebFile(GlobalVar.ResRootUrl + "data/url/Image.txt", @"Data\Url\", "Image.txt");
            }
            catch (Exception e)
            {
                this.ShowMessageAsync("错误", e.Message);
            }
            //新实例化一个ServerInfo，并使用带参数的构造函数设置IP以及端口
            /*
            try
            {
                ServerInfo info = new ServerInfo("mc.nepnepnep.top", 25565);
                info.StartGetServerInfo();
            }
            catch (Exception e)
            {
                this.ShowMessageAsync("获取服务器信息异常", e.Message);
            }
            */
            GetBackgroundImage();
            // 检测配置文件
            if (File.Exists(@"Data\Config.ini") == false)
            {
                FileStream fs = new FileStream(@"Data\Config.ini", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fs.Close();
                this.ShowMessageAsync("检测到您首次启动游戏", "最大内存默认为1024MB。(条件允许的情况下推荐设置为4096MB)\n背景音乐默认关闭。\n如需修改请点击右上角设置按钮。");
                IniFileUtils.SetValue("Config", "MaxMemory", "1024", @"Data\Config.ini");
                IniFileUtils.SetValue("Config", "LoginMode", "1", @"Data\Config.ini");
                IniFileUtils.SetValue("Config", "BGM", "1", @"Data\Config.ini");
                IniFileUtils.SetValue("Config", "UserName", " ", @"Data\Config.ini");
                IniFileUtils.SetValue("Config", "Account", " ", @"Data\Config.ini");
                IniFileUtils.SetValue("Config", "Password", " ", @"Data\Config.ini");
                IniFileUtils.SetValue("Config", "JavaPath", KMCCC.Tools.SystemTools.FindJava().Last(), @"Data\Config.ini");
            }
            // 读取配置文件
            GlobalVar.UserName = IniFileUtils.GetValue("Config", "UserName", @"Data\Config.ini");
            GlobalVar.Account = IniFileUtils.GetValue("Config", "Account", @"Data\Config.ini");
            GlobalVar.Password = IniFileUtils.GetValue("Config", "Password", @"Data\Config.ini");
            GlobalVar.LoginMode = int.Parse(IniFileUtils.GetValue("Config", "LoginMode", @"Data\Config.ini"));
            GlobalVar.MaxMemory = int.Parse(IniFileUtils.GetValue("Config", "MaxMemory", @"Data\Config.ini"));
            GlobalVar.BGM = int.Parse(IniFileUtils.GetValue("Config", "BGM", @"Data\Config.ini"));
            GlobalVar.JavaPath = IniFileUtils.GetValue("Config", "JavaPath", @"Data\Config.ini");
            // 应用控件配置
            this.ComboBox_LoginMode.SelectedIndex = GlobalVar.LoginMode;
            if (GlobalVar.BGM == 0)
            {
                Thread GBM = new Thread(GetBackgroundMusic);
                GBM.Start();
            }
            CheckUpdate();
        }
        #endregion
        private async void CheckUpdate()
        {
            // 检查更新
            string webFilePath = GlobalVar.ResRootUrl + "version.ini"; // 远程数据
            string saveDirPath = System.IO.Path.GetTempPath() + @"Qianye\StarLight\sl_Launcher\"; // 本地数据
            try
            {
                MiniUtils.ReadWebFile(GlobalVar.ResRootUrl + "version.ini", saveDirPath, "version.ini");  // 下载远程数据
                MiniUtils.ReadWebFile(GlobalVar.ResRootUrl + "update_log.txt", saveDirPath, "update_log.txt"); // 下载更新日志
            }
            catch (Exception e)
            {
                await this.ShowMessageAsync("错误", e.Message);
            }
            string latestVer = IniFileUtils.GetValue("Config", "LatestVer", saveDirPath + "version.ini"); // 最新版本
            string latestVerCode = IniFileUtils.GetValue("Config", "LatestVerCode", saveDirPath + "version.ini"); // 最新版本识别号
            string updateLog = File.ReadAllText(saveDirPath + "update_log.txt"); // 更新日志
            File.Delete(saveDirPath + "version.ini"); // 删除临时文件
            File.Delete(saveDirPath + "update_log.txt"); // 同上
            if (int.Parse(GlobalVar.ThisVerCode) < int.Parse(latestVerCode)) // 判断版本信息
            {
                await this.ShowMessageAsync("更新", "发现新版本 V" + latestVer + "(Build " + latestVerCode + ")\n更新日志：\n" + updateLog, MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "更新" });
                {
                    //c = await this.ShowProgressAsync("更新", "");
                    try
                    {
                        var update = await this.ShowProgressAsync("更新", "");
                        update.SetMessage("正在下载更新包...\n初始化...");
                        System.Windows.Forms.Application.DoEvents();
                        /*
                        float percent = 0;

                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        HttpWebRequest Myrq;
                        switch (GlobalVar.ThisVer)
                        {
                            case "3.4":
                                Myrq = (HttpWebRequest)WebRequest.Create(GlobalVar.ResRootUrl + "file/ota/StarLight_OTA_V" + GlobalVar.ThisVer + "-V" + latestVer + ".zip");
                                break;
                            default:
                                Myrq = (HttpWebRequest)WebRequest.Create(GlobalVar.ResRootUrl + "file/ota/StarLight_OTA_V" + GlobalVar.ThisVer + "-V" + latestVer + ".zip");
                                break;
                        }
                        HttpWebResponse myrp = (HttpWebResponse)Myrq.GetResponse();
                        long totalBytes = myrp.ContentLength;
                        update.Maximum = (int)totalBytes;
                        System.IO.Stream st = myrp.GetResponseStream();
                        System.IO.Stream so = new System.IO.FileStream(@"update.zip.tmp", System.IO.FileMode.Create);
                        long totalDownloadedByte = 0;
                        byte[] by = new byte[1024];
                        int osize = st.Read(by, 0, (int)by.Length);

                        while (osize > 0)
                        {
                            totalDownloadedByte = osize + totalDownloadedByte;
                            System.Windows.Forms.Application.DoEvents();
                            so.Write(by, 0, osize);
                            update.SetProgress((int)totalDownloadedByte);
                            osize = st.Read(by, 0, (int)by.Length);

                            percent = (float)totalDownloadedByte / (float)totalBytes * 100;

                            string totalSizeType;
                            float totalSizeVar;
                            string totalDownloadedSizeType;
                            float totalDownloadedSizeVar;
                            if (totalDownloadedByte / 1024 <= 1024)
                            {
                                totalDownloadedSizeType = "KB";
                                totalDownloadedSizeVar = (float)totalDownloadedByte / 1024;
                            }
                            else
                            {
                                totalDownloadedSizeType = "MB";
                                totalDownloadedSizeVar = (float)totalDownloadedByte / 1024 / 1024;
                            }
                            if (totalBytes / 1024 <= 1024)
                            {
                                totalSizeType = "KB";
                                totalSizeVar = (float)totalBytes / 1024;
                            }
                            else
                            {
                                totalSizeType = "MB";
                                totalSizeVar = (float)totalBytes / 1024 / 1024;
                            }
                            update.SetTitle("更新 - " + percent.ToString("0") + "%");
                            update.SetMessage("正在下载更新包 - " +
                                totalDownloadedSizeVar.ToString("f2") +
                                totalDownloadedSizeType + "/" +
                                totalSizeVar.ToString("f2") +
                                totalSizeType +
                                "\n\n更新日志：\n" + updateLog);

                            System.Windows.Forms.Application.DoEvents(); //必须加注这句代码，否则label1将因为循环执行太快而来不及显示信息
                        }
                        so.Close();
                        st.Close();
                        */
                        update.SetTitle("解压");
                        update.SetMessage("下载完成，准备解压...");
                        System.Windows.Forms.Application.DoEvents();
                        ZipHandler handler = ZipHandler.GetInstance();
                        var fromZip = @"update.zip.tmp"; // 需要解压的压缩文件路径
                        var toDic = @"."; // 解压到的文件夹路径
                        handler.UnpackAll(fromZip, toDic, (num) =>
                        {
                            update.SetTitle("解压 - " + num.ToString("0") + "%");
                            update.SetMessage("正在解压更新文件中...");
                            System.Windows.Forms.Application.DoEvents();
                        });
                        update.SetMessage("解压完成");
                        System.Windows.Forms.Application.DoEvents();
                        File.Delete(@"update.zip.tmp");
                        Environment.Exit(0);
                        await update.CloseAsync();

                    }
                    catch (Exception e)
                    {
                        await this.ShowMessageAsync("错误", e.Message);
                        Environment.Exit(0);
                    }

                    //System.Diagnostics.Process.Start("http://url.iqianye.cn/slupdate");
                    //Environment.Exit(0);
                }
            }
        }

        #region 获取背景音乐
        private void GetBackgroundMusic()
        {
            GC.Collect();
            BackgroundMusicPlayer.Dispatcher.Invoke(new Action(() => { BackgroundMusicPlayer.Stop(); }));
            BackgroundMusicPlayer.Dispatcher.Invoke(new Action(() => { BackgroundMusicPlayer.Source = null; }));
            string dirPath;
            string filePath;
            int number;
            Random r = new Random();
            number = r.Next();
            number = number % 36;
            dirPath = System.IO.Path.GetTempPath() + @"Qianye\StarLight\sl_Launcher\";
            filePath = MiniUtils.GetRandomString(number) + ".qdl";
            string[] rurl = File.ReadAllLines(@"Data\Url\Music.txt");
            Random ur = new Random();
            string url = rurl[ur.Next(rurl.Length)];
            MiniUtils.ReadWebFile(url, dirPath, filePath);
            // MiniTools.ReadWebFile("https://api.iqianye.cn/get/get_random_music.php?musictype=netease&displaytype=play", dirPath, filePath);
            string hashMd5 = HashHelper.ComputeMD5(dirPath + filePath);
            if (File.Exists(dirPath + hashMd5 + ".tmp.mp3"))
            {
                File.Delete(dirPath + filePath);
            }
            else
            {
                File.Move(dirPath + filePath, dirPath + hashMd5 + ".tmp.mp3");
            }
            filePath = hashMd5 + ".tmp.mp3";
            BackgroundMusicPlayer.Dispatcher.Invoke(new Action(() => { BackgroundMusicPlayer.Source = new Uri(dirPath + filePath); }));
            BackgroundMusicPlayer.Dispatcher.Invoke(new Action(() => { BackgroundMusicPlayer.Play(); }));
            GC.Collect();
        }
        #endregion

        #region 获取背景图片
        private void GetBackgroundImage()
        {
            GC.Collect();
            string dirPath;
            string filePath;
            int number;
            Random r = new Random();
            number = r.Next();
            number = number % 36;
            dirPath = System.IO.Path.GetTempPath() + @"Qianye\StarLight\sl_Launcher\";
            filePath = MiniUtils.GetRandomString(number) + ".qdl";
            string[] rurl = File.ReadAllLines(@"Data\Url\Image.txt");
            Random ur = new Random();
            string url = rurl[ur.Next(rurl.Length)];
            MiniUtils.ReadWebFile(url, dirPath, filePath);
            // MiniTools.ReadWebFile("https://api.iqianye.cn/get/get_random_image.php?imagetype=mc&displaytype=view", dirPath, filePath);
            string hashMd5 = HashHelper.ComputeMD5(dirPath + filePath);
            if (File.Exists(dirPath + hashMd5 + ".tmp.jpg"))
            {
                File.Delete(dirPath + filePath);
            }
            else
            {
                File.Move(dirPath + filePath, dirPath + hashMd5 + ".tmp.jpg");
            }
            filePath = hashMd5 + ".tmp.jpg";
            ImageBrush b = new ImageBrush();
            b.ImageSource = new BitmapImage(new Uri(dirPath + filePath));
            b.Stretch = Stretch.Fill;
            this.Background = b;
            GC.Collect();
        }
        #endregion

        #region 开始游戏
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            Core = LauncherCore.Create(new LauncherCoreCreationOption(javaPath: GlobalVar.JavaPath, gameRootPath: "Game"));
            switch (ComboBox_LoginMode.SelectedIndex)
            {
                case 0:
                    if ("".Equals(Name_TextBox.Text) | "".Equals(PasswordBox.Password))
                    {
                        this.ShowMessageAsync("错误", "请输入账号或密码。");
                    }
                    else
                    {
                        GlobalVar.Account = Name_TextBox.Text;
                        GlobalVar.Password = PasswordBox.Password;
                        IniFileUtils.SetValue("Config", "Account", GlobalVar.Account, @"Data\Config.ini");
                        IniFileUtils.SetValue("Config", "Password", GlobalVar.Password, @"Data\Config.ini");
                        this.Play.IsEnabled = false;
                        this.ComboBox_LoginMode.IsEnabled = false;
                        this.Name_TextBox.IsEnabled = false;
                        this.PasswordBox.IsEnabled = false;
                        Thread Play_Mojang = new Thread(PlayGame_Mojang);
                        Play_Mojang.Start();
                    }
                    break;
                case 1:
                    if ("".Equals(Name_TextBox.Text))
                    {
                        this.ShowMessageAsync("错误", "请输入名字。");
                    }
                    else
                    {
                        GlobalVar.UserName = Name_TextBox.Text;
                        IniFileUtils.SetValue("Config", "UserName", GlobalVar.UserName, @"Data\Config.ini");
                        this.Play.IsEnabled = false;
                        this.ComboBox_LoginMode.IsEnabled = false;
                        this.Name_TextBox.IsEnabled = false;
                        this.PasswordBox.IsEnabled = false;
                        Thread Play_Offline = new Thread(PlayGame_Offline);
                        Play_Offline.Start();
                    }
                    break;
            }
        }
        #endregion

        #region 启动游戏 正版
        private void PlayGame_Mojang(object obj)
        {
            LaunchResult result = Launch.Mojang(GlobalVar.Account, GlobalVar.Password, GlobalVar.MaxMemory);

            if (!result.Success)
            {
                //MessageBox.Show(result.ErrorMessage, result.ErrorType.ToString());
                switch (result.ErrorType)
                {
                    case ErrorType.NoJAVA:
                        this.Dispatcher.Invoke(new Action(() => { this.ShowMessageAsync("启动失败！", "你系统的Java有异常，可能你非正常途径删除过Java，请尝试重新安装Java。\n" + result.ErrorMessage); }));
                        BackgroundMusicPlayer.Dispatcher.Invoke(new Action(() => { BackgroundMusicPlayer.Play(); }));
                        Play.Dispatcher.Invoke(new Action(() => { Play.IsEnabled = true; }));
                        //MessageBox.Show("你系统的Java有异常，可能你非正常途径删除过Java，请尝试重新安装Java\n详细信息：" + result.ErrorMessage, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    case ErrorType.AuthenticationFailed:
                        this.Dispatcher.Invoke(new Action(() => { this.ShowMessageAsync("启动失败！", "正版验证失败！请检查你的账号密码。"); }));
                        Play.Dispatcher.Invoke(new Action(() => { Play.IsEnabled = true; }));
                        //MessageBox.Show(this, "正版验证失败！请检查你的账号密码", "账号错误\n详细信息：" + result.ErrorMessage, MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    case ErrorType.UncompressingFailed:
                        this.Dispatcher.Invoke(new Action(() => { this.ShowMessageAsync("启动失败！", "可能的多开或文件损坏，请确认文件完整且不要多开。\n如果你不是多开游戏的话，请检查libraries文件夹是否完整。\n" + result.ErrorMessage); }));
                        Play.Dispatcher.Invoke(new Action(() => { Play.IsEnabled = true; }));
                        //MessageBox.Show(this, "可能的多开或文件损坏，请确认文件完整且不要多开\n如果你不是多开游戏的话，请检查libraries文件夹是否完整\n详细信息：" + result.ErrorMessage, "可能的多开或文件损坏", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    default:
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            this.ShowMessageAsync(
                            "启动失败，请将此窗口截图向开发者寻求帮助。",
                            result.ErrorMessage + "\n" +
                            (result.Exception == null ? string.Empty : result.Exception.StackTrace) + result.ErrorMessage);
                        }));
                        Play.Dispatcher.Invoke(new Action(() => { Play.IsEnabled = true; }));

                        //MessageBox.Show(this,
                        //    result.ErrorMessage + "\n" +
                        //    (result.Exception == null ? string.Empty : result.Exception.StackTrace),
                        //    "启动错误，请将此窗口截图向开发者寻求帮助");
                        break;
                }

            }

        }
        #endregion

        #region 启动游戏 离线
        private void PlayGame_Offline(object obj)
        {
            LaunchResult result = Launch.Offline(GlobalVar.UserName, GlobalVar.MaxMemory);

            if (!result.Success)
            {
                //MessageBox.Show(result.ErrorMessage, result.ErrorType.ToString());
                switch (result.ErrorType)
                {
                    case ErrorType.NoJAVA:
                        this.Dispatcher.Invoke(new Action(() => { this.ShowMessageAsync("启动失败！", "你系统的Java有异常，可能你非正常途径删除过Java，请尝试重新安装Java。\n" + result.ErrorMessage); }));
                        Play.Dispatcher.Invoke(new Action(() => { Play.IsEnabled = true; }));
                        //MessageBox.Show("你系统的Java有异常，可能你非正常途径删除过Java，请尝试重新安装Java\n详细信息：" + result.ErrorMessage, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    case ErrorType.AuthenticationFailed:
                        this.Dispatcher.Invoke(new Action(() => { this.ShowMessageAsync("启动失败！", "用户名不可为空，请检查。"); }));
                        Play.Dispatcher.Invoke(new Action(() => { Play.IsEnabled = true; }));
                        //MessageBox.Show(this, "正版验证失败！请检查你的账号密码", "账号错误\n详细信息：" + result.ErrorMessage, MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    case ErrorType.UncompressingFailed:
                        this.Dispatcher.Invoke(new Action(() => { this.ShowMessageAsync("启动失败！", "可能的多开或文件损坏，请确认文件完整且不要多开。\n如果你不是多开游戏的话，请检查libraries文件夹是否完整。\n" + result.ErrorMessage); }));
                        Play.Dispatcher.Invoke(new Action(() => { Play.IsEnabled = true; }));
                        //MessageBox.Show(this, "可能的多开或文件损坏，请确认文件完整且不要多开\n如果你不是多开游戏的话，请检查libraries文件夹是否完整\n详细信息：" + result.ErrorMessage, "可能的多开或文件损坏", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    default:
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            this.ShowMessageAsync(
                            "启动失败，请将此窗口截图向开发者寻求帮助。",
                            result.ErrorMessage + "\n" +
                            (result.Exception == null ? string.Empty : result.Exception.StackTrace) + result.ErrorMessage);
                        }));
                        Play.Dispatcher.Invoke(new Action(() => { Play.IsEnabled = true; }));
                        //MessageBox.Show(this,
                        //    result.ErrorMessage + "\n" +
                        //    (result.Exception == null ? string.Empty : result.Exception.StackTrace),
                        //    "启动错误，请将此窗口截图向开发者寻求帮助");
                        break;
                }

            }

        }
        #endregion

        private void MetroWindow_Closed(object sender, EventArgs e)
        {

        }

        #region 背景音乐循环
        private void BackgroundMusicPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            Thread GBM = new Thread(GetBackgroundMusic);
            GBM.Start();
        }
        #endregion

        #region 模式下拉框检测
        private void ComboBox_LoginMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ComboBox_LoginMode.SelectedIndex)
            {
                case 0:
                    this.Name.Content = "账号:";
                    this.Password.Visibility = Visibility.Visible;
                    this.PasswordBox.Visibility = Visibility.Visible;
                    Name_TextBox.Text = GlobalVar.Account;
                    PasswordBox.Password = GlobalVar.Password;
                    IniFileUtils.SetValue("Config", "LoginMode", "0", @"Data\Config.ini");
                    break;
                case 1:
                    this.Name.Content = "名字:";
                    this.Password.Visibility = Visibility.Collapsed;
                    this.PasswordBox.Visibility = Visibility.Collapsed;
                    Name_TextBox.Text = GlobalVar.UserName;
                    IniFileUtils.SetValue("Config", "LoginMode", "1", @"Data\Config.ini");
                    break;
            }
        }
        #endregion

        #region 设置按钮
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings a = new Settings();
            a.Owner = this;
            a.ShowDialog();
        }
        #endregion

        private void Name_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            switch (ComboBox_LoginMode.SelectedIndex)
            {
                case 0:
                    GlobalVar.Account = Name_TextBox.Text;
                    IniFileUtils.SetValue("Config", "Account", GlobalVar.Account, @"Data\Config.ini");
                    break;
                case 1:
                    GlobalVar.UserName = Name_TextBox.Text;
                    IniFileUtils.SetValue("Config", "UserName", GlobalVar.UserName, @"Data\Config.ini");
                    break;
            }
        }

        private void PasswordBox_TextInput(object sender, TextCompositionEventArgs e)
        {
            GlobalVar.Password = PasswordBox.Password;
            IniFileUtils.SetValue("Config", "Password", GlobalVar.Password, @"Data\Config.ini");
        }
    }
}