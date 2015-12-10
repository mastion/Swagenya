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
            using (var fs = new FileStream(filePath, FileMode.Create))
            using (var sw = new StreamWriter(fs))
                sw.Write(data);
        }
    }
}