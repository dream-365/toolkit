using System;
using System.IO;

namespace OnlineStroage
{
    public class LocalBlob
    {
        private string _root = System.Configuration.ConfigurationManager.AppSettings["storage.root"];

        public static LocalBlob Inst = new LocalBlob();

        private LocalBlob()
        {

        }

        public bool Set(string path, Stream stream)
        {
            var fullPath = Path.Combine(_root, path);

            var dir = Path.GetDirectoryName(fullPath);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using (var fs = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                stream.CopyTo(fs);
            }

            return true;
        }

        public Stream Get(string path)
        {
            var fullPath = Path.Combine(_root, path);

            var stream = new MemoryStream();

            using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
            {
                fs.CopyTo(stream);
            }

            stream.Position = 0;

            return stream;
        }

        public bool Delete(string path)
        {
            var fullPath = Path.Combine(_root, path);

            if(!File.Exists(fullPath))
            {
                return true;
            }

            try
            {
                File.Delete(fullPath);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}