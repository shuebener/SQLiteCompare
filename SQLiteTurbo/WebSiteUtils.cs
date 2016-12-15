using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace SQLiteTurbo
{
    public class WebSiteUtils
    {
        public static void OpenBuyPage()
        {
            OpenPage("http://sqlitecompare.com/Buy.html");
        }

        public static void OpenBugFeaturePage()
        {
            OpenPage("http://sqlitecompare.com/Support.html");
        }

        public static void OpenUserGuidePage()
        {
            OpenPage("http://sqlitecompare.com/GettingStarted.html");
        }

        public static void OpenProductPage()
        {
            OpenPage("http://sqlitecompare.com");
        }

        public static void OpenPage(string url)
        {
            Process p = new Process();
            ProcessStartInfo psi = new ProcessStartInfo(url);
            p.StartInfo = psi;
            psi.UseShellExecute = true;
            p.Start();
        }
    }
}
