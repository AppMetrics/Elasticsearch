# App Metrics Elasticsearch <img src="https://avatars0.githubusercontent.com/u/29864085?v=4&s=200" alt="App Metrics" width="50px"/> 
[![Official Site](https://img.shields.io/badge/site-appmetrics-blue.svg?style=flat-square)](http://app-metrics.io/reporting/elasticsearch.html) [![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg?style=flat-square)](https://opensource.org/licenses/Apache-2.0)

## What is it?

This repo contains Elasticsearch extension packages to [App Metrics](https://github.com/AppMetrics/AppMetrics).

## Latest Builds, Packages & Repo Stats

|Branch|AppVeyor|Travis|Coverage|
|------|:--------:|:--------:|:--------:|
|dev|[![AppVeyor](https://img.shields.io/appveyor/ci/alhardy/elasticsearch/dev.svg?style=flat-square&label=appveyor%20build)](https://ci.appveyor.com/project/alhardy/elasticsearch/branch/dev)|[![Travis](https://img.shields.io/travis/alhardy/Elasticsearch/dev.svg?style=flat-square&label=travis%20build)](https://travis-ci.org/alhardy/Elasticsearch)|[![Coveralls](https://img.shields.io/coveralls/AppMetrics/Elasticsearch/dev.svg?style=flat-square)](https://coveralls.io/github/AppMetrics/Elasticsearch?branch=dev)
|master|[![AppVeyor](https://img.shields.io/appveyor/ci/alhardy/elasticsearch/master.svg?style=flat-square&label=appveyor%20build)](https://ci.appveyor.com/project/alhardy/elasticsearch/branch/master)| [![Travis](https://img.shields.io/travis/alhardy/Elasticsearch/master.svg?style=flat-square&label=travis%20build)](https://travis-ci.org/alhardy/Elasticsearch)| [![Coveralls](https://img.shields.io/coveralls/AppMetrics/Elasticsearch/master.svg?style=flat-square)](https://coveralls.io/github/Elasticsearch/Elasticsearch?branch=master)|

|Package|Dev Release|PreRelease|Latest Release|
|------|:--------:|:--------:|:--------:|
|App.Metrics.Reporting.Elasticsearch|[![MyGet Status](https://img.shields.io/myget/appmetrics/v/App.Metrics.Reporting.Elasticsearch.svg?style=flat-square)](https://www.myget.org/feed/appmetrics/package/nuget/App.Metrics.Reporting.Elasticsearch)|[![NuGet Status](https://img.shields.io/nuget/vpre/App.Metrics.Reporting.Elasticsearch.svg?style=flat-square)](https://www.nuget.org/packages/App.Metrics.Reporting.Elasticsearch/)|[![NuGet Status](https://img.shields.io/nuget/v/App.Metrics.Reporting.Elasticsearch.svg?style=flat-square)](https://www.nuget.org/packages/App.Metrics.Reporting.Elasticsearch/)
|App.Metrics.Formatters.Elasticsearch|[![MyGet Status](https://img.shields.io/myget/appmetrics/v/App.Metrics.Formatters.Elasticsearch.svg?style=flat-square)](https://www.myget.org/feed/appmetrics/package/nuget/App.Metrics.Formatters.Elasticsearch)|[![NuGet Status](https://img.shields.io/nuget/vpre/App.Metrics.Formatters.Elasticsearch.svg?style=flat-square)](https://www.nuget.org/packages/App.Metrics.Formatters.Elasticsearch/)|[![NuGet Status](https://img.shields.io/nuget/v/App.Metrics.Formatters.Elasticsearch.svg?style=flat-square)](https://www.nuget.org/packages/App.Metrics.Formatters.Elasticsearch/)


#### Grafana/Elasticsearch Web Monitoring

![Grafana/Elasticsearch Generic Web Dashboard Demo](https://github.com/AppMetrics/AppMetrics.DocFx/blob/master/images/generic_grafana_dashboard_demo.gif)

> Grab the dashboard [here](https://grafana.com/dashboards/2140)

#### Grafana/Elasticsearch OAuth2 Client Web Monitoring

![Grafana/Elasticsearch Generic OAuth2 Web Dashboard Demo](https://github.com/AppMetrics/AppMetrics.DocFx/blob/master/images/generic_grafana_oauth2_dashboard_demo.gif)

> Grab the dashboard [here](https://grafana.com/dashboards/2143)

### Grafana/Elasticsearch Web Application Setup

- Download and run [Elasticsearch](https://www.elastic.co/downloads/elasticsearch).
- Create a new [ES Index and metric field mappings](https://github.com/AppMetrics/Elasticsearch/blob/dev/visualization/App.Metrics.Sandbox-Elasticsearch-IndexAndMappingSetup.json). *Keep note of this for configuring the Elasticsearch reporter in your web application and configuring the Elasticsearch Datasource in Grafana*
- Download and install [Grafana](https://grafana.com/grafana/download), then create a new [Elasticsearch Datasource](http://docs.grafana.org/features/datasources/elasticsearch/)  pointing to the Index just created and [import](http://docs.grafana.org/reference/export_import/#importing-a-dashboard) App.Metrics [web dashboard](https://grafana.com/dashboards/2140)
- See the [docs](https://www.app-metrics.io/reporting/reporters/elasticsearch/) on how to configure Elasticsearch reporting.
- Run your app and Grafana at visit `http://localhost:3000`

## How to build

[AppVeyor](https://ci.appveyor.com/project/alhardy/appmetrics-extensions-elasticsearch/branch/master) and [Travis CI](https://travis-ci.org/alhardy/AppMetrics.Extensions.Elasticsearch) builds are triggered on commits and PRs to `dev` and `master` branches.

See the following for build arguments and running locally.

|Configuration|Description|Default|Environment|Required|
|------|--------|:--------:|:--------:|:--------:|
|BuildConfiguration|The configuration to run the build, **Debug** or **Release** |*Release*|All|Optional|
|PreReleaseSuffix|The pre-release suffix for versioning nuget package artifacts e.g. `beta`|*ci*|All|Optional|
|CoverWith|**DotCover** or **OpenCover** to calculate and report code coverage, **None** to skip. When not **None**, a coverage file and html report will be generated at `./artifacts/coverage`|*OpenCover*|Windows Only|Optional|
|SkipCodeInspect|**false** to run ReSharper code inspect and report results, **true** to skip. When **true**, the code inspection html report and xml output will be generated at `./artifacts/resharper-reports`|*false*|Windows Only|Optional|
|BuildNumber|The build number to use for pre-release versions|*0*|All|Optional|


### Windows

Run `build.ps1` from the repositories root directory.

```
	.\build.ps1'
```

**With Arguments**

```
	.\build.ps1 --ScriptArgs '-BuildConfiguration=Release -PreReleaseSuffix=beta -CoverWith=OpenCover -SkipCodeInspect=false -BuildNumber=1'
```

### Linux & OSX

Run `build.sh` from the repositories root directory. Code Coverage reports are now supported on Linux and OSX, it will be skipped running in these environments.

```
	.\build.sh'
```

**With Arguments**

```
	.\build.sh --ScriptArgs '-BuildConfiguration=Release -PreReleaseSuffix=beta -BuildNumber=1'
```

## Contributing

See the [contribution guidlines](https://github.com/alhardy/AppMetrics/blob/master/CONTRIBUTING.md) in the [main repo](https://github.com/alhardy/AppMetrics) for details.

## Acknowledgements

* [Elasticsearch](https://www.elastic.co/)
* [DocFX](https://dotnet.github.io/docfx/)
* [Fluent Assertions](http://www.fluentassertions.com/)
* [XUnit](https://xunit.github.io/)
* [StyleCopAnalyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers)
* [Cake](https://github.com/cake-build/cake)
* [OpenCover](https://github.com/OpenCover/opencover)

***Thanks for providing free open source licensing***

* [Jetbrains](https://www.jetbrains.com/dotnet/) 
* [AppVeyor](https://www.appveyor.com/)
* [Travis CI](https://travis-ci.org/)
* [Coveralls](https://coveralls.io/)

## License

This library is release under Apache 2.0 License ( see LICENSE ) Copyright (c) 2016 Allan Hardy
