using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Data;
using System.Text.Json;
using System.Runtime;
using System.Diagnostics;

namespace Lab12
{
    class SIVLog
    {
        private const string PATH_TO_FILE = "SIVlogfile.json";

        public static void WriteToFile(string methodName, string className)
        {
            Data datas = new(methodName, className);
            datas.Print();
            var newData = JsonSerializer.Serialize(datas);
            using (StreamWriter sw = new(PATH_TO_FILE, true))
            {
                sw.WriteLine(newData);
            }
        }
        public static void FindByRangeDate(DateTime start, DateTime end)
        {
            using (StreamReader sr = new(PATH_TO_FILE))
            {
                var data = sr.ReadToEnd();
                var datas = JsonSerializer.Deserialize<Data>(data);
                List<Data> datas1 = new();
                datas1.Add(datas);
                datas1.Where(d => d.date >= start && d.date <= end).ToList().ForEach(d => d.Print());
            }
        }
        public static void FindDate(DateTime date)
        {
            using (StreamReader sr = new(PATH_TO_FILE))
            {
                var data = sr.ReadToEnd();
                var datas = JsonSerializer.Deserialize<Data>(data);
                List<Data> datas1 = new();
                datas1.Add(datas);
                datas1.Where(x => x.date == date).ToList().ForEach(x => x.Print());
            }
        }
    }
    class Data
    {
        public string Name { get; set; }
        public DateTime date { get; set; }
        public string NameClass { get; set; }

        public Data(string methodName, string className)
        {
            Name = methodName ?? "undefined";
            NameClass = className ?? "undefined";
            date = DateTime.Now;
        }
        public void Print()
        {
            Console.WriteLine($"Method name: {Name} Class name: {NameClass} Date: {date}");
        }
    }
    class SIVDiskInfo
    {
        public static void GetDiskInfo()
        {
            DriveInfo[] infs = DriveInfo.GetDrives();
            foreach (DriveInfo inff in infs)
            {
                Console.WriteLine($"Имя диска : {inff.Name}");
                Console.WriteLine($"Свободное место на диске : {inff.AvailableFreeSpace}");
                Console.WriteLine($"Файловая система : {inff.DriveFormat}");
                if (inff.IsReady)
                {
                    Console.WriteLine($"Объем диска: {inff.TotalSize}");
                    Console.WriteLine($"Свободное пространство: {inff.TotalFreeSpace}");
                    Console.WriteLine($"Метка: {inff.VolumeLabel}");
                }
            }
            SIVLog.WriteToFile(MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().DeclaringType.Name);
        }

    }
     class SIVFileInfo
    {
        public static void GetFileInfo(string path)
        {
                FileInfo fileInf = new FileInfo(path);
                if (fileInf.Exists)
                {
                    Console.WriteLine("Имя файла: {0}", fileInf.Name);
                    Console.WriteLine("Полный путь: {0}", fileInf.DirectoryName);
                    Console.WriteLine("Расширение: {0}", fileInf.Extension);
                    Console.WriteLine("Время создания: {0}", fileInf.CreationTime);
                    Console.WriteLine("Размер: {0}", fileInf.Length);
                    Console.WriteLine($"Время последнего доступа к файлу: {0}", fileInf.LastAccessTime);
                    Console.WriteLine($"Время последнего изменения файла: {0}", fileInf.LastWriteTime);
                }
                SIVLog.WriteToFile(MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().DeclaringType.Name);
        }
            

    }
   class SIVDirInfo
    {
        public static void GetDirInfo(string path)
        {
            DirectoryInfo dirInf = new(path);
            if (dirInf.Exists)
            {
                Console.WriteLine("Имя каталога: {0}", dirInf.Name);
                Console.WriteLine("Полный путь: {0}", dirInf.FullName);
                Console.WriteLine("Время создания: {0}", dirInf.CreationTime);
                Console.WriteLine("Корневой каталог: {0}", dirInf.Root);
                Console.WriteLine("Родительский каталог: {0}", dirInf.Parent);
                Console.WriteLine("Количество файлов: {0}", dirInf.GetFiles().Length);
                Console.WriteLine("Количество подкаталогов: {0}", dirInf.GetDirectories().Length);
            }
            SIVLog.WriteToFile(MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().DeclaringType.Name);
        }
    }
   class SIVFileManager
    {
        public static void GetFilesAndFoulders(string path)
        {
            DirectoryInfo dir = new(path);
            var files = dir.GetFiles();
            var folders = dir.GetDirectories();

            dir.CreateSubdirectory("SIVInspect");
            dir = new(path + "\\SIVInspect");

            string path_dirInfo = "SIVdirinfo.txt";
            using StreamWriter sw = new(dir.FullName + "\\" + path_dirInfo);
            sw.WriteLine("Файлы:");
            foreach (var file in files)
            {
                sw.WriteLine(file.Name);
            }
            sw.WriteLine("\n");
            sw.WriteLine("Папки:");
            foreach (var folder in folders)
            {
                sw.WriteLine(folder.Name);
            }
        }

        public static void CreateCopyOfFile(string path)
        {
            
            FileInfo file = new(path);
            DirectoryInfo dir = file.Directory ??
                          throw new ArgumentNullException("Directory is not found");
            string fullNameOfCopy = $"{dir.FullName}\\copy_{file.Name}";
            Remove(fullNameOfCopy);
            file.CopyTo(fullNameOfCopy);
        }
        public static void Remove(string path)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            FileInfo file = new(path);
            file.Delete();

            SIVLog.WriteToFile(MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().DeclaringType.Name);
        }

        public static void CreateDirectory(string path, string name)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            DirectoryInfo dir = new(path);
            dir.CreateSubdirectory(name);

            SIVLog.WriteToFile(MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().DeclaringType.Name);
        }

        public static void ReplaceTo(string pathFrom, string pathTo, params string[] extens)
        {
            DirectoryInfo dirFrom = new(pathFrom);
            DirectoryInfo dirTo = new(pathTo);

            var files = dirFrom.GetFiles();

            foreach (var file in files)
            {
                if (extens.Length == 0 ||
                    extens.Contains(file.Extension))
                {
                    Remove(dirTo.FullName + "\\" + file.Name);
                    file.MoveTo(dirTo.FullName + "\\" + file.Name);
                }
            }
            SIVLog.WriteToFile(MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().DeclaringType.Name);
        }
        public static void Replace(string newPath, string oldPath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(oldPath);
            if (dirInfo.Exists && !Directory.Exists(newPath))
            {
                Directory.Move(oldPath, newPath);

            }
            SIVLog.WriteToFile(MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().DeclaringType.Name);
        }
        public static void CreateZip(string path)
        {
            string zipPath = $"C:\\Пацей\\Lab12\\Lab12\\bin\\Debug\\net6.0\\SIVFiles.zip";
            ZipFile.CreateFromDirectory(path, zipPath);
            SIVLog.WriteToFile(MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().DeclaringType.Name);
        }
        public static void UnZip()
        {
            string zipPath = $"C:\\Пацей\\Lab12\\Lab12\\bin\\Debug\\net6.0\\SIVFiles.zip";
            string extractPath = $"C:\\Пацей\\Lab12\\Lab12\\bin\\Debug\\net6.0\\SIVFiles.extract";
            ZipFile.ExtractToDirectory(zipPath,extractPath);
            SIVLog.WriteToFile(MethodBase.GetCurrentMethod().Name, MethodBase.GetCurrentMethod().DeclaringType.Name);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Тест метода GetDiskInfo()");
                SIVDiskInfo.GetDiskInfo();

                Console.WriteLine("\n\nТест метода GetFileInfo()");
                SIVFileInfo.GetFileInfo("C:\\Пацей\\Lab12\\12_Потоки_файловая система.pdf");

                Console.WriteLine("\n\nТест метода GetDirInfo()");
                SIVDirInfo.GetDirInfo("C:\\Пацей");

                Console.WriteLine("\n\nТест метода GetFilesAndFoulders()");
                SIVFileManager.GetFilesAndFoulders("..\\net6.0");

                Console.WriteLine("\n\nТест метода CreateCopyOfFile()");
                SIVFileManager.CreateCopyOfFile("..\\net6.0\\SIVInspect\\SIVdirinfo.txt");

                Console.WriteLine("\n\nТест метода Remove()");
                SIVFileManager.Remove("..\\net6.0\\SIVInspect\\SIVdirinfo.txt");

                Console.WriteLine("\n\nСоздание директория SIVFiles");
                SIVFileManager.CreateDirectory("..\\net6.0", "SIVFiles");

                Console.WriteLine("\n\nКопирование файлов с расширением .dll и .exe в директорию SIVFiles");
                SIVFileManager.ReplaceTo("..\\net6.0", "SIVFiles", ".dll", ".exe");

                Console.WriteLine("\n\nПеремещение SIVFiles в директорий SIVInspect ");
                SIVFileManager.Replace("C:\\Пацей\\Lab12\\Lab12\\bin\\Debug\\net6.0\\SIVInspect", "C:\\Пацей\\Lab12\\Lab12\\bin\\Debug\\net6.0\\SIVFiles");

                Console.WriteLine("\n\nУпаковка в zip-архив SIVFiles");
                SIVFileManager.CreateZip("C:\\Пацей\\Lab12\\Lab12\\bin\\Debug\\net6.0\\SIVFiles");

                Console.WriteLine("\n\nРаспаковка zip-архива SIVFiles");
                SIVFileManager.UnZip();
                DateTime startTime = new DateTime(2022,12,06);
                DateTime endTime = new DateTime(2022, 12, 07);
                SIVLog.FindByRangeDate(startTime,endTime);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }




        }
    }      
}