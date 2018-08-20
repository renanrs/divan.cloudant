using System;
using System.Collections.Generic;
using System.Text;

namespace Divan.Cloudant
{
    public class ResultObject
    {
        public string id { get; set; }
        public string ok { get; set; }
        public string rev  { get; set; }
        public string reason { get; set; }
        public string error { get; set; }
    }
}
