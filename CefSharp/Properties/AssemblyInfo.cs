using System;
using System.Reflection;
using System.Runtime.InteropServices;
using CefSharp;

[assembly: AssemblyTitle("CefSharp")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyCompany(AssemblyInfo.AssemblyCompany)]
[assembly: AssemblyProduct(AssemblyInfo.AssemblyProduct)]
[assembly: AssemblyCopyright(AssemblyInfo.AssemblyCopyright)]
[assembly: ComVisible(AssemblyInfo.ComVisible)]
[assembly: AssemblyVersion(AssemblyInfo.AssemblyVersion)]
[assembly: AssemblyFileVersion(AssemblyInfo.AssemblyFileVersion)]
[assembly: CLSCompliant(AssemblyInfo.ClsCompliant)]

namespace CefSharp
{
    /// <exclude />
    public static class AssemblyInfo
    {
        public const bool ClsCompliant = false;
        public const bool ComVisible = false;
        public const string AssemblyCompany = "The CefSharp Authors";
        public const string AssemblyProduct = "CefSharp";
        public const string AssemblyVersion = "100.0.230";
        public const string AssemblyFileVersion = "100.0.230.0";
        public const string AssemblyCopyright = "Copyright © 2022 The CefSharp Authors";
        public const string CefSharpCoreProject = "CefSharp.Core, PublicKey=" + PublicKey;
        public const string CefSharpBrowserSubprocessProject = "CefSharp.BrowserSubprocess, PublicKey=" + PublicKey;
        public const string CefSharpBrowserSubprocessCoreProject = "CefSharp.BrowserSubprocess.Core, PublicKey=" + PublicKey;
        public const string CefSharpWpfProject = "CefSharp.Wpf, PublicKey=" + PublicKey;
        public const string CefSharpWinFormsProject = "CefSharp.WinForms, PublicKey=" + PublicKey;
        public const string CefSharpOffScreenProject = "CefSharp.OffScreen, PublicKey=" + PublicKey;
        public const string CefSharpTestProject = "CefSharp.Test, PublicKey=" + PublicKey;

        // Use "%ProgramFiles%\Microsoft SDKs\Windows\v7.0A\bin\sn.exe" -Tp <assemblyname> to get PublicKey
        public const string PublicKey = "00240000048000009400000006020000002400005253413100040000010001000d10ba46d72eed5b5791296572ed162f74968b0b5cead1bf77d1b2daa528f84eb642e20bfbd61fe8875523066a1840c99cf0b471307c85114e1f03fdc50b26a12b46be261b9872bc25f8b82192b405230da8086e5f53a7d5dfcb1608e9f958073bc142d1ee15383fa6798d46b043537edb40a5f598fdf82bf26deba6b9ee7bc6";								 
    }
}
