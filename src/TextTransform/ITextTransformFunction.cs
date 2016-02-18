using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextTransform
{
    public interface ITextTransformFunction
    {
        string Call(string value, string[] parameters);
    }
}
