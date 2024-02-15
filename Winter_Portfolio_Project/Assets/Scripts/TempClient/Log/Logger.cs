#undef UNITY_EDITOR

using System;
using System.IO;
using UnityEngine;

public static class Logger
{
    private static bool _initialized = false;
#if UNITY_EDITOR
    private static string _logFilePath = "Assets\\GameFiles\\log.txt";
#else
    private static string _logFilePath = Application.persistentDataPath + "/log.txt";
#endif
    private static object logLockObj = new object();
    public static void WriteLog(string message)
    {
        lock(logLockObj)
        {
            if (!_initialized && File.Exists(_logFilePath))
            {
                File.Delete(_logFilePath);
                _initialized = true;
            }

            using (StreamWriter sw = new StreamWriter(_logFilePath, true))
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                sw.WriteLine(timestamp + " | " + message);
            }
            return;
        }
    }
};