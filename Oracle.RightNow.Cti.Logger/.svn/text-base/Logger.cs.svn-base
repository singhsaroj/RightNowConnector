using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Xml;
using System.Xml.XPath;


namespace Oracle.RightNow.Cti.Logger
{
    /// <summary>
    /// Wrapper class that has instance of log4net, configured in DLL's configuration file
    /// </summary>
    public static class Logger
    {
        static string configFilePath;
        static string logLevel;

        /// <summary>
        /// Gets the Log4Net Log object, for the appenders specified in the configuration file
        /// </summary>
        public static ILog Log { get; private set; }

        /// <summary>
        /// Static method to initialize the logger from the configuration file
        /// </summary>
        /// <param name="dllPath">Full path of the assembly that contains log4net configuration</param>
        public static void Configure(string dllPath)
        {
            configFilePath = dllPath + ".config";
            Configure();
        }

        public static string LogLevel
        {
            get
            {
                return logLevel;
            }
            set
            {
                if (SetLogLevel(value))
                {
                    Configure();
                }
            }
        }

        static void Configure()
        {
            if (System.IO.File.Exists(configFilePath))
            {
                // Get the log4net configuration specified in this DLL's configuration file
                log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(configFilePath));
                Log = log4net.LogManager.GetLogger("Avaya_Cti_Logger");

                //Get loglevel from the logger file
                logLevel = GetLogLevel();
                if (logLevel == "ALL")
                {
                    logLevel = "DEBUG";
                }
                else if (logLevel == "OFF")
                {
                    logLevel = "FATAL";
                }
            }
            else
            {
                //TODO: create default logger
            }
        }

        static string GetLogLevel()
        {
            XmlDocument xDocument = new XmlDocument();
            xDocument.Load(configFilePath);

            XPathNavigator xPathNavigator = xDocument.CreateNavigator();
            XPathExpression xPathNavigation = xPathNavigator.Compile("/configuration/log4net/logger/level");
            XPathNodeIterator XPathNodeIterator = xPathNavigator.Select(xPathNavigation);

            if (XPathNodeIterator.MoveNext())
            {
                XPathNavigator nodeNavigator = XPathNodeIterator.Current.Clone();
                if (nodeNavigator.MoveToAttribute("value", string.Empty))
                {
                    return nodeNavigator.Value;
                }
            }
            return "DEBUG";
        }

        static bool SetLogLevel(string level)
        {
            try
            {
                XmlDocument xDocument = new XmlDocument();
                xDocument.Load(configFilePath);

                XPathNavigator xPathNavigator = xDocument.CreateNavigator();
                XPathExpression xPathNavigation = xPathNavigator.Compile("/configuration/log4net/logger/level");
                XPathNodeIterator XPathNodeIterator = xPathNavigator.Select(xPathNavigation);

                while (XPathNodeIterator.MoveNext())
                {
                    XPathNavigator nodeNavigator = XPathNodeIterator.Current.Clone();

                    SetAttribute(nodeNavigator, "value", xPathNavigator.NamespaceURI, level);
                }
                xDocument.Save(configFilePath);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Error in LogLevel change in .config file", ex);
                return false;
            }
            return true;
        }

        static void SetAttribute(XPathNavigator navigator, String localName, String namespaceURI, String value)
        {
            if (navigator.CanEdit && navigator.MoveToAttribute(localName, namespaceURI))
            {
                navigator.SetValue(value);
                navigator.MoveToParent();
            }
        }
    }
}
