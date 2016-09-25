using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace DirUtilities
{
    public class DirectoryEnumerator
    {
        private Action<DirectoryObject> _processFunction;

        private static Regex WITHOUT_DRIVER_REGEX = new Regex(@"^[a-z]+\:", RegexOptions.IgnoreCase);

        private string _path;

        public DirectoryEnumerator(string path)
        {
            _path = path;
        }

        public void ForEach(Action<DirectoryObject> processFunction, string rootId = null)
        {
            _processFunction = processFunction;

            var dir = new DirectoryInfo(_path);

            ForEach(dir, rootId);
        }

        public static DirectoryObject ConvertDirectoryInfoToObject(DirectoryInfo info)
        {
            string path = WITHOUT_DRIVER_REGEX.Replace(info.FullName, "");

            string hash = HASH.ComputeMD5Hash(path);

            return new DirectoryObject
            {
                Id = Guid.NewGuid().ToString(),
                Name = info.Name,
                CreatedOn = info.CreationTime,
                LastModifiedOn = info.LastWriteTime,
                ObjectType = DirectoryObjectType.Directory,
                Hash = hash
            };
        }

        public static DirectoryObject ConvertFileInfoToObject(FileInfo info)
        {
            string path = WITHOUT_DRIVER_REGEX.Replace(info.FullName, "");

            string hash = HASH.ComputeMD5Hash(path);

            return new DirectoryObject
            {
                Id = Guid.NewGuid().ToString(),
                Name = info.Name,
                CreatedOn = info.CreationTime,
                LastModifiedOn = info.LastWriteTime,
                ObjectType = DirectoryObjectType.File,
                Size = info.Length,
                Extension = Path.GetExtension(info.Name),
                Hash = hash
            };
        }


        protected void ForEach(DirectoryInfo dir, string parentId)
        {
            try
            {
                IEnumerable<FileInfo> files = dir.EnumerateFiles();

                foreach (var file in files)
                {
                    DirectoryObject obj = ConvertFileInfoToObject(file);

                    obj.ParentId = parentId;

                    _processFunction.Invoke(obj);
                }

                IEnumerable<DirectoryInfo> directories = dir.EnumerateDirectories();

                foreach (var directory in directories)
                {
                    if (directory.Name.Equals("$RECYCLE.BIN", StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    DirectoryObject obj = ConvertDirectoryInfoToObject(directory);

                    obj.ParentId = parentId;

                    _processFunction.Invoke(obj);

                    ForEach(directory, obj.Id);
                }

            }
            catch(UnauthorizedAccessException) { }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
