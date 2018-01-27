[![Build status](https://ci.appveyor.com/api/projects/status/3x04uwu4bu4fy28s/branch/master?svg=true)](https://ci.appveyor.com/project/colethecoder/chronological/branch/master)

# chronological
Chronological is a library to simplify access to the Azure Time Series Insights API.This project is a work in progress, feedback and pull requests are welcomed.

## Download

The latest release of the Chronological is [available on NuGet](https://www.nuget.org/packages/Chronological/)

## Example

```cs
var connection = new Chronological.Connection(YourApplicationClientID,
                YourApplicationClientSecret, YourTenant);

var environments = await connection.GetEnvironmentsAsync();

var environment = environments.First();

var json = await environment.EventQuery("Test")
    .WithSearch(Search.Span(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow)
    .WithLimit(Limit.Top(200, Sort.Ascending(Property.TimeStamp)))
    .Where(Filter.Equal(Property.Custom("id", DataType.String), "1234"))
    .ResultsToJObjectAsync();
```

## Documentation

All documentation can now be found in the [wiki](https://github.com/colethecoder/chronological/wiki)


