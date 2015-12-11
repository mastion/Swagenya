using System;
using System.IO;
using System.Web.Configuration;

namespace ApiGeneratorApi.Util
{
    public interface IFolderWriter
    {
        string GetFolderName(string level);
    }
    public class FolderWriter :IFolderWriter
    {
        public string GetFolderName(string level)
        {
            var s = string.Format("{0}/{2}{3}{4}/{1}/", WebConfigurationManager.AppSettings["OutputFolder"], level, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            Directory.CreateDirectory(s);
            return s;
        }
    }
}