using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ChromiumWebBrowser.Core.Common.Io
{
    public static class FileAsync
    {
        /// <summary>
        /// This is the same default buffer size as
        /// <see cref="StreamReader" /> and <see cref="FileStream" />.
        /// </summary>
        private const int _DEFAULT_BUFFER_SIZE = 4096;

        /// <summary>
        /// Indicates that
        /// 1. The file is to be used for asynchronous reading.
        /// 2. The file is to be accessed sequentially from beginning to end.
        /// </summary>
        private const FileOptions _DEFAULT_OPTIONS = FileOptions.Asynchronous | FileOptions.SequentialScan;

        public static Task<string[]> ReadAllLinesAsync(string path)
        {
            return ReadAllLinesAsync(path, Encoding.UTF8);
        }
        public static Task<string> ReadAllTextAsync(string path)
        {
            return ReadAllTextAsync(path, Encoding.UTF8);
        }

        public static async Task<string[]> ReadAllLinesAsync(string path, Encoding encoding)
        {
            var lines = new List<string>();

            // Open the FileStream with the same FileMode, FileAccess
            // and FileShare as a call to File.OpenText would've done.
            using (
                var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, _DEFAULT_BUFFER_SIZE,
                    _DEFAULT_OPTIONS))
            using (var reader = new StreamReader(stream, encoding))
            {
                string line;
                while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
                    lines.Add(line);
            }

            return lines.ToArray();
        }

        public static async Task<string> ReadAllTextAsync(string path, Encoding encoding)
        {
            // Open the FileStream with the same FileMode, FileAccess
            // and FileShare as a call to File.OpenText would've done.
            using (
                var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, _DEFAULT_BUFFER_SIZE,
                    _DEFAULT_OPTIONS))
            using (var reader = new StreamReader(stream, encoding))
            {
                return await reader.ReadToEndAsync().ConfigureAwait(false);
            }
        }
    }
}