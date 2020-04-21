namespace Elasticsearch.Monitoring.Data
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public class DataPointTranslator : IDataPointTranslator
    {
        public string TranslateToLineProtocol(IEnumerable<DataPoint> dataPoints)
        {
            const string NewLine = "\n";

            return string.Join(NewLine, dataPoints.Select(this.GetLine));
        }

        private static string EscapeSpecialCharacters(string value)
        {
            // Spaces and commas must be escaped in tag keys, tag values, and field keys,
            // see https://docs.influxdata.com/influxdb/v1.7/write_protocols/line_protocol_reference/
            return value.Replace(" ", @"\ ").Replace(",", @"\,");
        }

        private static string GetUnixTimeInNanoseconds(DateTime time)
        {
            // The TimeStamp must be the number of nanoseconds since the Unix epoch in non-scientific representation.
            var nanoseconds = new DateTimeOffset(time).ToUnixTimeMilliseconds() * Math.Pow(10, 6);
            return ((decimal)nanoseconds).ToString(CultureInfo.InvariantCulture).Replace(',', '.');
        }

        private string GetLine(DataPoint dataPoint)
        {
            const string Comma = ",";
            const string Space = " ";

            // 'measurement' name must be preceded with a prefix followed by a  period.
            var fullMeasurementName = $"{EscapeSpecialCharacters(dataPoint.Measurement)}";

            var line = fullMeasurementName;
            line += Comma + string.Join(
                        Comma,
                        dataPoint.TagSet.Select(x => $"{EscapeSpecialCharacters(x.Key)}={EscapeSpecialCharacters(x.Value)}"));
            line += Space + string.Join(Comma, dataPoint.FieldSet.Select(x => $"{EscapeSpecialCharacters(x.Key)}={x.Value}"));
            line += Space + GetUnixTimeInNanoseconds(dataPoint.TimeStamp);

            return line;
        }
    }
}