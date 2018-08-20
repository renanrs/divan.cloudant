using System;
using System.Collections.Generic;

namespace Divan.Cloudant
{
    public class RowValue{
        public string rev { get; set; }
    }

    public class RowsResponse{
        public string id { get; set; }
        public string key { get; set; }
        public RowValue value { get; set; }
    }

    public class AllDocumentsResponse
    {
        public int total_rows { get; set; } 
        public int offset { get; set; }
        public List<RowsResponse> rows { get; set; }
    }
}