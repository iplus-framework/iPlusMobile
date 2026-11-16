// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿#nullable enable
using System;
using gip.vbm.mobile.Services;
using gip.vbm.mobile.Views;
using gip.core.webservices;
using gip.vbm.mobile.ViewModels;
using gip.vbm.mobile.Helpers;
using gip.core.datamodel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using gip.vbm.mobile.Controls;
#if ANDROID
using Android.Runtime;
#endif

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace gip.vbm.mobile
{
    public partial class App : Application
    {
        public static VBUserRights? UserRights { get; set; }
        public static bool IsUserLoggedIn
        {
            get
            {
                return UserRights != null;
            }
        }

        private static App? _Root;
        public static App? Root
        {
            get
            {
                return _Root;
            }
        }

        public App()
        {
            _Root = this;
            InitializeComponent();

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var exception = e.ExceptionObject as Exception;
                LogAndSaveCrash(exception, $"AppDomain.UnhandledException (IsTerminating={e.IsTerminating})");
            };

            
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                LogAndSaveCrash(e.Exception, "TaskScheduler.UnobservedTaskException");
                e.SetObserved(); // Prevents the app from crashing due to an unobserved task
            };

#if ANDROID
            // IMPORTANT: AppDomain.UnhandledException does NOT catch Android UI-thread crashes!
            // This catches .NET exceptions from Activity/Service lifecycle:
            AndroidEnvironment.UnhandledExceptionRaiser += (sender, e) =>
            {
                LogAndSaveCrash(e.Exception, "AndroidEnvironment.UnhandledExceptionRaiser");
            };

            // This catches Java/native crashes (e.g. EMDK, linker, JNI) that .NET handlers don't see:
            Java.Lang.Thread.DefaultUncaughtExceptionHandler = new UncaughtCrashHandler(LogAndSaveCrash);
#endif

            if (!IsUserLoggedIn)
            {
                MainPage = new LoginPage();
                //MainPage = new NavigationPage(new LoginPage());
                //MainPage = new NavigationPageBSO(new LoginPage    ());
            }
            else
            {
                //MainPage = new MainPage();
                //MainPage = new NavigationPage(new MainPage());
            }
        }

        private void LogAndSaveCrash(Exception? ex, string source)
        {
            if (ex == null) return;
            Helpers.CrashLog.Record(source, ex);
        }

#if ANDROID
        private class UncaughtCrashHandler : Java.Lang.Object, Java.Lang.Thread.IUncaughtExceptionHandler
        {
            private readonly Action<Exception?, string> _log;
            public UncaughtCrashHandler(Action<Exception?, string> log) { _log = log; }

            public void UncaughtException(Java.Lang.Thread? thread, Java.Lang.Throwable? ex)
            {
                _log(ex != null ? new JavaExceptionWrapper(ex) : null,
                     $"Java.UncaughtException (thread={thread?.Name})");
                // The process is about to be killed by Android - give the file/setting
                // writes in LogAndSaveCrash time to flush to disk.
                System.Threading.Thread.Sleep(300);
            }
        }

        private class JavaExceptionWrapper : Exception
        {
            public JavaExceptionWrapper(Java.Lang.Throwable ex)
                : base($"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}") { }
        }
#endif

        public void DisplayMainPage()
        {
            MainPage = new MainPage();
            //MainPage = new AboutPage();
            //MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        public async Task<object> OpenBarcodeCamera()
        {
            return await (MainPage as MainPage).OpenBarcodeCamera();
        }

        public static SettingsViewModel SettingsViewModel
        {
            get
            {
                return SettingsViewModel.StaticSettingModel;
            }
        }

        private static PerformanceLogger? _PerfLogger;// new PerformanceLogger("Code-Analysis");
        public static PerformanceLogger PerfLogger
        {
            get
            {
                if (_PerfLogger == null)
                {
                    _PerfLogger = new PerformanceLogger("Code-Analysis");
                    _PerfLogger.Active = SettingsViewModel.PerfLoggingOn;
                }
                return _PerfLogger;
            }
        }
    }
}
