using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace office365.Utils
{
    public class FileHelp
    {
        public static string FileStream(string filePath)
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            using StreamReader reader = new StreamReader(fileStream);
            string line = reader.ReadLine();
            return line;
        }

        public static void FileWriter(string filePath,string txt)
        {
            File.Delete(filePath);
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, txt);
            }
        }

    }
}