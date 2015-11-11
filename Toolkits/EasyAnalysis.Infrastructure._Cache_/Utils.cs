using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Infrastructure.Cache
{
    internal class Utils
    {
        public static string ComputeStringMD5Hash(string text, Encoding encode)
        {
            var md5 = MD5.Create();

            byte[] bytes = encode.GetBytes(text);

            return BytesToHexString(md5.ComputeHash(bytes));
        }

        private static string BytesToHexString(byte[] bytes)
        {
            if (bytes == null)
                return null;

            StringBuilder sb = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
                sb.AppendFormat("{0:x2}", bytes[i]);

            return sb.ToString();
        }
    }
}
