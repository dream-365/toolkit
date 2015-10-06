using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.HttpCache
{
    public interface IWebPageNavigation
    {
        void NavigateTo(int pageIndex);

        void Previous();

        void Next();

        Task<Stream> GetAsync();
    }
}
