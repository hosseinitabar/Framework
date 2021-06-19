using Holism.Framework;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Holism.Framework
{
    public class Config
    {
        static IConfigurationRoot ConfigurationRoot { get; set; }

        public static string ExpandEnvironmentVariables(string path)
        {
            if (!path.Contains("%"))
            {
                return path;
            }
            var newPath = Environment.ExpandEnvironmentVariables(path);
            if (path == newPath)
            {
                throw new ServerException($"{path} uses environment varaibles, but they are not defined. Please define them first");
            }
            return newPath;
        }

        public static string GetEnvironmentVariable(string key)
        {
            return GetEnvironmentVariable(key, null);
        }

        public static string GetEnvironmentVariable(string key, string alternative)
        {
            var result = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.User);
            if (result.IsNothing())
            {
                result = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Machine);
            }
            if (result.IsNothing())
            {
                result = alternative;
            }
            if (result.IsNothing())
            {
                throw new ServerException($"{key} should be defined in Environment Variables, or a fallback should be provided.");
            }
            return result;
        }

        public static ParallelOptions ParallelOptions
        {
            get
            {
                var options = new ParallelOptions();
                options.MaxDegreeOfParallelism = Environment.ProcessorCount;
                return options;
            }
        }

        private static string logFolderPath;

        public static string LogFolderPath
        {
            get
            {
                if (logFolderPath.IsSomething())
                {
                    return logFolderPath;
                }
                var keys = new string[] { "LogFolderPath", "LogsFolderPath", "LogsFolder", "LogFolder", "Logs", "Log", "LogPath", "LogsPath" };
                foreach (string key in keys)
                {
                    if (HasSetting(key))
                    {
                        logFolderPath = GetSetting(key);
                        return logFolderPath;
                    }
                }
                logFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                return logFolderPath;
            }
        }

        public static MessageType LogVerbosity
        {
            get
            {
                string key = "LogVerbosityLevel";
                if (HasSetting(key))
                {
                    return GetSetting(key).ToEnum<MessageType>();
                }
                return MessageType.Error;
            }
        }

        public static T GetModel<T>(string key) where T : class, new()
        {
            InitializeConfiguration();
            var type = new T();
            ConfigurationRoot.Bind(key, type);
            return type;
        }

        public static string GetSetting(string key)
        {
            InitializeConfiguration();
            string result = ConfigurationRoot[key];
            if (result.IsNull())
            {
                throw new ServerException($"There is no app setting for {key} in Settings.json file, or in SettingsOverride.json file, or these files are not present at the base directory of execution path.");
            }
            return result;
        }

        private static void InitializeConfiguration()
        {
            if (ConfigurationRoot == null)
            {
                ConfigurationRoot = new ConfigurationBuilder()
                            .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.json"), true)
                            .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SettingsOverride.json"), true)
                            .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConnectionStrings.json"), true)
                            .Build();
            }
        }

        public static bool HasSetting(string key)
        {
            InitializeConfiguration();
            return ConfigurationRoot[key] != null;
        }

        public static bool IsDeveloping
        {
            get
            {
                var key = "IsDeveloping";
                if (!HasSetting(key))
                {
                    return false;
                }
                var isDeveloping = GetSetting(key).ToBoolean();
                return isDeveloping;
            }
        }

        public static int DefaultPageSize
        {
            get
            {
                if (HasSetting("DefaultPageSize"))
                {
                    string pageSize = GetSetting("DefaultPageSize");
                    if (pageSize.IsNumeric())
                    {
                        return Convert.ToInt32(pageSize);
                    }
                }
                return 10;
            }
        }

        public static string GetConnectionString(string name)
        {
            InitializeConfiguration();
            var connectionString = ConfigurationRoot[name];
            if (connectionString.IsNull())
            {
                throw new ServerException($"No connection string with name {name} can be found. Please have a ConnectionStrings.json file and make it copy to output always, and make sure connection string key is present in it.");
            }
            return connectionString;
        }
    }
}