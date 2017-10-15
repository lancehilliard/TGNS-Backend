using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGNS.Migration
{
    class Program
    {
        static void Main(string[] args)
        {
            var realmName = "ns2";
            var outLines = new List<string>();
            foreach (string file in GetFiles(@"C:\TGNS\tgns1\config\tgnsdata"))
            {
                try
                {
                    var dataTypeName = new DirectoryInfo(file).Parent.Name;
                    var recordId = Path.GetFileNameWithoutExtension(file);
                    var data = File.ReadAllText(file).Replace("'", "''");
                    outLines.Add(string.Format(@"'{0}','{1}','{2}','{3}'", realmName, dataTypeName, recordId, data));
                    // LOAD DATA LOCAL INFILE 'C:/TGNS/tgns1/backend/TGNS/TGNS.Migration/bin/Debug/out.txt' INTO TABLE data COLUMNS TERMINATED BY ',' ENCLOSED BY '\'' LINES TERMINATED BY '\r\n' (DataRealm, DataTypeName, DataRecordId, DataValue);
                }
                catch (Exception)
                {
                }
            }
            //File.WriteAllLines("out.txt", outLines.Skip(90).Take(1));
            File.WriteAllLines("out.txt", outLines);
        }

        static IEnumerable<string> GetFiles(string path)
        {
            Queue<string> queue = new Queue<string>();
            queue.Enqueue(path);
            while (queue.Count > 0)
            {
                path = queue.Dequeue();
                try
                {
                    foreach (string subDir in Directory.GetDirectories(path))
                    {
                        queue.Enqueue(subDir);
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
                string[] files = null;
                try
                {
                    files = Directory.GetFiles(path);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
                if (files != null)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        yield return files[i];
                    }
                }
            }
        }
    }
}
