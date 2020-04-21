namespace Elasticsearch.Monitoring.Data
{
    using System;
    using System.Collections.Generic;

    public class DataPoint
    {
        public Dictionary<string, double> FieldSet { get; set; }

        public string Measurement { get; set; }

        public Dictionary<string, string> TagSet { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}