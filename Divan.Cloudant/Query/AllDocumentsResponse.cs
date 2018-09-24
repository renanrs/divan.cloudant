using System;
using System.Collections.Generic;

namespace Divan.Cloudant
{
    public class RowValue{
        public string rev { get; set; }
    }

    public class RowsResponse<T> where T : class{
        public string id { get; set; }
        public string key { get; set; }
        public RowValue value { get; set; }
        public T doc { get; set; }
    }

    public class AllDocsViewResponse<T> where T : class
    {
        public int total_rows { get; set; } 
        public int offset { get; set; }
        public List<RowsResponse<T>> rows { get; set; }
        
    }
}