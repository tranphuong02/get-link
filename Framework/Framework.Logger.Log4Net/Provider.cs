//////////////////////////////////////////////////////////////////////
// File Name    : Provider
// System Name  : Framework.Logger
// Summary      :
// Author       : dinh.nguyen
// Change Log   : 11/3/2015 4:50:28 PM - Create Date
/////////////////////////////////////////////////////////////////////

using Framework.Logger.Contracts;
using Framework.Logger.Contracts.Interfaces;
using Framework.Utility.Exception;
using log4net;
using log4net.Config;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Framework.Logger.Log4Net
{
    /// <summary>
    /// Logger Provider
    /// </summary>
    public class Provider : IProvider
    {
        #region Fields

        /// <summary>
        /// Logger instance
        /// </summary>
        private static volatile Provider _instance;

        /// <summary>
        /// Get logger
        /// </summary>
        private static ILog _logger = LogManager.GetLogger(typeof(Provider));

        #endregion Fields

        #region Instance

        /// <summary>
        ///     Singleton initialization
        /// </summary>
        public static Provider Instance => _instance ?? (_instance = new Provider());

        #endregion Instance

        /// <summary>
        ///     Write <paramref name="message"/> to log
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logLevel"></param>
        public void WriteLog(string message, Enums.LogLevels logLevel = Enums.LogLevels.Info)
        {
            switch (logLevel)
            {
                case Enums.LogLevels.Debug:
                    {
                        ThreadContext.Properties["logPath"] = Constants.Paths.Debug;
                    }
                    break;

                case Enums.LogLevels.Info:
                    {
                        ThreadContext.Properties["logPath"] = Constants.Paths.Info;
                    }
                    break;

                case Enums.LogLevels.Warn:
                    {
                        ThreadContext.Properties["logPath"] = Constants.Paths.Warn;
                    }
                    break;

                case Enums.LogLevels.Error:
                    {
                        ThreadContext.Properties["logPath"] = Constants.Paths.Error;
                    }
                    break;

                case Enums.LogLevels.Fatal:
                    {
                        ThreadContext.Properties["logPath"] = Constants.Paths.Fatal;
                    }
                    break;
            }
            _logger = LogManager.GetLogger(GetCaller());
            XmlConfigurator.Configure();
            switch (logLevel)
            {
                case Enums.LogLevels.Debug:
                    {
                        _logger.Debug(message);
                    }
                    break;

                case Enums.LogLevels.Info:
                    {
                        _logger.Info(message);
                    }
                    break;

                case Enums.LogLevels.Warn:
                    {
                        _logger.Warn(message);
                    }
                    break;

                case Enums.LogLevels.Error:
                    {
                        _logger.Error(message);
                    }
                    break;

                case Enums.LogLevels.Fatal:
                    {
                        _logger.Fatal(message);
                    }
                    break;
            }

            LogManager.GetRepository().Shutdown();
        }

        /// <summary>
        ///     Write log exception to log file
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="customMessage"></param>
        /// <param name="format">Format log</param>
        /// <param name="memberName"></param>
        /// <param name="sourceFilePath"></param>
        /// <param name="sourceLineNumber"></param>
        public void LogError(Exception ex, string customMessage = "", bool format = true, [CallerMemberName]string memberName = "", [CallerFilePath]string sourceFilePath = "", [CallerLineNumber]int sourceLineNumber = 0)
        {
            if (format)
            {
                if (string.IsNullOrWhiteSpace(customMessage))
                {
                    customMessage = ex.Message;
                }
                string message = ex.GetExceptionMessage(customMessage, memberName, sourceFilePath, sourceLineNumber);

                LogError(message);
                return;
            }

            ThreadContext.Properties["logPath"] = Constants.Paths.Error;
            _logger = LogManager.GetLogger(GetCaller());
            XmlConfigurator.Configure();
            _logger.Error(ex);
            LogManager.GetRepository().Shutdown();
        }

        /// <summary>
        ///     Write log <paramref name="message"/> to log file
        /// </summary>
        /// <param name="message"></param>
        public void LogError(string message)
        {
            ThreadContext.Properties["logPath"] = Constants.Paths.Error;
            _logger = LogManager.GetLogger(GetCaller());
            XmlConfigurator.Configure();
            _logger.Error(message);
            LogManager.GetRepository().Shutdown();
        }

        /// <summary>
        ///     Get caller type
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private static Type GetCaller(int level = 2)
        {
            return new StackTrace().GetFrame(level).GetMethod().DeclaringType;
        }
    }
}