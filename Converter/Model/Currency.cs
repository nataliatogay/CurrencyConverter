using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Model
{
    public class Currency
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Code { get; set; }

        public override string ToString()
        {
            return $"{Code}\t{Name}";
        }
    }
}
