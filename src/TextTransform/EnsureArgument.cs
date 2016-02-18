using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextTransform
{
    class EnsureArgument
    {
        public static void IsNotNull(object obj)
        {
            if(obj == null)
            {
                throw new ArgumentNullException();
            }
        }

        public static void LengthEqual(string[] parameters, int exLength)
        {
            if(parameters.Length != exLength)
            {
                throw new ArgumentException(string.Format("the expected argument length is {0}, the actual arguments length is {1}", exLength, parameters.Length));
            }
        }
    }
}
