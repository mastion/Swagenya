using System;
using System.Configuration;
using System.Data.SqlClient;
using Incomm.Libraries.Cryptography;

namespace Giftango.Component.Utility
{
    public static class ConnectionHelper
    {
        private const string IsConnectionStringEncryptedConfigKey = "IsConnectionStringEncrypted";

        private const string DefaultConnectionNameConfigKey = "DefaultConnectionName";

        private const string DefaultConnectionNameDefaultValue = "ConnStr";

        private const string LogNameDefaultValue = "LogStr";

        private const string ReportingNameDefaultValue = "ReportingDBConnStr";

        private const bool IsConnectionStringEncryptedDefaultValue = true;

        private static readonly bool IsConnectionStringEncrypted;

        private static readonly string DefaultConnectionName;

        static ConnectionHelper()
        {
            // Determine whether the connection string is encrypted
            IsConnectionStringEncrypted = IsConnectionStringEncryptedDefaultValue;
            string isConnectionStringEncryptedValueFromConfig = ConfigurationManager.AppSettings[IsConnectionStringEncryptedConfigKey];
            if (!String.IsNullOrEmpty(isConnectionStringEncryptedValueFromConfig))
            {
                Boolean.TryParse(isConnectionStringEncryptedValueFromConfig, out IsConnectionStringEncrypted);
            }

            // Determine the default connection name
            DefaultConnectionName = ConfigurationManager.AppSettings[DefaultConnectionNameConfigKey];
            if (String.IsNullOrEmpty(DefaultConnectionName))
            {
                DefaultConnectionName = DefaultConnectionNameDefaultValue;
            }
        }

        public static string GetConnectionString()
        {
            return GetConnectionString(DefaultConnectionName);
        }

        public static string GetConnectionString(string connectionName)
        {
            // Attempt to load the connection string from the connectionStrings section. 
            // If it doesn't exist, attempt to load it from the appSettings section.
            ConnectionStringSettings connectionStringSetting = ConfigurationManager.ConnectionStrings[connectionName];
            string connectionString = connectionStringSetting != null
                ? connectionStringSetting.ConnectionString
                : ConfigurationManager.AppSettings[connectionName];

            if (connectionString == null)
            {
                throw new ConfigurationErrorsException(String.Format("A connection string named '{0}' could not be found.", connectionName));
            }

            // Decrypt the connection string if necessary
            if (IsConnectionStringEncrypted)
            {
                connectionString = EncryptDecrypt.DecryptTripleDES(connectionString);
            }

            return connectionString;
        }

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(GetConnectionString());
        }

        public static SqlConnection GetLoggingConnection()
        {
            return new SqlConnection(GetLogConnectionString());
        }
        public static string GetLogConnectionString()
        {
            return GetConnectionString(LogNameDefaultValue);
        }

        public static SqlConnection GetReportingConnection()
        {
            return new SqlConnection(GetReportingConnectionString());
        }
        public static string GetReportingConnectionString()
        {
            return GetConnectionString(ReportingNameDefaultValue);
        }
    }
}