# Elasticsearch in .NET Core

This is an ASP.NET Core 3.1 Web Application to implement indexing and search functionality with Elasticsearch using NEST 6.8.5 library.

## Indexing

Indexing is implemented as a background task using `IHostedService`. The frequency by which this background task is run can be specified in _appsettings_ file. Further, at most one Indexing background task should be running at any given time since we delete and swap the old index as part of indexing. Two indexing background task could be running at the same time if the indexing takes longer than the frequency of the background task. This is handled using a lock. The limitation of this approach is that at most one instance of this can run at any given time. One alternative would be to use _application lock_ if a database can be accessed OR using some distributed lock such as _Zookeeper_ or _Redis_.

## Search

The search functionality is exposed in an endpoint which given a search query, returns the top 100 results matching the search term.

## Monitoring

The metrics of the application are _posted_ to an endpoint expecting metrics in [InfluxDB Line Protocol][InfluxDB line protocol] in the body.

## Managing Credentials
For local development, create a new file, `DevelopmentCredentials.json`, in the root directory of the project, and add the following fields:

```
{
  "ES_READER_USERNAME": "value",
  "ES_READER_PASSWORD": "value",
  "ES_WRITER_USERNAME": "value",
  "ES_WRITER_PASSWORD": "value",
  "METRICS_API_KEY": "value"
}
```

Never commit this file to source control because this file contains sensitive strings (e.g. passwords, API keys).

For the actual deployment, specify the above credentials as environment variables.



[//]: # (These are reference links used in the body)

   [InfluxDB line protocol]: <https://docs.influxdata.com/influxdb/v1.7/write_protocols/line_protocol_tutorial/>
