using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;

using StarLight.Launcher.Tools;

namespace StarLight.Launcher
{
    /// <summary>
    /// Settings.xaml 的交互逻辑
    /// </summary>
    public partial class Settings : MetroWindow
    {
        public Settings()
        {
            InitializeComponent();
            MaxMemory.Text = GlobalVar.MaxMemory.ToString();
            BGM.SelectedIndex = GlobalVar.BGM;
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            GlobalVar.MaxMemory = int.Parse(MaxMemory.Text);
            GlobalVar.BGM = BGM.SelectedIndex;
            IniFileHelper.SetValue("Config", "MaxMemory", GlobalVar.MaxMemory.ToString(), @"Data\Config.ini");
            IniFileHelper.SetValue("Config", "BGM", GlobalVar.BGM.ToString(), @"Data\Config.ini");
        }
    }
}
