namespace Elasticsearch.Monitoring.Data
{
    using System.Collections.Generic;

    public interface IDataPointTranslator
    {
        string TranslateToLineProtocol(IEnumerable<DataPoint> dataPoints);
    }
}