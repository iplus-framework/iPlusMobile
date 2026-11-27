// Copyright (c) 2024, gipSoft d.o.o. Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.IO;

namespace gip.vbm.mobile.Helpers
{
    /// <summary>
    /// Central crash/exception recording: appends entries to SettingsViewModel.LastCrashDump
    /// (sent to the server via ExecuteSendPerfLog) and to a local crash_log file, plus a
    /// breadcrumb ring-buffer of recent app events to give context around a crash.
    /// </summary>
    public static class CrashLog
    {
        const int MaxBreadcrumbs = 30;
        const int MaxDumpChars = 50_000;

        static readonly object _Lock = new object();
        static readonly Queue<string> _Breadcrumbs = new Queue<string>();

        /// <summary>
        /// Records a short app event (navigation, WS call, ShowDialog, scan, ...) for
        /// crash context. Keep the message short and free of user data.
        /// </summary>
        public static void AddBreadcrumb(string evt)
        {
            lock (_Lock)
            {
                _Breadcrumbs.Enqueue($"{DateTime.Now:HH:mm:ss.fff} {evt}");
                while (_Breadcrumbs.Count > MaxBreadcrumbs)
                    _Breadcrumbs.Dequeue();
            }
        }

        public static string DumpBreadcrumbs()
        {
            lock (_Lock)
            {
                return string.Join("\r\n", _Breadcrumbs);
            }
        }

        /// <summary>
        /// Records an exception: appends to LastCrashDump (with breadcrumbs) and to the
        /// local crash_log file. Safe to call from any thread.
        /// </summary>
        public static void Record(string source, Exception ex)
        {
            if (ex == null)
                return;

            string entry = string.Format("[{0:yyyy-MM-dd HH:mm:ss.fff}] [{1}] Thread=#{2}(main={3})\r\n{4}: {5}\r\n{6}",
                DateTime.Now, source, System.Threading.Thread.CurrentThread.ManagedThreadId,
                Microsoft.Maui.ApplicationModel.MainThread.IsMainThread,
                ex.GetType().Name, ex.Message, ex.StackTrace);

            // Walk the InnerException chain: wrapper exceptions (TargetInvocationException
            // from XAML bindings, AggregateException from tasks, JavaProxyThrowable) hide
            // the real cause several levels deep - without this the dump is diagnosis-proof.
            Exception inner = ex.InnerException;
            int depth = 0;
            while (inner != null && depth < 5)
            {
                entry += string.Format("\r\n--- Inner exception {0} ---\r\n{1}: {2}\r\n{3}",
                    depth + 1, inner.GetType().Name, inner.Message, inner.StackTrace);
                inner = inner.InnerException;
                depth++;
            }

            string crumbs = DumpBreadcrumbs();
            if (!string.IsNullOrEmpty(crumbs))
                entry += "\r\n--- Breadcrumbs ---\r\n" + crumbs;

            try
            {
                var settings = ViewModels.SettingsViewModel.StaticSettingModel;
                if (settings != null)
                {
                    string combined = string.IsNullOrEmpty(settings.LastCrashDump)
                        ? entry
                        : settings.LastCrashDump + "\r\n\r\n" + entry;
                    if (combined.Length > MaxDumpChars)
                        combined = combined[^MaxDumpChars..];
                    settings.LastCrashDump = combined;
                }
            }
            catch
            {
                // SettingsViewModel may not be initialized yet during early crashes
            }

            WriteCrashFile(entry);
        }

        /// <summary>
        /// Appends the entry to crash_log files so a hard crash right after a caught
        /// exception doesn't lose the earlier history. Never throws.
        /// </summary>
        static void WriteCrashFile(string entry)
        {
            string crashReport = entry + "\r\n\r\n";
            try
            {
                string filePath = Path.Combine(FileSystem.AppDataDirectory, $"crash_log_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                File.AppendAllText(filePath, crashReport);
            }
            catch
            {
                //
            }
            try
            {
#if ANDROID
                // The external dir is accessible via adb even for release (non-debuggable) builds:
                // adb pull /sdcard/Android/data/com.gipsoft.mobilemaui/files/
                var extDir = Android.App.Application.Context.GetExternalFilesDir(null);
                if (extDir != null)
                {
                    string extPath = Path.Combine(extDir.AbsolutePath, $"crash_log_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                    File.AppendAllText(extPath, crashReport);
                }
#endif
            }
            catch
            {
                //
            }
        }
    }
}
