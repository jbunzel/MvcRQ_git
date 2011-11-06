using System;
using System.Configuration;
using System.Web;
using RQState.Components.Storage;

namespace RQState.Components
{
    public static class Config
    {
        private const string DEFAULT_STORAGE_DIR = @"C:/Temp/Storage/";
        private const StorageType STORAGE_TYPE = StorageType.AppicationStorage;
        public static string DirectoryStoragePath
        {
            get
            {
                string directory = ConfigurationManager.AppSettings["DirectoryStoragePath"];
                return string.IsNullOrEmpty(directory) ? DEFAULT_STORAGE_DIR : directory;
            }
        }


        public static StorageType StorageType
        {
            get
            {
                string state = ConfigurationManager.AppSettings["StorageType"];
                if (string.IsNullOrEmpty(state))
                    return STORAGE_TYPE;
                try
                {
                    return (StorageType) Enum.Parse(typeof (StorageType), state);
                }
                catch (ArgumentException e)
                {
                    throw new ApplicationException("Unable to parse 'StorageType' property", e);
                }
            }
        }


        public static string ConnectionString
        {
            get
            {
                string dbFile = HttpContext.Current.ApplicationInstance.Server.MapPath("~/App_Data/State.mdb");
                return "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + dbFile;
            }
        }
    }
}