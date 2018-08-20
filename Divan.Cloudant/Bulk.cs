using System;
using System.Collections.Generic;
using System.Text;

namespace Divan.Cloudant
{
    public sealed class Bulk<T> where T : class
    {        
        public IEnumerable<T> docs { get; set; }
        public Bulk(){}
        public Bulk(IEnumerable<T> list){
            docs = list;
        }
    }
}
