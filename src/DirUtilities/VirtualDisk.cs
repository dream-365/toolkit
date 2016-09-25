namespace DirUtilities
{
    public class VirtualDisk
    {
        private readonly string  _symbol;

        private readonly string _path;

        protected VirtualDisk(string path, string symbol)
        {
            _symbol = symbol;

            _path = path;
        }

        public static VirtualDisk From(string path, string symbol)
        {
            return new VirtualDisk(path, symbol);
        }

        public void ExportTo(string filePath)
        {
            using (JsonFileWriter<DirectoryObject> writer = new JsonFileWriter<DirectoryObject>(filePath))
            {
                DirectoryEnumerator directoryEnumerator = new DirectoryEnumerator(_path);

                directoryEnumerator.ForEach(writer.Write, _symbol);
            }
        }
    }
}
