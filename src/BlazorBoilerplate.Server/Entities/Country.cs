using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorBoilerplate.Server.Entities
{
    public class Country
    {
        public int    Id      { get; set; }
        public string Moniker { get; set; }

        public string Name     { get; set; }
        public string IsoCode2 { get; set; }
        public string IsoCode3 { get; set; }
    }
}