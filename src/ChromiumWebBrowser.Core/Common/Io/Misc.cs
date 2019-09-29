using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChromiumWebBrowser.Core.Common.Hash;
using Newtonsoft.Json;
using NLog;

namespace ChromiumWebBrowser.Core.Common.Io
{
    public class Misc
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public static void CreateDirIfNotExist(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static void ClearFolder(string path)
        {
            var dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                foreach (var fi in dir.GetFiles())
                {
                    fi.IsReadOnly = false;
                    fi.Delete();
                }
                foreach (var di in dir.GetDirectories())
                {
                    ClearFolder(di.FullName);
                    di.Delete();
                }
            }
        }

        public static void RemoveFilesFromFolder(string path, string pattern)
        {
            var dir = new DirectoryInfo(path);

            foreach (var fi in dir.GetFiles(pattern))
            {
                fi.IsReadOnly = false;
                fi.Delete();
            }
        }

        public static void RemoveFolder(string path)
        {
            if (Directory.Exists(path))
            {
                var dir = new DirectoryInfo(path);

                foreach (var fi in dir.GetFiles())
                {
                    fi.IsReadOnly = false;
                    fi.Delete();
                }

                foreach (var di in dir.GetDirectories())
                {
                    ClearFolder(di.FullName);
                    di.Delete();
                }
                dir.Delete();
            }
        }

        public static void RemoveFile(string path)
        {
            var fileInfo = new FileInfo(path);


            fileInfo.Delete();
        }

        public static void RemoveFileAndWait(string filepath, int timeout = 30000)
        {
            if (File.Exists(filepath))
            {
                var dirname = Path.GetDirectoryName(filepath);
                var filename = Path.GetFileName(filepath);
                if (dirname == null || filename == null) return;
                using (var fw = new FileSystemWatcher(dirname, filename))
                using (var mre = new ManualResetEventSlim())
                {
                    fw.EnableRaisingEvents = true;
                    fw.Deleted += (object sender, FileSystemEventArgs e) =>
                    {
                        mre.Set();
                    };
                    File.Delete(filepath);
                    mre.Wait(timeout);
                }
            }


        }
        public static void RenameFile(string src, string dst)
        {
            File.Move(src, dst);
        }


        public static string[] CleanEmptyLines(string[] lines)
        {
            return lines.Where(x => x.Trim().Length > 0).ToArray();
        }

        public static string[] CleanComments(string[] lines)
        {
            return lines.Where(x => !x.Trim().StartsWith("#")).ToArray();
        }

        public static void SaveJson<T>(string path, T obj, Formatting formatting = Formatting.Indented,
            FileMode fileMode = FileMode.Create)
        {
            using (var fs = File.Open(path, fileMode))
            {
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    using (JsonWriter jw = new JsonTextWriter(sw))
                    {
                        jw.Formatting = formatting;

                        var serializer = new JsonSerializer();
                        serializer.Serialize(jw, obj);
                    }
                }
            }
        }
        public static async Task CopyFilesAsync(List<(string src, string dst)> files, Action<int> percentageProgressFn)
        {
            var count = 0;
            var numberEntries = files.Count;

            foreach (var file in files)
            {
                count++;
                await CopyFileAsync(file.src, file.dst);
                percentageProgressFn(GetNormalizedValue(numberEntries, count));
            }
        }
        private static int GetNormalizedValue(int max, int current)
        {
            return current * 100 / max;
        }
        public static async Task CopyFileAsync(string sourcePath, string destinationPath)
        {
            using (Stream source = File.Open(sourcePath, FileMode.Open))
            {
                var d = Path.GetDirectoryName(destinationPath);
                CreateDirIfNotExist(d);

                using (Stream destination = File.Create(destinationPath))
                {
                    await source.CopyToAsync(destination);
                }
            }
        }
        public static T LoadJson<T>(string path)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }

        public static string GetFileName(string name)
        {
            var temp = name
                .Replace(":", "-")
                .Replace("/", "-")
                .RemoveSpace("-").RemoveQuotationMarks("").Trim().ToLower();
            var withOutPolish = temp.ReplacePolishChars();
            var onlyAscsii = withOutPolish.IsOnlyAsciiChars();
            return onlyAscsii ? withOutPolish : Other.CreateMD5(temp);

            //return name.RemoveSpace("-").RemoveQuotationMarks("").Trim().ToLower();
            //return name.RemoveNonAscii().RemoveSpace("-").RemoveQuotationMarks("").Trim().ToLower();
        }

        public static string GetFileName2(string name)
        {
            return name.Replace('"', '\'').ToLower();
        }

        public static string ReadFileWithRetry(string path, int numberOfRetries = 6, int delayOnRetry = 500)
        {
            var result = string.Empty;
            for (var i = 1; i <= numberOfRetries; ++i)
                try
                {
                    result = File.ReadAllText(path);
                    break; // When done we can break loop
                }
                catch (IOException)
                {
                    _log.Debug($"Unable to read file: {path}; Try number: {i} ");
                    // You may check error code to filter some exceptions, not every error
                    // can be recovered.
                    if (i == numberOfRetries) // Last one, (re)throw exception and exit
                        throw;

                    Thread.Sleep(delayOnRetry);
                }
            return result;
        }

        public static FileStream WaitForFile(string fullPath, FileMode mode, FileAccess access, FileShare share)
        {
            for (var numTries = 0; numTries < 10; numTries++)
            {
                FileStream fs = null;

                try
                {
                    fs = new FileStream(fullPath, mode, access, share);

                    fs.ReadByte();
                    fs.Seek(0, SeekOrigin.Begin);

                    return fs;
                }
                catch (IOException)
                {
                    fs?.Dispose();
                    Thread.Sleep(50);
                }
            }

            return null;
        }

        public static bool WaitForFile(string fullPath)
        {
            var numTries = 0;
            while (true)
            {
                ++numTries;
                try
                {
                    // Attempt to open the file exclusively.
                    using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 100))
                    {
                        fs.ReadByte();

                        // If we got this far the file is ready
                        break;
                    }
                }
                catch (Exception ex)
                {
                    _log.Debug($"WaitForFile {fullPath} failed to get an exclusive lock: {ex}");

                    if (numTries > 10)
                    {
                        _log.Debug($"WaitForFile {fullPath} giving up after 10 tries");
                        return false;
                    }

                    // Wait for the lock to be released
                    Thread.Sleep(500);
                }
            }

            _log.Debug($"WaitForFile {fullPath} ; File is ready to use after: {numTries} tries");
            return true;
        }
    }
}