using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ModuleLauncher.Re.Authenticator;
using ModuleLauncher.Re.Utils;
using StarLight.sl_Launcher;

namespace sl_Launcher
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

           ReGetBgImg(null,null);
        }

        private void ShowSettingsWindow(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().Show();
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Name_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void PasswordBox_TextInput(object sender, TextCompositionEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ComboBox_LoginMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ReGetBgImg(object sender, RoutedEventArgs e)
        {
            var tempPath = System.IO.Path.GetTempPath();
            var fileName = StringHelper.GetRandomString("zhenxin");
            ReadWebFile("https://api.mmcee.cn/acgimg/acgurl.php", tempPath, fileName);
            Background = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(tempPath + fileName)),
                Stretch = Stretch.Fill
            };
        }

        private static void ReadWebFile(string url, string dirPath, string filepath)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var client = new WebClient();
            client.Headers.Add("User-Agent",
                "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            client.Headers.Add("UserAgent",
                "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            client.DownloadFile(url, dirPath + filepath);
        }
    }
}