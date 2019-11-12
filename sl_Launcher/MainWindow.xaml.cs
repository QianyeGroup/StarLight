#region 系统引用
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;
using System.Threading;
#endregion

#region 第三方引用
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using KMCCC.Launcher;
using StarLight.Launcher.Tools;
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
            MiniTools.ReadWebFile()
            this.Title = "星际之光客户端 V" + GlobalVar.ThisVer;
            Reporter.SetClientName("星际之光客户端 V" + GlobalVar.ThisVer);
            MiniTools.ReadWebFile(GlobalVar.ResRootUrl + "Data/Url/Music.txt", @"Data\Url\", "Music.txt");
            MiniTools.ReadWebFile(GlobalVar.ResRootUrl + "Data/Url/Image.txt", @"Data\Url\", "Image.txt");
            GetBackgroundImage();
            // 检测配置文件
            if (File.Exists(@"Data\Config.ini") == false)
            {
                FileStream fs = new FileStream(@"Data\Config.ini", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fs.Close();
                this.ShowMessageAsync("检测到您首次启动游戏", "最大内存默认为1024MB。\n背景音乐默认关闭。\n如需修改请点击右上角设置按钮。");
                IniFileHelper.SetValue("Config", "MaxMemory", "1024", @"Data\Config.ini");
                IniFileHelper.SetValue("Config", "LoginMode", "1", @"Data\Config.ini");
                IniFileHelper.SetValue("Config", "BGM", "1", @"Data\Config.ini");
                IniFileHelper.SetValue("Config", "UserName", " ", @"Data\Config.ini");
                IniFileHelper.SetValue("Config", "Account", " ", @"Data\Config.ini");
                IniFileHelper.SetValue("Config", "Password", " ", @"Data\Config.ini");
                IniFileHelper.SetValue("Config", "JavaPath", KMCCC.Tools.SystemTools.FindJava().Last(), @"Data\Config.ini");
            }
            // 读取配置文件
            GlobalVar.UserName = IniFileHelper.GetValue("Config", "UserName", @"Data\Config.ini");
            GlobalVar.Account = IniFileHelper.GetValue("Config", "Account", @"Data\Config.ini");
            GlobalVar.Password = IniFileHelper.GetValue("Config", "Password", @"Data\Config.ini");
            GlobalVar.LoginMode = int.Parse(IniFileHelper.GetValue("Config", "LoginMode", @"Data\Config.ini"));
            GlobalVar.MaxMemory = int.Parse(IniFileHelper.GetValue("Config", "MaxMemory", @"Data\Config.ini"));
            GlobalVar.BGM = int.Parse(IniFileHelper.GetValue("Config", "BGM", @"Data\Config.ini"));
            GlobalVar.JavaPath = IniFileHelper.GetValue("Config", "JavaPath", @"Data\Config.ini");
            // 应用控件配置
            this.ComboBox_LoginMode.SelectedIndex = GlobalVar.LoginMode;
            if (GlobalVar.BGM == 0)
            {
                Thread GBM = new Thread(GetBackgroundMusic);
                GBM.Start();
            }

        }
        #endregion

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
            filePath = MiniTools.GetRandomString(number) + ".qdl";
            string[] rurl = File.ReadAllLines(@"Data\Url\Music.txt");
            Random ur = new Random();
            string url = rurl[ur.Next(rurl.Length)];
            MiniTools.ReadWebFile(url, dirPath, filePath);
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
            filePath = MiniTools.GetRandomString(number) + ".qdl";
            string[] rurl = File.ReadAllLines(@"Data\Url\Image.txt");
            Random ur = new Random();
            string url = rurl[ur.Next(rurl.Length)];
            MiniTools.ReadWebFile(url, dirPath, filePath);
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
                    GlobalVar.Account = Name_TextBox.Text;
                    GlobalVar.Password = PasswordBox.Password;
                    IniFileHelper.SetValue("Config", "Account", GlobalVar.Account, @"Data\Config.ini");
                    IniFileHelper.SetValue("Config", "Password", GlobalVar.Password, @"Data\Config.ini");
                    Thread Play_Mojang = new Thread(PlayGame_Mojang);
                    Play_Mojang.Start();
                    break;
                case 1:
                    GlobalVar.UserName = Name_TextBox.Text;
                    IniFileHelper.SetValue("Config", "UserName", GlobalVar.UserName, @"Data\Config.ini");
                    Thread Play_Offline = new Thread(PlayGame_Offline);
                    Play_Offline.Start();
                    break;
            }
            this.Play.IsEnabled = false;
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
                this.Hide();
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
                this.Hide();
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
                    IniFileHelper.SetValue("Config", "LoginMode", "0", @"Data\Config.ini");
                    break;
                case 1:
                    this.Name.Content = "名字:";
                    this.Password.Visibility = Visibility.Collapsed;
                    this.PasswordBox.Visibility = Visibility.Collapsed;
                    Name_TextBox.Text = GlobalVar.UserName;
                    IniFileHelper.SetValue("Config", "LoginMode", "1", @"Data\Config.ini");
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
                    IniFileHelper.SetValue("Config", "Account", GlobalVar.Account, @"Data\Config.ini");
                    break;
                case 1:
                    GlobalVar.UserName = Name_TextBox.Text;
                    IniFileHelper.SetValue("Config", "UserName", GlobalVar.UserName, @"Data\Config.ini");
                    break;
            }
        }

        private void PasswordBox_TextInput(object sender, TextCompositionEventArgs e)
        {
            GlobalVar.Password = PasswordBox.Password;
            IniFileHelper.SetValue("Config", "Password", GlobalVar.Password, @"Data\Config.ini");
        }
    }
}