using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assambra.FreeClient.Utilities
{
    public static class CustomLogger
    {
        public static void Log(string message,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Debug.Log(FormatMessage("LOG", message, caller, filePath, lineNumber));
        }

        public static void LogWarning(string message,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Debug.LogWarning(FormatMessage("WARNING", message, caller, filePath, lineNumber));
        }

        public static void LogError(string message,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Debug.LogError(FormatMessage("ERROR", message, caller, filePath, lineNumber));
        }

        public static void LogException(Exception exception,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Debug.LogError(FormatMessage("EXCEPTION", exception.ToString(), caller, filePath, lineNumber));
        }

        public static void LogAssertion(string message,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Debug.LogAssertion(FormatMessage("ASSERTION", message, caller, filePath, lineNumber));
        }

        private static string FormatMessage(string logType, string message, string caller, string filePath, int lineNumber)
        {
            string fileName = System.IO.Path.GetFileName(filePath);
            return $"[{logType}] [{caller}] {message} (at {fileName}:{lineNumber})";
        }
    }
}


