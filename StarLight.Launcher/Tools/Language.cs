using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StarLight.Launcher.Tools
{
    class LanguageHelper
    {
        public static void ReadLanguage(String LangCode)
        {
            GlobalVar.Lang_Title = IniFileHelper.GetValue("lang", "Lang_Title", @"Data\Language\" + LangCode + ".lang");
            GlobalVar.Lang_Play = IniFileHelper.GetValue("lang", "Lang_Play", @"Data\Language\" + LangCode + ".lang");
            GlobalVar.Lang_Name_Table = IniFileHelper.GetValue("lang", "Lang_Name_Table", @"Data\Language\" + LangCode + ".lang");
            GlobalVar.Lang_Account_Table = IniFileHelper.GetValue("lang", "Lang_Account_Table", @"Data\Language\" + LangCode + ".lang");
            GlobalVar.Lang_Password_Table = IniFileHelper.GetValue("lang", "Lang_Password_Table", @"Data\Language\" + LangCode + ".lang");
            GlobalVar.Lang_Settings = IniFileHelper.GetValue("lang", "Lang_Settings", @"Data\Language\" + LangCode + ".lang");
        }
    }
}