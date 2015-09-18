using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyAnalysis.Models
{
    public class Category
    {
        public int index { get; set; }

        public string name { get; set; }
    }

    public class TypeObject
    {
        public int id { get; set; }

        public string name { get; set; }
    }

    public class CategoryResult
    {
        public IEnumerable<Category> categories { get; set; }

        public IEnumerable<IEnumerable<TypeObject>> typeGroups { get; set; }
    }
}