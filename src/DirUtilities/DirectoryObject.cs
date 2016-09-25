using System;

namespace DirUtilities
{

    public enum DirectoryObjectType
    {
        Directory = 0,
        File = 1
    }

    public class DirectoryObject
    {
        public string Id { get; set; }

        public string ParentId { get; set; }

        public string Name { get; set; }

        public long Size { get; set; }

        public DirectoryObjectType ObjectType { get; set; }

        public string Extension { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime LastModifiedOn { get; set; }

        public string Hash { get; set; }
    }
}
