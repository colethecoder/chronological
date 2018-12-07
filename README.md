[![Build status](https://ci.appveyor.com/api/projects/status/3x04uwu4bu4fy28s/branch/master?svg=true)](https://ci.appveyor.com/project/colethecoder/chronological/branch/master)

# chronological
Chronological is a library to simplify access to the Azure Time Series Insights API. This project is a work in progress, feedback and pull requests are welcomed.

Note: this currently only supports the GA version not the new Preview version.

## Download

The latest release of the Chronological is [available on NuGet](https://www.nuget.org/packages/Chronological/)

## Example

Entity:

```cs
public class TimeSeriesEntity
{
    [ChronologicalEventField("DeviceId")]
    public string Id { get; set; }

    [ChronologicalEventField(BuiltIn.EventTimeStamp)]
    public DateTime Date { get; set; }

    [ChronologicalEventField("EventType")]
    public string Type { get; set; }

    [ChronologicalEventField("Measurement.Value")]
    public double? Value { get; set; }
}
```

Get an environment:

```cs
var connection = new Chronological.Connection(YourApplicationClientID,
                YourApplicationClientSecret, YourTenant);

var environments = await connection.GetEnvironmentsAsync();

var environment = environments.First();
```

Query for events:

```cs
var events = await environment.EventQuery<TimeSeriesEntity>(FromDate, ToDate, Limit.Take, 200)
                    .Where(x => x.Value > 5)
                    .ExecuteAsync();
```

Query for aggregate data:

```cs
var aggregates = await environment.AggregateQuery<TestType1>(FromDate, ToDate)
                    .Select(builder => builder.UniqueValues(x => x.Type, 10,
                                builder.UniqueValues(x => x.Id, 10,
                                    builder.DateHistogram(x => x.Date, Breaks.InDays(1),
                                        new
                                        {
                                            Count = builder.Count(),
                                            Max = builder.Maximum(x => x.Value)
                                        }))))
                    .Where(x => x.Value > 5)
                    .ExecuteAsync();
```

## Setting Up Time Series Insights ready for use with Chronological

To access the Time Series Insights API you first have to setup an environment in Azure, details here:

[https://docs.microsoft.com/en-us/azure/time-series-insights/time-series-insights-get-started](https://docs.microsoft.com/en-us/azure/time-series-insights/time-series-insights-get-started)

You then need to configure authentication, instructions here:

[https://docs.microsoft.com/en-us/azure/time-series-insights/time-series-insights-authentication-and-authorization](https://docs.microsoft.com/en-us/azure/time-series-insights/time-series-insights-authentication-and-authorization)

Once you have completed these steps and have your authentication details you can begin to use Chronological to access data.

## Creating a Connection

To use the API you first have to create a connection using the Active Directory details for your instance:

```cs
var connection = new Chronological.Connection(YourApplicationClientID,
                YourApplicationClientSecret, YourTenant);
```

## Getting Environments 

An Environment is the specific instance of Time Series Insights you want to access. Once you have a Connection object you can then retrieve a list of available Environments (IEnumerable<Environment>):

```cs
var environments = await connection.GetEnvironmentsAsync();
```

This is using the [Get Environments API](https://docs.microsoft.com/en-us/rest/api/time-series-insights/time-series-insights-reference-queryapi#get-environments-api)

Environments are identified by their FQDN which takes the form:

<GUID>.env.timeseries.azure.com

If you know the FQDN you can create an Environment instance using:

```cs
var environment = await connection.GetEnvironmentAsync(myEnvironmentFqdn);
```

An Environment object can then be used to interrogate your data.

## Availability

Availability of data can be queried as below:

```cs
var availability =  await environment.GetAvailabilityAsync();
```

This is using the [Get Environment Availability API](https://docs.microsoft.com/en-us/rest/api/time-series-insights/time-series-insights-reference-queryapi#get-environment-availability-api)

The return type looks like:

```cs
public class Availability
{
    public AvailabilityRange Range { get; }
    public TimeSpan IntervalSize { get; }

    public Dictionary<DateTime, int> Distribution { get; }
}

public class AvailabilityRange
{
    public DateTime From { get; }
    public DateTime To { get; }
}
```

This is useful to narrow down subsequent queries to ranges when data is available

## Metadata

Metadata gives you information about the particular properties available within the data for the date range specified, this could be useful in scenarios in which the schema sent in has changed over time for example.

```cs
var meta = await environment.GetMetadataAsync(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow);
```

This is using the [Get Environment Metadata API](https://docs.microsoft.com/en-us/rest/api/time-series-insights/time-series-insights-reference-queryapi#get-environment-metadata-api)

## Queries

Queries via Chronological are strongly typed. To create a query you must first create a class to represent the Events you want to query. This is done by decorating a class with the ChronologicalEventFieldAttribute

*Future feature plans include inferring the name for fields from the property name if you omit the attribute but since any nested JSON in Time Series Insights ends up separated by "." the usefulness of this is limited*

```cs
public class TimeSeriesEntity
{
    [ChronologicalEventField("DeviceId")]
    public string Id { get; set; }

    [ChronologicalEventField(BuiltIn.EventTimeStamp)]
    public DateTime Date { get; set; }

    [ChronologicalEventField("EventType")]
    public string Type { get; set; }

    [ChronologicalEventField("Measurement.Value")]
    public double? Value { get; set; }
}
```

BuiltIn includes some helpers for Time Series Insights common properties.

## Event Queries

Events are queried using the [Get Environment Events Streamed API](https://docs.microsoft.com/en-us/rest/api/time-series-insights/time-series-insights-reference-queryapi#get-environment-events-streamed-api)

A simple Event Query could look like:

```cs
var events = await environment.EventQuery<TimeSeriesEntity>(FromDate, ToDate, Limit.Take, 200)
                    .Where(x => x.Value > 5)
                    .ExecuteAsync();
```

In this case FromDate and ToDate are DateTime objects and TimeSeriesEntity is the object defined above, this will return IEnumerable<TimeSeriesEntity>. The query is the equivalent on the Time Series Insights JSON query:

```json
{
  "headers": {
    "x-ms-client-application-name": "ChronologicalQuery",
    "Authorization": "Bearer xxxxx"
  },
  "content": {
    "searchSpan": {
      "from": "2017-11-29T21:26:05.0487455Z",
      "to": "2018-02-27T21:26:05.0487455Z"
    },
    "predicate": {
      "predicateString": "([Measurement.Value] > 5)"
    },
    "take": 200
  }
}

```
