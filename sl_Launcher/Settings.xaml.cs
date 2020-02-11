﻿using System;
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

using StarLight.Launcher.Utils;
using Microsoft.Win32;
using System.Windows.Forms;

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
            JavaPath.Text = GlobalVar.JavaPath;
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            GlobalVar.MaxMemory = int.Parse(MaxMemory.Text);
            GlobalVar.JavaPath = JavaPath.Text;
            IniFileUtils.SetValue("Config", "MaxMemory", GlobalVar.MaxMemory.ToString(), @"Data\Config.ini");
            IniFileUtils.SetValue("Config", "BGM", GlobalVar.BGM.ToString(), @"Data\Config.ini");
            IniFileUtils.SetValue("Config", "JavaPath", GlobalVar.JavaPath, @"Data\Config.ini");
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Multiselect = true;//该值确定是否可以选择多个文件
            dialog.Title = "请选择Java路径";
            dialog.Filter = "Java(javaw.exe)|javaw.exe";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = dialog.FileName;
                JavaPath.Text = GlobalVar.JavaPath;
                
            }

        }

        private void AutoMem_Click(object sender, RoutedEventArgs e)
        {
            MaxMemory.Text = (KMCCC.Tools.SystemTools.GetRunmemory() / 2).ToString();
        }

        private void AutoJava_Click(object sender, RoutedEventArgs e)
        {
            JavaPath.Text = KMCCC.Tools.SystemTools.FindJava().Last();
        }
    }
}
