﻿using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using ModernWindow.PackageEngine;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace ModernWindow.Structures
{
    public class AppTools
    {

        public MainApp App;

        public dynamic Globals;
        public dynamic Tools;
        public dynamic Core;
        public ThemeListener ThemeListener;
        public List<AbstractOperation> OperationQueue = new();

        private static AppTools instance;

        public static AppTools Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AppTools();
                }
                return instance;
            }
        }

        private AppTools()
        {
            App = (MainApp)Application.Current;
            Globals = App.Globals;
            Tools = App.Tools;
            Core = App.Core;
            ThemeListener = new ThemeListener();
        }

        public bool GetSettings(string setting, bool invert = false)
        {
            return App.GetSettings(setting) ^ invert;
        }

        public void SetSettings(string setting, bool value)
        {
            App.SetSettings(setting, value);
        }

        public string GetSettingsValue(string setting)
        {
            return App.GetSettingsValue(setting);
        }

        public void SetSettingsValue(string setting, string value)
        {
            App.SetSettingsValue(setting, value);
        }

        public string Translate(string text)
        {
            return App.Translate(text);
        }

        public void RestartApp()
        {
            Console.WriteLine(Environment.GetCommandLineArgs()[0].Replace(".dll", ".exe"));
            System.Diagnostics.Process.Start(Environment.GetCommandLineArgs()[0].Replace(".dll", ".exe"));
            App.DisposeAndQuit();
        }

        public async Task<string> Which(string command)
        {
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "cmd.exe",
                    Arguments = "/C where " + command,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            process.Start();
            var line = await process.StandardOutput.ReadLineAsync();
            string output;
            if (line == null)
                output = "";
            else
                output = line.Trim();
            await process.WaitForExitAsync();
            if(process.ExitCode != 0 || output == "")
                return Path.Join(Environment.GetLogicalDrives()[0], "ThisExe\\WasNotFound\\InPath", command);
            else
                return output;
        }

        public string FormatAsName(string name)
        {
            name = name.Replace(".install", "").Replace(".portable", "").Replace("-", " ").Replace("_", " ").Split("/")[^1];
            string newName = "";
            for(int i = 0; i<name.Length; i++)
            {
                if (i == 0 || name[i - 1] == ' ')
                    newName += name[i].ToString().ToUpper();
                else 
                    newName += name[i];
            }
            return newName;
        }

        public void AddOperationToList(AbstractOperation operation)
        {
            App.mainWindow.NavigationPage.OperationStackPanel.Children.Add(operation);
        }
        
    }

}