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

using MahApps.Metro.Controls;
using System.Diagnostics;
using System.IO;
using System.Threading;
using MahApps.Metro.Controls.Dialogs;
using KMCCC.Launcher;
using StarLight.Launcher.Tools;

namespace StarLight.Launcher
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            // 检测配置文件
            if (File.Exists(@"Data\Config.ini") == false)
            {
                FileStream fs = new FileStream(@"Data\Config.ini", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fs.Close();
            }
            // 读取配置文件
            GlobalVar.UserName = IniFileHelper.GetValue("Config", "UserName", @"Data\Config.ini");
            GlobalVar.LoginMode = IniFileHelper.GetValue("Config", "LoginMode", @"Data\Config.ini");
            // 语言赋值
            this.Title = "星际之光客户端 V" + GlobalVar.ThisVer;
            Reporter.SetClientName("星际之光客户端 V" + GlobalVar.ThisVer);
            GetBackgroundImage();
            Thread GBM = new Thread(GetBackgroundMusic);
            GBM.Start();
            MiniTools.ReadWebFile(GlobalVar.ResRootUrl + "Data/Url/Music.txt", @"Data\Url\", "Music.txt");
            MiniTools.ReadWebFile(GlobalVar.ResRootUrl + "Data/Url/Image.txt", @"Data\Url\", "Image.txt");
        }
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

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            GlobalVar.Account = "1307993674@qq.com";
            GlobalVar.Password = "zx20010514..";
            GlobalVar.MaxMemory = 512;
            Thread Play = new Thread(PlayGame);
            Play.Start();
        }

        private void PlayGame(object obj)
        {
            LaunchResult result = Launch.Mojang(GlobalVar.Account, GlobalVar.Password, GlobalVar.MaxMemory);

            if (!result.Success)
            {
                //MessageBox.Show(result.ErrorMessage, result.ErrorType.ToString());
                switch (result.ErrorType)
                {
                    case ErrorType.NoJAVA:
                        this.Dispatcher.Invoke(new Action(() => { this.ShowMessageAsync("启动失败！", "你系统的Java有异常，可能你非正常途径删除过Java，请尝试重新安装Java。\n" + result.ErrorMessage); }));
                        
                        //MessageBox.Show("你系统的Java有异常，可能你非正常途径删除过Java，请尝试重新安装Java\n详细信息：" + result.ErrorMessage, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    case ErrorType.AuthenticationFailed:
                        this.Dispatcher.Invoke(new Action(() => { this.ShowMessageAsync("启动失败！", "正版验证失败！请检查你的账号密码。"); }));
                        
                        //MessageBox.Show(this, "正版验证失败！请检查你的账号密码", "账号错误\n详细信息：" + result.ErrorMessage, MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    case ErrorType.UncompressingFailed:
                        this.Dispatcher.Invoke(new Action(() => { this.ShowMessageAsync("启动失败！", "可能的多开或文件损坏，请确认文件完整且不要多开。\n如果你不是多开游戏的话，请检查libraries文件夹是否完整。\n" + result.ErrorMessage); }));
                        
                        //MessageBox.Show(this, "可能的多开或文件损坏，请确认文件完整且不要多开\n如果你不是多开游戏的话，请检查libraries文件夹是否完整\n详细信息：" + result.ErrorMessage, "可能的多开或文件损坏", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    default:
                        this.Dispatcher.Invoke(new Action(() => {
                            this.ShowMessageAsync(
                            "启动失败，请将此窗口截图向开发者寻求帮助。",
                            result.ErrorMessage + "\n" +
                            (result.Exception == null ? string.Empty : result.Exception.StackTrace) + result.ErrorMessage); }));
                        
                        //MessageBox.Show(this,
                        //    result.ErrorMessage + "\n" +
                        //    (result.Exception == null ? string.Empty : result.Exception.StackTrace),
                        //    "启动错误，请将此窗口截图向开发者寻求帮助");
                        break;
                }
            }
        }



        private void MetroWindow_Closed(object sender, EventArgs e)
        {

        }

        private void BackgroundMusicPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            Thread GBM = new Thread(GetBackgroundMusic);
            GBM.Start();
        }

        private void LoginMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ComboBox_LoginMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ComboBox_LoginMode.SelectedIndex)
            {
                case 0:
                    this.Name.Content = "账号:";
                    this.Password.Visibility = Visibility.Visible;
                    this.Password_TextBox.Visibility = Visibility.Visible;
                    break;
                case 1:
                    this.Name.Content = "名字:";
                    this.Password.Visibility = Visibility.Collapsed;
                    this.Password_TextBox.Visibility = Visibility.Collapsed;
                    break;
            }
        }
    }
}