using System.IO;

namespace ApiGeneratorApi.Models
{
    public class FileWriter
    {
        public static void WriteFile(string filePath, string data)
        {
            Directory.CreateDirectory(filePath.Remove(filePath.LastIndexOf('/')));
            using (var fs = new FileStream(filePath, FileMode.Create))
            using (var sw = new StreamWriter(fs))
                sw.Write(data);
        }
    }
}