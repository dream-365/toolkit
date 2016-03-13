using OnlineStroage.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadToBlob
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new OnlineStroageClient("http://localhost:62443");

            Console.WriteLine("start to upload file...");

            using (var memoryStream = new MemoryStream())
            using (var sw = new StreamWriter(memoryStream))
            {
                sw.WriteLine("Hello World");

                sw.Flush();

                memoryStream.Position = 0;

                var task = client.UploadStreamAsync("test/helloword.txt", memoryStream);

                task.Wait();
            }

            Console.WriteLine("end of uploading file!");
        }
    }
}
