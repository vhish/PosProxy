using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Extensions
{
    public static class FileExtensions
    {
        private static string ToOrdersDir(string accountId)
        {
            var uploadDir = $"wwwroot{Path.AltDirectorySeparatorChar}{accountId}{Path.AltDirectorySeparatorChar}orders";
            try
            {
                if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);
            }
            catch (Exception e)
            {
                throw new IOException($"File Path does NOT exist: {e.Message}");
            }
            return uploadDir;
        }

        public static string ToFilePath(this string fileName, string accountId)
        {
            var directory = Path.Combine(Directory.GetCurrentDirectory(), ToOrdersDir(accountId));
            Directory.CreateDirectory(directory);
            return Path.Combine(Directory.GetCurrentDirectory(), ToOrdersDir(accountId), $"{Guid.NewGuid()}-{fileName}");
        }
    }
}
