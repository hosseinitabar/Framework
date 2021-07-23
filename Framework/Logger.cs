using Holism.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Holism.Framework
{
    public static class Logger
    {
        static object lockToken = new object();

        public static void Log(dynamic @object, MessageType type)
        {
            Console.WriteLine("{0} - {1}", DateTime.Now.ToString("HH:mm:ss"), @object);
            Console.ForegroundColor = ConsoleColor.White;
            if (@object is string)
            {
                @object.Insert(0, $"{type}: ");
            }
            try
            {
                LogToFile(type, @object);
            }
            catch (Exception ex)
            {
                throw new ServerException(ex.ToString());
            }
        }

        internal static void LogToFile(MessageType type, dynamic @object)
        {
            var text = CreateLogEntry(type, @object);
            string logPath;
            lock (lockToken)
            {
                logPath = FindLogPath(type);
                File.AppendAllText(logPath, text, Encoding.UTF8);
            }
        }

        private static dynamic CreateLogEntry(MessageType type, dynamic @object)
        {
            return string.Format("\r\n{0}-{1}: {2}", DateTime.Now, type.ToString(), @object);
        }

        public static string FindLogPath(MessageType messageType)
        {
            string logPath = string.Format(Path.Combine(Config.LogFolderPath, $"{DateTime.Now.ToString("yyyy-MM-dd")}-{messageType.ToString()}.txt"));
            CreateDirectoryIfNotExist(logPath);
            return logPath;
        }

        private static void CreateDirectoryIfNotExist(string logPath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(logPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logPath));
            }
        }

        public static void LogException(this Exception ex)
        {
            LogError(ExceptionHelper.BuildExceptionString(ex));
        }

        public static void LogError(this string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Log(text, MessageType.Error);
        }

        public static void LogInfo(this string text)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Log(text, MessageType.Info);
        }

        public static void LogInfo(dynamic obj)
        {
            Log(obj, MessageType.Info);
        }

        public static void LogSuccess(this string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Log(text, MessageType.Success);
        }

        public static void LogWarning(this string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Log(text, MessageType.Warning);
        }

        public static void Count(int number)
        {
            Console.Write("\r                 ");
            Console.Write("\r" + number);
        }
    }
}
