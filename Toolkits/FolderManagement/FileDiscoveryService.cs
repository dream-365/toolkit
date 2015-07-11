using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FolderManagement
{
    public class FileStorageService
    {
        public IEnumerable<string> QueryFolders(string root, string regex)
        {
            var result = new List<string>();

            var targetDirectory = new DirectoryInfo(root);

            var find = new Regex(regex, RegexOptions.IgnoreCase);

            IEnumerable<DirectoryInfo> subDirs = targetDirectory.EnumerateDirectories();

            foreach (var subDir in subDirs)
            {
                var matchResult = find.Match(subDir.Name);

                if (matchResult.Success)
                {
                    result.Add(subDir.Name);
                }
            }

            return result;
        }

        public string ReanmePreview(string name, string regex, string format)
        {
            return InternalRename(name, regex, format);
        }

        public string RenameFile(string fullPath, string regex, string format)
        {
            var dir = Path.GetDirectoryName(fullPath);

            var renameTo = InternalRename(Path.GetFileNameWithoutExtension(fullPath), regex, format);

            var targetPath = Path.Combine(dir, renameTo + Path.GetExtension(fullPath));

            CreateDirectoryIfNotExists(Path.GetDirectoryName(targetPath));

            File.Move(fullPath, targetPath);

            return targetPath;
        }



        public string RenameFolder(string fullPath, string regex, string format)
        {
            var dir = Path.GetDirectoryName(fullPath);

            var renameTo = InternalRename(Path.GetFileName(fullPath), regex, format);

            var targetPath = Path.Combine(dir, renameTo);

            Directory.Move(fullPath, targetPath);

            return targetPath;
        }

        private static void CreateDirectoryIfNotExists(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        private static string InternalRename(string name, string regex, string format)
        {
            string renameTo = string.Empty;

            var match = new Regex(regex, RegexOptions.IgnoreCase);

            var matchResult = match.Match(name);

            var groupValueList = new List<String>();

            foreach (Group group in matchResult.Groups)
            {
                groupValueList.Add(group.Value);
            }

            if (matchResult.Success)
            {
                renameTo = String.Format(format, groupValueList.ToArray());
            }

            return renameTo;
        }
    
        public IEnumerable<string> QueryFiles(string root, string regex)
        {
            var result = new List<string>();

            var targetDirectory = new DirectoryInfo(root);

            var find = new Regex(regex, RegexOptions.IgnoreCase);

            IEnumerable<FileInfo> files = targetDirectory.GetFiles();

            foreach (var file in files)
            {
                var matchResult = find.Match(file.Name);

                if (matchResult.Success)
                {
                    result.Add(file.Name);
                }
            }

            return result;
        }
    }
}
