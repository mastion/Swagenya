using System.IO;

namespace ApiGeneratorApi.Util
{
    public interface IFileWriter
    {
        void WriteFile(string filePath, string data);
    }

    public class FileWriter : IFileWriter
    {
        public void WriteFile(string filePath, string data)
        {
            Directory.CreateDirectory(filePath.Remove(filePath.LastIndexOf('/')));
            using (var fs = new FileStream(filePath, FileMode.Create))
            using (var sw = new StreamWriter(fs))
                sw.Write(data);
        }
    }
}