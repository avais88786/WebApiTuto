using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApisTutorial.Models
{
    public class ItemList
    {
        public string Category { get; set; }

        public IEnumerable<string> Items { get; set; }
    }
}
